using MermaidGraph.Diagrams.Base;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MermaidGraph.Diagrams;

/// <summary>
/// Generates a Mermaid class diagram.
/// </summary>
public sealed class ClassDiagram : MermaidDiagram
{
    /// <inheritdoc />
    public override void Header(string title)
    {
        base.Header(title);
        Graph.AppendLine("classDiagram");
    }

    /// <inheritdoc />
    public override string Solution(FileInfo file, string? filter = null)
    {
        Header(file.Name);
        var solutionFile = SolutionFile.Parse(file.FullName);
        var solutionName = Path.GetFileNameWithoutExtension(file.Name);
        var solutionId = $"{solutionName}";
        Graph.AppendLine($$"""
                                class {{solutionName}}{
                                    type solution
                                }
                            """);

        using var projectCollection = new ProjectCollection();
        
        foreach (var project in solutionFile.ProjectsInOrder)
        {
            if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;
            
            var projectPath = project.AbsolutePath;
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            if (!string.IsNullOrEmpty(filter) &&
                projectName.Contains(filter, StringComparison.Ordinal)) 
                continue;

            Graph.AppendLine($"    {solutionId} --> {projectName}");
            var projectFile = new FileInfo(projectPath);
            if (projectFile.Exists)
            {
                var referenceProject = projectCollection.LoadProject(projectFile.FullName);
                GraphProject(referenceProject, filter);
            }
        }

        Graph.AppendLine(Fence);

        projectCollection.UnloadAllProjects();

        return Graph.ToString();
    }

    internal override void GraphProject(Project project, string? filter = null)
    {
        var projectName = Path.GetFileNameWithoutExtension(project.FullPath);
        var type = project.GetPropertyValue("OutputType");
        var targetFramework = project.GetPropertyValue("TargetFramework") ?? 
                              project.GetPropertyValue("TargetFrameworks");

        Graph.AppendLine($$"""
                              class {{projectName}}{
                                  type {{type}}
                                  target {{targetFramework}}
                              }
                          """);

        foreach (var item in project.GetItems("ProjectReference"))
        {
            var refPath = item.EvaluatedInclude;
            var refName = Path.GetFileNameWithoutExtension(refPath);
            if (!string.IsNullOrEmpty(filter) &&
                projectName.Contains(filter, StringComparison.Ordinal)) 
                continue;

            Graph.AppendLine($"    {projectName} ..> {refName}");
        }

        foreach (var item in project.GetItems("PackageReference"))
        {
            var packageName = item.EvaluatedInclude;
            var version = item.GetMetadataValue("Version");
            Graph.AppendLine($$"""
                                    class {{packageName}}{
                                        type NuGet
                                        version {{version}}
                                    }
                                """);

            Graph.AppendLine($"    {projectName} ..> {packageName}");
        }
    }
}