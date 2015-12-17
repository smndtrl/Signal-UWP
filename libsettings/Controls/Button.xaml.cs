using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Settings.UI.Xaml.Controls
{
    public sealed partial class Button : BaseUserControl
    {

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(Button), new PropertyMetadata(string.Empty, OnButtonTextPropertyChanged));

        private static void OnButtonTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Button;
            if (control != null)
            {
                control.ButtonText = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(Button), new PropertyMetadata(null, OnButtonCommandPropertyChanged));

        private static void OnButtonCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Button;
            if (control != null)
            {
                control.ButtonCommand = (ICommand)e.NewValue;
            }
        }

        public Button()
        {
            this.InitializeComponent();
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
            set { SetValue(ButtonCommandProperty, value); }
        }
    }
}
