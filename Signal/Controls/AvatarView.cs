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
    public sealed class AvatarView : Control
    {
        public static readonly DependencyProperty RecipientProperty = DependencyProperty.Register("Type", typeof(Recipient), typeof(AvatarView), new PropertyMetadata(new Recipient(), new PropertyChangedCallback(OnRecipientChanged)));

        public Recipient Recipient
        {
            get { return (Recipient)GetValue(RecipientProperty); }
            set { SetValue(RecipientProperty, value); }
        }

        private static void OnRecipientChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as AvatarView;

            control?.Update((Recipient)e.NewValue);
        }

        public AvatarView()
        {
            this.DefaultStyleKey = typeof(AvatarView);
        }

        private void Update(Recipient recipient)
        {

        }
    }
}
