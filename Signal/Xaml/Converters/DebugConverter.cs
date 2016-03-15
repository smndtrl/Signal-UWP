using Signal.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Signal.Xaml.Converters
{
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var t = value;

            return t;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var t = value;

            return t;
        }
    }
}
