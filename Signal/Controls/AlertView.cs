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
using Signal.Models;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Signal.Controls
{
    public enum AlertType
    {
        None = 0,
        Failed = 1,
        PendingApproval = 2
    }

    public sealed class AlertView : Control
    {
        public static readonly DependencyProperty AlertTypeProperty = DependencyProperty.Register("Type", typeof(AlertType), typeof(DeliveryStatusView), new PropertyMetadata(AlertType.Failed, new PropertyChangedCallback(OnStateChanged)));

        public AlertType State
        {
            get { return (AlertType)GetValue(AlertTypeProperty); }
            set { SetValue(AlertTypeProperty, value); }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AlertView;

            control?.Update((AlertType)e.NewValue);
        }

        public AlertView()
        {
            this.DefaultStyleKey = typeof(AlertView);
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                Update(m.IsFailed ? AlertType.Failed : AlertType.None);

            }
        }
        private void Update(AlertType type)
        {
            switch (type)
            {
                case AlertType.Failed:
                    VisualStateManager.GoToState(this, "Failed", true);
                    break;
                case AlertType.PendingApproval:
                    VisualStateManager.GoToState(this, "PendingApproval", true);
                    break;
                default:
                    break;
            }
        }

    }
}