using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.Contacts;
using Nito.AsyncEx;

namespace Signal.Resources
{


    public class ContactPictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var id = (string)value;
                
                return NotifyTaskCompletion.Create<BitmapImage>(GetImageForId(id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<BitmapImage> GetImageForId(string id)
        {

            var contactStore = await ContactManager.RequestStoreAsync();
            var contact = await contactStore.GetContactAsync(id);

            if (contact.Thumbnail == null)
            {
                BitmapImage image = new BitmapImage();
                image.DecodePixelHeight = 100;
                image.DecodePixelWidth = 100;

                return image;
            }

            using (var stream = await contact.Thumbnail.OpenReadAsync())
            {

                BitmapImage image = new BitmapImage();
                image.DecodePixelHeight = 100;
                image.DecodePixelWidth = 100;

                image.SetSource(stream);

                return image;
            }
        }

       /* private BitmapImage CreateImageForName(string name)
        {
            System.Drawing.Graphics graphicsObj;

        }*/

        private async Task<BitmapImage> GetImage(string number)
        {

            var contactStore = await ContactManager.RequestStoreAsync();
            var contact = await contactStore.GetContactAsync(number);
            using (var stream = await contact.Thumbnail.OpenReadAsync())
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
        }

        private async Task<Contact> GetContactForNumber(string number)
        {
            var contactStore = await ContactManager.RequestStoreAsync();
            var contacts = await contactStore.FindContactsAsync(number);

            foreach (var contact in contacts)
            {

                return contact;

                /*foreach (var number in contact.Phones)
                {
                    try
                    {
                        string e164number = PhoneNumberFormatter.formatNumber(number.Number, localNumber);
                        results.Add(number.Number); //apply formatting
                    }
                    catch (InvalidNumberException e)
                    {
                        Debug.WriteLine($"Directory: Invalid number: {number}");
                    }

                }*/
            }

            return null;
        }
    }
}
