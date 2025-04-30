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
}

