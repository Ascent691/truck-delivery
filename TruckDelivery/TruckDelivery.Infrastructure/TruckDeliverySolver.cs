namespace TruckDelivery.Infrastructure;

public class TruckDeliverySolver
{
    public static ScenarioAnswer DetermineAnswer(Scenario scenario)
    {
        var results = new List<long>();
        var tree = BuildRoadTree(scenario.Roads);

        foreach (var delivery in scenario.Deliveries)
        {
            var tolls = FindTollsOnPath(tree, delivery);

            if (tolls.Count == 0)
            {
                results.Add(0);
            }
            else
            {
                long gcd = 0; 
                foreach (var toll in tolls)
                {
                    gcd = MathHelper.GreatestCommonDivisor(gcd, toll);
                }
                results.Add(gcd);
            }
        }

        return new ScenarioAnswer(results.ToArray());
    }

    private static Dictionary<long, List<(long Target, long TollCharge, long LoadLimit)>> BuildRoadTree(Road[] roads)
    {
        var tree = new Dictionary<long, List<(long, long, long)>>();

        foreach (var road in roads)
        {
            AddRoad(tree, road.FirstCityId, road.SecondCityId, road.TollCharge, road.LoadLimit);
            AddRoad(tree, road.SecondCityId, road.FirstCityId, road.TollCharge, road.LoadLimit);
        }

        return tree;
    }

    private static void AddRoad(Dictionary<long, List<(long, long, long)>> tree, long fromCity, long toCity, long toll, long loadLimit)
    {
        if (!tree.ContainsKey(fromCity))
        {
            tree[fromCity] = new List<(long, long, long)>();
        }
        tree[fromCity].Add((toCity, toll, loadLimit));
    }

    private static List<long> FindTollsOnPath(Dictionary<long, List<(long TargetCityId, long TollCharge, long LoadLimit)>> tree, Delivery delivery)
    {
        var cityConnections = new Dictionary<long, (long ParentCityId, long TollCharge, long LoadLimit)>();
        var visitedCities = new HashSet<long> { delivery.FromCityId };
        var citiesToExplore = new List<long> { delivery.FromCityId };

        for (int i = 0; i < citiesToExplore.Count; i++)
        {
            var currentCityId = citiesToExplore[i];

            if (currentCityId == delivery.ToCityId)
                break;

            foreach (var road in tree[currentCityId])
            {
                if (!visitedCities.Contains(road.TargetCityId))
                {
                    visitedCities.Add(road.TargetCityId);
                    cityConnections[road.TargetCityId] = (currentCityId, road.TollCharge, road.LoadLimit);
                    citiesToExplore.Add(road.TargetCityId);
                }
            }
        }

        var tollChargesOnPath = new List<long>();
        var cityIdOnPath = delivery.ToCityId;

        while (cityConnections.ContainsKey(cityIdOnPath))
        {
            var roadInfo = cityConnections[cityIdOnPath];
            if (delivery.LoadWeight >= roadInfo.LoadLimit)
                tollChargesOnPath.Add(roadInfo.TollCharge);

            cityIdOnPath = roadInfo.ParentCityId;
        }

        return tollChargesOnPath;
    }


}