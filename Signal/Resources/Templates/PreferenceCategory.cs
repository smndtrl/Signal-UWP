using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Signal.Resources.Templates
{
    public sealed class PreferenceCategory : ItemsControl
    {
        public static readonly DependencyProperty NameIProperty = DependencyProperty.Register("NameI", typeof(string), typeof(PreferenceCategory), new PropertyMetadata(string.Empty, OnCategoryPropertyChanged));

        private static void OnCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PreferenceCategory;
            if (control != null)
            {
                control.Name = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(PreferenceCategory), new PropertyMetadata(string.Empty, OnIconPropertyChanged));

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PreferenceCategory;
            if (control != null)
            {
                control.Icon = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(PreferenceCategory), new PropertyMetadata(string.Empty, OnDescriptionPropertyChanged));

        private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PreferenceCategory;
            if (control != null)
            {
                control.Description = (string)e.NewValue;
            }
        }

        public PreferenceCategory()
        {
            this.DefaultStyleKey = typeof(PreferenceCategory);
        }

        public string NameI
        {
            get { return (string)GetValue(NameIProperty); }
            set { SetValue(NameIProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
    }
}
