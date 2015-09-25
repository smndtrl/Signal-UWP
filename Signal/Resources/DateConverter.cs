using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Signal.Resources
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timestamp = (DateTime)value;

            var diffSec = (DateTime.Now - timestamp).TotalSeconds;
            if (diffSec < 120)
            {
                return "now";
            } else if (diffSec < 3600)
            {
                return (int)(DateTime.Now - timestamp).TotalMinutes + " minutes ago";
            } else if (diffSec < 3600*24)
            {
                return (int)(DateTime.Now - timestamp).TotalHours + " hours ago";
            }
            else if (diffSec < 3600 * 24 * 3)
            {
                return String.Format("{0:t}", timestamp);
            }
            else
            {
                return String.Format("{0:g}", timestamp);
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
