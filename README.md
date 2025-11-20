# BGPSimulator_HMC (C# .NET 8)
# Author: Humphrey Mbaulu Chinyama
# Student No.: 24157696
# Date: 2025-09-25

## What
A simple BGP-style routing simulator in C# that:
- runs discrete iterations,
- routes are advertised by neighbors,
- uses a simplified BGP decision process,
- outputs routing tables per iteration,
- creates GraphViz DOT files and tries to render PNGs with `dot`.

## Requirements
- .NET 8 SDK (or change TargetFramework in csproj to your installed version)
- (optional) Graphviz installed and `dot` in PATH to produce PNGs

## Run in Visual Studio
1. Open `BGPSimulator_HMC.sln`.
2. Set `BGPSimulator_HMC` as startup project.
3. Run (F5) or Ctrl+F5.

## Command-line
From project folder:# BGPSimulator_HMC (C# .NET 8)

## What
A simple BGP-style routing simulator in C# that:
- runs discrete iterations,
- routes are advertised by neighbors,
- uses a simplified BGP decision process,
- outputs routing tables per iteration,
- creates GraphViz DOT files and tries to render PNGs with `dot`.
- copy and paste output of `dot` command to an online renderer like https://dreampuf.github.io/GraphvizOnline/ if you don't have Graphviz installed.

## Requirements
- .NET 8 SDK (or change TargetFramework in csproj to your installed version)
- (optional) Graphviz installed and `dot` in PATH to produce PNGs

## Run in Visual Studio
1. Open `BGPSimulator_HMC.sln`.
2. Set `BGPSimulator_HMC` as startup project.
3. Run (F5) or Ctrl+F5.

## Command-line
From project folder:

## GraphViz
Install Graphviz from https://graphviz.org/download/ (Windows installer).
Make sure `dot` is available on your PATH. The renderer will still write DOT files even if `dot` isn't present.

DOT and PNG files are written to the `graphs` folder.

## Running from Docker
1. docker build -t bgp-sim .
2. docker run --rm -it -v "%cd%/graphs:/app/graphs" bgp-sim

## Assumptions
Be sure to play with the assumptions and init data in programs.cs

// Initially, A originates prefix 10.0.0.0/24, C originates 20.0.0.0/24, D originates 30.0.0.0/24
A.RoutingTable["10.0.0.0/24"] = new Route { Prefix = "10.0.0.0/24", NextHop = "A", AsPath = new System.Collections.Generic.List<string> { "A" } };
C.RoutingTable["20.0.0.0/24"] = new Route { Prefix = "20.0.0.0/24", NextHop = "C", AsPath = new System.Collections.Generic.List<string> { "C" } };
D.RoutingTable["30.0.0.0/24"] = new Route { Prefix = "30.0.0.0/24", NextHop = "D", AsPath = new System.Collections.Generic.List<string> { "D" } };

