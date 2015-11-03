using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Signal.Resources.Templates
{
    [ContentPropertyAttribute(Name = "Children")]
    public sealed partial class SettingsView : UserControl
    {
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register("Children", typeof(IEnumerable<SettingsCategory>), typeof(SettingsView), new PropertyMetadata(new ObservableCollection<PreferenceBase>(), OnChildrenPropertyChanged));

        private static void OnChildrenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(IEnumerable<SettingsCategory>), typeof(SettingsView), new PropertyMetadata(string.Empty, OnKeyPropertyChanged));

        private static void OnKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        public SettingsView()
        {
            this.DefaultStyleKey = typeof(SettingsView);
            this.InitializeComponent();
            Children = new ObservableCollection<PreferenceBase>();
        }

        public ObservableCollection<PreferenceBase> Children
        {
            get
            {
                var t = (ObservableCollection<PreferenceBase>)GetValue(ChildrenProperty);
                return (ObservableCollection<PreferenceBase>)GetValue(ChildrenProperty);
            }
            set { SetValue(ChildrenProperty, value); }
        }

        public string Key
        {
            get
            {
                var t = (string)GetValue(KeyProperty);
                return (string)GetValue(KeyProperty);
            }
            set { SetValue(KeyProperty, value); }
        }
    }
}
