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
using SQLite.Net;
using Signal.Models;
using System.Threading.Tasks;
using libtextsecure.util;
using Signal.Util;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Signal.Database
{
    public class ContactsDatabase
    {
        SQLiteConnection conn;

        public ContactsDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Contact>();
        }

        public Contact Get(long contactId)
        {
            return conn.Get<Contact>(contactId);
        }

        public async Task<List<string>> GetNumbersAsync(string number)
        {
            await ReloadLocalContacts(number);
            var query = conn.Table<Contact>().Where(d => true);

            return query.Select(d => d.Number).ToList();
        }

        public Contact GetForNumber(string number)
        {
            var query = conn.Table<Contact>().Where(d => d.Number == number);

            return query.First();
        }

        private async Task ReloadLocalContacts(string localNumber)
        {
            var contactStore = await Windows.ApplicationModel.Contacts.ContactManager.RequestStoreAsync();
            var contacts = await contactStore.FindContactsAsync();

            foreach (var contact in contacts)
            {
                foreach (var number in contact.Phones)
                {
                    try
                    {
                        string e164number = PhoneNumberFormatter.formatNumber(number.Number, localNumber);

                        var directory = GetForNumber(e164number);

                        
                            

                        if (directory != null)
                        {
                            directory.Name = contact.DisplayName;
                            directory.Number = e164number;
                            directory.SystemId = contact.Id;
                            conn.Update(directory);
                        }
                        else
                        {
                            var newdir = new Contact()
                            {
                                Name = contact.DisplayName,
                                Number = e164number,
                                SystemId = contact.Id
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

    }
}