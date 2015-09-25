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
using SQLite;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database.mappings;

namespace TextSecure.crypto.storage
{
    public class TextSecurePreKeyStore : PreKeyStore, SignedPreKeyStore
    {

        public static readonly String PREKEY_DIRECTORY = "prekeys.db";
        private static readonly String PREKEY_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, PREKEY_DIRECTORY);
        //public static readonly String SIGNED_PREKEY_DIRECTORY = "signed_prekeys";


        private static readonly uint CURRENT_VERSION_MARKER = 1;

        SQLiteConnection conn;
        
        public TextSecurePreKeyStore()
        {
            
            conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), PREKEY_PATH);

            init();

        }

        private void init()
        {
            conn.CreateTable<PreKeyRecordMapping>();
            conn.CreateTable<PreKeyRecordIndexMapping>();
            conn.CreateTable<SignedPreKeyRecordMapping>();
            conn.CreateTable<SignedPreKeyRecordIndexMapping>();

        }

        public bool containsPreKey(uint preKeyId)
        {
            var query = conn.Table<PreKeyRecordMapping>().Where(v => v.id.Equals(preKeyId));
            return query.Count() == 1;
        }

        public bool containsSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Table<SignedPreKeyRecordMapping>().Where(v => v.id.Equals(signedPreKeyId));
            return query.Count() == 1;
        }

        public PreKeyRecord loadPreKey(uint preKeyId)
        {
            var query = conn.Table<PreKeyRecordMapping>().Where(v => v.id.Equals(preKeyId));
            var preKey = query.First();
            return new PreKeyRecord(preKey.record);
        }

        public SignedPreKeyRecord loadSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Table<SignedPreKeyRecordMapping>().Where(v => v.id.Equals(signedPreKeyId));
            var signedPreKey = query.First();
            return new SignedPreKeyRecord(signedPreKey.record);
        }

        public List<SignedPreKeyRecord> loadSignedPreKeys()
        {
            var query = conn.Table<SignedPreKeyRecordMapping>().Where(v => true);
            var signedPreKey = query.ToList().Select(x => new SignedPreKeyRecord(x.record));
            return signedPreKey.ToList();
        }

        public void removePreKey(uint preKeyId)
        {
            var query = conn.Delete(preKeyId);//.Table<SignedPreKeyRecordMapping>().Where(v => v.id.Equals(signedPreKeyId));
            return;//  query == preKeyId;
        }

        public void removeSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Delete(signedPreKeyId);
            return;// query.Count() == 1;
        }

        public void storePreKey(uint preKeyId, PreKeyRecord record)
        {
            conn.Insert(new PreKeyRecordMapping() { id = preKeyId, record = record.serialize() });
            return;
        }

        public void storeSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            conn.Insert(new SignedPreKeyRecordMapping() { id = signedPreKeyId, record = record.serialize() });
            return;
        }
    }
}
