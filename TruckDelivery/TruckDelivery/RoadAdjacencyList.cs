using TruckDelivery.Infrastructure;

namespace TruckDelivery;

public class RoadAdjacencyList
{
    private readonly Dictionary<long, List<Road>> _adjacencyList = [];

    public RoadAdjacencyList(Road[] roads)
    {
        foreach (var road in roads)
        {
            AddRoad(road);
        }
    }

    public void AddRoad(Road road)
    {
        if (!_adjacencyList.TryGetValue(road.FirstCityId, out var list))
        {
            list = _adjacencyList[road.FirstCityId] = [];
        }
        list.Add(road);

        if (!_adjacencyList.TryGetValue(road.SecondCityId, out list))
        {
            list = _adjacencyList[road.SecondCityId] = [];
        }
        list.Add(road);
    }

    public IEnumerable<(long dest, Road road)> GetConnectedNodes(long node)
    {
        if (_adjacencyList.TryGetValue(node, out var list))
        {
            foreach (var r in list)
            {
                yield return (node == r.FirstCityId ? r.SecondCityId : r.FirstCityId, r);
            }
        }
    }
}