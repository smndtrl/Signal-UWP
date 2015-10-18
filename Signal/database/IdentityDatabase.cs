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
using Signal.Util;
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
        private const String TABLE_NAME = "Identities";
        private static readonly String ID = "_id";
        public static readonly String RECIPIENT = "recipient";
        public static readonly String IDENTITY_KEY = "key";
        public static readonly String MAC = "mac";

        [Table(TABLE_NAME)]
        public class Identity
        {
            [PrimaryKey, AutoIncrement]
            public long IdentityId { get; set; }
            [Unique]
            public long RecipientId { get; set; } // TODO: FK
            public string Key { get; set; }
            public string Mac { get; set; }
        }

        public class IdentityKeyRecord
        {
            [PrimaryKey]
            public string name { get; set; }
            public byte[] publicKey { get; set; }
        }

        SQLiteConnection conn;

        public IdentityDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Identity>();
        }

        public IList<Identity> GetIdentities()
        {
            var query = conn.Table<Identity>().Where(v => true);

            return query.ToList();
        }

        public bool IsValidIdentity(long recipientId,
                               IdentityKey theirIdentity)
        {
            try
            {
                var query = conn.Table<Identity>().Where(v => v.RecipientId.Equals(recipientId));

                if (query.Count() > 0)
                {
                    var identity = query.First();
                    String serializedIdentity = identity.Key;
                    String mac = identity.Mac;


                    IdentityKey ourIdentity = new IdentityKey(Base64.decode(serializedIdentity), 0);

                    return ourIdentity.Equals(theirIdentity);
                }
                else
                {
                    return true;
                }
            }
            catch (IOException e) { return false; }
            catch (InvalidKeyException e) { return false; }
            catch (Exception e) { return false; }
            
        }
        
        private bool HasIdentity(long recipientId)
        {
            var query = conn.Table<Identity>().Where(i => i.RecipientId == recipientId);
            return query.Count() == 1;
        }

        public long SaveIdentity(long recipientId, IdentityKey identityKey)
        {
            String identityKeyString = Base64.encodeBytes(identityKey.serialize()); // TODO: real mac
            var identity = new Identity() { RecipientId = recipientId, Key = identityKeyString, Mac = Base64.encode(identityKeyString) };

            return conn.InsertOrReplace(identity);
        }

        public long DeleteIdentity(long id)
        {
            return conn.Table<Identity>().Delete(i => i.RecipientId == id);
        }
    }
}
