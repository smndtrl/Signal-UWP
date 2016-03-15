using Signal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Signal.Xaml.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timestamp = (DateTime)value;

            return getBriefRelativeTimeSpanString(timestamp);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static bool isWithin(DateTime date, TimeSpan span)
        {
            return TimeUtil.GetDateTimeMillis().Subtract(date) <= span;
        }

        private static TimeSpan convertDelta(DateTime date)
        {
            return TimeUtil.GetDateTimeMillis().Subtract(date);
        }

        /*private static String getFormattedDateTime(DateTime date, String template, Locale locale)
        {
            return date.ToString(template);
        }*/

        private static String getFormattedDateTime(DateTime date, String template)
        {
            return date.ToString(template);
        }

        public static String getBriefRelativeTimeSpanString(DateTime time)
        {
            if (isWithin(time, TimeSpan.FromMinutes(1)))
            {
                return "now";
            }
            else if (isWithin(time, TimeSpan.FromHours(1)))
            {
                var span = convertDelta(time);
                return $"{time.Minute} minutes ago";
            }
            else if (isWithin(time, TimeSpan.FromDays(1)))
            {
                var span = convertDelta(time);
                return span.Hours == 1 ? $"1 hour ago" : $"{span.Hours} hours ago";
            }
            else if (isWithin(time, TimeSpan.FromDays(6)))
            {
                return getFormattedDateTime(time, "dddd"/*, locale*/);
            }
            else if (isWithin(time, TimeSpan.FromDays(365)))
            {
                return getFormattedDateTime(time, "MMM d"/*, locale*/);
            }
            else
            {
                return getFormattedDateTime(time, "MMM d, yyyy"/*, locale*/);
            }
        }

       /* public static String getExtendedRelativeTimeSpanString(final Context c, final Locale locale, final long timestamp)
        {
            if (isWithin(timestamp, 1, TimeUnit.MINUTES))
            {
                return c.getString(R.string.DateUtils_now);
            }
            else if (isWithin(timestamp, 1, TimeUnit.HOURS))
            {
                int mins = (int)TimeUnit.MINUTES.convert(System.currentTimeMillis() - timestamp, TimeUnit.MILLISECONDS);
                return c.getResources().getString(R.string.DateUtils_minutes_ago, mins);
            }
            else
            {
                StringBuilder format = new StringBuilder();
                if (isWithin(timestamp, 6, TimeUnit.DAYS)) format.append("EEE ");
                else if (isWithin(timestamp, 365, TimeUnit.DAYS)) format.append("MMM d, ");
                else format.append("MMM d, yyyy, ");

                if (DateFormat.is24HourFormat(c)) format.append("HH:mm");
                else format.append("hh:mm a");

                return getFormattedDateTime(timestamp, format.toString(), locale);
            }
        }

        public static SimpleDateFormat getDetailedDateFormatter(Context context, Locale locale)
        {
            String dateFormatPattern;

            if (DateFormat.is24HourFormat(context))
            {
                dateFormatPattern = "MMM d, yyyy HH:mm:ss zzz";
            }
            else
            {
                dateFormatPattern = "MMM d, yyyy hh:mm:ss a zzz";
            }

            return new SimpleDateFormat(dateFormatPattern, locale);
        }*/
    }
}
