using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Signal.Xaml.Converters
{
    public class RadioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine("Convert stat");
            var v = (string) value;
            //

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Debug.WriteLine("ConvertBack start");

            var keyvalue = (KeyValuePair<string, string>)value;

            Debug.WriteLine($"ConvertBack Value: {keyvalue.Key}, Parameter: {keyvalue.Value}");

            //Debug.WriteLine($"ConvertBack Value: {value}, Parameter: {parameter}");

            return keyvalue.Key;// (bool)value ? parameter : null;
        }
    }
}
