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
    public class MessageCollection : IncrementalCollection<MessageRecord>
    {
        IDataService service;

        long max = long.MaxValue;
        long threadId = -1;
        private bool IsUpdating = false;

        public MessageCollection(IDataService service)
            : this(service, -1)
        {
        }


        public MessageCollection(IDataService service, long threadId)
        {
            this.service = service;
            this.threadId = threadId;

            max = service.getMessagesCount(threadId);

            Messenger.Default.Register<RefreshThreadMessage>(
                this,
                async message =>
                {

                    if (IsUpdating) return;

                    IsUpdating = true;
                    Debug.WriteLine($"(MessageCollection)Refreshing message Collection for Thread {message.ThreadId}");

                    // Refresh Collection loader
                    if (threadId != message.ThreadId)
                    {
                        Debug.WriteLine($"Thread #{threadId} got message for {message.ThreadId}, sleeping.");
                        return;
                    }

                    max = service.getMessagesCount(threadId);


                    Debug.WriteLine($"Refreshing message Collection for Trhead {message.ThreadId}");
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Clear();
                        IsUpdating = false;
                    });

                }
            );
        }


        protected override bool HasMoreItemsInternal()
        {
            return Count < max;
        }

        protected override async Task<IEnumerable<MessageRecord>> LoadMoreItemsInternal(CancellationToken c, uint count)
        {
            //Debug.WriteLine($"Messages: Load {count} more, has already {Count}");

            return (await service.getMessages(threadId)).ToList(); // Skip(Count).Take((int)count);
        }


    }
}
