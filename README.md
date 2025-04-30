![MermaidGraph.NET](mermaid-graph.png "MermaidGraph.NET")

# MermaidGraph.NET ![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/A9G-Data-Droid/24afd0c237dc4c9b56453dce09b24687/raw/MermaidGraph.NET.code-coverage.json)

Create a mermaid graph of the dependency diagram for a project, or whole solution.

## Dotnet tool [![NuGet version (mermaid-graph)](https://img.shields.io/nuget/v/mermaid-graph.svg?style=flat-round)](https://www.nuget.org/packages/mermaid-graph/)

You can install as a dotnet tool so you can easily map all of your software projects:

`dotnet tool install --global mermaid-graph`

## Usage
```
Description:
  Outputs a mermaid graph of the dependency diagram for a project, or whole solution.

Usage:
  mermaid-graph [options]

Options:
  --path <path>         Full path to the solution (*.sln) or project (*.csproj) file that will be mapped.
  --type <Class|Graph>  The type of diagram to generate (e.g., Graph or Class). [default: Graph]
  --filter <filter>     Exclude projects whose name matches the filter. (e.g., Test) []
  --version             Show version information
  -?, -h, --help        Show help and usage information
```

## Example output from this solution

You can run the following command to generate a class diagram for this solution:

```powershell
.\mermaid-graph.exe --path "MermaidGraph.NET.sln" --type Class
```

This will generate a mermaid graph in the console output, which can be piped to a file and used in a markdown document.

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
        version 17.13.9
    }
    mermaid-graph ..> Microsoft.Build
    class Microsoft.Build.Locator{
        type NuGet
        version 1.9.1
    }
    mermaid-graph ..> Microsoft.Build.Locator
    class Microsoft.Build.Utilities.Core{
        type NuGet
        version 17.13.9
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
    class Microsoft.ClearScript.V8{
        type NuGet
        version 7.5.0
    }
    MermaidGraphTests ..> Microsoft.ClearScript.V8
    class Microsoft.ClearScript.V8.Native.win-x64{
        type NuGet
        version 7.5.0
    }
    MermaidGraphTests ..> Microsoft.ClearScript.V8.Native.win-x64
    class Microsoft.NET.Test.Sdk{
        type NuGet
        version 17.13.0
    }
    MermaidGraphTests ..> Microsoft.NET.Test.Sdk
    class MSTest.TestAdapter{
        type NuGet
        version 3.8.3
    }
    MermaidGraphTests ..> MSTest.TestAdapter
    class MSTest.TestFramework{
        type NuGet
        version 3.8.3
    }
    MermaidGraphTests ..> MSTest.TestFramework
    class NUnit{
        type NuGet
        version 4.3.2
    }
    MermaidGraphTests ..> NUnit
    class NUnit.Analyzers{
        type NuGet
        version 4.7.0
    }
    MermaidGraphTests ..> NUnit.Analyzers
    class NUnit3TestAdapter{
        type NuGet
        version 5.0.0
    }
    MermaidGraphTests ..> NUnit3TestAdapter
```
