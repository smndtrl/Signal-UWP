using Signal.Database;
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

using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Signal.database.loaders
{
    public class DirectoryCollection : IncrementalCollection<TextSecureDirectory.Directory>
    {
        IDataService service;
        IEnumerable<Contact> _storage;

        int max = 10;

        public DirectoryCollection(IDataService service)
        {
            this.service = service;
            /*var list = (await service.getDictionary()).ToList();
            max = list.Count;
            _storage = list.Take(10);
            foreach (var con in _storage)
            {
                Debug.WriteLine($"Adding {con.name}");
                Add(con);
            }*/
        }

        protected override bool HasMoreItemsInternal()
        {
            return Count < max;
        }

        protected override async Task<IEnumerable<TextSecureDirectory.Directory>> LoadMoreItemsInternal(CancellationToken c, uint count)
        {
            Debug.WriteLine($"Loading {count} more from directory");
            return (await service.getDictionary()).Where(d => d.Registered == 1).ToList().Skip(Count).Take((int)count);
        }


    }
}
