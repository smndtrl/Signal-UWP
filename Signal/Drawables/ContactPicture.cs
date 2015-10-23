using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace Signal.Drawables
{
    class ContactPicture : Canvas
    {
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register("Radius", typeof(double), typeof(ContactPicture), new PropertyMetadata(50.0, OnSizePropertyChanged));
        public static readonly DependencyProperty ContactProperty = DependencyProperty.Register("Contact", typeof(Contact), typeof(ContactPicture), new PropertyMetadata(null, OnContactPropertyChanged));
        //public static readonly DependencyProperty ContactIdProperty = DependencyProperty.Register("ContactId", typeof(string), typeof(ContactPicture), new PropertyMetadata(string.Empty, OnContactPropertyChanged));

        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContactPicture;
            control.SetControlSize();
            control.Draw();
        }

        private static void OnContactPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContactPicture;
            control.Draw();
        }


        public ContactPicture()
        {
            this.Loaded += OnLoaded;
        }

        /*public string ContactId
        {
            get
            {
                return (string)GetValue(ContactIdProperty);
            }
            set
            {
                SetValue(ContactIdProperty, value);
            }
        }*/

        public Contact Contact
        {
            get
            {
                return (Contact)GetValue(ContactProperty);
            }
            set
            {
                SetValue(ContactProperty, value);
            }
        }

        public double Radius
        {
            get
            {
                return (double)GetValue(RadiusProperty);
            }
            set
            {
                SetValue(RadiusProperty, value);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetControlSize();
            Draw();
        }

        private void SetControlSize()
        {
            Width = Radius * 2;
            Height = Radius * 2;
        }


        private async void Draw()
        {
            Children.Clear();

            var grid = new Grid();
            grid.Height = Height;
            grid.Width = Width;

            var circle = new Ellipse();
            circle.Height = Height;
            circle.Width = Width;

            grid.Children.Add(circle);

            var text = new TextBlock();
            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Center;          
            text.FontSize = 24;
            text.TextLineBounds = TextLineBounds.Tight;
            text.Foreground = new SolidColorBrush(Colors.White);
            text.FontFamily = new FontFamily("Segoe UI Semibold");
            text.Visibility = Visibility.Collapsed;

            grid.Children.Add(text);


            if (Contact != null && Contact.Thumbnail != null)
            {
                var brush = new ImageBrush();
                var stream = await Contact.Thumbnail.OpenReadAsync();
                var image = new BitmapImage();
                image.DecodePixelHeight = 100;
                image.DecodePixelWidth = 100;
                image.SetSource(stream);


                brush.ImageSource = image;
                circle.Fill = brush;
            }
            else if (Contact != null)
            {
                text.Text = Contact.FirstName[0].ToString() + Contact.LastName[0].ToString();
                circle.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                text.Visibility = Visibility.Visible;
            }

            Children.Add(grid);
        }
    }
}
