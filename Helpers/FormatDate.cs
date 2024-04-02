using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamerasInfo.Helpers
{
    public static class FormatDate
    {
        public static string ToLocalTime(DateTime pingTime)
        {
            // Get the time zone information for Brazil
            TimeZoneInfo brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            // Convert the DateTime to Brazil local time
            DateTime brazilLocalTime = TimeZoneInfo.ConvertTimeFromUtc(pingTime, brazilTimeZone);
            // Format the local time as needed
            string formattedPingTime = brazilLocalTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return formattedPingTime;
        }
    }
}
