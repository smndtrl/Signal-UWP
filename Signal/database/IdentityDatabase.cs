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

using libaxolotl;
using SQLite;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;

namespace TextSecure.database
{
    public class IdentityDatabase
    {
        private const String TABLE_NAME = "identities";
        private static readonly String ID = "_id";
        public static readonly String RECIPIENT = "recipient";
        public static readonly String IDENTITY_KEY = "key";
        public static readonly String MAC = "mac";

        private static readonly String DIRECTORY_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "identity.db");

        [Table(TABLE_NAME)]
        public class Identity
        {
            [PrimaryKey, AutoIncrement]
            public long? _id { get; set; } = null;
            [Unique]
            public long recipient { get; set; }
            public string key { get; set; }
            public string mac { get; set; }
        }


        SQLiteConnection conn;

        public IdentityDatabase()
        {
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), DIRECTORY_PATH) { };


            conn.CreateTable<Identity>();
        }

        public IList<Identity> getIdentities()
        {
            /*SQLiteDatabase database = databaseHelper.getReadableDatabase();
            Cursor cursor = database.query(TABLE_NAME, null, null, null, null, null, null);

            if (cursor != null)
                cursor.setNotificationUri(context.getContentResolver(), CHANGE_URI);*/
            var query = conn.Table<Identity>().Where(v => true);

            return query.ToList();
        }

        public bool isValidIdentity(long recipientId,
                               IdentityKey theirIdentity)
        {
            try
            {
                /*cursor = database.query(TABLE_NAME, null, RECIPIENT + " = ?",
                                        new String[] { recipientId + "" }, null, null, null);*/

                var query = conn.Table<Identity>().Where(v => v.recipient.Equals(recipientId));

                if (query != null &&  query.Count() > 0)
                {
                    var key = query.First();
                    String serializedIdentity = key.key;
                    String mac = key.mac;

                   /* if (!masterCipher.verifyMacFor(recipientId + serializedIdentity, Base64.decode(mac))) TODO: verify mac
                    {
                        //Log.w("IdentityDatabase", "MAC failed");
                        return false;
                    }*/

                    IdentityKey ourIdentity = new IdentityKey(Base64.decode(serializedIdentity), 0);

                    return ourIdentity.Equals(theirIdentity);
                }
                else
                {
                    return true;
                }
            }
            catch (IOException e)
            {
                //Log.w("IdentityDatabase", e);
                return false;
            }
            catch (InvalidKeyException e)
            {
                //Log.w("IdentityDatabase", e);
                return false;
            }
            finally
            {
                /*if (cursor != null)
                {
                    cursor.close();
                }*/
            }
        }

        public long saveIdentity(long recipientId, IdentityKey identityKey)
        {
            String identityKeyString = Base64.encodeBytes(identityKey.serialize());
            /*String macString = Base64.encodeBytes(masterCipher.getMacFor(recipientId +
                                                                                      identityKeyString));*/ // TODO: enable mac gen

            /*ContentValues contentValues = new ContentValues();
            contentValues.put(RECIPIENT, recipientId);
            contentValues.put(IDENTITY_KEY, identityKeyString);
            contentValues.put(MAC, macString);*/

            var identity = new Identity() { recipient = recipientId, key = identityKeyString, mac = Base64.encode(identityKeyString) };

            return conn.InsertOrReplace(identity);

            /*database.replace(TABLE_NAME, null, contentValues);

            context.getContentResolver().notifyChange(CHANGE_URI, null);*/
        }

        public long deleteIdentity(long id)
        {
            return conn.Table<Identity>().Delete(t => t.recipient == id);
            //return await conn.Delete(new Identity() { recipient = id });
            /*SQLiteDatabase database = databaseHelper.getWritableDatabase();
            database.delete(TABLE_NAME, ID_WHERE, new String[] { id + "" });

            context.getContentResolver().notifyChange(CHANGE_URI, null);*/
        }
    }
}
