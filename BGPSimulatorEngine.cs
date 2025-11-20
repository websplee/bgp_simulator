using BGPSimulator_HMC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace BGPSimulator_HMC
{
    public class BGPSimulator_HMCEngine
    {
        public List<Router> Routers { get; } = new();

        public void AddRouter(Router r) => Routers.Add(r);

        // Run the simulation for `iterations` discrete rounds.
        // Returns a snapshot of routing tables per iteration (for possible use by renderer).
        public List<Dictionary<string, Dictionary<string, Route>>> RunIterations(int iterations, Action<int, List<Router>>? perIterationHook = null)
        {
            var snapshots = new List<Dictionary<string, Dictionary<string, Route>>>();

            for (int iter = 1; iter <= iterations; iter++)
            {
                Console.WriteLine($"\n=========== ITERATION {iter} ===========");

                // Collect advertisements each router will receive from its neighbors.
                var advertisementsForRouter = new Dictionary<Router, List<Route>>();

                foreach (var r in Routers)
                {
                    var incoming = new List<Route>();

                    foreach (var neigh in r.Neighbors)
                    {
                        // neighbor advertises its routes with neighbor.Id as next-hop
                        incoming.AddRange(neigh.AdvertisedRoutes());
                    }

                    advertisementsForRouter[r] = incoming;
                }

                // Apply advertisements
                foreach (var r in Routers)
                {
                    r.ApplyRoutes(advertisementsForRouter[r]);
                }

                // Output routing tables to console and capture snapshot
                var snapshot = new Dictionary<string, Dictionary<string, Route>>();
                foreach (var r in Routers)
                {
                    Console.WriteLine($"\nRouter {r.Id} Routing Table:");
                    var tableCopy = new Dictionary<string, Route>();
                    foreach (var rt in r.RoutingTable.OrderBy(x => x.Key))
                    {
                        Console.WriteLine("  " + rt.Value);
                        tableCopy[rt.Key] = new Route
                        {
                            Prefix = rt.Value.Prefix,
                            AsPath = new List<string>(rt.Value.AsPath),
                            LocalPref = rt.Value.LocalPref,
                            Med = rt.Value.Med,
                            NextHop = rt.Value.NextHop
                        };
                    }
                    snapshot[r.Id] = tableCopy;
                }

                snapshots.Add(snapshot);

                // Hook for external actions like graph rendering (passes iteration index and routers)
                perIterationHook?.Invoke(iter, Routers);
            }

            return snapshots;
        }
    }
}
