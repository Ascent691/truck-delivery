
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery
{
    internal class DFS
    {
        // GOALS: iterative DFS as opposed to recursive
        public static List<long> IterativeDFS(long start, long target, List<(long, long)> cityPairs)
        {
            var path = new List<long>();
            var adjacencies = BuildAdjacencies(cityPairs);

            var stack = new Stack<long>();
            var visited = new HashSet<long>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                var city = stack.Pop();
                if (visited.Contains(city)) continue;

                path.Add(city);
                visited.Add(city);

                var reversedAdjacencies = new List<long>(adjacencies.GetValueOrDefault(city, []));
                reversedAdjacencies.Reverse();
                foreach(var neighbour in reversedAdjacencies)
                {
                    stack.Push(neighbour); // for order consistency
                }
            }

            return path ?? [];
        }

        public static List<long>? FindPath(long start, long target, List<(long, long)> cityPairs)
        {
            var adjacencies = BuildAdjacencies(cityPairs);
            var visited = new HashSet<long>();
            var path = new List<long>();

            bool Dfs(long city)
            {
                if (visited.Contains(city)) return false;
                visited.Add(city);
                path.Add(city);

                if (city == target)
                    return true;

                if (adjacencies.TryGetValue(city, out List<long>? neighbors))
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
