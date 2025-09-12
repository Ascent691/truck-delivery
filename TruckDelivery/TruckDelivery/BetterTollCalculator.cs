using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckDelivery.Infrastructure;

namespace TruckDelivery
{
    public class BetterTollCalculator
    {
        private readonly Dictionary<long, List<Road>> _pathsToHome;
        public BetterTollCalculator(IEnumerable<Road> roads)
        {
            _pathsToHome = ScanForPaths(roads);
        }

        public IEnumerable<long> GetTolls(Delivery delivery)
        {
            return _pathsToHome.GetValueOrDefault(delivery.FromCityId, [])
                    .Where((road) => delivery.LoadWeight >= road.LoadLimit)
                    .Select((road) => road.TollCharge);
        }

        private Dictionary<long, List<Road>> ScanForPaths(IEnumerable<Road> roads)
        {
            var result = new Dictionary<long, List<Road>>();
            var untravelledRoads = new List<Road>(roads);
            var queue = new List<Chain>() { new (1, []) };

            while (queue.Count > 0)
            {
                foreach (var chain in queue.ToArray())
                {
                    var toTravel = untravelledRoads.Where((x) => x.FirstCityId == chain.Tail || x.SecondCityId == chain.Tail).ToArray();

                    foreach (var road in toTravel)
                    {
                        untravelledRoads.Remove(road);
                        var newChain = new Chain(GetDestination(chain.Tail, road), [.. chain.Roads, road]);
                        queue.Add(newChain);
                        result.Add(newChain.Tail, newChain.Roads);
                    }

                    queue.Remove(chain);
                }
            }

            return result;
        }

        private static long GetDestination(long fromCity, Road road)
        {
            return road.FirstCityId == fromCity ? road.SecondCityId : road.FirstCityId;
        }
    }

    internal record Chain(long Tail, List<Road> Roads);
}
