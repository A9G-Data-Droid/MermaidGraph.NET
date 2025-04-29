using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace MermaidGraph.Diagrams;

internal class GraphDiagram : MermaidDiagram
{
    /// <inheritdoc />
    public override void Header(string title)
    {
        base.Header(title);
        Graph.AppendLine("graph TD");
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Project.
    /// </summary>
    /// <param name="file">`.csproj` file.</param>
    public override string Project(FileInfo file)
    {
        Header(file.Name);
        using var projectCollection = new ProjectCollection();
        var project = projectCollection.LoadProject(file.FullName);
        GraphProject(project);
        Graph.AppendLine(Fence);

        projectCollection.UnloadAllProjects();

        return Graph.ToString();
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

    private void GraphProject(Project project)
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