using MermaidGraph.Diagrams.Base;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MermaidGraph.Diagrams;

/// <summary>
/// Generates a Mermaid dependency graph for Visual Studio projects and solutions.
/// </summary>
public sealed class GraphDiagram : MermaidDiagram
{
    /// <inheritdoc />
    public override void Header(string title)
    {
        base.Header(title);
        Graph.AppendLine("graph TD");
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="file">`.sln` file.</param>
    public override string Solution(FileInfo file)
    {
        Header(file.Name);
        var solutionFile = SolutionFile.Parse(file.FullName);
        var solutionName = Path.GetFileNameWithoutExtension(file.Name);
        var solutionId = $"s{solutionFile.GetHashCode()}({solutionName})";

        using var projectCollection = new ProjectCollection();

        foreach (var project in solutionFile.ProjectsInOrder)
        {
            if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;

            var projectPath = project.AbsolutePath;
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            Graph.AppendLine($"    {solutionId} --> {projectName}");
            var projectFile = new FileInfo(projectPath);
            if (projectFile.Exists)
            {
                var referenceProject = projectCollection.LoadProject(projectFile.FullName);
                GraphProject(referenceProject);
            }
        }

        Graph.AppendLine(Fence);

        projectCollection.UnloadAllProjects();

        return Graph.ToString();
    }

    internal override void GraphProject(Project project)
    {
        var projectName = Path.GetFileNameWithoutExtension(project.FullPath);

        foreach (var item in project.GetItems("ProjectReference"))
        {
            var refPath = item.EvaluatedInclude;
            var refName = Path.GetFileNameWithoutExtension(refPath);
            Graph.AppendLine($"    {projectName} --> {refName}");
        }

        foreach (var item in project.GetItems("PackageReference"))
        {
            var packageName = item.EvaluatedInclude;
            Graph.AppendLine($"    {projectName} -->|NuGet| {packageName}");
        }
    }
}