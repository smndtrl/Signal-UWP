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


    public class ContactRetriever : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var id = (string)value;
                
                return NotifyTaskCompletion.Create<Contact>(GetContactForId(id));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<Contact> GetContactForId(string id)
        {
            if (id == null) return null;

            var contactStore = await ContactManager.RequestStoreAsync();
            return await contactStore.GetContactAsync(id);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }


    }
}
