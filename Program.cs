using BGPSimulator_HMC.Models;
using System;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;

namespace BGPSimulator_HMC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== BGP Routing Simulator ===");
            Console.WriteLine("✔️ 1. Use Example Topology");
            Console.WriteLine("✔️ 2. Create Custom Topology");
            Console.WriteLine("✔️ 3. Auto-Generate Random Topology");
            Console.Write("Choose an option (1–3): ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    RunExampleTopology();
                    break;
                case "2":
                    RunCustomTopology();
                    break;
                case "3":
                    RunRandomTopology();
                    break;
                default:
                    Console.WriteLine("❌ Invalid option. Exiting.");
                    return;
            }
        }

        // ==============================================================
        // OPTION 1 — Example Topology
        // ==============================================================

        static void RunExampleTopology()
        {
            Console.WriteLine("\nRunning example topology...");

            var A = new Router("A");
            var B = new Router("B");
            var C = new Router("C");
            var D = new Router("D");

            A.AddNeighbor(B);
            B.AddNeighbor(C);
            C.AddNeighbor(D);
            B.AddNeighbor(D);

            A.RoutingTable["10.0.0.0/24"] = new Route { Prefix = "10.0.0.0/24", NextHop = "A", AsPath = new List<string> { "A" } };
            C.RoutingTable["20.0.0.0/24"] = new Route { Prefix = "20.0.0.0/24", NextHop = "C", AsPath = new List<string> { "C" } };
            D.RoutingTable["30.0.0.0/24"] = new Route { Prefix = "30.0.0.0/24", NextHop = "D", AsPath = new List<string> { "D" } };

            RunSimulation(new List<Router> { A, B, C, D });
        }

        // ==============================================================
        // OPTION 2 — Custom Topology (with prefix validation!)
        // ==============================================================

        static void RunCustomTopology()
        {
            Console.Write("\nEnter number of routers (0–10): ");

            if (!int.TryParse(Console.ReadLine()?.Trim(), out int n) || n < 1 || n > 10)
            {
                Console.WriteLine("❌ Invalid number. Exiting.");
                return;
            }

            var routers = new List<Router>();
            for (int i = 0; i < n; i++)
                routers.Add(new Router(((char)('A' + i)).ToString()));

            Console.WriteLine("\n--- Configure Router Neighbors ---");
            Console.WriteLine("Enter neighbors as comma-separated letters (example: B,C)");

            foreach (var r in routers)
            {
                Console.Write($"Neighbors for router {r.Id}: ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input))
                    continue;

                foreach (var token in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var id = token.Trim().ToUpper();
                    var neighbor = routers.Find(x => x.Id == id);

                    if (neighbor != null && neighbor != r)
                        r.AddNeighbor(neighbor);
                }
            }

            Console.WriteLine("\n--- Assign Origin Prefixes (CIDR Validated) ---");

            foreach (var r in routers)
            {
                Console.Write($"Prefix originated by {r.Id}: ");
                string prefix = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(prefix))
                    continue;

                if (!IsValidCIDR(prefix))
                {
                    Console.WriteLine("❌ Invalid CIDR format. Skipping prefix.");
                    continue;
                }

                r.RoutingTable[prefix] = new Route
                {
                    Prefix = prefix,
                    NextHop = r.Id,
                    AsPath = new List<string> { r.Id }
                };
            }

            Console.Write("\nEnter number of simulation iterations: ");
            int iterations = int.TryParse(Console.ReadLine(), out int iters) ? iters : 5;

            RunSimulation(routers, iterations);
        }

        // ==============================================================
        // OPTION 3 — Auto-Generated Random Topology
        // ==============================================================

        static void RunRandomTopology()
        {
            var rand = new Random();

            Console.Write("\nEnter number of routers (1–10): ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n < 1 || n > 10)
            {
                Console.WriteLine("❌ Invalid number. Exiting.");
                return;
            }

            var routers = new List<Router>();
            for (int i = 0; i < n; i++)
                routers.Add(new Router(((char)('A' + i)).ToString()));

            Console.WriteLine("\n✔️ Generating random neighbors...");
            foreach (var r in routers)
            {
                foreach (var other in routers)
                {
                    if (other == r) continue;
                    if (rand.NextDouble() < 0.4) // 40% chance of a link
                        r.AddNeighbor(other);
                }
            }

            Console.WriteLine("✔️ Assigning random prefixes...");
            foreach (var r in routers)
            {
                if (rand.NextDouble() < 0.5) // 50% chance router originates a prefix
                {
                    string prefix = $"10.{rand.Next(0, 256)}.{rand.Next(0, 256)}.0/24";
                    r.RoutingTable[prefix] = new Route
                    {
                        Prefix = prefix,
                        NextHop = r.Id,
                        AsPath = new List<string> { r.Id }
                    };
                }
            }

            Console.Write("\nEnter number of simulation iterations: ");
            int iterations = int.TryParse(Console.ReadLine(), out int iters) ? iters : 5;

            RunSimulation(routers, iterations);
        }

        // ==============================================================
        // Simulation Wrapper
        // ==============================================================

        static void RunSimulation(List<Router> routers, int iterations = 5)
        {
            var sim = new BGPSimulatorEngine();
            var renderer = new GraphVizRenderer("graphs");

            foreach (var r in routers)
                sim.AddRouter(r);

            sim.RunIterations(iterations, (i, rs) =>
            {
                renderer.RenderIteration(i, rs);
            });

            Console.WriteLine("\n✔️ Simulation complete!");
            Console.WriteLine("✔️ Routing tables displayed above.");
            Console.WriteLine("✔️ GraphViz DOT/PNG files saved to 'graphs/'");
        }

        // ==============================================================
        // CIDR VALIDATION (IPv4/CIDR)
        // ==============================================================

        static bool IsValidCIDR(string input)
        {
            var match = Regex.Match(input, @"^(\d{1,3}\.){3}\d{1,3}\/\d{1,2}$");
            if (!match.Success) return false;

            var parts = input.Split('/');
            string ip = parts[0];
            int prefix = int.Parse(parts[1]);

            if (prefix < 0 || prefix > 32)
                return false;

            foreach (var segment in ip.Split('.'))
            {
                if (!int.TryParse(segment, out int num) || num < 0 || num > 255)
                    return false;
            }

            return true;
        }
    }
}
