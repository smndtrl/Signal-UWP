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
        [Table("sessions")]
        private class Session
        {
            [AutoIncrement, PrimaryKey]
            private long _id { get; set; }
            public string name { get; set; }
            public long deviceId { get; set; }
            public byte[] record { get; set; }
        }

        SQLiteConnection conn;

        public TextSecureSessionStore()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "session.db");
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path) {  };

            conn.CreateTable<Session>();
        }

        public bool containsSession(AxolotlAddress address)
        {
            var name = address.getName();
            var deviceId = address.getDeviceId();
            var query = conn.Table<Session>().Where(v => v.name == name && v.deviceId == deviceId);
            return query.Count() != 0;
        }

        public void deleteAllSessions(string name)
        {
            var query = conn.Table<Session>().Delete(t => t.name == name);
        }

        public void deleteSession(AxolotlAddress address)
        {
            var name = address.getName();
            var deviceId = address.getDeviceId();
            var query = conn.Table<Session>().Delete(t => t.name == name && t.deviceId == deviceId);
        }

        public List<uint> getSubDeviceSessions(string name)
        {
            var query = conn.Table<Session>().Where(t => t.name == name);
            var list = query.ToList();
            var output = list.Select(t => (uint)t.deviceId).ToList();
            return output;
        }

        public SessionRecord loadSession(AxolotlAddress address)
        {
            var name = address.getName();
            var deviceId = address.getDeviceId();
            var query = conn.Table<Session>().Where(t => t.name == name && t.deviceId == deviceId);

            if (query != null && query.Count() > 0)
            {
                return new SessionRecord(query.First().record);
            }
            else
            {
                return new SessionRecord();
            }
            
        }

        public void storeSession(AxolotlAddress address, SessionRecord record)
        {
            deleteSession(address); // TODO: sqlite-net combined private keys for insertOrReplace

            var session = new Session() { deviceId = address.getDeviceId(), name = address.getName(), record = record.serialize() };
            conn.InsertOrReplace(session);
            return;
        }
    }
}
