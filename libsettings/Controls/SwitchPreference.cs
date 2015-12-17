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
    public sealed class SwitchPreference : PreferenceBase
    {

        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(SwitchPreference), new PropertyMetadata(false, OnIsOnPropertyChanged));

        private static void OnIsOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SwitchPreference;
            if (control != null)
            {
                control.IsOn = (bool)e.NewValue;
                Debug.WriteLine($"OnIsOnPropertyChanged {control.IsOn}");
            }
        }

        public SwitchPreference()
        {
            this.DefaultStyleKey = typeof(SwitchPreference);
        }

        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); Debug.WriteLine($"Set IsOn to {IsOn}"); }
        }

        bool _toggleison;
        public bool ToggleIsOn
        {
            get { return _toggleison; }
            set { _toggleison = value; Debug.WriteLine($"Set ToggleIsOn to {_toggleison}"); }
        }


    }
}
