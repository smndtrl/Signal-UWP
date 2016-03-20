using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ZXing;
using ZXing.Mobile;
using BarcodeWriter = ZXing.BarcodeWriter;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Signal.Xaml.Controls
{
    public sealed partial class QrCode : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(QrCode), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnMessageRecordChanged)));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnMessageRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as QrCode;
            if (d != null)
            {
                control.Update();
            }
        }

        public ImageSource ImageSource { get; set; }

        public QrCode()
        {
            this.InitializeComponent();
        }

        private async void Update()
        {
            var writer1 = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200
                },

            };

            var image = writer1.Write(Text);//Write(text);


            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(image);
                    await writer.StoreAsync();
                }

                var output = new BitmapImage();
                await output.SetSourceAsync(ms);
                ImageSource = output;
            }


        }


    }
}
