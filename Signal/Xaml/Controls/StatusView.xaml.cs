using System;
using System.Collections.Generic;
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
using Signal.Models;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Signal.Xaml.Controls
{
    

    public sealed partial class StatusView : UserControl
    {
        public enum Status
        {
            Pending = 0,
            Sent = 1,
            Delivered = 2,
            Failed = 3
        }

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(Status), typeof(StatusView), new PropertyMetadata(Status.Pending, new PropertyChangedCallback(OnStateChanged)));

        public Status State
        {
            get { return (Status)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as StatusView;
            if (control != null)
            {
                control.UpdateState((Status)e.NewValue);
            }
        }

        public StatusView()
        {
            this.InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                if (m.IsFailed)
                {
                    UpdateState(Status.Failed);
                }
                else
                {
                    UpdateState(m.IsPending ? Status.Pending : (m.IsDelivered ? Status.Delivered : Status.Sent));
                }

            }
        }
        private void UpdateState(Status status)
        {
            switch (status)
            {
                case Status.Failed:
                    VisualStateManager.GoToState(this, "Failed", true);
                    break;
                case Status.Pending:
                    VisualStateManager.GoToState(this, "Pending", true);
                    break;
                case Status.Sent:
                    VisualStateManager.GoToState(this, "Sent", true);
                    break;
                case Status.Delivered:
                    VisualStateManager.GoToState(this, "Delivered", true);
                    break;
                default:
                    break;
            }
        }
    }
}
