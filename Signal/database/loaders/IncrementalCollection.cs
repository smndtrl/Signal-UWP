using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public abstract class IncrementalCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading where T : INotifyPropertyChanged
    {
        protected long Skip { get; set; }
        protected long Take { get; set; }

        public bool HasMoreItems
        {
            get
            {
                return HasMoreItemsInternal();
            }
        }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        // private
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var items = await LoadMoreItemsInternal(c, count);
                //var baseindex = storage.Count;

                foreach (var item in items)
                {
                    item.PropertyChanged += (s, e) => { Debug.WriteLine("property changed"); };
                    Add(item);
                }

                //NotifyOfInsertedItems(baseindex, items.Count);

                return new LoadMoreItemsResult { Count = (uint)items.LongCount() };
            }
            finally
            {

            }
        }

        public async void Refresh()
        {
            var count = Count;
            Clear();
            await LoadMoreItemsAsync((uint)count);
        }

        // overridable
        protected abstract bool HasMoreItemsInternal();
        protected abstract Task<IEnumerable<T>> LoadMoreItemsInternal(CancellationToken c, uint count);
    }
}
