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
using Signal.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Signal.Controls
{
    public sealed partial class MessageView : UserControl
    {
        public static readonly DependencyProperty MessageRecordProperty = DependencyProperty.Register("MessageRecord", typeof(MessageRecord), typeof(MessageView), new PropertyMetadata(new MessageRecord(), new PropertyChangedCallback(OnMessageRecordChanged)));

        public MessageRecord MessageRecord
        {
            get { return (MessageRecord)GetValue(MessageRecordProperty); }
            set
            {
                Log.Debug($"set");
                SetValue(MessageRecordProperty, value);

            }
        }

        private static void OnMessageRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Log.Debug($"OnMessageRecordChanged");

            MessageView control = d as MessageView;
            control.MessageRecord = (Models.MessageRecord)e.NewValue;
            control.UpdateDirectionState();

        }

        public MessageView()
        {
            this.InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                this.MessageRecord = m;
                VisualStateManager.GoToState(this, m.IsOutgoing ? "Outgoing" : "Incoming", true);


                if (m.IsFailed)
                {
                    VisualStateManager.GoToState(this, "Failed", true);
                }
                else
                {
                    if (!m.IsOutgoing) VisualStateManager.GoToState(this, "None", true);
                    else if (m.IsPending) VisualStateManager.GoToState(this, "Pending", true);
                    else if (m.IsDelivered) VisualStateManager.GoToState(this, "Delivered", true);
                    else VisualStateManager.GoToState(this, "Sent", true);
                }

                if (m.IsKeyExchange)
                {
                    VisualStateManager.GoToState(this, "KeyExchange", true);
                }
            }
        }


        private StatusView _statusView;

        protected override void OnApplyTemplate()
        {
            _statusView = GetTemplateChild("StatusView") as StatusView;

            UpdateDirectionState();
        }


        private void UpdateDirectionState()
        {
            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                VisualStateManager.GoToState(this, m.IsOutgoing ? "Outgoing" : "Incoming", true);
            }
        }

    }
}

