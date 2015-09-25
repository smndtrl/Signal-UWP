using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Signal.database.loaders
{
    public abstract class IncrementalCollection<T> : ObservableCollection<T>/* where T : IComparable<T>*/, ISupportIncrementalLoading
    {

        public bool HasMoreItems
        {
            get
            {
                return HasMoreItemsInternal();
            }
        }
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return Info.Run((c) => LoadMoreItemsAsync(c, count));
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
                    Add(item);
                }

                //NotifyOfInsertedItems(baseindex, items.Count);

                return new LoadMoreItemsResult { Count = (uint)items.LongCount() };
            }
            finally
            {

            }
        }


        // overridable
        protected abstract bool HasMoreItemsInternal();
        protected abstract Task<IEnumerable<T>> LoadMoreItemsInternal(CancellationToken c, uint count);
    }
}
