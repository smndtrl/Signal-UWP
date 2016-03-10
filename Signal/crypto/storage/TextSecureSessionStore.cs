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

using libaxolotl.state;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl;
using SQLite;
using System.IO;
using SQLite.Net.Attributes;
using SQLite.Net;

namespace TextSecure.crypto.storage
{
    public class TextSecureSessionStore : SessionStore
    {
        [Table("Sessions")]
        private class Session
        {
            [AutoIncrement, PrimaryKey]
            private long SessionId { get; set; }
            public string Name { get; set; } // TODO:: K AxolotlAddress
            public long DeviceId { get; set; } // TODO:: K AxolotlAddress
            public byte[] Record { get; set; }
        }

        SQLiteConnection conn;

        public TextSecureSessionStore(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Session>();
        }

        public bool ContainsSession(AxolotlAddress address)
        {
            var name = address.Name;
            var deviceId = address.DeviceId;
            var query = conn.Table<Session>().Where(v => v.Name == name && v.DeviceId == deviceId);

            return query.Count() != 0;
        }

        public void DeleteAllSessions(string name)
        {
            var query = conn.Table<Session>().Delete(t => t.Name == name);
        }

        public void DeleteSession(AxolotlAddress address)
        {
            var name = address.Name;
            var deviceId = address.DeviceId;
            var query = conn.Table<Session>().Delete(t => t.Name == name && t.DeviceId == deviceId);
        }

        public List<uint> GetSubDeviceSessions(string name)
        {
            var query = conn.Table<Session>().Where(t => t.Name == name);
            var list = query.ToList();
            var output = list.Select(t => (uint)t.DeviceId).ToList();
            return output;
        }

        public SessionRecord LoadSession(AxolotlAddress address)
        {
            var name = address.Name;
            var deviceId = address.DeviceId;
            var query = conn.Table<Session>().Where(t => t.Name == name && t.DeviceId == deviceId);

            if (query != null && query.Any())
            {
                return new SessionRecord(query.First().Record);
            }
            else
            {
                return new SessionRecord();
            }
            
        }

        public void StoreSession(AxolotlAddress address, SessionRecord record)
        {
            DeleteSession(address); // TODO: sqlite-net combined private keys for insertOrReplace

            var session = new Session() { DeviceId = address.DeviceId, Name = address.Name, Record = record.serialize() };
            conn.InsertOrReplace(session);
            return;
        }
    }
}
