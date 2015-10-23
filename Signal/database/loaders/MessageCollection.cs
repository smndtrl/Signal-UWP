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
using Windows.UI.Xaml.Data;

namespace Signal.database.loaders
{
    public class MessageCollection : IncrementalCollection<Message>
    {
        IDataService service;
        //IEnumerable<Contact> _storage;

        int max = int.MaxValue;
        long threadId = -1;

        /*public MessageCollection(IDataService service)
        {
            this.service = service;

            Messenger.Default.Register<RefreshThreadMessage>(
    this,
    async message =>
    {
        Debug.WriteLine($"(MessageCollection)Refreshing message Collection for Thread {message.ThreadId}");

                    // Refresh Collection loader
                    if (threadId != message.ThreadId)
        {
            Debug.WriteLine($"Thread #{threadId} got message for {message.ThreadId}, sleeping.");
            return;
        }


        Debug.WriteLine($"Refreshing message Collection for Trhead {message.ThreadId}");
        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
        {
            Clear();
        });

    }
);
        }

        public void Show(long threadId)
        {
            this.threadId = threadId;
        }*/

        public MessageCollection(IDataService service, long threadId)
        {
            this.service = service;
            this.threadId = threadId;
            /*var list = service.getMessages(threadId).ToList();
            max = list.Count;
            //_storage = list.Take(10);
            foreach (var con in list)
            {
                Add(con);
            }*/

            Messenger.Default.Register<RefreshThreadMessage>(
                this,
                async message =>
                {
                    Debug.WriteLine($"(MessageCollection)Refreshing message Collection for Thread {message.ThreadId}");

                    // Refresh Collection loader
                    if (threadId != message.ThreadId)
                    {
                        Debug.WriteLine($"Thread #{threadId} got message for {message.ThreadId}, sleeping.");
                        return;
                    }


                    Debug.WriteLine($"Refreshing message Collection for Trhead {message.ThreadId}");
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

        protected override async Task<IEnumerable<Message>> LoadMoreItemsInternal(CancellationToken c, uint count)
        {
            if (max == int.MaxValue)
            {
                max = (await service.getMessages(threadId)).ToList().Count();
            }

            //Debug.WriteLine($"Messages: Load {count} more, has already {Count}");

            return (await service.getMessages(threadId)).ToList().Skip(Count).Take((int)count);
        }


    }
}
