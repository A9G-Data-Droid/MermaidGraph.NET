using MermaidGraph.Diagrams.Base;

namespace MermaidGraph;

/// <summary>
/// The commands that can be run by `mermaid-graph`.
/// </summary>
public class Commands
{
    /// <summary>
    /// Generate the dependency graph of a Visual Studio Project.
    /// </summary>
    /// <param name="file">`.csproj` file.</param>
    /// <param name="diagramType"></param>
    public static string Project(FileInfo file, DiagramType diagramType = DiagramType.Graph)
    {
        var graph = MermaidDiagram.GetDiagramType(diagramType);
        
        return graph.Project(file);
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="file">`.sln` file.</param>
    /// <param name="diagramType"></param>
    public static string Solution(FileInfo file, DiagramType diagramType = DiagramType.Graph)
    {
        var graph = MermaidDiagram.GetDiagramType(diagramType);
        
        return graph.Solution(file);
    }
}