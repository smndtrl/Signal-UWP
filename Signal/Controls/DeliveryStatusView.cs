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
    public enum DeliveryStatus
    {
        Pending = 0,
        Sent = 1,
        Delivered = 2
    }

    public sealed class DeliveryStatusView : Control
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(DeliveryStatus), typeof(DeliveryStatusView), new PropertyMetadata(DeliveryStatus.Pending, new PropertyChangedCallback(OnStateChanged)));

        public DeliveryStatus State
        {
            get { return (DeliveryStatus)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as DeliveryStatusView;
            if (control != null)
            {
                control.UpdateState((DeliveryStatus)e.NewValue);
            }
        }

        

        public DeliveryStatusView()
        {
            this.DefaultStyleKey = typeof(DeliveryStatusView);
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                UpdateState(m.IsPending ? DeliveryStatus.Pending : (m.IsDelivered ? DeliveryStatus.Delivered : DeliveryStatus.Sent));

            }
        }
        private void UpdateState(DeliveryStatus status)
        {
            switch (status)
            {
                case DeliveryStatus.Pending:
                    VisualStateManager.GoToState(this, "Pending", true);
                    break;
                case DeliveryStatus.Sent:
                    VisualStateManager.GoToState(this, "Sent", true);
                    break;
                case DeliveryStatus.Delivered:
                    VisualStateManager.GoToState(this, "Delivered", true);
                    break;
                default:
                    break;
            }
        }

    }
}