using Microsoft.Build.Locator;

namespace MermaidGraph;

// ReSharper disable UnusedMember.Global

/// <summary>
/// mermaid-graph.exe
/// </summary>
internal sealed class Program
{
    /// <summary>
    /// Outputs a mermaid graph of the dependency diagram for a project, or whole solution.
    /// </summary>
    /// <param name="path">Full path to the solution (*.sln) or project (*.csproj) file that will be mapped.</param>
    /// <returns>HResult</returns>
    internal static int Main(string path)
    {
        var file = new FileInfo(path);
        if (!file.Exists)
        {
            Console.WriteLine($"Error: File not found - {path}");
            return 1;
        }

        try
        {
            // Ensure MSBuild is registered
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }

            if (path.EndsWith(".csproj"))
            {
                Commands.Project(file.FullName);
                return 0;
            }

            if (path.EndsWith(".sln"))
            {
                Commands.Solution(file.FullName);
                return 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ex.HResult;
        }

        Console.WriteLine($"Error: Unsupported file type - {path}");
        return 2;
    }
}