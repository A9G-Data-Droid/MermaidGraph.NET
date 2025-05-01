using System;
using System.IO;
using System.Linq;
using MermaidGraph.Diagrams.Base;
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
        new object[] { DiagramType.Graph, "flowchart" }
    ];

    internal static readonly object[] FilterTestCases =
    [
        new object[] { DiagramType.Class, "Test" },
        new object[] { DiagramType.Graph, "Test" }
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
        Assert.That(graph, Is.Not.Null.Or.Empty, "Graph should not be null or empty.");
        Assert.That(graph, Does.Contain($"title: {info.Name}"));
        Assert.That(graph, Does.Contain("mermaid-graph"));
        Assert.That(graph, Does.Contain("MermaidGraphTests"));
        var graphType = DetectType(ExtractMermaid(graph));
        Console.WriteLine($"Detected type: {graphType}");

        Assert.That(graphType, Is.EqualTo(typeName));
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
        Assert.That(graph, Is.Not.Null.Or.Empty, "Graph should not be null or empty.");
        Assert.That(graph, Does.Contain($"title: {info.Name}"));
        Assert.That(graph, Does.Contain("mermaid-graph"));
        Assert.That(graph, Does.Contain("MermaidGraphTests"));
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

    [Test]
    [TestCaseSource(nameof(FilterTestCases))]
    public void DiagramsShouldNotContainFilteredContent(DiagramType type, string filter)
    {
        var solutionPath = FindFileDownTree("*.sln");
        Assert.That(solutionPath, Is.Not.Null);
        var info = new FileInfo(solutionPath!);
        Assert.That(info.Exists);
        var graph = Commands.Solution(info, type);
        Assert.That(graph, Does.Contain(filter),
            $"Original Graph should contain filtered content: {filter}");

        graph = Commands.Solution(info, type, filter);
        Console.WriteLine(graph);
        Assert.That(graph, Does.Not.Contain(filter),
            $"Graph should not contain filtered content: {filter}");
    }

    [Test]
    [TestCase(DiagramType.Graph)]
    [TestCase(DiagramType.Class)]
    public void Project_ShouldExcludeNuget_WhenExcludeNugetIsTrue(DiagramType type)
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        var info = new FileInfo(filePath!);
        Assert.That(info.Exists);

        var graph = Commands.Project(info, type, excludeNuget: true);
        Assert.That(graph, Does.Not.Contain("NuGet"), "Graph should not contain NuGet references when noNuget is true.");
    }

    [Test]
    [TestCase(DiagramType.Graph)]
    [TestCase(DiagramType.Class)]
    public void Solution_ShouldExcludeNuget_WhenExcludeNugetIsTrue(DiagramType type)
    {
        var solutionPath = FindFileDownTree("*.sln");
        Assert.That(solutionPath, Is.Not.Null);
        var info = new FileInfo(solutionPath!);
        Assert.That(info.Exists);

        var graph = Commands.Solution(info, type, excludeNuget: true);
        Assert.That(graph, Does.Not.Contain("NuGet"), "Graph should not contain NuGet references when noNuget is true.");
    }

    [Test]
    [TestCase(DiagramType.Graph)]
    [TestCase(DiagramType.Class)]
    public void Project_ShouldIncludeNuget_WhenExcludeNugetIsFalse(DiagramType type)
    {
        var filePath = FindFileDownTree("*.csproj");
        Assert.That(filePath, Is.Not.Null);
        var info = new FileInfo(filePath!);
        Assert.That(info.Exists);

        var graph = Commands.Project(info, type, excludeNuget: false);
        Assert.That(graph, Does.Contain("NuGet"), "Graph should contain NuGet references when noNuget is false.");
    }

    [Test]
    [TestCase(DiagramType.Graph)]
    [TestCase(DiagramType.Class)]
    public void Solution_ShouldIncludeNuget_WhenExcludeNugetIsFalse(DiagramType type)
    {
        var solutionPath = FindFileDownTree("*.sln");
        Assert.That(solutionPath, Is.Not.Null);
        var info = new FileInfo(solutionPath!);
        Assert.That(info.Exists);

        var graph = Commands.Solution(info, type, excludeNuget: false);
        Assert.That(graph, Does.Contain("NuGet"), "Graph should contain NuGet references when noNuget is false.");
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