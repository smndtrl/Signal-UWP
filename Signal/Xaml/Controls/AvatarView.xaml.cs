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
    public sealed partial class AvatarView : UserControl
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
            this.InitializeComponent();
        }

        private void Update(Recipient recipient)
        {

        }
    }
}
