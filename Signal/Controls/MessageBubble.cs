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
using Signal.Util;
using Windows.Foundation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Signal.Controls
{
    public sealed class MessageBubble : Control
    {
        public static readonly DependencyProperty MessageRecordProperty = DependencyProperty.Register("MessageRecord", typeof(MessageRecord), typeof(MessageBubble), new PropertyMetadata(new MessageRecord(), new PropertyChangedCallback(OnMessageRecordChanged)));

        public MessageRecord MessageRecord
        {
            get { return (MessageRecord)GetValue(MessageRecordProperty); }
            set {
                Log.Debug($"set");
                SetValue(MessageRecordProperty, value);

            }
        }

        private static void OnMessageRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Log.Debug($"OnMessageRecordChanged");

            MessageBubble control = d as MessageBubble;
            control.MessageRecord = (Models.MessageRecord)e.NewValue;
            control.UpdateDirectionState();

        }

        public MessageBubble()
        {
            this.DefaultStyleKey = typeof(MessageBubble);

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            var m = this.DataContext as MessageRecord;
            if (m != null)
            {
                VisualStateManager.GoToState(this, m.IsOutgoing ? "Outgoing" : "Incoming", true);
            }
        }


        private DeliveryStatusView _statusView;

        protected override void OnApplyTemplate()
        {
            Log.Debug($"OnApplyTemplate");

            _statusView = GetTemplateChild("DeliveryStatusView") as DeliveryStatusView;;

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
