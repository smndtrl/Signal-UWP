using Signal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextSecure.database;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Signal.database.loaders
{
    public class GroupedDirectoryCollection : ObservableCollection<IGrouping<long, TextSecureDirectory.Directory>>
    {
        IDataService service;
        IEnumerable<Contact> _storage;

        int max = 10;

        public GroupedDirectoryCollection(IDataService service)
        {
            this.service = service;
            /*var list = (await service.getContacts()).ToList();
            max = list.Count;
            _storage = list.Take(10);
            foreach (var con in _storage)
            {
                Debug.WriteLine($"Adding {con.name}");
                Add(con);
            }*/

            var contacts = (service.getDictionary().Result).ToList();
            var grouped = from contact in contacts group contact by contact.Registered;
            foreach (var group in grouped)
            {
                if (group.Key == 1) Add(group); // TODO: only add registered for now
                Debug.WriteLine($"We have {grouped.Count()} groups, {group.Key} with {group.Count()} members.");
            }
        }
    }
}
