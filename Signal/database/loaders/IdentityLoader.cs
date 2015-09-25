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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace TextSecure.database.loaders
{
    public class IdentityLoader : BaseLoader
    {
        int _count = 0;
        int max_count = -1;

        public IdentityLoader()
        {

        }

        protected override bool HasMoreItemsInternal()
        {
            return _count < max_count;
        }

        protected override async Task<IList<object>> LoadMoreItemsInternal(CancellationToken c, uint count)
        {
            if (max_count == -1)
            {
                var identities = DatabaseFactory.getIdentityDatabase().getIdentities();
                max_count = identities.Count;
            }

            var list = await DatabaseFactory.getIdentityDatabase().getIdentities();
            _count += list.Count();
            Debug.WriteLine($"Loaded {list.Count} identities");
            return (IList < object > )list;
        }
    }
}
