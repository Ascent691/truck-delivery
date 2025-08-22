using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    public class TollCalculator(Delivery delivery, IEnumerable<Road> roads)
    {
        public IEnumerable<long> CalculateTolls()
        {
            var path = new PathFinder(delivery, roads).GetPathToHomeCity();

            return path.Select((road) => delivery.LoadWeight >= road.LoadLimit ? road.TollCharge : 0)
                .Where((toll) => toll != 0);
        }
    }

    internal class PathFinder(Delivery delivery, IEnumerable<Road> roads)
    {
        public IEnumerable<Road> GetPathToHomeCity()
        {
            return GetPossiblePaths(delivery.FromCityId, 1, []).First();
        }

        private IEnumerable<IEnumerable<Road>> GetPossiblePaths(long fromCity, long toCity, IEnumerable<Road> travelledRoads)
        {
            if (fromCity == toCity) return [travelledRoads];

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
