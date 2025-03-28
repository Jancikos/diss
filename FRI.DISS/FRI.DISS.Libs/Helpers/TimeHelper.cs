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
            return hours * 60 * 60;
        }

        public static int MinutesToSeconds(int minutes)
        {
            return minutes * 60;
        }
    }
}
