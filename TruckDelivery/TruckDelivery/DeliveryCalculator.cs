using TruckDelivery.Infrastructure;

namespace TruckDelivery;

public class DeliveryCalculator
{
    public static ScenarioAnswer Calculate(Scenario scenario)
    {
        var deliveries = scenario.Deliveries;
        var map = CreateMap(scenario.Roads);

        var answers = new long[scenario.Deliveries.Length];
        var currAns = 0L;

        foreach (var (from, to, weight) in deliveries)
        {
            var path = new List<long>();
            CheckConnected(from, to, path, []);

            var tolls = new long[path.Count];
            for (var i = 0; i < path.Count - 1; i++)
            {
                var road = map.At(path[i], path[i + 1]);
                tolls[i] = weight >= road.LoadLimit ? road.TollCharge : 0;
            }

            var ans = MathHelper.GreatestCommonDivisor(tolls);
            answers[currAns++] = ans;
        }

        return new ScenarioAnswer(answers);

        bool CheckConnected(long node, long target, List<long> path, HashSet<long> visited)
        {
            visited.Add(node);
            path.Add(node);

            if (node == target) return true;

            foreach (var dest in map.GetConnected(node))
            {
                if (visited.Contains(dest))
                    continue;

                if (CheckConnected(dest, target, path, visited))
                    return true;
            }

            path.RemoveAt(path.Count - 1);
            return false;
        }
    }

    private static RoadMatrix CreateMap(Road[] roads)
    {
        var map = new RoadMatrix(roads.Length + 2);

        foreach (var road in roads)
        {
            map.AddRoad(road.FirstCityId, road.SecondCityId, road);
        }

        return map;
    }
}