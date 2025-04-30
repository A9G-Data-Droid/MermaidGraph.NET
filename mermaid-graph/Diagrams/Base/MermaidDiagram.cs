using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace MermaidGraph.Diagrams.Base;

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
    
    internal readonly StringBuilder Graph = new(256);

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
    /// Factory method to get the appropriate graph type based on the provided DiagramType enum value.
    /// </summary>
    /// <param name="diagramType">The type of graph to generate.</param>
    /// <returns>The appropriate methods for generating a diagram of that type.</returns>
    /// <exception cref="NotImplementedException">If an enum type is added without corresponding diagram class.</exception>
    public static IMermaidDiagram GetDiagramType(DiagramType diagramType) => diagramType switch
    {
        DiagramType.Class => new ClassDiagram(),
        DiagramType.Graph => new GraphDiagram(),
        _ => throw new NotImplementedException($"Option not supported: {diagramType}"),
    };

    /// <summary>
    /// Initialize the graph output.
    /// </summary>
    public virtual void Header(string title)
    {
        Graph.Clear();
        Graph.AppendLine($"""
                           {MermaidBegin}
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
    public virtual string Project(FileInfo file)
    {
        Header(file.Name);
        using var projectCollection = new ProjectCollection();
        var project = projectCollection.LoadProject(file.FullName);
        GraphProject(project);
        Graph.AppendLine(Fence);

        projectCollection.UnloadAllProjects();

        return Graph.ToString();
    }

    /// <inheritdoc />
    public abstract string Solution(FileInfo file);

    /// <summary>
    /// This method must be implemented in all derived classes to generate the graph for a project.
    /// </summary>
    /// <param name="project">A project to graph.</param>
    internal abstract void GraphProject(Project project);
}
