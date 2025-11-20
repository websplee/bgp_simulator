using BGPSimulator_HMC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;

namespace BGPSimulator_HMC
{
    public class GraphVizRenderer
    {
        private readonly string _outDir;

        public GraphVizRenderer(string outDir = "graphs")
        {
            _outDir = outDir;
            Directory.CreateDirectory(_outDir);
        }

        // Produce a DOT file and (optionally) a PNG using 'dot' executable (Graphviz).
        public string RenderIteration(int iteration, IEnumerable<Router> routers)
        {
            string dotPath = Path.Combine(_outDir, $"iter_{iteration}.dot");
            string pngPath = Path.Combine(_outDir, $"iter_{iteration}.png");

            var sb = new StringBuilder();
            sb.AppendLine("digraph G {");
            sb.AppendLine("  graph [rankdir=LR];");
            sb.AppendLine("  node [shape=ellipse, style=filled, fillcolor=lightgray];");

            // create router nodes
            foreach (var r in routers)
            {
                sb.AppendLine($"  \"{r.Id}\" [shape=box, fillcolor=lightblue];");
            }

            // create physical/neighbor links (undirected appearance)
            var seenEdges = new HashSet<string>();
            foreach (var r in routers)
            {
                foreach (var n in r.Neighbors)
                {
                    var key = string.Compare(r.Id, n.Id, StringComparison.Ordinal) < 0
                        ? $"{r.Id}-{n.Id}"
                        : $"{n.Id}-{r.Id}";
                    if (seenEdges.Add(key))
                    {
                        sb.AppendLine($"  \"{r.Id}\" -> \"{n.Id}\" [dir=both, style=dashed, color=gray];");
                    }
                }
            }

            // attach prefixes as rounded nodes and draw who owns/announces them
            foreach (var r in routers)
            {
                foreach (var kv in r.RoutingTable)
                {
                    var prefix = kv.Key;
                    // We'll show prefix node only for routes where this router is the origin (AS_PATH length ==1 and NextHop==router)
                    // But to illustrate learned routes we'll show arrows from next-hop -> router for that prefix
                    string prefixNodeId = $"p_{iteration}_{prefix.Replace("/", "_").Replace(".", "_")}";
                    // create prefix node many times may collide — use unique iteration-specific node id above
                    sb.AppendLine($"  \"{prefixNodeId}\" [label=\"{prefix}\", shape=oval, fillcolor=white];");

                    // arrow from the NextHop router to this router to show where the route came from
                    var nextHop = kv.Value.NextHop;
                    if (!string.IsNullOrEmpty(nextHop))
                    {
                        sb.AppendLine($"  \"{nextHop}\" -> \"{r.Id}\" [label=\"{prefix}\", color=black];");
                        sb.AppendLine($"  \"{prefixNodeId}\" -> \"{r.Id}\" [style=dotted, color=blue];");
                    }
                    else
                    {
                        // if origin (no next-hop), make it attach to r
                        sb.AppendLine($"  \"{prefixNodeId}\" -> \"{r.Id}\" [color=green];");
                    }
                }
            }

            sb.AppendLine("}");

            File.WriteAllText(dotPath, sb.ToString());

            // Try to run dot to render PNG. If dot not found, skip rendering.
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dot",
                    Arguments = $"-Tpng \"{dotPath}\" -o \"{pngPath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (var proc = Process.Start(startInfo))
                {
                    proc?.WaitForExit(3000);
                }
            }
            catch (Exception ex)
            {
                // dot not available or failed — we'll return the dotPath and note failure via console.
                Console.WriteLine($"[GraphVizRenderer] Could not run 'dot' to create PNG: {ex.Message}");
                Console.WriteLine($"[GraphVizRenderer] DOT file written: {dotPath}");
                return dotPath;
            }

            Console.WriteLine($"[GraphVizRenderer] PNG created: {pngPath}");
            return pngPath;
        }
    }
}
