using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public static class TimeFrame
    {
        public static string CandlePeriod(int sec)
        {
            string period = "1M";
            if (sec <= 60) period = "1m";
            else if (sec <= 300) period = "5m";
            else if (sec <= 900) period = "15m";
            else if (sec <= 1800) period = "30m";
            else if (sec <= 3600) period = "1h";
            else if (sec <= 10800) period = "3h";
            else if (sec <= 21600) period = "6h";
            else if (sec <= 43200) period = "12h";
            else if (sec <= 86400) period = "1D";
            else if (sec <= 604800) period = "7D";
            else if (sec <= 1209600) period = "14D";
            return period;
        }
    }
}
