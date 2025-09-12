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
        private readonly IEnumerable<Road> _roads;
        private readonly Dictionary<long, List<Road>> _pathsToHome;
        public BetterTollCalculator(IEnumerable<Road> roads)
        {
            _roads = roads;
            _pathsToHome = ScanForPaths();
        }

        public IEnumerable<long> GetTolls(Delivery delivery)
        {
            return _pathsToHome.GetValueOrDefault(delivery.FromCityId, [])
                    .Where((road) => delivery.LoadWeight >= road.LoadLimit)
                    .Select((road) => road.TollCharge);
        }

        private Dictionary<long, List<Road>> ScanForPaths()
        {
            var scanned = new HashSet<Road>();
            var chains = new List<Chain>();
            var queue = new Queue<Chain>();

            queue.Enqueue(new Chain(1, []));

            while (queue.Count > 0)
            {
                var chain = queue.Dequeue();

                var toScan = _roads
                    .Where((x) => x.FirstCityId == chain.Tail || x.SecondCityId == chain.Tail)
                    .Where((x) => !scanned.Contains(x));

                foreach (var road in toScan)
                {
                    var newChain = new Chain(GetDestination(chain.Tail, road), [..chain.Roads, road]);
                    queue.Enqueue(newChain);
                    scanned.Add(road);
                    chains.Add(newChain);
                }
            }

            return chains.ToDictionary((chain) => chain.Tail, (chain) => chain.Roads);
        }

        private static long GetDestination(long fromCity, Road road)
        {
            return road.FirstCityId == fromCity ? road.SecondCityId : road.FirstCityId;
        }
    }

    internal record Chain(long Tail, List<Road> Roads);
}
