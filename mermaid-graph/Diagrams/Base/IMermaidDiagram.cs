namespace MermaidGraph.Diagrams.Base;

/// <summary>
/// This file defines the IMermaidDiagram interface and the MermaidDiagram abstract class.
/// The IMermaidDiagram interface provides methods for generating Mermaid diagrams
/// from Visual Studio project (*.csproj) and solution (*.sln) files.
/// </summary>
public interface IMermaidDiagram
{
    /// <summary>
    /// Generate the diagram from a visual studio project file (*.csproj)
    /// </summary>
    /// <param name="file">The project file</param>
    /// <param name="filter">Exclude projects whose name matches the filter. (e.g., Test)</param>
    /// <param name="excludeNuget">Do not include NuGet packages in the graph</param>
    /// <returns>Mermaid Markdown</returns>
    public string Project(FileInfo file, string? filter = null, bool excludeNuget = false);

    /// <summary>
    /// Generate the diagram from a visual studio solution file (*.sln)
    /// </summary>
    /// <param name="file">The solution file.</param>
    /// <param name="filter">Exclude projects whose name matches the filter. (e.g., Test)</param>
    /// <param name="excludeNuget">Do not include NuGet packages in the graph</param>
    /// <returns>Mermaid Markdown</returns>
    public string Solution(FileInfo file, string? filter = null, bool excludeNuget = false);
}