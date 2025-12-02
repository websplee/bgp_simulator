# BGPSimulator_HMC (C# .NET 8)
# Author: Humphrey Mbaulu Chinyama
# Student No.: 24157696
# Date: 2025-09-25

# BGP Routing Simulator (C# .NET 8)

A fully interactive BGP-like routing protocol simulator written in C# and built for Visual Studio and .NET 8.

The simulator supports:
- Router-to-router adjacency
- Per-iteration route advertisement and updates
- Simplified BGP best-path selection
- CIDR prefix validation for user inputs
- GraphViz rendering of each iteration (DOT + PNG)
- Example, Custom, and Random topology generation

I specifically built this project as part of my IS 585 Networking course.
I have made it public for others to use and it is ideal for:

✔️ Networking students  
✔️ Routing / BGP learners  
✔️ Algorithm and topology simulation projects  
✔️ Classroom demonstrations  
✔️ Experimenting with routing behaviors  

---

## 🔧 Features

### ✔️ BGP-style simulation
- AS-Path growth
- Loop prevention
- Decision process: LocalPref → AS-Path length → MED → Tiebreak

### ✔️ Three modes of operation
At program startup, choose:

**1️⃣ Example Topology**  
Runs a predefined A–B–C–D network with several prefix origins.

**2️⃣ Custom Topology**  
User defines:
- Number of routers (1–10)
- Adjacency lists (neighbors)
- CIDR prefixes each router originates
- Number of simulation iterations

Input validation ensures:
- Correct format: `x.x.x.x/x`
- Correct IPv4 ranges
- Proper prefix lengths

**3️⃣ Random Topology Generator**  
Automatically builds:
- Random neighbors (40% link probability)
- Random prefix origins (50% chance per router)
- Random /24 networks such as `10.x.y.0/24`

A great way to test routing behavior with unpredictable designs.

---

## 🖼️ Graph Visualization (GraphViz)

For **each iteration**, the system generates:

- A `.dot` file
- A `.png` file (if GraphViz is installed)

Output is saved to:

/graphs/




GraphViz shows:
- Routers (boxes)
- Links (bidirectional dashed edges)
- Prefix origin + propagation
- Route flow via NextHop

---

## 🚀 Running the Simulator

### ### Visual Studio (Windows)
1. Open `BGPSimulator_HMC.sln`
2. Ensure **.NET 8** is installed
3. Set `BGPSimulator_HMC` as the startup project
4. Run (F5)

### Command Line (.NET 8 SDK installed)

dotnet run --project BGPSimulator_HMC/BGPSimulator_HMC.csproj




---

## 🐳 Running with Docker (with GraphViz)

This project includes a **Dockerfile** that supports GraphViz.

### Build

docker build -t bgp-sim .

shell


### Run

Windows (PowerShell):
docker run --rm -it -v "${PWD}/graphs:/app/graphs" bgp-sim

bash


Linux/macOS:
docker run --rm -it -v "$(pwd)/graphs:/app/graphs" bgp-sim




PNG and DOT files will appear on your host.

---

## 📁 Project Structure

BGPSimulator_HMC/
├─ BGPSimulator_HMC.sln
├─ Dockerfile
├─ BGPSimulator_HMC/
│ ├─ Program.cs # User menu + topology creation
│ ├─ BGPSimulatorEngine.cs # Route propagation logic
│ ├─ GraphVizRenderer.cs # DOT + PNG rendering
│ ├─ Models/
│ │ ├─ Router.cs # Router model
│ │ └─ Route.cs # Route model (LocalPref, AS-Path, MED)
│ ├─ BGPSimulator_HMC.csproj
│ └─ README.md
└─ graphs/ # DOT + PNG output




---

## 🔍 How the Simulation Works

Each iteration performs:
1. Every router advertises its current routes to neighbors  
2. Each router receives incoming routes  
3. Routes are evaluated via BGP decision logic  
4. Routing tables are updated  
5. GraphViz output is generated  

This continues for the number of iterations chosen by the user.

---

## 🧪 Example Output (Console)

=========== ITERATION 1 ===========

Router A Routing Table:
10.0.0.0/24 | NextHop=A | LP=100 | MED=0 | ASPath=A

Router B Routing Table:
10.0.0.0/24 | NextHop=A | ASPath=A B




---

## 🛠️ Future Enhancements (optional ideas)

- Support for:
  - Withdrawals
  - Communities
  - LocalPref policies per neighbor
  - Route reflectors / iBGP / eBGP distinction
- Export tables to JSON or CSV
- GUI (WPF or WinForms) - I will post this in a week or two
- Web API for real-time visualization - I will also work on this in the coming weeks

---

## 👨‍💻 Author Notes

This simulator is intentionally simplified to make BGP behaviors easy to visualize and learn.  
It is **not** intended for production routing or full RFC compliance.

---

## ✔️ Requirements

- .NET 8 SDK  
- GraphViz (optional but recommended)

GraphViz download:  
https://graphviz.org/download/

---
