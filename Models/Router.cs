using System;
using System.Collections.Generic;
using System.Linq;

namespace BGPSimulator_HMC.Models
{
    public class Router
    {
        public string Id { get; }
        public List<Router> Neighbors { get; } = new();
        public Dictionary<string, Route> RoutingTable { get; } = new();

        public Router(string id) => Id = id;

        public void AddNeighbor(Router r)
        {
            if (!Neighbors.Contains(r))
                Neighbors.Add(r);
            if (!r.Neighbors.Contains(this))
                r.Neighbors.Add(this);
        }

        // Routes this router will advertise to neighbors (using its own router id as next-hop)
        public IEnumerable<Route> AdvertisedRoutes()
        {
            foreach (var route in RoutingTable.Values)
                yield return route.CloneWithPrepend(Id, Id);
        }

        // Apply routes learned from neighbors (receivedRoutes should be the advertisements from neighbors)
        public void ApplyRoutes(IEnumerable<Route> receivedRoutes)
        {
            foreach (var route in receivedRoutes)
            {
                // loop prevention: if our id already in AS_PATH skip
                if (route.AsPath.Contains(Id))
                    continue;

                if (!RoutingTable.TryGetValue(route.Prefix, out var current) ||
                    IsBetter(route, current))
                {
                    // store a copy for our table
                    RoutingTable[route.Prefix] = new Route
                    {
                        Prefix = route.Prefix,
                        AsPath = new List<string>(route.AsPath),
                        LocalPref = route.LocalPref,
                        Med = route.Med,
                        NextHop = route.NextHop
                    };
                }
            }
        }

        private bool IsBetter(Route newR, Route oldR)
        {
            // Simplified BGP decision: LocalPref > Shorter AS_PATH > Lower MED > NextHop string tie-break
            if (newR.LocalPref != oldR.LocalPref)
                return newR.LocalPref > oldR.LocalPref;

            if (newR.AsPath.Count != oldR.AsPath.Count)
                return newR.AsPath.Count < oldR.AsPath.Count;

            if (newR.Med != oldR.Med)
                return newR.Med < oldR.Med;

            return string.Compare(newR.NextHop, oldR.NextHop, StringComparison.Ordinal) < 0;
        }
    }
}
