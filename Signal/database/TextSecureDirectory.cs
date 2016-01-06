

using GalaSoft.MvvmLight;
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
using Signal.Util;
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

namespace Signal.Database
{

    public class TextSecureDirectory
    {
        public class Directory : ObservableObject
        {
            [PrimaryKey, AutoIncrement]
            public int DirectoryId { get; set; }
            [Unique]
            public string Number { get; set; }
            public string Name { get; set; }
            public long Registered { get; set; }
            public string Relay { get; set; }
            public DateTime Time { get; set; }
            public string ContactId { get; set; }
        }


        private static readonly int INTRODUCED_CHANGE_FROM_TOKEN_TO_E164_NUMBER = 2;

        private static readonly String DATABASE_NAME = "directory.db";
        private static readonly String DIRECTORY_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecureDirectory.DATABASE_NAME);
        private static readonly int DATABASE_VERSION = 2;


        private static readonly Object instanceLock = new Object();
        private static volatile TextSecureDirectory instance;
        private SQLiteConnection conn;

        /*public static TextSecureDirectory getInstance()
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
        }*/

        public TextSecureDirectory(SQLiteConnection conn)
        {
            this.conn = conn;
            //conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DIRECTORY_PATH);

            conn.CreateTable<Directory>();
        }

        public Directory Get(long directoryId)
        {
            return conn.Get<Directory>(directoryId);
        }

        public List<Directory> GetAll()
        {
            var query = conn.Table<Directory>().Where(v => true);

            return query.ToList();
        }

        public async Task<List<Directory>> GetAllAsync()
        {
            var query = conn.Table<Directory>().Where(v => true);

            return query.ToList();
        }

        public Directory GetForNumber(string number)
        {
            var query = conn.Table<Directory>().Where(d => d.Number == number);

            return query.Count() == 1 ? query.First() : null;
        }

        public List<string> GetNumbers(string number)
        {
            var query = conn.Table<Directory>().Where(d => true);

            return query.Select(d => d.Number).ToList();
        }

        public async Task<List<string>> GetNumbersAsync(string number)
        {
            await ReloadLocalContacts(number);
            var query = conn.Table<Directory>().Where(d => true);

            return query.Select(d => d.Number).ToList();
        }

        public string GetNumberForId(long recipientId)
        {
            return Get(recipientId).Number;
        }
        /*public bool isSmsFallbackSupported(String e164number)
        {
            var query = conn.Table<Directory>().Where(v => v.Number == e164number);

            if (query != null)
            {
                return query.First().supports_sms == 1;
            }
            else
            {
                return false;
            }

        }*/

        public bool isActiveNumber(String e164number)// throws NotInDirectoryException
        {
            if (e164number == null || e164number.Length == 0)
            {
                return false;
            }

            var query = conn.Table<Directory>().Where(v => v.Number == e164number);

            if (query != null)
            {
                return query.First().Registered == 1;
            }
            else
            {
                return false;
            }

        }

        public String getRelay(String e164number)
        {
            var query = conn.Table<Directory>().Where(v => v.Number == e164number);

            if (query != null)
            {
                return query.First().Relay;
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
                Number = token.getNumber(),
                Relay = token.getRelay(),
                Registered = active ? (uint)1 : 0,
                //supports_sms = token.isSupportsSms() ? (uint)1 : 0,
                Time = TimeUtil.GetDateTimeMillis()
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

                    var directory = GetForNumber(token.getNumber());

                    directory.Relay = token.getRelay();
                    directory.Registered = 1;
                    directory.Time = TimeUtil.GetDateTimeMillis();

                    conn.Update(directory);

                }

                foreach (String token in inactiveTokens)
                {
                    var directory = GetForNumber(token);

                    directory.Relay = null;
                    directory.Registered = 0;
                    directory.Time = TimeUtil.GetDateTimeMillis();

                    conn.Update(directory);

                }

            }
            finally
            {
            }
        }

        private async Task ReloadLocalContacts(string localNumber)
        {
            try
            {
                var contactStore = await ContactManager.RequestStoreAsync();
                var contacts = await contactStore.FindContactsAsync();

                foreach (var contact in contacts)
                {
                    //Debug.WriteLine($"Name: {contact.DisplayName}");
                    foreach (var number in contact.Phones)
                    {
                        //Debug.WriteLine($"Number: {number.Number}");
                        try
                        {
                            string e164number = PhoneNumberFormatter.formatNumber(number.Number, localNumber);

                            var directory = GetForNumber(e164number);

                            if (directory != null)
                            {
                                directory.Name = contact.DisplayName;
                                directory.Time = TimeUtil.GetDateTimeMillis();
                                directory.ContactId = contact.Id;

                                conn.Update(directory);
                            } else
                            {
                                var newdir = new Directory()
                                {
                                    Name = contact.DisplayName,
                                    Number = e164number,
                                    Relay = null,
                                    Registered = 0,
                                    Time = TimeUtil.GetDateTimeMillis(),
                                    ContactId = contact.Id
                                };
                                conn.Insert(newdir);
                            }

                           

                        }
                        catch (InvalidNumberException e)
                        {
                            Debug.WriteLine($"Directory: Invalid number: {number}");
                        }
                        catch (SQLiteException e)
                        {
                            if (e.Message.Equals("Constraint")) continue;
                        }

                    }
                }


            }
            catch (Exception e) { }
       }

        /*public async Task<List<Tuple<string, Contact>>> getPushEligibleContactNumbers(String localNumber)
        {



            try
            {
                var contactStore = await ContactManager.RequestStoreAsync();
                var contacts = await contactStore.FindContactsAsync();

                IList<Tuple<string, Contact>> results = new List<Tuple<string, Contact>>();

                foreach (var contact in contacts)
                {
                    foreach (var number in contact.Phones)
                    {
                        try
                        {
                            string e164number = PhoneNumberFormatter.formatNumber(number.Number, localNumber);
                            results.Add(Tuple.Create(number.Number, contact)); //apply formatting
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
                    //results.Add(Tuple.Create(PhoneNumberFormatter.formatNumber(contact.Number, localNumber), contact.)); //apply formatting
                }

                return results.Distinct().ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
            }
        }*/

        public async Task<List<Directory>> getActiveDirectoryList()
        {
            /*final List< String > results = new ArrayList<String>();
            Cursor cursor = null;*/
            try
            {
                var query = conn.Table<Directory>().Where(t => t.Registered == 1);
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
                var query = conn.Table<Directory>().Where(t => t.Registered == 1);
                var list = query.ToList();
                /*cursor = databaseHelper.getReadableDatabase().query(TABLE_NAME, new String[] { NUMBER },
                    REGISTERED + " = 1", null, null, null, null);

                while (cursor != null && cursor.moveToNext())
                {
                    results.add(cursor.getString(0));
                }*/
                return list.Select(t => t.Number).ToList();
            }
            finally
            {
                /*if (cursor != null)
                    cursor.close();*/
            }
        }

    }
}
