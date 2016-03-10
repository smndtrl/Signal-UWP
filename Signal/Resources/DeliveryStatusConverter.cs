using Signal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Signal.Resources
{
    public class DeliveryStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var message = (MessageRecord)value;

            //Debug.WriteLine($"{message.Body}: Failed->{message.IsFailed}, Pending->{message.IsPending}, Delivered->{message.IsDelivered}");

            if (message.IsFailed) return 0;
            else if (message.IsPending) return 1;
            else if (message.IsDelivered) return 2;
            else return 3;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class RadioButtonCheckedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            string culture)
        {
            Debug.WriteLine($"Value: {value}, Parameter {parameter}");
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string culture)
        {
            return value == parameter;
        }
    }
}

