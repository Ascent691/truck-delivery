namespace TruckDelivery.Infrastructure
{
    public class TollCalculator(IEnumerable<Delivery> deliveries, IEnumerable<Road> roads)
    {
        private readonly Dictionary<long, IEnumerable<Road>> _knownPaths = [];

        public IEnumerable<IEnumerable<long>> GetTollsToHomeCity()
        {
            return deliveries.Select((delivery) => 
            {
                var path = GetPossiblePaths(delivery.FromCityId, 1, []).First();
                _knownPaths.TryAdd(delivery.FromCityId, path);

                return path.Where((road) => delivery.LoadWeight >= road.LoadLimit)
                    .Select((road) => road.TollCharge);
            });
        }

        private IEnumerable<IEnumerable<Road>> GetPossiblePaths(long fromCity, long toCity, IEnumerable<Road> travelledRoads)
        {
            if (_knownPaths.TryGetValue(fromCity, out var path)) return [travelledRoads.Concat(path)];

            if (fromCity == toCity) return [travelledRoads];

            var connectedRoads = roads.Where((road) => road.FirstCityId == fromCity || road.SecondCityId == fromCity);
            var untravelledRoads = connectedRoads.Where((road) => !travelledRoads.Contains(road));
            
            return untravelledRoads.SelectMany((road) => GetPossiblePaths(GetDestination(fromCity, road), toCity, [..travelledRoads, road]))
                .Where((paths) => paths.Any());
        }

        private long GetDestination(long fromCity, Road road)
        {
            return fromCity == road.FirstCityId ? road.SecondCityId : road.FirstCityId;
        }
    }
}
