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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.crypto.storage;
using TextSecure.database.mappings;
using Windows.Security.Cryptography;

namespace TextSecure.crypto
{
    public class PreKeyUtil
    {
        public static readonly uint BATCH_SIZE = 100;

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
            PreKeyStore preKeyStore = new TextSecurePreKeyStore();
            List<PreKeyRecord> records = new List<PreKeyRecord>();
            uint preKeyIdOffset = getNextPreKeyId();

            for (uint i = 0; i < BATCH_SIZE; i++)
            {
                uint preKeyId = (preKeyIdOffset + i) % Medium.MAX_VALUE;

                PreKeyRecord record = await generatePreKey(preKeyId);

                preKeyStore.storePreKey(preKeyId, record);
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
                SignedPreKeyStore signedPreKeyStore = new TextSecurePreKeyStore();
                uint signedPreKeyId = getNextSignedPreKeyId();
                /*ECKeyPair keyPair = Curve.generateKeyPair();
                byte[] signature = Curve.calculateSignature(identityKeyPair.getPrivateKey(), keyPair.getPublicKey().serialize());
                SignedPreKeyRecord record = new SignedPreKeyRecord(signedPreKeyId, KeyHelper.getTime(), keyPair, signature);*/
                SignedPreKeyRecord record = await generateSignedPreKey(identityKeyPair, signedPreKeyId);

                signedPreKeyStore.storeSignedPreKey(signedPreKeyId, record);
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

                PreKeyStore preKeyStore = new TextSecurePreKeyStore();

                if (preKeyStore.containsPreKey(Medium.MAX_VALUE))
                {
                    try
                    {
                        //return preKeyStore.loadPreKey(Medium.MAX_VALUE);
                        return preKeyStore.loadPreKey(Medium.MAX_VALUE);
                    }
                    catch (InvalidKeyIdException e)
                    {
                        //Log.w("PreKeyUtil", e);
                        preKeyStore.removePreKey(Medium.MAX_VALUE);
                    }
                }

                ECKeyPair keyPair = Curve.generateKeyPair();
                PreKeyRecord record = new PreKeyRecord(Medium.MAX_VALUE, keyPair);

                preKeyStore.storePreKey(Medium.MAX_VALUE, record);

                return record;
            });
        }

        private static void setNextPreKeyId(uint id)
        { // for now do this
            SQLiteConnection conn = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecurePreKeyStore.PREKEY_DIRECTORY), false);

            var preKey = new PreKeyRecordIndexMapping { id = 1, next = id };
            conn.Update(preKey);

            /*try
            {
                File nextFile = new File(getPreKeysDirectory(context), PreKeyIndex.FILE_NAME);
                FileOutputStream fout = new FileOutputStream(nextFile);
                fout.write(JsonUtils.toJson(new PreKeyIndex(id)).getBytes());
                fout.close();
            }
            catch (IOException e)
            {
                Log.w("PreKeyUtil", e);
            }*/
        }

        private static void setNextSignedPreKeyId(uint id)
        {
            SQLiteConnection conn = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecurePreKeyStore.PREKEY_DIRECTORY));
            var signedPreKey = new SignedPreKeyRecordIndexMapping { id = 1, next = id };
            conn.Update(signedPreKey);



            //conn.Execute($"update SignedPreKeyRecordMappings SET next = true WHERE id = {id}");
            //conn.Update();
            /*try
            {
                File nextFile = new File(getSignedPreKeysDirectory(context), SignedPreKeyIndex.FILE_NAME);
                FileOutputStream fout = new FileOutputStream(nextFile);
                fout.write(JsonUtils.toJson(new SignedPreKeyIndex(id)).getBytes());
                fout.close();
            }
            catch (IOException e)
            {
                Log.w("PreKeyUtil", e);
            }*/
        }

        private static uint getNextPreKeyId()
        {
            SQLiteConnection conn = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecurePreKeyStore.PREKEY_DIRECTORY));
            var query = conn.Table<PreKeyRecordIndexMapping>().Where(v => v.id == 1);

            if (query.Count() == 0)
            {
                return CryptographicBuffer.GenerateRandomNumber() % Medium.MAX_VALUE;
            } else
            {
                var preKey = query.First();
                return preKey.id;
            }
            
            /*try
            {
                File nextFile = new File(getPreKeysDirectory(context), PreKeyIndex.FILE_NAME);

                if (!nextFile.exists())
                {
                    return Util.getSecureRandom().nextInt(Medium.MAX_VALUE);
                }
                else
                {
                    InputStreamReader reader = new InputStreamReader(new FileInputStream(nextFile));
                    PreKeyIndex index = JsonUtils.fromJson(reader, PreKeyIndex.class);
        reader.close();
        return index.nextPreKeyId;
      }
} catch (IOException e) {
      Log.w("PreKeyUtil", e);
      return Util.getSecureRandom().nextInt(Medium.MAX_VALUE);
    }*/
        }

        private static uint getNextSignedPreKeyId()
        {
            SQLiteConnection conn = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, TextSecurePreKeyStore.PREKEY_DIRECTORY));
            var query = conn.Table<PreKeyRecordMapping>().Where(v => v.id == 1);
            

            if (query.Count() == 0)
            {
                return CryptographicBuffer.GenerateRandomNumber() % Medium.MAX_VALUE;
            }
            else
            {
                var preKey = query.First();
                return preKey.id;
            }
            /* try
             {
                 File nextFile = new File(getSignedPreKeysDirectory(context), SignedPreKeyIndex.FILE_NAME);

                 if (!nextFile.exists())
                 {
                     return Util.getSecureRandom().nextInt(Medium.MAX_VALUE);
                 }
                 else
                 {
                     InputStreamReader reader = new InputStreamReader(new FileInputStream(nextFile));
                     SignedPreKeyIndex index = JsonUtils.fromJson(reader, SignedPreKeyIndex.class);
         reader.close();
         return index.nextSignedPreKeyId;
       }
 } catch (IOException e) {
       Log.w("PreKeyUtil", e);
       return Util.getSecureRandom().nextInt(Medium.MAX_VALUE);
     }*/
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
