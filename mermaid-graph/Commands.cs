using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MermaidGraph;

/// <summary>
/// The commands that can be run by `mermaid-graph`
/// </summary>
public static class Commands
{
    private static readonly StringBuilder Graph = new();

    private const string Fence = "```";

    static Commands()
    {
        Graph.AppendLine(Fence + "mermaid");
        Graph.AppendLine("graph TD");
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Project.
    /// </summary>
    /// <param name="path">Full path to `.csproj` file.</param>
    public static void Project(string path)
    {
        GraphProject(path);
        Graph.AppendLine(Fence);
        Console.WriteLine(Graph.ToString());
    }

    private static void GraphProject(string path)
    {
        // Load project
        var project = new Project(path);
        var projectName = Path.GetFileNameWithoutExtension(path);

        foreach (var item in project.GetItems("ProjectReference"))
        {
            string refPath = item.EvaluatedInclude;
            string refName = Path.GetFileNameWithoutExtension(refPath);
            Graph.AppendLine($"    {projectName} --> {refName}");
        }

        foreach (var item in project.GetItems("PackageReference"))
        {
            string packageName = item.EvaluatedInclude;
            Graph.AppendLine($"    {projectName} -->|NuGet| {packageName}");
        }
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="path">Full path to `.sln` file.</param>
    public static void Solution(string path)
    {
        var solutionFile = SolutionFile.Parse(path);
        var solutionName = Path.GetFileNameWithoutExtension(path);
        foreach (var project in solutionFile.ProjectsInOrder)
        {
            if (project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat)
            {
                var projectPath = project.AbsolutePath;
                var projectName = Path.GetFileNameWithoutExtension(projectPath);
                Graph.AppendLine($"    {solutionName} --> {projectName}");
                if (File.Exists(projectPath))
                {
                    GraphProject(projectPath);
                }
            }
        }

        Graph.AppendLine(Fence);
        Console.WriteLine(Graph.ToString());
    }
}