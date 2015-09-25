using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure
{
    using System.Diagnostics;
    using Windows.ApplicationModel.Contacts;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public class ContactPictureConverter// : IValueConverter
    {
        /*public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Contact c = value as Contact;
                return NotifyTaskCompletion.Create<BitmapImage>(GetImage(c));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<BitmapImage> GetImage(Contact con)
        {
            using (var stream = await con.Thumbnail.OpenRead())
            {
                
                BitmapImage image = new BitmapImage();
                image.DecodePixelHeight = 100;
                image.DecodePixelWidth = 100;

                image.SetSource(stream);

                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }*/
    }
}
