using Signal.Model;
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
        long threadId;

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
