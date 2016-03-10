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
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.crypto.storage
{
    [Table("PreKeys")]
    public class PreKeyRecordI
    {
        [PrimaryKey, AutoIncrement]
        public uint PreKeyId { get; set; }
        public byte[] Record { get; set; }
    }

    [Table("PreKeyIndex")]
    public class PreKeyIndex
    {
        [PrimaryKey]
        public uint PreyKeyIndex { get; set; }
        public uint Next { get; set; }
    }

    [Table("SignedPreKeys")]
    public class SignedPreKeyRecordI
    {
        [PrimaryKey, AutoIncrement]
        public uint SignedPreKeyId { get; set; }
        public byte[] Record { get; set; }
    }

    [Table("SingedPreKeyIndex")]
    public class SignedPreKeyIndex
    {
        [PrimaryKey]
        public uint SignedPreyKeyIndex { get; set; }
        public uint Next { get; set; }
    }

    public class TextSecurePreKeyStore : PreKeyStore, SignedPreKeyStore
    {


        private static readonly uint CURRENT_VERSION_MARKER = 1;

        SQLiteConnection conn;

        public TextSecurePreKeyStore(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<PreKeyRecordI>();
            conn.CreateTable<PreKeyIndex>();
            conn.CreateTable<SignedPreKeyRecordI>();
            conn.CreateTable<SignedPreKeyIndex>();
        }

        public bool ContainsPreKey(uint preKeyId)
        {
            var query = conn.Table<PreKeyRecordI>().Where(k => k.PreKeyId.Equals(preKeyId));

            return query.Any();
        }

        public bool ContainsSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Table<SignedPreKeyRecordI>().Where(k => k.SignedPreKeyId.Equals(signedPreKeyId));

            return query.Any();
        }

        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            var preKey = conn.Get<PreKeyRecordI>(preKeyId);
            return new PreKeyRecord(preKey.Record);
        }

        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            var signedPreKey = conn.Get<SignedPreKeyRecordI>(signedPreKeyId);
            return new SignedPreKeyRecord(signedPreKey.Record);
        }

        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            var query = conn.Table<SignedPreKeyRecordI>().Where(v => true);
            return query.ToList().Select(s => new SignedPreKeyRecord(s.Record)).ToList();
        }

        public void RemovePreKey(uint preKeyId)
        {
            var query = conn.Delete(preKeyId);
        }

        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Delete(signedPreKeyId);
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            conn.Insert(new PreKeyRecordI() { PreKeyId = preKeyId, Record = record.serialize() });
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            conn.Insert(new SignedPreKeyRecordI() { SignedPreKeyId = signedPreKeyId, Record = record.serialize() });
        }
    }
}
