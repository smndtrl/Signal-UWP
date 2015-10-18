using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Util
{
    public class TimeUtil
    {
        /*public static long GetTimeInMilliseconds()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }*/

        public static long GetUnixTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static long GetUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        public static long GetUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long GetUnixTimestampMillis(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DateTime GetDateTimeMillis()
        {
            return GetDateTime(GetUnixTimestampMillis());
        }

        public static DateTime GetDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            if (unixTimeStamp > GetUnixTimestamp()*10)
            {
                dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            } else
            {
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            }
            
            return dtDateTime;
        }

        /*public static DateTime GetDateTimeMili(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }*/
    }
}
