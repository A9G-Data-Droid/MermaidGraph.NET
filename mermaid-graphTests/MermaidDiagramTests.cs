using System;
using MermaidGraph.Diagrams;
using MermaidGraph.Diagrams.Base;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace MermaidGraph.Tests;

[TestFixture]
public class MermaidDiagramTests
{
    [Test]
    public void GetDiagramType_ShouldReturnCorrectInstance()
    {
        // Arrange & Act
        var classDiagram = MermaidDiagram.GetDiagramType(DiagramType.Class);
        var graphDiagram = MermaidDiagram.GetDiagramType(DiagramType.Graph);

        // Assert
        Assert.That(classDiagram, Is.TypeOf<ClassDiagram>());
        Assert.That(graphDiagram, Is.TypeOf<GraphDiagram>());
    }

    [Test]
    public void GetDiagramType_ShouldThrowForUnsupportedType()
    {
        // Arrange & Act
        Assert.Throws<NotImplementedException>(()=>
            MermaidDiagram.GetDiagramType((DiagramType)999));
    }

    [Test]
    [TestCase(DiagramType.Class)]
    [TestCase(DiagramType.Graph)]
    public void Header_ShouldInitializeGraphWithTitle(DiagramType type)
    {
        var diagram = (MermaidDiagram)MermaidDiagram.GetDiagramType(type);
        
        // Arrange
        const string title = "Test Diagram";

        // Act
        diagram.Header(title);

        // Assert
        Assert.That(diagram.ToString(), Does.Contain($"title: {title}"));
    }
}

