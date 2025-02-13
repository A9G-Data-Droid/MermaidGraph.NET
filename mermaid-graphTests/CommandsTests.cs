using Microsoft.ClearScript.V8;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MermaidGraph.Tests;

[TestFixture()]
public class CommandsTests
{
    [Test()]
    public void DogFoodSolutionTest()
    {
        var solutionPath = FindFileDownTree("*.sln");
        Assert.That(solutionPath, Is.Not.Null);
        var info = new FileInfo(solutionPath!);
        Assert.That(info.Exists);
        var graph = new Commands().Solution(info);

        Console.WriteLine(graph);
        var graphType = DetectType(ExtractMermaid(graph));
        Console.WriteLine(graphType);
    }

    [Test()]
    public async Task DogFoodProjectTestAsync()
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        var info = new FileInfo(filePath!);
        Assert.That(info.Exists);
        var graph = new Commands().Project(info);

        Console.WriteLine(graph);

        var graphType = await MermaidRender(ExtractMermaid(graph));
        Console.WriteLine(graphType);
    }

    [Test()]
    public void CommandLineProjectTest()
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);

        Assert.That(Program.Main(filePath), Is.EqualTo(0));
    }

    [Test()]
    public void CommandLineSolutionTest()
    {
        var filePath = FindFileDownTree("*.sln");
        Assert.That(filePath, Is.Not.Null);

        Assert.That(Program.Main(filePath), Is.EqualTo(0));
    }

    [Test()]
    [TestCase(null, 1)]
    [TestCase("File Not Found", 2)]
    [TestCase("mermaid-graph.dll", 3)]

    public void CommandLineFailTests(string? file, int hResult)
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);

        Assert.That(Program.Main(file), Is.EqualTo(hResult));
    }

    private static string ExtractMermaid(string markup)
    {
        Assert.That(markup, Does.StartWith(Commands.MermaidBegin));
        markup = markup.Substring(Commands.MermaidBegin.Length + Environment.NewLine.Length);

        Assert.That(markup, Does.EndWith(Commands.Fence + Environment.NewLine));
        return markup.Substring(0, markup.Length - Commands.MermaidBegin.Length + Environment.NewLine.Length);
    }

    private static string DetectType(string markup)
    {
        using var engine = new V8ScriptEngine();
        var mermaidJs = File.ReadAllText("js\\mermaid.min.js");
        engine.Execute(mermaidJs);

        return engine.Script.mermaid.detectType(markup);
    }

    private static async Task<string> MermaidRender(string markup)
    {
        using var engine = new V8ScriptEngine();
        var mermaidJs = File.ReadAllText("js\\mermaid.min.js");
        engine.Execute(mermaidJs);

        var svg = await engine.Script.mermaid.render(markup);
        Assert.That(svg, Is.Not.Null);
        return svg.ToString();
    }

    private static string? FindFileDownTree(string searchPattern)
    {
        var currentDir = Directory.GetCurrentDirectory();

        while (currentDir != null)
        {
            var solutionFiles = Directory.EnumerateFiles(currentDir, searchPattern);
            var file = solutionFiles.FirstOrDefault();
            if (file is not null)
            {
                return file; 
            }

            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        return null;
    }
}