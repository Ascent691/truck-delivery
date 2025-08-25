using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery
{
    internal class DFS
    {
        public static List<long>? FindPath(long start, long target, List<(long, long)> cityPairs)
        {
            var adjacencies = BuildAdjacencies(cityPairs);
            var visited = new HashSet<long>();
            var path = new List<long>();

            bool Dfs(long node)
            {
                if (visited.Contains(node)) return false;
                visited.Add(node);
                path.Add(node);

                if (node == target)
                    return true;

                if (adjacencies.TryGetValue(node, out List<long>? neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (Dfs(neighbor)) return true;
                    }
                }

                // Backtrack
                path.RemoveAt(path.Count - 1);
                return false;
            }

            return Dfs(start) ? path : null;
        }

        private static Dictionary<long, List<long>> BuildAdjacencies(List<(long, long)> pairs)
        {
            var adj = new Dictionary<long, List<long>>();
            foreach (var (a, b) in pairs)
            {
                if (!adj.ContainsKey(a)) adj[a] = [];
                if (!adj.ContainsKey(b)) adj[b] = [];
                adj[a].Add(b);
                adj[b].Add(a);
            }
            return adj;
        }
    }
}
