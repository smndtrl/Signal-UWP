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
using libaxolotl;
using libaxolotl.ecc;
using libaxolotl.state;
using libtextsecure.crypto;
using libtextsecure.messages.multidevice;
using libtextsecure.push;
using libtextsecure.util;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static libtextsecure.push.ProvisioningProtos;

namespace libtextsecure
{
    /**
 * The main interface for creating, registering, and
 * managing a TextSecure account.
 *
 * @author
 */
    public class TextSecureAccountManager
    {

        private readonly PushServiceSocket pushServiceSocket;
        private readonly String user;

        /**
         * Construct a TextSecureAccountManager.
         *
         * @param url The URL for the TextSecure server.
         * @param trustStore The {@link org.whispersystems.textsecure.api.push.TrustStore} for the TextSecure server's TLS certificate.
         * @param user A TextSecure phone number.
         * @param password A TextSecure password.
         */
        public TextSecureAccountManager(String url, TrustStore trustStore,
                                        String user, String password)
        {
            this.pushServiceSocket = new PushServiceSocket(url, trustStore, new StaticCredentialsProvider(user, password, null));
            this.user = user;
        }

        /**
         * Register/Unregister a Google Cloud Messaging registration ID.
         *
         * @param gcmRegistrationId The GCM id to register.  A call with an absent value will unregister.
         * @throws IOException
         */
        public async Task<bool> setWnsId(May<String> wnsRegistrationId)// throws IOException
        {
            if (wnsRegistrationId.HasValue)
            {
                return await this.pushServiceSocket.registerWnsId(wnsRegistrationId.ForceGetValue());
            }
            else
            {
                return await this.pushServiceSocket.unregisterWnsId();
            }
        }

        /**
         * Request an SMS verification code.  On success, the server will send
         * an SMS verification code to this TextSecure user.
         *
         * @throws IOException
         */
        public async void requestSmsVerificationCode()// throws IOException
        {
           await this.pushServiceSocket.createAccount(false);
        }

        /**
         * Request a Voice verification code.  On success, the server will
         * make a voice call to this TextSecure user.
         *
          * @throws IOException
         */
        public async void requestVoiceVerificationCode()// throws IOException
        {
            await this.pushServiceSocket.createAccount(true);
        }

        /**
         * Verify a TextSecure account.
         *
         * @param verificationCode The verification code received via SMS or Voice
         *                         (see {@link #requestSmsVerificationCode} and
         *                         {@link #requestVoiceVerificationCode}).
         * @param signalingKey 52 random bytes.  A 32 byte AES key and a 20 byte Hmac256 key,
         *                     concatenated.
         * @param supportsSms Indicate whether this client is capable of supporting encrypted SMS.
         * @param axolotlRegistrationId A random 14-bit number that identifies this TextSecure install.
         *                              This value should remain consistent across registrations for the
         *                              same install, but probabilistically differ across registrations
         *                              for separate installs.
         *
         * @throws IOException
         */
        public async Task<bool> verifyAccount(String verificationCode, String signalingKey,
                                  bool supportsSms, uint axolotlRegistrationId)
        //throws IOException
        {
            await this.pushServiceSocket.verifyAccount(verificationCode, signalingKey,
                                                 supportsSms, axolotlRegistrationId);
            return true;
        }

        /**
         * Register an identity key, last resort key, signed prekey, and list of one time prekeys
         * with the server.
         *
         * @param identityKey The client's long-term identity keypair.
         * @param lastResortKey The client's "last resort" prekey.
         * @param signedPreKey The client's signed prekey.
         * @param oneTimePreKeys The client's list of one-time prekeys.
         *
         * @throws IOException
         */
        public async Task<bool> setPreKeys(IdentityKey identityKey, PreKeyRecord lastResortKey,
                               SignedPreKeyRecord signedPreKey, IList<PreKeyRecord> oneTimePreKeys)
        //throws IOException
        {
            await this.pushServiceSocket.registerPreKeys(identityKey, lastResortKey, signedPreKey, oneTimePreKeys);
            return true;
        }

        /**
         * @return The server's count of currently available (eg. unused) prekeys for this user.
         * @throws IOException
         */
        public async Task<int> getPreKeysCount()// throws IOException
        {
            return await this.pushServiceSocket.getAvailablePreKeys();
        }

        /**
         * Set the client's signed prekey.
         *
         * @param signedPreKey The client's new signed prekey.
         * @throws IOException
         */
        public async void setSignedPreKey(SignedPreKeyRecord signedPreKey)// throws IOException
        {
            await this.pushServiceSocket.setCurrentSignedPreKey(signedPreKey);
        }

        /**
         * @return The server's view of the client's current signed prekey.
         * @throws IOException
         */
        public async Task<SignedPreKeyEntity> getSignedPreKey()// throws IOException
        {
            return await this.pushServiceSocket.getCurrentSignedPreKey();
        }

        /**
         * Checks whether a contact is currently registered with the server.
         *
         * @param e164number The contact to check.
         * @return An optional ContactTokenDetails, present if registered, absent if not.
         * @throws IOException
         */
        public async Task<May<ContactTokenDetails>> getContact(String e164number)// throws IOException
        {
            String contactToken = createDirectoryServerToken(e164number, true);
            ContactTokenDetails contactTokenDetails = await this.pushServiceSocket.getContactTokenDetails(contactToken);

            if (contactTokenDetails != null)
            {
                contactTokenDetails.setNumber(e164number);
            }

            return new May<ContactTokenDetails>(contactTokenDetails);
        }

        /**
         * Checks which contacts in a set are registered with the server.
         *
         * @param e164numbers The contacts to check.
         * @return A list of ContactTokenDetails for the registered users.
         * @throws IOException
         */
        public async Task<List<ContactTokenDetails>> getContacts(IList<String> e164numbers)
        {
            IDictionary<String, String> contactTokensMap = createDirectoryServerTokenMap(e164numbers);
            List<ContactTokenDetails> activeTokens = await this.pushServiceSocket.retrieveDirectory(contactTokensMap.Keys);

            foreach (ContactTokenDetails activeToken in activeTokens)
            {
                string number;
                contactTokensMap.TryGetValue(activeToken.getToken(), out number);
                activeToken.setNumber(number);
            }

            return activeTokens;
        }

        public async Task<String> getNewDeviceVerificationCode()// throws IOException
        {
            return await this.pushServiceSocket.getNewDeviceVerificationCode();
        }

        public async void addDevice(String deviceIdentifier,
                              ECPublicKey deviceKey,
                              IdentityKeyPair identityKeyPair,
                              String code)
        //throws InvalidKeyException, IOException
        {
            ProvisioningCipher cipher = new ProvisioningCipher(deviceKey);
            ProvisionMessage message = ProvisionMessage.CreateBuilder()
                                                         .SetIdentityKeyPublic(ByteString.CopyFrom(identityKeyPair.getPublicKey().serialize()))
                                                         .SetIdentityKeyPrivate(ByteString.CopyFrom(identityKeyPair.getPrivateKey().serialize()))
                                                         .SetNumber(user)
                                                         .SetProvisioningCode(code)
                                                         .Build();

            byte[] ciphertext = cipher.encrypt(message);
            await this.pushServiceSocket.sendProvisioningMessage(deviceIdentifier, ciphertext);
        }

        public async Task<List<DeviceInfo>> getDevices()
        {
            return await this.pushServiceSocket.getDevices();
        }

        public async void removeDevice(long deviceId)
        {
            await this.pushServiceSocket.removeDevice(deviceId);
        }

        private String createDirectoryServerToken(String e164number, bool urlSafe)
        {
            try
            {
                byte[] token = Util.trim(Hash.sha1(Encoding.UTF8.GetBytes(e164number)), 10);
                String encoded = Base64.encodeBytesWithoutPadding(token);

                if (urlSafe) return encoded.Replace('+', '-').Replace('/', '_');
                else return encoded;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private IDictionary<String, String> createDirectoryServerTokenMap(IList<String> e164numbers)
        {
            IDictionary<String, String> tokenMap = new Dictionary<String, String>(e164numbers.Count);

            foreach (String number in e164numbers)
            {
                var token = createDirectoryServerToken(number, false);
                if (!tokenMap.ContainsKey(token)) // mimic java set behaviour
                {
                    tokenMap.Add(token, number);

                }
            }

            return tokenMap;
        }

    }
}
