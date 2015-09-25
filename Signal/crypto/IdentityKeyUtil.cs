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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.util;
using Windows.Storage;
namespace TextSecure.crypto
{
    public class IdentityKeyUtil
    {
        private static readonly String IDENTITY_PUBLIC_KEY_DJB_PREF = "pref_identity_public_curve25519";
        private static readonly String IDENTITY_PRIVATE_KEY_DJB_PREF = "pref_identity_private_curve25519";

        public static bool hasIdentityKey()
        {
            return
                ApplicationData.Current.LocalSettings.Values.ContainsKey(IDENTITY_PUBLIC_KEY_DJB_PREF) &&
                ApplicationData.Current.LocalSettings.Values.ContainsKey(IDENTITY_PRIVATE_KEY_DJB_PREF);
        }

        public static IdentityKey getIdentityKey()
        {
            if (!hasIdentityKey()) return null;

            try
            {
                byte[] publicKeyBytes = System.Convert.FromBase64String(retrieve(IDENTITY_PUBLIC_KEY_DJB_PREF));
                return new IdentityKey(publicKeyBytes, 0);
            }
            /*catch (IOException ioe)
            {
                Log.w("IdentityKeyUtil", ioe);
                return null;
            }*/
            catch (InvalidKeyException e)
            {
                //Log.w("IdentityKeyUtil", e);
                return null;
            }
        }

        public static IdentityKeyPair getIdentityKeyPair()
        {
            if (!hasIdentityKey())
                return null;

            try
            {
                //MasterCipher masterCipher = new MasterCipher(masterSecret);
                IdentityKey publicKey = getIdentityKey();
                //ECPrivateKey privateKey = /*masterCipher.decryptKey(*/Base64.decode(retrieve(IDENTITY_PRIVATE_KEY_DJB_PREF))/*)*/;
                ECPrivateKey privateKey = Curve.decodePrivatePoint(Base64.decode(retrieve(IDENTITY_PRIVATE_KEY_DJB_PREF)));

                return new IdentityKeyPair(publicKey, privateKey);
            }
            catch (/*IOException | */InvalidKeyException e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void generateIdentityKeys()
        {
            ECKeyPair djbKeyPair = Curve.generateKeyPair();

            //MasterCipher masterCipher = new MasterCipher(masterSecret);
            IdentityKey djbIdentityKey = new IdentityKey(djbKeyPair.getPublicKey());
            byte[] djbPrivateKey = /*masterCipher.encryptKey(*/djbKeyPair.getPrivateKey().serialize()/*)*/;

            save(IDENTITY_PUBLIC_KEY_DJB_PREF, System.Convert.ToBase64String(djbIdentityKey.serialize()));
            save(IDENTITY_PRIVATE_KEY_DJB_PREF, System.Convert.ToBase64String(djbPrivateKey));
        }

        public static bool hasCurve25519IdentityKeys()
        {
            return
                retrieve(IDENTITY_PUBLIC_KEY_DJB_PREF) != null &&
                retrieve(IDENTITY_PRIVATE_KEY_DJB_PREF) != null;
        }

        public static void generateCurve25519IdentityKeys()
        {
            //MasterCipher masterCipher = new MasterCipher(masterSecret);
            ECKeyPair djbKeyPair = Curve.generateKeyPair();
            IdentityKey djbIdentityKey = new IdentityKey(djbKeyPair.getPublicKey());
            byte[] djbPrivateKey = /*masterCipher.encryptKey(*/djbKeyPair.getPrivateKey().serialize()/*)*/;

            save(IDENTITY_PUBLIC_KEY_DJB_PREF, System.Convert.ToBase64String(djbIdentityKey.serialize()));
            save(IDENTITY_PRIVATE_KEY_DJB_PREF, System.Convert.ToBase64String(djbPrivateKey));
        }

        public static String retrieve(String key)
        {
            return (string)ApplicationData.Current.LocalSettings.Values[key];
            /*SharedPreferences preferences = context.getSharedPreferences(MasterSecretUtil.PREFERENCES_NAME, 0);
            return preferences.getString(key, null);*/
        }

        public static void save(String key, String value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
            /*SharedPreferences preferences = context.getSharedPreferences(MasterSecretUtil.PREFERENCES_NAME, 0);
            Editor preferencesEditor = preferences.edit();

            preferencesEditor.putString(key, value);
            if (!preferencesEditor.commit()) throw new AssertionError("failed to save identity key/value to shared preferences");*/
        }
    }
}
