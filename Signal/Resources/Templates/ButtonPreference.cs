using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Signal.Resources.Templates
{
    public sealed class ButtonPreference : PreferenceBase
    {
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(ButtonPreference), new PropertyMetadata(string.Empty, OnButtonTextPropertyChanged));

        private static void OnButtonTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ButtonPreference;
            if (control != null)
            {
                control.ButtonText = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ButtonPreference), new PropertyMetadata(null, OnButtonCommandPropertyChanged));

        private static void OnButtonCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ButtonPreference;
            if (control != null)
            {
                control.ButtonCommand = (ICommand)e.NewValue;
            }
        }

        public ButtonPreference()
        {
            this.DefaultStyleKey = typeof(ButtonPreference);
        }

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set
            {
                //if (value == null || value == SelectedValue) return;
                Debug.WriteLine($"Setting Button Text to {value}");
                SetValue(ButtonTextProperty, value);
            }
        }

        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set {  SetValue(ButtonCommandProperty, value); }
        }
    }
}
