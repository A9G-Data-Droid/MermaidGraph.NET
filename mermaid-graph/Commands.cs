using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MermaidGraph;

/// <summary>
/// The commands that can be run by `mermaid-graph`
/// </summary>
public class Commands
{
    private readonly StringBuilder _graph;

    private const string Fence = "```";

    /// <summary>
    /// Initialize the graph output
    /// </summary>
    public Commands()
    {
        _graph = new StringBuilder();
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Project.
    /// </summary>
    /// <param name="file">`.csproj` file.</param>
    public string Project(FileInfo file)
    {
        Header(file.Name);
        GraphProject(file);
        _graph.AppendLine(Fence);
        var graph = _graph.ToString();
        _graph.Clear();
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
        var solutionId = $"s{solutionFile.GetHashCode()}({solutionName})";
        foreach (var project in solutionFile.ProjectsInOrder)
        {
            if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;

            var projectPath = project.AbsolutePath;
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            _graph.AppendLine($"    {solutionId} --> {projectName}");
            var projectFile = new FileInfo(projectPath);
            if (projectFile.Exists)
            {
                GraphProject(projectFile);
            }
        }

        _graph.AppendLine(Fence);
        var graph = _graph.ToString();
        _graph.Clear();
        return graph;
    }

    private void Header(string title)
    {
        _graph.AppendLine(Fence + "mermaid");
        _graph.AppendLine($"""
                           ---
                           title: {title}
                           ---
                           """);

        _graph.AppendLine("graph TD");
    }

    private void GraphProject(FileInfo path)
    {
        var project = new Project(path.FullName);
        var projectName = Path.GetFileNameWithoutExtension(path.Name);

        foreach (var item in project.GetItems("ProjectReference"))
        {
            var refPath = item.EvaluatedInclude;
            var refName = Path.GetFileNameWithoutExtension(refPath);
            _graph.AppendLine($"    {projectName} --> {refName}");
        }

        foreach (var item in project.GetItems("PackageReference"))
        {
            var packageName = item.EvaluatedInclude;
            _graph.AppendLine($"    {projectName} -->|NuGet| {packageName}");
        }
    }
}