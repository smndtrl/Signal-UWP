using GalaSoft.MvvmLight.Messaging;
using Signal.Models;
using Signal.ViewModel.Messages;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Signal.database.loaders
{
    public class ThreadCollection : IncrementalCollection<Thread>
    {
        IDataService service;
        //IEnumerable<Contact> _storage;

        int max = int.MaxValue;

        public ThreadCollection(IDataService service)
        {
            this.service = service;
            /*var list = (await service.getThreads()).ToList();
            max = list.Count;
            //_storage = list.Take(10);
            foreach (var con in list)
            {
                Debug.WriteLine($"Adding thread for {con.Recipients.ShortString}");
                Add(con);
            }*/

            Messenger.Default.Register<RefreshThreadMessage>(
                this,
                async message =>
                {
                    // Refresh Collection loader
                    Debug.WriteLine("Refreshing Thread Collection");
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Clear();
                    });
                   
                }
            );
        }

        protected override bool HasMoreItemsInternal()
        {
            return Count < max;
        }

        protected override async Task<IEnumerable<Thread>> LoadMoreItemsInternal(CancellationToken c, uint count)
        {
            if (max == int.MaxValue)
            {
                max = (await service.getThreads()).Count();
            }

            //Debug.WriteLine($"Loading {count} more");
            return (await service.getThreads()).ToList().Skip(Count).Take((int)count);
        }


    }
}
