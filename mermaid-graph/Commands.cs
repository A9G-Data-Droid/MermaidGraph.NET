using System.Diagnostics;
using MermaidGraph.Diagrams;

namespace MermaidGraph;

/// <summary>
/// Specifies the type of mermaid diagram to generate.
/// </summary>
public enum DiagramType
{
    /// <summary>
    /// Represents a general dependency graph.
    /// </summary>
    Graph,

    /// <summary>
    /// Represents a class diagram.
    /// </summary>
    Class
}

/// <summary>
/// The commands that can be run by `mermaid-graph`
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
        var graph = GetGraphType(diagramType);
        
        return graph.Project(file);
    }

    /// <summary>
    /// Generate the dependency graph of a Visual Studio Solution.
    /// </summary>
    /// <param name="file">`.sln` file.</param>
    /// <param name="diagramType"></param>
    public static string Solution(FileInfo file, DiagramType diagramType = DiagramType.Graph)
    {
        var graph = GetGraphType(diagramType);
        
        return graph.Solution(file);
    }

    private static Diagram GetGraphType(DiagramType diagramType) => diagramType switch
    {
        DiagramType.Class => new ClassDiagram(),
        DiagramType.Graph => new GraphDiagram(),
        _ => throw new NotImplementedException($"Option not supported: {diagramType}"),
    };
}