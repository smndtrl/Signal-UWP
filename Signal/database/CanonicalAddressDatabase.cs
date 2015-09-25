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

using SQLite;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.database
{
    public class CanonicalAddressDatabase
    {

        private static readonly int DATABASE_VERSION = 1;
        private static readonly String DATABASE_NAME = "canonical_address.db";
        private const String TABLE = "canonical_addresses";
        private static readonly String ID_COLUMN = "_id";
        private static readonly String ADDRESS_COLUMN = "address";

        [Table(TABLE)]
        public class CanonicalAddress
        {
            [PrimaryKey, AutoIncrement]
            public long? _id { get; set; } = null;
            public string address { get; set; }
        }


        private static readonly String DATABASE_CREATE = "CREATE TABLE " + TABLE + " (" + ID_COLUMN + " integer PRIMARY KEY, " + ADDRESS_COLUMN + " TEXT NOT NULL);";
        private static readonly String SELECTION_NUMBER = "PHONE_NUMBERS_EQUAL(" + ADDRESS_COLUMN + ", ?)";
        private static readonly String SELECTION_OTHER = ADDRESS_COLUMN + " = ? COLLATE NOCASE";
        private static readonly Object locko = new Object();

        private static CanonicalAddressDatabase instance;
        private SQLiteConnection conn;

        private readonly ConcurrentDictionary<String, long> addressCache = new ConcurrentDictionary<String, long>();
        private readonly ConcurrentDictionary<long, String> idCache = new ConcurrentDictionary<long, String>();

        public static CanonicalAddressDatabase getInstance()
        {
            lock (locko)
            {
                if (instance == null)
                    instance = new CanonicalAddressDatabase();

                return instance;
            }
        }

        private CanonicalAddressDatabase()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "test.db");
            //conn = new SQLiteConnection(path);
            conn = new SQLiteConnection(path);

            conn.CreateTable<CanonicalAddress>();

            fillCache();
        }

        public void reset()
        {
            // clear
            fillCache();
        }

        private void fillCache()
        {

            try
            {
                var query = conn.Table<CanonicalAddress>().Where(v => true);

                foreach (var canonicaladdress in query.ToList())
                {
                    long id = (long)canonicaladdress._id;
                    String address = canonicaladdress.address;

                    if (address == null || address.Trim().Length == 0)
                        address = "Anonymous";

                    idCache.AddOrUpdate(id, address, (k, v) => address);
                    addressCache.AddOrUpdate(address, id, (k, v) => id);
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
            }
        }

        public String getAddressFromId(long id)
        {
            String cachedAddress;
            idCache.TryGetValue(id, out cachedAddress);

            if (cachedAddress != null)
                return cachedAddress;

            try
            {
                Debug.WriteLine("CanonicalAddressDatabase", "Hitting DB on query [ID].");

                var query = conn.Table<CanonicalAddress>().Where(v => v._id == id);

                if (query.Count() == 0)
                    return "Anonymous";

                String address = query.First().address;

                if (address == null || address.Trim().Equals(""))
                {
                    return "Anonymous";
                }
                else
                {
                    idCache.AddOrUpdate(id, address, null);
                    return address;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }
        }

        public void close()
        {
            instance = null;
        }

        public long getCanonicalAddressId(String address)
        {
            long canonicalAddressId;

            if ((canonicalAddressId = getCanonicalAddressFromCache(address)) != -1)
                return canonicalAddressId;

            canonicalAddressId = getCanonicalAddressIdFromDatabase(address);
            idCache.AddOrUpdate(canonicalAddressId, address, (k, v) => address);
            addressCache.AddOrUpdate(address, canonicalAddressId, (k, v) => canonicalAddressId);
            return canonicalAddressId;
        }

        public List<long> getCanonicalAddressIds(List<String> addresses)
        {
            List<long> addressList = new List<long>();

            foreach (String address in addresses)
            {
                addressList.Add(getCanonicalAddressId(address));
            }

            return addressList;
        }

        private long getCanonicalAddressFromCache(String address)
        {
           

            if (addressCache.ContainsKey(address)) // trygetvalue gives 0 for long not found.. so a litte hack here
            {
                long cachedAddress = -1;
                addressCache.TryGetValue(address, out cachedAddress);

                return cachedAddress;
            } else
            {
                return -1L;
            }

        }

        private long getCanonicalAddressIdFromDatabase(String address)
        {

            try
            {

                bool isNumber = isNumberAddress(address);
                /*cursor = db.query(TABLE, null,
                                                       isNumber ? SELECTION_NUMBER : SELECTION_OTHER,
                                                       selectionArguments, null, null, null);*/

                var query = conn.Table<CanonicalAddress>().Where(v => v.address == address);

                if (query.Count() == 0)
                {

                    return conn.Insert(new CanonicalAddress() { address = address });
                }
                else
                {
                    long canonicalId = (long)query.First()._id;
                    String oldAddress = query.First().address;
                    if (oldAddress == null || !oldAddress.Equals(address))
                    {
                        /*ContentValues contentValues = new ContentValues(1);
                        contentValues.put(ADDRESS_COLUMN, address);
                        db.update(TABLE, contentValues, ID_COLUMN + " = ?", new String[] { canonicalId + "" });*/

                        conn.Update(new CanonicalAddress() { _id = canonicalId, address = address });

                        long val;
                        addressCache.TryRemove(oldAddress, out val);
                    }
                    return canonicalId;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
            }
        }


        static bool isNumberAddress(String number)
        {
            if (number.Contains("@"))
                return false;
            //if (GroupUtil.isEncodedGroup(number))
            //    return false;

            /*String networkNumber = PhoneNumberUtils.extractNetworkPortion(number);
            if ((networkNumber.Length == 0)
                return false;
            if (networkNumber.Length < 3)
                return false;*/

            return true;// PhoneNumberUtils.isWellFormedSmsAddress(number);
        }


    }
}
