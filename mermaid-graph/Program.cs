using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace mermaid_graph;

internal class Program
{
    /// <summary>
    /// Outputs a mermaid graph of the dependency diagram for a project, or whole solution.
    /// </summary>
    /// <param name="path">Full path to the solution (*.sln) or project (*.csproj) file that will be mapped.</param>
    /// <returns>HResult</returns>
    static int Main(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Error: File not found - {path}");
            return 1;
        }

        // Register the MSBuild instance (needed for Microsoft.Build.Evaluation)
        MSBuildLocator.RegisterDefaults();

        if (path.EndsWith(".csproj"))
        {
            return Commands.Project(path);
        }

        if (path.EndsWith(".sln"))
        {
            return Commands.Solution(path);
        }

        Console.WriteLine($"Error: Unsupported file type - {path}");
        return 2;
    }
}

public static class Commands
{
    /// <summary>
    /// Generate
    /// </summary>
    
    public static int Project(string path)
    {
        // Load project
        var project = new Project(path);

        var sb = new StringBuilder("```");
        sb.AppendLine("graph TD;");

        string projectName = Path.GetFileNameWithoutExtension(path);

        // Add ProjectReferences
        foreach (var item in project.GetItems("ProjectReference"))
        {
            string refPath = item.EvaluatedInclude;
            string refName = Path.GetFileNameWithoutExtension(refPath);
            sb.AppendLine($"    {projectName} --> {refName}");
        }

        // Add PackageReferences
        foreach (var item in project.GetItems("PackageReference"))
        {
            string packageName = item.EvaluatedInclude;
            sb.AppendLine($"    {projectName} -->|NuGet| {packageName}");
        }

        // Output Mermaid.js graph
        Console.WriteLine(sb.ToString());

        return 0;
    }

    internal static int Solution(string path)
    {
        throw new NotImplementedException();
    }
}