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
    public static string Project(FileInfo file, DiagramType diagramType = DiagramType.Graph, string? filter = null, bool excludeNuget = false)
    {
        var graph = MermaidDiagram.GetDiagramType(diagramType);
        
        return graph.Project(file, filter, excludeNuget);
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    public static string Solution(FileInfo file, DiagramType diagramType = DiagramType.Graph, string? filter = null, bool excludeNuget = false)
    {
        var graph = MermaidDiagram.GetDiagramType(diagramType);
        
        return graph.Solution(file, filter, excludeNuget);
    }
}