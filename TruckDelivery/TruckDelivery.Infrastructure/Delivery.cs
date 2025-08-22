using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    [DebuggerDisplay($"{{{nameof(ToDebugString)}(),nq}}")]
    public record Delivery(int FromCityId, int ToCityId, int LoadWeight)
    {
        private string ToDebugString()
        {
            return $"Delivery from {FromCityId} to {ToCityId} with a load weighing {LoadWeight}";
        }
    }
}
