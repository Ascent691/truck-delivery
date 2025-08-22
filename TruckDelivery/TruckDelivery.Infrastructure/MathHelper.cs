using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckDelivery.Infrastructure
{
    public static class MathHelper
    {
        public static long GreatestCommonDivisor(params long[] values)
        {
            if (values == null) return 0;
            long value = values.Length;

            if (value < 3)
            {
                if (value == 2) return GreatestCommonDivisor(values[0], values[1]);
                else if (value == 1) return values[0];
                return 0;
            }

            value = GreatestCommonDivisor(values[0], values[1]);
            for (int i = 2; i < values.Length; i++) value = GreatestCommonDivisor(values[i], value);
            return value;
        }

        public static long GreatestCommonDivisor(long u, long v)
        {
            if (u == 0 || v == 0) return u | v;

            int shift;

            for (shift = 0; ((u | v) & 1) == 0; ++shift)
            {
                u >>= 1;
                v >>= 1;
            }

            while ((u & 1) == 0) u >>= 1;

            do
            {
                while ((v & 1) == 0) v >>= 1;

                if (u < v) v -= u;
                else
                {
                    long diff = u - v;
                    u = v;
                    v = diff;
                }

                v >>= 1;
            }
            while (v != 0);

            return u << shift;
        }
    }
}
