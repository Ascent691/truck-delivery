using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    public class TollCalculator(IEnumerable<Delivery> deliveries, IEnumerable<Road> roads)
    {
        private readonly Dictionary<long, IEnumerable<Road>> _knownPaths = [];

        public IEnumerable<IEnumerable<long>> GetTollsToHomeCity()
        {
            return deliveries.Select((delivery) => 
            {
                return GetPossiblePaths(delivery.FromCityId, 1, [])
                    .First()
                    .Select((road) => delivery.LoadWeight >= road.LoadLimit ? road.TollCharge : 0)
                    .Where((toll) => toll != 0);
                }
            );
        }

        private IEnumerable<IEnumerable<Road>> GetPossiblePaths(long fromCity, long toCity, IEnumerable<Road> travelledRoads)
        {
            if (_knownPaths.TryGetValue(fromCity, out var path)) return [path];

            if (fromCity == toCity) {
                return [travelledRoads];
            };

            var connectedRoads = roads.Where((road) => road.FirstCityId == fromCity || road.SecondCityId == fromCity);
            var untravelledRoads = connectedRoads.Where((road) => !travelledRoads.Contains(road));

            if (!untravelledRoads.Any()) return [];
            
            return untravelledRoads.SelectMany((road) => GetPossiblePaths(GetDestination(fromCity, road), toCity, [..travelledRoads, road]))
                .Where((paths) => paths.Any());
        }

        private long GetDestination(long fromCity, Road road)
        {
            return fromCity == road.FirstCityId ? road.SecondCityId : road.FirstCityId;
        }
    }
}
