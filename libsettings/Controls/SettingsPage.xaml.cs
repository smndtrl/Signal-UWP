using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Settings.UI.Xaml.Controls
{
    [ContentPropertyAttribute(Name = "Categories")]
    public partial class SettingsPage : UserControl
    {
        public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register("Categories", typeof(IEnumerable<SettingsCategory>), typeof(SettingsPage), new PropertyMetadata(new ObservableCollection<SettingsCategory>(), OnCategoryPropertyChanged));

        private static void OnCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty ViewsProperty = DependencyProperty.Register("Views", typeof(IEnumerable<SettingsView>), typeof(SettingsPage), new PropertyMetadata(new ObservableCollection<SettingsView>(), OnViewPropertyChanged));

        private static void OnViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public SettingsPage()
        {
            this.DefaultStyleKey = typeof(SettingsPage);
            this.InitializeComponent();
        }

        public ObservableCollection<SettingsCategory> Categories
        {
            get {
                var t = (ObservableCollection<SettingsCategory>)GetValue(CategoriesProperty);
                return (ObservableCollection<SettingsCategory>)GetValue(CategoriesProperty);
            }
            set { SetValue(CategoriesProperty, value); }
        }

        public string PageKey = "Page1";

        /*public SettingsView SettingsView
        {
            get
            {
                var view = (SettingsView)this.Views.FindName("Page1");
                Debug.WriteLine($"SettinsView Children {view.Children.Count}");
                return view;
            }
        }*/

        public ObservableCollection<SettingsView> Views
        {
            get
            {
                var t = (ObservableCollection<SettingsView>)GetValue(ViewsProperty);
                return (ObservableCollection<SettingsView>)GetValue(ViewsProperty);
            }
            set { SetValue(CategoriesProperty, value); }
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            Debug.WriteLine("state changed");
        }
    }
}
