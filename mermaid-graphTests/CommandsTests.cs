using MermaidGraph.Diagrams;
using Microsoft.ClearScript.V8;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MermaidGraph.Tests;

[TestFixture]
public class CommandsTests
{
    private V8ScriptEngine? _engine;
    private V8ScriptEngine Js {
        get
        {
            if (_engine is null)
            {
                _engine ??= new V8ScriptEngine();
                _engine.Execute(File.ReadAllText("js\\mermaid.min.js"));
                _engine.Script.mermaid.initialize();
            }

            return _engine;
        }
    }

    internal static readonly object[] DiagramTypeTestCases =
    [
        new object[] { DiagramType.Class, "class" },
        new object[] { DiagramType.Class, "class" },
        new object[] { DiagramType.Graph, "flowchart" }
    ];

    [OneTimeTearDown]
    public void Disposal()
    {
        _engine?.Dispose();
    }

    [Test]
    [TestCaseSource(nameof(DiagramTypeTestCases))]
    public void DogFoodSolutionTest(DiagramType type, string typeName)
    {
        var solutionPath = FindFileDownTree("*.sln");
        Assert.That(solutionPath, Is.Not.Null);
        var info = new FileInfo(solutionPath!);
        Assert.That(info.Exists);
        var graph = Commands.Solution(info, type);

        Console.WriteLine(graph);

        var graphType = DetectType(ExtractMermaid(graph));
        Assert.That(graphType, Is.EqualTo(typeName));
        Console.WriteLine(graphType);
    }

    [Test]
    [TestCaseSource(nameof(DiagramTypeTestCases))]
    public void DogFoodProjectTest(DiagramType type, string typeName)
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        var info = new FileInfo(filePath!);
        Assert.That(info.Exists);
        var graph = Commands.Project(info, type);

        Console.WriteLine(graph);

        var graphType = DetectType(ExtractMermaid(graph));
        Assert.That(graphType, Is.EqualTo(typeName));
        Console.WriteLine(graphType);
    }

    [Test]
    public void CommandLineProjectTest()
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        Assert.That(Program.Main(filePath), Is.EqualTo(0));
    }

    [Test]
    public void CommandLineSolutionTest()
    {
        var filePath = FindFileDownTree("*.sln");
        Assert.That(filePath, Is.Not.Null);
        Assert.That(Program.Main(filePath), Is.EqualTo(0));
    }

    [Test]
    [TestCase(null, 1)]
    [TestCase("File Not Found", 2)]
    [TestCase("mermaid-graph.dll", 3)]

    public void CommandLineFailTests(string? file, int hResult)
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        Assert.That(Program.Main(file), Is.EqualTo(hResult));
    }

    private static string ExtractMermaid(string? markup)
    {
        Assert.That(markup, Does.StartWith(MermaidDiagram.MermaidBegin));
        markup = markup.Substring(MermaidDiagram.MermaidBegin.Length + Environment.NewLine.Length);

        Assert.That(markup, Does.EndWith(MermaidDiagram.Fence + Environment.NewLine));
        return markup.Substring(0, markup.Length - MermaidDiagram.MermaidBegin.Length + Environment.NewLine.Length);
    }

    private string? DetectType(string markup)
    {
        return Js.Script.mermaid.detectType(markup);
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