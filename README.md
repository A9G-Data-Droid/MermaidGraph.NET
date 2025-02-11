![MermaidGraph.NET](mermaid-graph.png "MermaidGraph.NET")

# MermaidGraph.NET
Create a mermaid graph of the dependency diagram for a project, or whole solution.

## Dotnet tool [![NuGet version (mermaid-graph)](https://img.shields.io/nuget/v/mermaid-graph.svg?style=flat-square)](https://www.nuget.org/packages/mermaid-graph/)

You can install as a dotnet tool so you can easily map all of your software projects:

`dotnet tool install --global mermaid-graph`

## Usage
```
Description:
  Outputs a mermaid graph of the dependency diagram for a project, or whole solution.

Usage:
  mermaid-graph [options]

Options:
  --path <path>   Full path to the solution (*.sln) or project (*.csproj) file that will be mapped.
  --version       Show version information
  -?, -h, --help  Show help and usage information
  ```

## Example output from this solution

```mermaid
---
title: MermaidGraph.NET.sln
config:
  class:
    hideEmptyMembersBox: true
---
classDiagram
    class MermaidGraph.NET{
        type solution
    }
    MermaidGraph.NET --> mermaid-graph
    class mermaid-graph{
        type Exe
        target net9.0
    }
    class Microsoft.Build{
        type NuGet
        version 17.12.6
    }
    mermaid-graph ..> Microsoft.Build
    class Microsoft.Build.Locator{
        type NuGet
        version 1.7.8
    }
    mermaid-graph ..> Microsoft.Build.Locator
    class Microsoft.Build.Utilities.Core{
        type NuGet
        version 17.12.6
    }
    mermaid-graph ..> Microsoft.Build.Utilities.Core
    class System.CommandLine.DragonFruit{
        type NuGet
        version 0.4.0-alpha.22272.1
    }
    mermaid-graph ..> System.CommandLine.DragonFruit
    MermaidGraph.NET --> MermaidGraphTests
    class MermaidGraphTests{
        type Exe
        target net9.0
    }
    MermaidGraphTests ..> mermaid-graph
    class coverlet.collector{
        type NuGet
        version 6.0.4
    }
    MermaidGraphTests ..> coverlet.collector
    class coverlet.msbuild{
        type NuGet
        version 6.0.4
    }
    MermaidGraphTests ..> coverlet.msbuild
    class Microsoft.NET.Test.Sdk{
        type NuGet
        version 17.13.0
    }
    MermaidGraphTests ..> Microsoft.NET.Test.Sdk
    class MSTest.TestAdapter{
        type NuGet
        version 3.7.3
    }
    MermaidGraphTests ..> MSTest.TestAdapter
    class MSTest.TestFramework{
        type NuGet
        version 3.7.3
    }
    MermaidGraphTests ..> MSTest.TestFramework
    class NUnit{
        type NuGet
        version 4.3.2
    }
    MermaidGraphTests ..> NUnit
    class NUnit.Analyzers{
        type NuGet
        version 4.6.0
    }
    MermaidGraphTests ..> NUnit.Analyzers
    class NUnit3TestAdapter{
        type NuGet
        version 4.3.2
    }
    MermaidGraphTests ..> NUnit3TestAdapter
```
