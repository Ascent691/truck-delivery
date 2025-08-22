using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    [DebuggerDisplay($"{{{nameof(ToDebugString)}(),nq}}")]
    public record Delivery(long FromCityId, long ToCityId, long LoadWeight)
    {
        private string ToDebugString()
        {
            return $"Delivery from city {FromCityId} to city {ToCityId} with a load weighing {LoadWeight}";
        }
    }
}
