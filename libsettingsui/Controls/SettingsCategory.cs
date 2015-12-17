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

namespace Settings.UI.Xaml.Controls
{
    public sealed class SettingsCategory : ItemsControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SettingsCategory), new PropertyMetadata(string.Empty, OnCategoryPropertyChanged));

        private static void OnCategoryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SettingsCategory;
            if (control != null)
            {
                control.Title = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(SettingsCategory), new PropertyMetadata(string.Empty, OnIconPropertyChanged));

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SettingsCategory;
            if (control != null)
            {
                control.Icon = (string)e.NewValue;
            }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(SettingsCategory), new PropertyMetadata(string.Empty, OnDescriptionPropertyChanged));

        private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SettingsCategory;
            if (control != null)
            {
                control.Description = (string)e.NewValue;
            }
        }

        public SettingsCategory()
        {
            this.DefaultStyleKey = typeof(SettingsCategory);
        }

        public string Title
        {
            get { Debug.WriteLine($"Property: {(string)GetValue(IconProperty)}");  return "Cut"; /* (string)GetValue(TitleProperty);*/ }
            set { SetValue(TitleProperty, value); }
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
