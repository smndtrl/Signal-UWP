using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.Resources.Templates
{
    //[ContentPropertyAttribute(Name = "Categories")]
    public partial class SettingsPage : Control
    {
        public static readonly DependencyProperty CategoriesProperty = DependencyProperty.Register("Categories", typeof(ObservableCollection<PreferenceCategory>), typeof(SettingsPage), new PropertyMetadata(new ObservableCollection<PreferenceCategory>(), OnCategoryPropertyChanged));

        private static void OnCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SettingsPage;
            if (control != null)
            {
                control.Categories = (ObservableCollection<PreferenceCategory>)e.NewValue;
            }
        }


        public SettingsPage()
        {
            this.DefaultStyleKey = typeof(SettingsPage);
        }

        public ObservableCollection<PreferenceCategory> Categories
        {
            get { return (ObservableCollection<PreferenceCategory>)GetValue(CategoriesProperty); }
            set { SetValue(CategoriesProperty, value); }
        }
    }
}
