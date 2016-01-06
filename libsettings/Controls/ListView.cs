using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;


// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Settings.UI.Xaml.Controls
{
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(Windows.UI.Xaml.Controls.ContentPresenter))]
    public sealed class ListView : Windows.UI.Xaml.Controls.ListView
    {

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(DataTemplate), typeof(ListView), new PropertyMetadata(null, OnPlaceholderChanged));

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListView;
            if (control != null)
            {
                control.Placeholder = (DataTemplate)e.NewValue;
            }
        }

        public DataTemplate Placeholder
        {
            get { return (DataTemplate)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
            //get; set;
        }

        private Windows.UI.Xaml.Controls.ContentPresenter part_presenter;

        public ListView()
        {
            this.DefaultStyleKey = typeof(Settings.UI.Xaml.Controls.ListView);
            //this.Template = typeof(Settings.UI.Xaml.Controls.ListView);

            SetValue(PlaceholderProperty, null);

        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            part_presenter = GetTemplateChild("PART_ContentPresenter") as Windows.UI.Xaml.Controls.ContentPresenter;
            if (part_presenter == null)
            {
                throw new NullReferenceException("Placeholder part not available!");
            }
        }

        protected override void OnItemsChanged(object e)
        {
            base.OnItemsChanged(e);
            //Debug.WriteLine("OnItemsChanged");

            UpdateItems();
        }

        private void UpdateItems()
        {
            //Debug.WriteLine($"Items {Items.Count}");

            

            if (Items.Count == 0)
            {
                
                if (part_presenter != null)
                {

                    part_presenter.Visibility = Visibility.Visible;
                }
            } else
            {
                if (part_presenter != null)
                {

                   part_presenter.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
