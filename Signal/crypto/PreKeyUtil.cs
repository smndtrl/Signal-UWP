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
using libaxolotl.ecc;
using libaxolotl.state;
using libaxolotl.util;
using SQLite;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.crypto.storage;
using Windows.Security.Cryptography;

namespace TextSecure.crypto
{
    public class PreKeyUtil
    {
        public static readonly uint BATCH_SIZE = 100;

        public static SQLiteConnection conn = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), TextSecureAxolotlStore.AXOLOTLDB_PATH);

        private static Task<PreKeyRecord> generatePreKey(uint preKeyId)
        {
            return Task.Run(() =>
            {
                ECKeyPair keyPair = Curve.generateKeyPair();
                PreKeyRecord record = new PreKeyRecord(preKeyId, keyPair);

                return record;
            });
        }

        public static async Task<List<PreKeyRecord>> generatePreKeys()
        {
            PreKeyStore preKeyStore = new TextSecurePreKeyStore(conn);
            List<PreKeyRecord> records = new List<PreKeyRecord>();
            uint preKeyIdOffset = getNextPreKeyId();

            for (uint i = 0; i < BATCH_SIZE; i++)
            {
                uint preKeyId = (preKeyIdOffset + i) % Medium.MAX_VALUE;

                PreKeyRecord record = await generatePreKey(preKeyId);

                preKeyStore.StorePreKey(preKeyId, record);
                records.Add(record);
            }

            setNextPreKeyId((preKeyIdOffset + BATCH_SIZE + 1) % Medium.MAX_VALUE);
            return records;
        }

        private static Task<SignedPreKeyRecord> generateSignedPreKey(IdentityKeyPair identityKeyPair, uint signedPreKeyId)
        {
            return Task.Run(() =>
            {
                ECKeyPair keyPair = Curve.generateKeyPair();
                byte[] signature = Curve.calculateSignature(identityKeyPair.getPrivateKey(), keyPair.getPublicKey().serialize());
                SignedPreKeyRecord record = new SignedPreKeyRecord(signedPreKeyId, KeyHelper.getTime(), keyPair, signature);

                return record;
            });
        }

        public static async Task<SignedPreKeyRecord> generateSignedPreKey(IdentityKeyPair identityKeyPair)
        {
            try
            {
                SignedPreKeyStore signedPreKeyStore = new TextSecurePreKeyStore(conn);
                uint signedPreKeyId = getNextSignedPreKeyId();
                /*ECKeyPair keyPair = Curve.generateKeyPair();
                byte[] signature = Curve.calculateSignature(identityKeyPair.getPrivateKey(), keyPair.getPublicKey().serialize());
                SignedPreKeyRecord record = new SignedPreKeyRecord(signedPreKeyId, KeyHelper.getTime(), keyPair, signature);*/
                SignedPreKeyRecord record = await generateSignedPreKey(identityKeyPair, signedPreKeyId);

                signedPreKeyStore.StoreSignedPreKey(signedPreKeyId, record);
                setNextSignedPreKeyId((signedPreKeyId + 1) % Medium.MAX_VALUE);

                return record;
            }
            catch (InvalidKeyException e)
            {
                throw new Exception(e.Message);
            }
        }

        public static Task<PreKeyRecord> generateLastResortKey()
        {
            return Task.Run(() =>
            {

                PreKeyStore preKeyStore = new TextSecurePreKeyStore(conn);

                if (preKeyStore.ContainsPreKey(Medium.MAX_VALUE))
                {
                    try
                    {
                        //return preKeyStore.loadPreKey(Medium.MAX_VALUE);
                        return preKeyStore.LoadPreKey(Medium.MAX_VALUE);
                    }
                    catch (InvalidKeyIdException e)
                    {
                        //Log.w("PreKeyUtil", e);
                        preKeyStore.RemovePreKey(Medium.MAX_VALUE);
                    }
                }

                ECKeyPair keyPair = Curve.generateKeyPair();
                PreKeyRecord record = new PreKeyRecord(Medium.MAX_VALUE, keyPair);

                preKeyStore.StorePreKey(Medium.MAX_VALUE, record);

                return record;
            });
        }

        private static void setNextPreKeyId(uint id)
        {
            var preKey = new PreKeyIndex() { PreyKeyIndex = 1, Next = id };
            conn.Update(preKey);
        }

        private static void setNextSignedPreKeyId(uint id)
        {
            var signedPreKey = new SignedPreKeyIndex { SignedPreyKeyIndex = 1, Next = id };
            conn.Update(signedPreKey);
        }

        private static uint getNextPreKeyId()
        {
            var query = conn.Table<PreKeyIndex>().Where(v => v.PreyKeyIndex == 1);

            if (query.Count() == 0)
            {
                return CryptographicBuffer.GenerateRandomNumber() % Medium.MAX_VALUE;
            } else
            {
                var preKey = query.First();
                return preKey.PreyKeyIndex;
            }
            
        }

        private static uint getNextSignedPreKeyId()
        {
            var query = conn.Table<SignedPreKeyIndex>().Where(v => v.SignedPreyKeyIndex == 1);
            

            if (query.Count() == 0)
            {
                return CryptographicBuffer.GenerateRandomNumber() % Medium.MAX_VALUE;
            }
            else
            {
                var preKey = query.First();
                return preKey.SignedPreyKeyIndex;
            }
        }
        /*
        private static File getPreKeysDirectory(Context context)
        {
            return getKeysDirectory(context, TextSecurePreKeyStore.PREKEY_DIRECTORY);
        }

        private static File getSignedPreKeysDirectory(Context context)
        {
            return getKeysDirectory(context, TextSecurePreKeyStore.SIGNED_PREKEY_DIRECTORY);
        }

        private static File getKeysDirectory(Context context, String name)
        {
            File directory = new File(context.getFilesDir(), name);

            if (!directory.exists())
                directory.mkdirs();

            return directory;
        }

        private static class PreKeyIndex
        {
            public static final String FILE_NAME = "index.dat";

    @JsonProperty
          private int nextPreKeyId;

            public PreKeyIndex() { }

            public PreKeyIndex(int nextPreKeyId)
            {
                this.nextPreKeyId = nextPreKeyId;
            }
        }

        private static class SignedPreKeyIndex
        {
            public static final String FILE_NAME = "index.dat";

    @JsonProperty
          private int nextSignedPreKeyId;

            public SignedPreKeyIndex() { }

            public SignedPreKeyIndex(int nextSignedPreKeyId)
            {
                this.nextSignedPreKeyId = nextSignedPreKeyId;
            }
        }*/
    }
}
