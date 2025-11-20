using BGPSimulator_HMC.Models;
using System;
using System.Diagnostics.Metrics;

namespace BGPSimulator_HMC
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build routers
            var A = new Router("A");
            var B = new Router("B");
            var C = new Router("C");
            var D = new Router("D");

            // Build links
            A.AddNeighbor(B);
            B.AddNeighbor(C);
            C.AddNeighbor(D);
            B.AddNeighbor(D); // extra link making a triangle between B-C-D

            // Initially, A originates prefix 10.0.0.0/24, C originates 20.0.0.0/24, D originates 30.0.0.0/24
            A.RoutingTable["10.0.0.0/24"] = new Route { Prefix = "10.0.0.0/24", NextHop = "A", AsPath = new System.Collections.Generic.List<string> { "A" } };
            C.RoutingTable["20.0.0.0/24"] = new Route { Prefix = "20.0.0.0/24", NextHop = "C", AsPath = new System.Collections.Generic.List<string> { "C" } };
            D.RoutingTable["30.0.0.0/24"] = new Route { Prefix = "30.0.0.0/24", NextHop = "D", AsPath = new System.Collections.Generic.List<string> { "D" } };

            // Make B prefer routes from D via LocalPref trick (optional demonstration)
            // Example: we will simulate D setting higher localpref when advertising to B — left as extension

            var sim = new BGPSimulator_HMCEngine();
            sim.AddRouter(A);
            sim.AddRouter(B);
            sim.AddRouter(C);
            sim.AddRouter(D);

            var renderer = new GraphVizRenderer("graphs");

            // run 5 iterations and render graph each iteration
            sim.RunIterations(5, (iter, routers) =>
            {
                Console.WriteLine($"[Main] Rendering graph for iteration {iter}...");
                var path = renderer.RenderIteration(iter, routers);
                Console.WriteLine($"[Main] Graph file: {path}");
            });

            Console.WriteLine("\nSimulation complete. See 'graphs' directory for DOT/PNG files.");
        }
    }
}
