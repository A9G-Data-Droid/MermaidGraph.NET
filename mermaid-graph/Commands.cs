using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace MermaidGraph;

/// <summary>
/// The commands that can be run by `mermaid-graph`
/// </summary>
public class Commands
{
    public const string MermaidBegin = Fence + "mermaid";
    public const string Fence = "```";

    private readonly StringBuilder _graph;

    /// <summary>
    /// Initialize the graph output
    /// </summary>
    public Commands()
    {
        _graph = new StringBuilder();

        // Ensure MSBuild is registered
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Project.
    /// </summary>
    /// <param name="file">`.csproj` file.</param>
    public string Project(FileInfo file)
    {
        Header(file.Name);
        using var projectCollection = new ProjectCollection();
        var project = projectCollection.LoadProject(file.FullName);
        GraphProject(project);
        _graph.AppendLine(Fence);
        var graph = _graph.ToString();

        // Cleanup
        _graph.Clear();
        projectCollection.UnloadAllProjects();

        return graph;
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="file">`.sln` file.</param>
    public string Solution(FileInfo file)
    {
        Header(file.Name);
        var solutionFile = SolutionFile.Parse(file.FullName);
        var solutionName = Path.GetFileNameWithoutExtension(file.Name);
        var solutionId = $"{solutionName}";
        _graph.AppendLine($$"""
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
            _graph.AppendLine($"    {solutionId} --> {projectName}");
            var projectFile = new FileInfo(projectPath);
            if (projectFile.Exists)
            {
                var referenceProject = projectCollection.LoadProject(projectFile.FullName);
                GraphProject(referenceProject);
            }
        }

        _graph.AppendLine(Fence);
        var graph = _graph.ToString();
        
        // Cleanup
        _graph.Clear();
        projectCollection.UnloadAllProjects();

        return graph;
    }

    private void Header(string title)
    {
        _graph.AppendLine(MermaidBegin);
        _graph.AppendLine($"""
                           ---
                           title: {title}
                           config:
                             class:
                               hideEmptyMembersBox: true
                           ---
                           """);

        _graph.AppendLine("classDiagram");
    }

    private void GraphProject(Project project)
    {
        var projectName = Path.GetFileNameWithoutExtension(project.FullPath);
        var type = project.GetPropertyValue("OutputType");
        var targetFramework = project.GetPropertyValue("TargetFramework") ?? project.GetPropertyValue("TargetFrameworks");
        _graph.AppendLine($$"""
                              class {{projectName}}{
                                  type {{type}}
                                  target {{targetFramework}}
                              }
                          """);

        foreach (var item in project.GetItems("ProjectReference"))
        {
            var refPath = item.EvaluatedInclude;
            var refName = Path.GetFileNameWithoutExtension(refPath);
            _graph.AppendLine($"    {projectName} ..> {refName}");
        }

        foreach (var item in project.GetItems("PackageReference"))
        {
            var packageName = item.EvaluatedInclude;
            var version = item.GetMetadataValue("Version");
            _graph.AppendLine($$"""
                                    class {{packageName}}{
                                        type NuGet
                                        version {{version}}
                                    }
                                """);

            _graph.AppendLine($"    {projectName} ..> {packageName}");
        }
    }
}