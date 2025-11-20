using System.Collections.Generic;
using System.Linq;

namespace BGPSimulator_HMC.Models
{
    public class Route
    {
        public string Prefix { get; set; } = "";
        public List<string> AsPath { get; set; } = new();
        public int LocalPref { get; set; } = 100;
        public int Med { get; set; } = 0;
        public string NextHop { get; set; } = "";

        public Route CloneWithPrepend(string routerId, string nextHop)
        {
            // Create a new route notifying that routerId is adding itself to the AS_PATH
            var newAsPath = new List<string>();
            newAsPath.Add(routerId);
            newAsPath.AddRange(AsPath);
            return new Route
            {
                Prefix = Prefix,
                LocalPref = LocalPref,
                Med = Med,
                NextHop = nextHop,
                AsPath = newAsPath
            };
        }

        public override string ToString()
        {
            return $"{Prefix} | NextHop={NextHop} | LP={LocalPref} | MED={Med} | ASPath={string.Join(" ", AsPath)}";
        }
    }
}
