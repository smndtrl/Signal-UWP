

using Signal.Database;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Database
{


    public class DatabaseFactory
    {
        public static string APPDB_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "app.db");

        private static Object locker = new Object();
        private static DatabaseFactory instance;

        private IdentityDatabase identityDatabase;
        private ThreadDatabase threadDatabase;
        private GroupDatabase groupDatabase;
        private MessageDatabase messageDatabase;
        private TextMessageDatabase textMessageDatabase;
        private PushDatabase pushDatabase;

        private RecipientDatabase recipientDatabase;
        //private ContactsDatabase contactDatabase;
        //private CanonicalAddressDatabase addressDatabase;
        private TextSecureDirectory directoryDatabase;

        private SQLiteConnection connection;

        public static DatabaseFactory getInstance()
        {

            lock (locker)
            {
                if (instance == null)
                    instance = new DatabaseFactory();

                return instance;
            }
        }

        public static IdentityDatabase getIdentityDatabase()
        {
            return getInstance().identityDatabase;
        }

        public static ThreadDatabase getThreadDatabase()
        {
            return getInstance().threadDatabase;
        }

        public static GroupDatabase getGroupDatabase()
        {
            return getInstance().groupDatabase;
        }

        public static MessageDatabase getMessageDatabase()
        {
            return getInstance().messageDatabase;
        }

        public static TextMessageDatabase getTextMessageDatabase()
        {
            return getInstance().textMessageDatabase;
        }

        /*public static ContactsDatabase getContactsDatabase()
        {
            return getInstance().contactDatabase;
        }*/

       /* public static CanonicalAddressDatabase getCanonicalAddressDatabase()
        {
            return getInstance().addressDatabase;
        }*/

        public static TextSecureDirectory getDirectoryDatabase()
        {
            return getInstance().directoryDatabase;
        }

        public static PushDatabase getPushDatabase()
        {
            return getInstance().pushDatabase;
        }

        public static RecipientDatabase getRecipientDatabase()
        {
            return getInstance().recipientDatabase;
        }

        private DatabaseFactory()
        {

           connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), APPDB_PATH);
           

            /*    var sync = new SQLiteConnection(APPDB_PATH);
            connection = new SQLiteConnection(APPDB_PATH);*/

            identityDatabase = new IdentityDatabase(connection);
            threadDatabase = new ThreadDatabase(connection);
            groupDatabase = new GroupDatabase(connection);
            messageDatabase = new MessageDatabase(connection);
            textMessageDatabase = new TextMessageDatabase(connection);

            //contactDatabase = new ContactsDatabase(connection);
            //addressDatabase = new CanonicalAddressDatabase(connection);
            directoryDatabase = new TextSecureDirectory(connection);
            pushDatabase = new PushDatabase(connection);
            recipientDatabase = new RecipientDatabase(connection);
        }
    }


}
