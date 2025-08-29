using TruckDelivery.Infrastructure;

namespace TruckDelivery;

public class DeliveryCalculator
{
    public static ScenarioAnswer Calculate(Scenario scenario)
    {
        var adjList = new RoadAdjacencyList(scenario.Roads);

        var answers = new long[scenario.Deliveries.Length];

        var pathMap = new Dictionary<long, List<Road>>();
        var visited = new bool[scenario.Roads.Length + 2];

        FindPathsRecursive(1, []);

        void FindPathsRecursive(long node, List<Road> currPath)
        {
            visited[node] = true;
            pathMap[node] = [..currPath];

            foreach (var (dest, road) in adjList.GetConnectedNodes(node))
            {
                if (visited[dest]) continue;

                currPath.Add(road);

                FindPathsRecursive(dest, currPath);
            }

            if (currPath.Count > 0)
                currPath.RemoveAt(currPath.Count - 1);
        }

        Parallel.ForEach(scenario.Deliveries, (delivery, _, i) =>
        {
            long divisor = 0;
            foreach (var r in pathMap[delivery.FromCityId])
            {
                var toll = delivery.LoadWeight >= r.LoadLimit ? r.TollCharge : 0;
                divisor = MathHelper.GreatestCommonDivisor(divisor, toll);
            }
            answers[i] = divisor;
        });

        return new ScenarioAnswer(answers);
    }
}