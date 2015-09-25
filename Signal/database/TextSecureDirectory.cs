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

using libtextsecure.push;
using libtextsecure.util;
using SQLite;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;
using Windows.ApplicationModel.Contacts;

namespace TextSecure.database
{

    public class TextSecureDirectory
    {
        public class Directory
        {
            [PrimaryKey, AutoIncrement]
            public int? id { get; set; } = null;
            [Unique]
            public string number { get; set; }
            public uint registered { get; set; }
            public string relay { get; set; }
            public uint supports_sms { get; set; }
            public uint timestamp { get; set; }
        }


        private static readonly int INTRODUCED_CHANGE_FROM_TOKEN_TO_E164_NUMBER = 2;

        private static readonly String DATABASE_NAME = "directory.db";
        private static readonly String DIRECTORY_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecureDirectory.DATABASE_NAME);
        private static readonly int DATABASE_VERSION = 2;


        private static readonly Object instanceLock = new Object();
        private static volatile TextSecureDirectory instance;
        private static SQLiteConnection conn;

        public static TextSecureDirectory getInstance()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TextSecureDirectory();
                    }
                }
            }

            return instance;
        }

        private TextSecureDirectory()
        {
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DIRECTORY_PATH);

            conn.CreateTable<Directory>();
        }

        public Task<List<Directory>> getDirectories()
        {
            /*SQLiteDatabase database = databaseHelper.getReadableDatabase();
            Cursor cursor = database.query(TABLE_NAME, null, null, null, null, null, null);

            if (cursor != null)
                cursor.setNotificationUri(context.getContentResolver(), CHANGE_URI);*/
            var query = conn.Table<Directory>().Where(v => true);

            return query.ToList();
        }

        public bool isSmsFallbackSupported(String e164number)
        {
            var query = conn.Table<Directory>().Where(v => v.number == e164number);

            if (query != null)
            {
                return query.First().supports_sms == 1;
            }
            else
            {
                return false;
            }

        }

        public bool isActiveNumber(String e164number)// throws NotInDirectoryException
        {
            if (e164number == null || e164number.Length == 0)
            {
                return false;
            }

            var query = conn.Table<Directory>().Where(v => v.number == e164number);

            if (query != null)
            {
                return query.First().registered == 1;
            }
            else
            {
                return false;
            }

        }

        public String getRelay(String e164number)
        {
            var query = conn.Table<Directory>().Where(v => v.number == e164number);

            if (query != null)
            {
                return query.First().relay;
            }
            else
            {
                return "";
            }

        }

        public void setNumber(ContactTokenDetails token, bool active)
        {
            Directory dir = new Directory()
            {
                number = token.getNumber(),
                relay = token.getRelay(),
                registered = active ? (uint)1 : 0,
                supports_sms = token.isSupportsSms() ? (uint)1 : 0,
                timestamp = (uint)Helper.getTime()
            };

            conn.InsertOrReplace(dir);
        }

        public void setNumbers(List<ContactTokenDetails> activeTokens, ICollection<String> inactiveTokens)
        {
            ulong timestamp = Helper.getTime();

            try
            {
                foreach (ContactTokenDetails token in activeTokens)
                {
                    Debug.WriteLine("Directory: Adding active token: " + token.getNumber() + ", " + token.getToken());
                    Directory dir = new Directory()
                    {
                        number = token.getNumber(),
                        relay = token.getRelay(),
                        registered = 1,
                        supports_sms = token.isSupportsSms() ? (uint)1 : 0,
                        timestamp = (uint)Helper.getTime()
                    };

                    conn.InsertOrReplace(dir);
                }

                foreach (String token in inactiveTokens)
                {
                    Directory dir = new Directory()
                    {
                        number = token,
                        registered = 0,
                        timestamp = (uint)Helper.getTime()
                    };

                    conn.InsertOrReplace(dir);

                }

            }
            finally
            {
            }
        }

        public async Task<List<String>> getPushEligibleContactNumbers(String localNumber)
        {
            /**final Uri         uri = Phone.CONTENT_URI;
            final Set< String > results = new HashSet<String>();
            Cursor cursor = null;*/
            



            try
            {
                var contactStore = await ContactManager.RequestStoreAsync();
                var contacts = await contactStore.FindContactsAsync();

                IList<string> results = new List<string>();

                foreach (var contact in contacts)
                {
                    foreach (var number in contact.Phones)
                    {
                        try
                        {
                            string e164number = PhoneNumberFormatter.formatNumber(number.Number, localNumber);
                            results.Add(number.Number); //apply formatting
                        }
                        catch (InvalidNumberException e)
                        {
                            Debug.WriteLine($"Directory: Invalid number: {number}");
                        }

                    }
                }

                var query = conn.Table<Directory>().Where(t => true);
                foreach (var contact in query)
                {
                    results.Add(PhoneNumberFormatter.formatNumber(contact.number, localNumber)); //apply formatting
                }

                return results.Distinct().ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                /* if (cursor != null)
                     cursor.close();*/
            }
        }

        public async Task<List<Directory>> getActiveDirectoryList()
        {
            /*final List< String > results = new ArrayList<String>();
            Cursor cursor = null;*/
            try
            {
                var query = conn.Table<Directory>().Where(t => t.registered == 1);
                /*cursor = databaseHelper.getReadableDatabase().query(TABLE_NAME, new String[] { NUMBER },
                    REGISTERED + " = 1", null, null, null, null);

                while (cursor != null && cursor.moveToNext())
                {
                    results.add(cursor.getString(0));
                }*/
                return query.ToList();
            }
            finally
            {
                /*if (cursor != null)
                    cursor.close();*/
            }
        }

        public async Task<List<String>> getActiveNumbers()
        {
            /*final List< String > results = new ArrayList<String>();
            Cursor cursor = null;*/
            try
            {
                var query = conn.Table<Directory>().Where(t => t.registered == 1);
                var list =  query.ToList();
                /*cursor = databaseHelper.getReadableDatabase().query(TABLE_NAME, new String[] { NUMBER },
                    REGISTERED + " = 1", null, null, null, null);

                while (cursor != null && cursor.moveToNext())
                {
                    results.add(cursor.getString(0));
                }*/
                return list.Select(t => t.number).ToList();
            }
            finally
            {
                /*if (cursor != null)
                    cursor.close();*/
            }
        }

    }
}
