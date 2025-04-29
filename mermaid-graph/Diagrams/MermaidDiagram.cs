using System.Text;
using Microsoft.Build.Locator;

namespace MermaidGraph.Diagrams;

/// <summary>
/// The MermaidDiagram abstract class implements shared functionality for Mermaid diagram generation,
/// including initializing the graph output and managing the graph buffer.
/// </summary>
public abstract class MermaidDiagram : IMermaidDiagram
{
    /// <summary>
    /// Code block fence.
    /// </summary>
    public const string Fence = "```";

    /// <summary>
    /// Mermaid code block.
    /// </summary>
    public const string MermaidBegin = Fence + "mermaid";
    
    internal readonly StringBuilder Graph = new();

    /// <summary>
    /// Initialize the MermaidDiagram class and ensure MSBuild is registered.
    /// </summary>
    protected MermaidDiagram()
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
    }

    /// <summary>
    /// Initialize the graph output.
    /// </summary>
    public virtual void Header(string title)
    {
        Graph.Clear();
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
    /// Get the mermaid diagram Markdown text.
    /// </summary>
    /// <returns>The contents of the graph buffer.</returns>
    public override string ToString() => Graph.ToString();

    /// <inheritdoc />
    public abstract string Project(FileInfo file);

    /// <inheritdoc />
    public abstract string Solution(FileInfo file);
}
