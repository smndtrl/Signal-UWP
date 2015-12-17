using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Settings.UI.Xaml.Controls
{
    public sealed partial class ToggleSwitch : BaseUserControl
    {
        public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(false, OnIsOnPropertyChanged));

        private static void OnIsOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SwitchPreference;
            if (control != null)
            {
                control.IsOn = (bool)e.NewValue;
            }
        }

        public ToggleSwitch()
        {
            this.DefaultStyleKey = typeof(ToggleSwitch);
            this.InitializeComponent();
        }

        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }
    }
}
