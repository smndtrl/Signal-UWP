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

        int max = Int32.MaxValue;

        public DirectoryCollection(IDataService service)
        {
            this.service = service;
        }

        public void Requery()
        {
            Clear();
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
