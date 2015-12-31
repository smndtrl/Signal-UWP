using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Settings.UI.Xaml.Controls
{
    public sealed partial class PrefixTextBox : TextBox
    {
        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register("Prefix", typeof(string), typeof(PrefixTextBox), new PropertyMetadata(string.Empty, OnPrefixPropertyChanged));

        private static void OnPrefixPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("OnPrefixPropertyChanged");
            var control = d as PrefixTextBox;
            if (control != null)
            {
                control.Prefix = (string)e.NewValue;
            }
        }

        public string Prefix
        {
            get { return (string)GetValue(PrefixProperty); }
            set {
                SetValue(PrefixProperty, value);
                if (value != null)
                {
                    Debug.WriteLine("visible");

                    Visibility = Visibility.Visible;
                }
                else
                {
                    Debug.WriteLine("collapsed");

                    Visibility = Visibility.Collapsed;
                }
                Debug.WriteLine("Prefix set");
            }
        }

        public PrefixTextBox()
        {
            this.DefaultStyleKey = typeof(PrefixTextBox);
        }
    }
}
