using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    [DebuggerDisplay($"{{{nameof(ToDebugString)}(),nq}}")]
    public record Road(long FirstCityId, long SecondCityId, long TollCharge, long LoadLimit, RoadVisited RoadVisited)
    {
        private string ToDebugString()
        {
            return $"Road linking city {FirstCityId} and city {SecondCityId}, toll of {TollCharge} when vehicle load exceeds {LoadLimit}";
        }
    }

    public class RoadVisited
    {
        public bool Visited { get; set; } = false;
    }
}
