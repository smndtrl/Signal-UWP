/** 
 * Copyright (C) 2015 smndtrl
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace TextSecure.database.loaders
{
    public abstract class BaseLoader : IList, ISupportIncrementalLoading, INotifyCollectionChanged
    {

        /*
         * IList
         */ 
        public object this[int index]
        {
            get
            {
                return storage[index];
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                return storage.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            return storage.Contains(value);
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)storage).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return storage.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return storage.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }


        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        // ISupportIncrementalLoading
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

        // INotifyCollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        // private
        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                var items = await LoadMoreItemsInternal(c, count);
                var baseindex = storage.Count;

                storage.AddRange(items);
                NotifyOfInsertedItems(baseindex, items.Count);

                return new LoadMoreItemsResult { Count = (uint)items.Count };
            } finally
            {

            }
        }

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            for (int i=0; i < count;i++)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, storage[i + baseIndex], i + baseIndex);
                CollectionChanged(this, args);
            }
        }

        // overridable
        protected abstract bool HasMoreItemsInternal();
        protected abstract Task<IList<object>> LoadMoreItemsInternal(CancellationToken c, uint count);

        // state
        List<object> storage = new List<object>();
    }
}
