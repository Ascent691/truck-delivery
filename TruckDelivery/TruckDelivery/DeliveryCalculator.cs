using TruckDelivery.Infrastructure;

namespace TruckDelivery;

public class DeliveryCalculator
{
    public static ScenarioAnswer Calculate(Scenario scenario)
    {
        var adjList = new RoadAdjacencyList(scenario.Roads);

        var answers = new long[scenario.Deliveries.Length];
        var currAns = 0;

        var pathMap = new Dictionary<long, Road[]>();
        var visited = new bool[scenario.Roads.Length + 2];

        FindPathsRecursive(1, [], visited);

        void FindPathsRecursive(long node, List<Road> currPath, bool[] visited)
        {
            visited[node] = true;
            pathMap[node] = [.. currPath];

            foreach (var (dest, road) in adjList.GetConnectedNodes(node))
            {
                if (visited[dest]) continue;

                currPath.Add(road);

                FindPathsRecursive(dest, currPath, visited);
            }

            if (currPath.Count > 0)
                currPath.RemoveAt(currPath.Count - 1);
        }

        foreach (var (from, to, weight) in scenario.Deliveries)
        {
            long divisor = 0;
            foreach (var r in pathMap[from])
            {
                var toll = weight >= r.LoadLimit ? r.TollCharge : 0;
                divisor = MathHelper.GreatestCommonDivisor(divisor, toll);
            }
            answers[currAns++] = divisor;
        }

        return new ScenarioAnswer(answers);
    }
}