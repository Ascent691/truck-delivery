using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery
{
    public static class Extensions
    {
        public static T[] Clone<T>(this T[] array) where T : ICloneable<T>
        {
            var newArray = new T[array.Length];
            for (var i = 0; i < array.Length; i++)
                newArray[i] = array[i].Clone();
            return newArray;
        }
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> items) where T : ICloneable<T>
        {
            foreach (var item in items)
                yield return item.Clone();
        }
    }
}
