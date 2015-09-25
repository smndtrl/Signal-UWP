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

using Google.ProtocolBuffers;
using libaxolotl.ecc;
using libaxolotl.kdf;
using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using static libtextsecure.push.ProvisioningProtos;

namespace libtextsecure.crypto
{
    class ProvisioningCipher
    {
        private static readonly String TAG = "asdf";

        private readonly ECPublicKey theirPublicKey;

        public ProvisioningCipher(ECPublicKey theirPublicKey)
        {
            this.theirPublicKey = theirPublicKey;
        }

        public byte[] encrypt(ProvisionMessage message)// throws InvalidKeyException
        {
            ECKeyPair ourKeyPair = Curve.generateKeyPair();
            byte[] sharedSecret = Curve.calculateAgreement(theirPublicKey, ourKeyPair.getPrivateKey());
            byte[] derivedSecret = new HKDFv3().deriveSecrets(sharedSecret, Encoding.UTF8.GetBytes("TextSecure Provisioning Message"), 64);
            byte[][] parts = Util.split(derivedSecret, 32, 32);

            byte[] version = { 0x01 };
            byte[] ciphertext = getCiphertext(parts[0], message.ToByteArray());
            byte[] mac = getMac(parts[1], Util.join(version, ciphertext));
            byte[] body = Util.join(version, ciphertext, mac);

            return ProvisionEnvelope.CreateBuilder()
                                    .SetPublicKey(ByteString.CopyFrom(ourKeyPair.getPublicKey().serialize()))
                                    .SetBody(ByteString.CopyFrom(body))
                                    .Build()
                                    .ToByteArray();
        }

        private byte[] getCiphertext(byte[] key, byte[] message)
        {
            try
            {
                SymmetricKeyAlgorithmProvider objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7); // TODO: PKCS5 padding
                IBuffer buffKey = CryptographicBuffer.CreateFromByteArray(key);
                CryptographicKey ckey = objAlg.CreateSymmetricKey(buffKey);


                IBuffer buffPlaintext = CryptographicBuffer.CreateFromByteArray(message);
                byte[] iv = BitConverter.GetBytes(CryptographicBuffer.GenerateRandomNumber());
                IBuffer buffIV = CryptographicBuffer.CreateFromByteArray(iv);
                IBuffer buffEncrypt = CryptographicEngine.Encrypt(ckey, buffPlaintext, buffIV);

                byte[] ret;
                CryptographicBuffer.CopyToByteArray(buffEncrypt, out ret);

                /*Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5Padding");
                cipher.init(Cipher.ENCRYPT_MODE, new SecretKeySpec(key, "AES"));*/

                return Util.join(iv, ret);
            }
            catch (/*NoSuchAlgorithmException | NoSuchPaddingException | java.security.InvalidKeyException | IllegalBlockSizeException | BadPaddingException*/ Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private byte[] getMac(byte[] key, byte[] message)
        {
            try
            {
                MacAlgorithmProvider provider = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);

                IBuffer buffPrk = CryptographicBuffer.CreateFromByteArray(key);
                CryptographicKey hmacKey = provider.CreateKey(buffPrk);

                IBuffer buffMsg = CryptographicBuffer.CreateFromByteArray(message);

                IBuffer buffHMAC = CryptographicEngine.Sign(hmacKey, buffMsg);

                byte[] ret;
                CryptographicBuffer.CopyToByteArray(buffHMAC, out ret);

                /*Mac mac = Mac.getInstance("HmacSHA256");
                mac.init(new SecretKeySpec(key, "HmacSHA256"));*/

                return ret;
            }
            catch (/*NoSuchAlgorithmException | java.security.InvalidKeyException*/Exception e) {
                throw new Exception(e.Message);
            }
            }

        }
    }
