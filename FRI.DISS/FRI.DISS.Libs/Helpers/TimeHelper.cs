using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRI.DISS.Libs.Helpers
{
    public class TimeHelper
    {
        public static int HoursToSeconds(int hours)
        {
            return hours * 3600;
        }
        /// <summary>
        /// hours to seconds
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int H2S(int hours)
        {
            return HoursToSeconds(hours);
        }

        public static int MinutesToSeconds(int minutes)
        {
            return minutes * 60;
        }
        /// <summary>
        /// minutes to seconds
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int M2S(int minutes)
        {
            return MinutesToSeconds(minutes);
        }
    }
}
