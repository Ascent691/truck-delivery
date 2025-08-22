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

        //new
        var tollDict = new Dictionary<long, Road[]>();
        FindTollsRecursive(1, [], []);

        void FindTollsRecursive(long node, List<Road> currTolls, HashSet<long> visited)
        {
            visited.Add(node);

            tollDict[node] = [..currTolls];

            foreach (var dest in map.GetConnected(node))
            {
                if (visited.Contains(dest))
                {
                    continue;
                }

                currTolls.Add(map.At(node, dest));

                FindTollsRecursive(dest, currTolls, visited);
            }

            if (currTolls.Count > 0)
                currTolls.RemoveAt(currTolls.Count - 1);
        }

        foreach (var (from, to, weight) in deliveries)
        {
            var tolls = tollDict[from].Select(r => weight >= r.LoadLimit ? r.TollCharge : 0);

            var ans = MathHelper.GreatestCommonDivisor(tolls.ToArray());
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