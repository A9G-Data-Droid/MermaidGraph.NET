using System.Text;

namespace MermaidGraph.Diagrams;

public class Diagram
{
    public const string Fence = "```";
    public const string MermaidBegin = Fence + "mermaid";
    
    internal readonly StringBuilder Graph = new();

    /// <summary>
    /// Initialize the graph output
    /// </summary>
    public virtual void Header(string title)
    {
        Graph.AppendLine(MermaidBegin);
        Graph.AppendLine($"""
                           ---
                           title: {title}
                           config:
                             class:
                               hideEmptyMembersBox: true
                           ---
                           """);
    }

    /// <summary>
    /// Get the mermaid diagram markdown text.
    /// </summary>
    /// <returns>The contents of the graph buffer.</returns>
    public override string ToString() => Graph.ToString();
}