using MermaidGraph.Diagrams;
using Microsoft.Build.Locator;

namespace MermaidGraph;

/// <summary>
/// The commands that can be run by `mermaid-graph`
/// </summary>
public class Commands
{

    public Commands()
    {
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
        var graph = new ClassDiagram();
        
        return graph.Project(file);
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="file">`.sln` file.</param>
    public string Solution(FileInfo file)
    {
        var graph = new ClassDiagram();
        
        return graph.Solution(file);
    }
}