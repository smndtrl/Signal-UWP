using Signal.Models;
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
using Signal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Signal.Views
{

    public sealed partial class MessageDetailsPage : SignalPage
    {
        /*private static DependencyProperty _messageProperty = DependencyProperty.Register("Message", typeof(Message), typeof(MessageDetailPage), new PropertyMetadata(null));

        public static DependencyProperty MessageProperty { get { return _messageProperty; } }

        public Message Message
        {
            get { return (Message)GetValue(_messageProperty); }
            set { SetValue(_messageProperty, value); }
        }*/

        public MessageDetailViewModel ViewModel => (MessageDetailViewModel)DataContext;

        public MessageDetailsPage()
        {
            this.InitializeComponent();
        }
    }
}
