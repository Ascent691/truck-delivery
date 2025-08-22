namespace TruckDelivery.Infrastructure
{
    public class TollCalculator(IEnumerable<Delivery> deliveries, IEnumerable<Road> roads)
    {
        private readonly Dictionary<long, IEnumerable<Road>> _knownPaths = [];

        public IEnumerable<IEnumerable<long>> GetTollsToHomeCity()
        {
            return deliveries.Select((delivery) => 
            {
                return GetPathFromCity(delivery.FromCityId, 1, [])
                    .Where((road) => delivery.LoadWeight >= road.LoadLimit)
                    .Select((road) => road.TollCharge);
            });
        }

        private IEnumerable<Road> GetPathFromCity(long fromCity, long toCity, IEnumerable<Road> travelledRoads)
        {
            if (_knownPaths.TryGetValue(fromCity, out var knownPath)) return knownPath;
            var connectedRoads = roads.Where((road) => road.FirstCityId == fromCity || road.SecondCityId == fromCity);
            var desitinationRoad = connectedRoads.FirstOrDefault((road) => (road.FirstCityId == fromCity && road.SecondCityId == toCity) || (road.FirstCityId == toCity && road.SecondCityId == fromCity));
            if (desitinationRoad != null) return [desitinationRoad];

            var untravelledRoads = connectedRoads.Where((road) => !travelledRoads.Contains(road));
            return untravelledRoads.Select((road) => {
                var path = GetPathFromCity(GetDestination(fromCity, road), toCity, [..travelledRoads, road]);
                return path.Any() ?[..path, road]: path;
            }).Select((path) =>
            {
                if (path.Any()) _knownPaths.TryAdd(fromCity, path);
                return path;
            }).First((path) => path.Any());
        }

        private long GetDestination(long fromCity, Road road)
        {
            return fromCity == road.FirstCityId ? road.SecondCityId : road.FirstCityId;
        }
    }
}
