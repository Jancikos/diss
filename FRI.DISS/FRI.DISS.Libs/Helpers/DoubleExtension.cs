using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Helpers
{
    public static class DoubleExtension
    {
        /// <summary>
        /// -1 - this is smaller
        ///  0 - this is equal
        /// +1 - this is bigger 
        /// </summary>
        /// <param name="me"></param>
        /// <param name="other"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static int CompareToWithE(this double me, double other, double epsilon = 0.000001)
        {
            double diff = me - other;
            if (Math.Abs(diff) < epsilon)
            {
                return 0;
            }

            return diff > 0 ? 1 : -1;
        }

        public static bool EqualsToWithE(this double me, double other, double epsilon = 0.000001)
        {
            return CompareToWithE(me, other, epsilon) == 0;
        }
    }
}
