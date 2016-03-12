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
using libaxolotl;
using Signal.Util;

namespace TextSecure.crypto.storage
{
    [Table("PreKeys")]
    public class PreKeyRecordI
    {
        [PrimaryKey]
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
        [PrimaryKey]
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

        [DebuggerHidden]
        public PreKeyRecord LoadPreKey(uint preKeyId)
        {
            try
            {
                var preKey = conn.Get<PreKeyRecordI>(preKeyId);
                return new PreKeyRecord(preKey.Record);
            }
            catch (Exception e)
            {
                throw new InvalidKeyIdException(e);
            }

        }

        /// <summary>
        /// Loads a SignedPreKey
        /// </summary>
        /// <param name="signedPreKeyId">Used to lookup a SignedPreKey.</param>
        /// <exception cref="InvalidKeyIdException"></exception>
        /// <returns>Returns a SignedPreKeyRecord if found.</returns>
        [DebuggerHidden] // please do not break
        public SignedPreKeyRecord LoadSignedPreKey(uint signedPreKeyId)
        {
            try
            {
                var signedPreKey = conn.Get<SignedPreKeyRecordI>(signedPreKeyId);
                return new SignedPreKeyRecord(signedPreKey.Record);
            }
            catch (Exception e)
            {
                throw new InvalidKeyIdException(e);
            }
            
        }

        /// <summary>
        /// Loads a List of SignedPreKeyRecords
        /// </summary>
        /// <returns></returns>
        public List<SignedPreKeyRecord> LoadSignedPreKeys()
        {
            List<SignedPreKeyRecord> results = new List<SignedPreKeyRecord>();

            try
            {
                var query = conn.Table<SignedPreKeyRecordI>().Where(v => true);
                results = query.ToList().Select(s => new SignedPreKeyRecord(s.Record)).ToList();
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }

            return results;

        }

        public void RemovePreKey(uint preKeyId)
        {
            var query = conn.Delete<PreKeyRecordI>(preKeyId);
        }

        /// <summary>
        /// Removes a SignedPreKeyRecord
        /// </summary>
        /// <param name="signedPreKeyId">Used to identify a SignedPreKeyRecord.</param>
        public void RemoveSignedPreKey(uint signedPreKeyId)
        {
            var query = conn.Delete<SignedPreKeyRecordI>(signedPreKeyId);
        }

        public void StorePreKey(uint preKeyId, PreKeyRecord record)
        {
            conn.InsertOrReplace(new PreKeyRecordI() { PreKeyId = preKeyId, Record = record.serialize() });
        }

        public void StoreSignedPreKey(uint signedPreKeyId, SignedPreKeyRecord record)
        {
            conn.InsertOrReplace(new SignedPreKeyRecordI() { SignedPreKeyId = signedPreKeyId, Record = record.serialize() });
        }
    }
}
