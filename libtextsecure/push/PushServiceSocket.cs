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
using libtextsecure.push;
using libtextsecure.push.exceptions;
using libtextsecure.util;
using libtextsecure;
using Newtonsoft.Json;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using libtextsecure.messages.multidevice;
using Windows.Web.Http.Filters;

namespace libtextsecure.push
{
    /**
     *
     * Network interface to the TextSecure server API.
     *
     * @author
     */
    class PushServiceSocket
    {

        private static readonly String TAG = "PushServiceSocket";

        private static readonly String CREATE_ACCOUNT_DEBUG_PATH = "/v1/accounts/test/code/{0}";
        private static readonly String CREATE_ACCOUNT_SMS_PATH = "/v1/accounts/sms/code/{0}";
        private static readonly String CREATE_ACCOUNT_VOICE_PATH = "/v1/accounts/voice/code/{0}";
        private static readonly String VERIFY_ACCOUNT_CODE_PATH = "/v1/accounts/code/{0}";
        private static readonly String VERIFY_ACCOUNT_TOKEN_PATH = "/v1/accounts/token/{0}";
        private static readonly String REGISTER_WNS_PATH = "/v1/accounts/wns/";
        private static readonly String REQUEST_TOKEN_PATH = "/v1/accounts/token";
        private static readonly String SET_ACCOUNT_ATTRIBUTES = "/v1/accounts/attributes";

        private static readonly String PREKEY_METADATA_PATH = "/v2/keys/";
        private static readonly String PREKEY_PATH = "/v2/keys/{0}";
        private static readonly String PREKEY_DEVICE_PATH = "/v2/keys/{0}/{1}";
        private static readonly String SIGNED_PREKEY_PATH = "/v2/keys/signed";

        private static readonly String PROVISIONING_CODE_PATH = "/v1/devices/provisioning/code";
        private static readonly String PROVISIONING_MESSAGE_PATH = "/v1/provisioning/{0}";
        private static readonly String DEVICE_PATH = "/v1/devices/{0}";

        private static readonly String DIRECTORY_TOKENS_PATH = "/v1/directory/tokens";
        private static readonly String DIRECTORY_VERIFY_PATH = "/v1/directory/{0}";
        private static readonly String MESSAGE_PATH = "/v1/messages/{0}";
        private static readonly String ACKNOWLEDGE_MESSAGE_PATH = "/v1/messages/{0}/{1}";
        private static readonly String RECEIPT_PATH = "/v1/receipt/{0}/{1}";
        private static readonly String ATTACHMENT_PATH = "/v1/attachments/{0}";

        private static readonly bool ENFORCE_SSL = true;

        private readonly String serviceUrl;
        //private readonly TrustManager[] trustManagers;
        private readonly CredentialsProvider credentialsProvider;
        private readonly string userAgent;

        public PushServiceSocket(String serviceUrl, TrustStore trustStore, CredentialsProvider credentialsProvider, string userAgent)
        {
            this.serviceUrl = serviceUrl;
            this.credentialsProvider = credentialsProvider;
            this.userAgent = userAgent;
            //this.trustManagers = BlacklistingTrustManager.createFor(trustStore);
        }

        public async Task<bool> createAccount(bool voice) //throws IOException
        {
#if DEBUG
            String path = CREATE_ACCOUNT_SMS_PATH;// CREATE_ACCOUNT_DEBUG_PATH;
#else
            String path = voice ? CREATE_ACCOUNT_VOICE_PATH : CREATE_ACCOUNT_SMS_PATH;
#endif
            await makeRequest(string.Format(path, credentialsProvider.GetUser()), "GET", null);
            return true;
        }

        public async Task<bool> verifyAccountCode(String verificationCode, String signalingKey,
                                   uint registrationId, bool voice)
        {
            AccountAttributes signalingKeyEntity = new AccountAttributes(signalingKey, registrationId, voice, "DEBUG DEVICE", true);
            await makeRequest(string.Format(VERIFY_ACCOUNT_CODE_PATH, verificationCode),
                "PUT", JsonUtil.toJson(signalingKeyEntity));
            return true;
        }

        public async Task<bool> verifyAccountToken(String verificationToken, String signalingKey,
                                   uint registrationId, bool voice)
        {
            AccountAttributes signalingKeyEntity = new AccountAttributes(signalingKey, registrationId, voice, "DEBUG DEVICE", true);
            await makeRequest(string.Format(VERIFY_ACCOUNT_TOKEN_PATH, verificationToken),
                "PUT", JsonUtil.toJson(signalingKeyEntity));
            return true;
        }

        public async Task<bool> setAccountAttributes(string signalingKey, uint registrationId,
                                  bool voice, bool fetchesMessages)
        {
            AccountAttributes accountAttributesEntity = new AccountAttributes(signalingKey, registrationId, voice, "DEBUG DEVICE", fetchesMessages);
            await makeRequest(SET_ACCOUNT_ATTRIBUTES,
                "PUT", JsonUtil.toJson(accountAttributesEntity));
            return true;
        }

        public async Task<String> getAccountVerificationToken()// throws IOException
        {
            String responseText = await makeRequest(REQUEST_TOKEN_PATH, "GET", null);
            return JsonUtil.fromJson<AuthorizationToken>(responseText).Token;
        }

        public async Task<String> getNewDeviceVerificationCode()// throws IOException
        {
            String responseText = await makeRequest(PROVISIONING_CODE_PATH, "GET", null);
            return JsonUtil.fromJson<DeviceCode>(responseText).getVerificationCode();
        }

        public async Task<bool> sendProvisioningMessage(String destination, byte[] body)// throws IOException
        {
            await makeRequest(string.Format(PROVISIONING_MESSAGE_PATH, destination), "PUT",
                    JsonUtil.toJson(new ProvisioningMessage(Base64.encodeBytes(body))));
            return true;
        }

        public async Task<List<DeviceInfo>> getDevices()// throws IOException
        {
            String responseText = await makeRequest(String.Format(DEVICE_PATH, ""), "GET", null);
            return JsonUtil.fromJson<DeviceInfoList>(responseText).getDevices();
        }

        public async Task<bool> removeDevice(long deviceId)// throws IOException
        {
            await makeRequest(String.Format(DEVICE_PATH, deviceId), "DELETE", null);
            return true;
        }

        public async Task<bool> sendReceipt(String destination, ulong messageId, May<string> relay)// throws IOException
        {
            String path = string.Format(RECEIPT_PATH, destination, messageId);

            if (relay.HasValue)
            {
                path += "?relay=" + relay.ForceGetValue();
            }

            await makeRequest(path, "PUT", null);
            return true;
        }

        public async Task<bool> registerWnsId(String wnsRegistrationId)// throws IOException
        {
            WnsRegistrationId registration = new WnsRegistrationId(wnsRegistrationId);
            return await makeRequest(REGISTER_WNS_PATH, "PUT", JsonUtil.toJson(registration)) != null;
        }

        public async Task<bool> unregisterWnsId()// throws IOException
        {
            return await makeRequest(REGISTER_WNS_PATH, "DELETE", null) != null;
        }

        public async Task<SendMessageResponse> sendMessage(OutgoingPushMessageList bundle)
        //throws IOException
        {
            try
            {
                String responseText = await makeRequest(String.Format(MESSAGE_PATH, bundle.getDestination()), "PUT", JsonUtil.toJson(bundle));

                if (responseText == null) return new SendMessageResponse(false);
                else return JsonUtil.fromJson<SendMessageResponse>(responseText);
            }
            catch (Exception nfe)
            {
                throw new UnregisteredUserException(bundle.getDestination(), nfe);
            }
        }

        public async Task<List<TextSecureEnvelopeEntity>> getMessages()// throws IOException
        {
            String responseText = await makeRequest(String.Format(MESSAGE_PATH, ""), "GET", null);
            return JsonUtil.fromJson<TextSecureEnvelopeEntityList>(responseText).getMessages();
        }

        public async Task<bool> acknowledgeMessage(String sender, ulong timestamp)// throws IOException
        {
            await makeRequest(string.Format(ACKNOWLEDGE_MESSAGE_PATH, sender, timestamp), "DELETE", null);
            return true;
        }

        public async Task<bool> registerPreKeys(IdentityKey identityKey,
                                    PreKeyRecord lastResortKey,
                                    SignedPreKeyRecord signedPreKey,
                                    IList<PreKeyRecord> records)
        //throws IOException
        {
            List<PreKeyEntity> entities = new List<PreKeyEntity>();

            foreach (PreKeyRecord record in records)
            {
                PreKeyEntity entity = new PreKeyEntity(record.getId(),
                                                       record.getKeyPair().getPublicKey());

                entities.Add(entity);
            }

            PreKeyEntity lastResortEntity = new PreKeyEntity(lastResortKey.getId(),
                                                     lastResortKey.getKeyPair().getPublicKey());

            SignedPreKeyEntity signedPreKeyEntity = new SignedPreKeyEntity(signedPreKey.getId(),
                                                                   signedPreKey.getKeyPair().getPublicKey(),
                                                                   signedPreKey.getSignature());

            await makeRequest(string.Format(PREKEY_PATH, ""), "PUT",
                JsonUtil.toJson(new PreKeyState(entities, lastResortEntity,
                                                signedPreKeyEntity, identityKey)));
            return true;
        }

        public async Task<int> getAvailablePreKeys()// throws IOException
        {
            String responseText = await makeRequest(PREKEY_METADATA_PATH, "GET", null);
            PreKeyStatus preKeyStatus = JsonUtil.fromJson<PreKeyStatus>(responseText);

            return preKeyStatus.getCount();
        }

        public async Task<List<PreKeyBundle>> getPreKeys(TextSecureAddress destination, uint deviceIdInteger)// throws IOException
        {
            try
            {
                String deviceId = deviceIdInteger.ToString();

                if (deviceId.Equals("1"))
                    deviceId = "*";

                String path = String.Format(PREKEY_DEVICE_PATH, destination.getNumber(), deviceId);

                if (destination.getRelay().HasValue)
                {
                    path = path + "?relay=" + destination.getRelay().ForceGetValue();
                }

                String responseText = await makeRequest(path, "GET", null);
                PreKeyResponse response = JsonUtil.fromJson<PreKeyResponse>(responseText);
                List<PreKeyBundle> bundles = new List<PreKeyBundle>();

                foreach (PreKeyResponseItem device in response.getDevices())
                {
                    ECPublicKey preKey = null;
                    ECPublicKey signedPreKey = null;
                    byte[] signedPreKeySignature = null;
                    int preKeyId = -1;
                    int signedPreKeyId = -1;

                    if (device.getSignedPreKey() != null)
                    {
                        signedPreKey = device.getSignedPreKey().getPublicKey();
                        signedPreKeyId = (int)device.getSignedPreKey().getKeyId(); // TODO: whacky
                        signedPreKeySignature = device.getSignedPreKey().getSignature();
                    }

                    if (device.getPreKey() != null)
                    {
                        preKeyId = (int)device.getPreKey().getKeyId();// TODO: whacky
                        preKey = device.getPreKey().getPublicKey();
                    }

                    bundles.Add(new PreKeyBundle(device.getRegistrationId(), device.getDeviceId(), (uint)preKeyId,
                                                         preKey, (uint)signedPreKeyId, signedPreKey, signedPreKeySignature,
                                                         response.getIdentityKey()));// TODO: whacky
                }

                return bundles;
            }
            /*catch (JsonUtil.JsonParseException e)
            {
                throw new IOException(e);
            }*/
            catch (NotFoundException nfe)
            {
                throw new UnregisteredUserException(destination.getNumber(), nfe);
            }
        }

        public async Task<PreKeyBundle> getPreKey(TextSecureAddress destination, uint deviceId)// throws IOException
        {
            try
            {
                String path = string.Format(PREKEY_DEVICE_PATH, destination.getNumber(),
                                            deviceId.ToString());

                if (destination.getRelay().HasValue)
                {
                    path = path + "?relay=" + destination.getRelay().ForceGetValue();
                }

                String responseText = await makeRequest(path, "GET", null);
                PreKeyResponse response = JsonUtil.fromJson<PreKeyResponse>(responseText);

                if (response.getDevices() == null || response.getDevices().Count < 1)
                    throw new Exception("Empty prekey list");

                PreKeyResponseItem device = response.getDevices()[0];
                ECPublicKey preKey = null;
                ECPublicKey signedPreKey = null;
                byte[] signedPreKeySignature = null;
                int preKeyId = -1;
                int signedPreKeyId = -1;

                if (device.getPreKey() != null)
                {
                    preKeyId = (int)device.getPreKey().getKeyId();// TODO: whacky
                    preKey = device.getPreKey().getPublicKey();
                }

                if (device.getSignedPreKey() != null)
                {
                    signedPreKeyId = (int)device.getSignedPreKey().getKeyId();// TODO: whacky
                    signedPreKey = device.getSignedPreKey().getPublicKey();
                    signedPreKeySignature = device.getSignedPreKey().getSignature();
                }

                return new PreKeyBundle(device.getRegistrationId(), device.getDeviceId(), (uint)preKeyId, preKey,
                                        (uint)signedPreKeyId, signedPreKey, signedPreKeySignature, response.getIdentityKey());
            }
            /*catch (JsonUtil.JsonParseException e)
            {
                throw new IOException(e);
            }*/
            catch (NotFoundException nfe)
            {
                throw new UnregisteredUserException(destination.getNumber(), nfe);
            }
        }

        public async Task<SignedPreKeyEntity> getCurrentSignedPreKey()// throws IOException
        {
            try
            {
                String responseText = await makeRequest(SIGNED_PREKEY_PATH, "GET", null);
                return JsonUtil.fromJson<SignedPreKeyEntity>(responseText);
            }
            catch (/*NotFound*/Exception e)
            {
                Debug.WriteLine(e.Message, TAG);
                return null;
            }
        }

        public async Task<bool> setCurrentSignedPreKey(SignedPreKeyRecord signedPreKey)// throws IOException
        {
            SignedPreKeyEntity signedPreKeyEntity = new SignedPreKeyEntity(signedPreKey.getId(),
                                                                           signedPreKey.getKeyPair().getPublicKey(),
                                                                           signedPreKey.getSignature());
            await makeRequest(SIGNED_PREKEY_PATH, "PUT", JsonUtil.toJson(signedPreKeyEntity));
            return true;
        }

        public async Task<ulong> sendAttachment(PushAttachmentData attachment)// throws IOException
        {
            String response = await makeRequest(string.Format(ATTACHMENT_PATH, ""), "GET", null);
            AttachmentDescriptor attachmentKey = JsonUtil.fromJson<AttachmentDescriptor>(response);

            if (attachmentKey == null || attachmentKey.getLocation() == null)
            {
                throw new Exception("Server failed to allocate an attachment key!");
            }

            Debug.WriteLine("Got attachment content location: " + attachmentKey.getLocation(), TAG);

            /*uploadAttachment("PUT", attachmentKey.getLocation(), attachment.getData(),
                             attachment.getDataSize(), attachment.getKey());
*/
            throw new NotImplementedException("PushServiceSocket sendAttachment");
            return attachmentKey.getId();
        }
        /*
        public void retrieveAttachment(String relay, long attachmentId, File destination)// throws IOException
        {
            String path = string.Format(ATTACHMENT_PATH, attachmentId.ToString());

            if (!Util.isEmpty(relay))
            {
                path = path + "?relay=" + relay;
            }

            String response = makeRequest(path, "GET", null);
            //AttachmentDescriptor descriptor = JsonUtil.fromJson(response, AttachmentDescriptor.class);

            //Log.w(TAG, "Attachment: " + attachmentId + " is at: " + descriptor.getLocation());

            //downloadExternalFile(descriptor.getLocation(), destination);

            throw new NotImplementedException();
        }*/

        public async Task<List<ContactTokenDetails>> retrieveDirectory(ICollection<String> contactTokens) // TODO: whacky
                                                                                              //throws NonSuccessfulResponseCodeException, PushNetworkException
        {
            LinkedList<HashSet<String>> temp = new LinkedList<HashSet<String>>();
            ContactTokenList contactTokenList = new ContactTokenList(contactTokens.ToList());
            String response = await makeRequest(DIRECTORY_TOKENS_PATH, "PUT", JsonUtil.toJson(contactTokenList));
            ContactTokenDetailsList activeTokens = JsonUtil.fromJson<ContactTokenDetailsList>(response);

            return activeTokens.getContacts();
        }

        public async Task<ContactTokenDetails> getContactTokenDetails(String contactToken)// throws IOException
        {
            try
            {
                String response = await makeRequest(string.Format(DIRECTORY_VERIFY_PATH, contactToken), "GET", null);
                return JsonUtil.fromJson<ContactTokenDetails>(response);
            }
            catch (/*NotFound*/Exception nfe)
            {
                return null;
            }
        }
        /*
        private void downloadExternalFile(String url, File localDestination)
        //throws IOException
        {
            URL downloadUrl = new URL(url);
            HttpURLConnection connection = (HttpURLConnection)downloadUrl.openConnection();
            connection.setRequestProperty("Content-Type", "application/octet-stream");
            connection.setRequestMethod("GET");
            connection.setDoInput(true);

            try
            {
                if (connection.getResponseCode() != 200)
                {
                    throw new NonSuccessfulResponseCodeException("Bad response: " + connection.getResponseCode());
                }

                OutputStream output = new FileOutputStream(localDestination);
                InputStream input = connection.getInputStream();
                byte[] buffer = new byte[4096];
                int read;

                while ((read = input.read(buffer)) != -1)
                {
                    output.write(buffer, 0, read);
                }

                output.close();
                Log.w(TAG, "Downloaded: " + url + " to: " + localDestination.getAbsolutePath());
            }
            catch (IOException ioe)
            {
                throw new PushNetworkException(ioe);
            }
            finally
            {
                connection.disconnect();
            }
        }*/

        /*
        private void uploadAttachment(String method, String url, InputStream data, long dataSize, byte[] key)
        //throws IOException
        {
            URL uploadUrl = new URL(url);
            HttpsURLConnection connection = (HttpsURLConnection)uploadUrl.openConnection();
            connection.setDoOutput(true);

            if (dataSize > 0)
            {
                connection.setFixedLengthStreamingMode((int)AttachmentCipherOutputStream.getCiphertextLength(dataSize));
            }
            else
            {
                connection.setChunkedStreamingMode(0);
            }

            connection.setRequestMethod(method);
            connection.setRequestProperty("Content-Type", "application/octet-stream");
            connection.setRequestProperty("Connection", "close");
            connection.connect();

            try
            {
                OutputStream stream = connection.getOutputStream();
                AttachmentCipherOutputStream out    = new AttachmentCipherOutputStream(key, stream);

                Util.copy(data, out);
      out.flush();

                if (connection.getResponseCode() != 200)
                {
                    throw new IOException("Bad response: " + connection.getResponseCode() + " " + connection.getResponseMessage());
                }
            }
            finally
            {
                connection.disconnect();
            }
        }*/

        private async Task<String> makeRequest(String urlFragment, String method, String body)
        //throws NonSuccessfulResponseCodeException, PushNetworkException
        {

            String connection = await makeBaseRequest(urlFragment, method, body); //makeBaseRequest(urlFragment, method, body);

            //var connection = await Task.Run(makeBaseRequest(urlFragment, method, body));

            try
            {
                String response = connection;
                //connection.disconnect();

                return response;
            }
            catch (Exception ioe)
            {
                throw new PushNetworkException(ioe);
            }
        }

        private async Task<String> makeBaseRequest(String urlFragment, String method, String body)
        {
            HttpResponseMessage connection = await getConnection(urlFragment, method, body);
            HttpStatusCode responseCode;
            String responseMessage;
            String response;

            try
            {
                responseCode = connection.StatusCode;
                responseMessage = await connection.Content.ReadAsStringAsync();
            }
            catch (Exception ioe)
            {
                throw new PushNetworkException(ioe);
            }

            switch (responseCode)
            {
                case HttpStatusCode.RequestEntityTooLarge: // 413
                    throw new RateLimitException("Rate limit exceeded: " + responseCode);
                case HttpStatusCode.Unauthorized: // 401
                case HttpStatusCode.Forbidden: // 403
                    throw new AuthorizationFailedException("Authorization failed!");
                case HttpStatusCode.NotFound: // 404
                    throw new NotFoundException("Not found");
                case HttpStatusCode.Conflict: // 409
                    try
                    {
                        response = await connection.Content.ReadAsStringAsync();
                    }
                    catch (/*IO*/Exception e)
                    {
                        throw new PushNetworkException(e);
                    }
                    throw new MismatchedDevicesException(JsonUtil.fromJson<MismatchedDevices>(response));
                case HttpStatusCode.Gone: // 410
                    try
                    {
                        response = await connection.Content.ReadAsStringAsync();  //Util.readFully(connection.getErrorStream());
                    }
                    catch (/*IO*/Exception e)
                    {
                        throw new PushNetworkException(e);
                    }
                    throw new StaleDevicesException(JsonUtil.fromJson<StaleDevices>(response));
                case HttpStatusCode.LengthRequired://411:
                    try
                    {
                        response = await connection.Content.ReadAsStringAsync();  //Util.readFully(connection.getErrorStream());
                    }
                    catch (Exception e)
                    {
                        throw new PushNetworkException(e);
                    }
                    throw new DeviceLimitExceededException(JsonUtil.fromJson<DeviceLimit>(response));
                case HttpStatusCode.ExpectationFailed: // 417
                    throw new ExpectationFailedException();

            }

            if (responseCode != HttpStatusCode.Ok && responseCode != HttpStatusCode.NoContent) // 200 & 204
            {
                throw new NonSuccessfulResponseCodeException("Bad response: " + (int)responseCode + " " +
                                                             responseMessage);
            }

            response = await connection.Content.ReadAsStringAsync();
            return response;
        }

        private async Task<HttpResponseMessage> getConnection(String urlFragment, String method, String body)
        {
            try
            {
                /*SSLContext context = SSLContext.getInstance("TLS");
                context.init(null, trustManagers, null);*/

                Uri url = new Uri(String.Format("{0}{1}", serviceUrl, urlFragment));
                //Log.w(TAG, "Push service URL: " + serviceUrl);
                //Log.w(TAG, "Opening URL: " + url);
                var filter = new HttpBaseProtocolFilter();
#if DEBUG
                filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
                filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
                filter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
#endif
                //HttpURLConnection connection = (HttpURLConnection)url.openConnection();

                HttpClient connection = new HttpClient(filter);

                /*if (ENFORCE_SSL)
                {
                    ((HttpsURLConnection)connection).setSSLSocketFactory(context.getSocketFactory());
                    ((HttpsURLConnection)connection).setHostnameVerifier(new StrictHostnameVerifier());
                }*/

                var headers = connection.DefaultRequestHeaders;

                //connection.setRequestMethod(method);
                //headers.Add("Content-Type", "application/json");

                if (credentialsProvider.GetPassword() != null)
                {
                    headers.Add("Authorization", getAuthorizationHeader());
                }

                if (userAgent != null)
                {
                    headers.Add("X-Signal-Agent", userAgent);
                }

                /*if (body != null)
                {
                    connection.setDoOutput(true);
                }*/

                //connection.connect();

                HttpStringContent content;
                if (body != null)
                {
                    content = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                    Debug.WriteLine(body);
                }
                else
                {
                    content = new HttpStringContent("");
                }
                switch (method)
                {
                    case "POST":
                        return await connection.PostAsync(url, content);
                    case "PUT":
                        return await connection.PutAsync(url, content);
                    case "DELETE":
                        return await connection.DeleteAsync(url);
                    case "GET":
                        return await connection.GetAsync(url);
                    default:
                        throw new Exception("Unknown method: " + method);
                }

            }
            catch (UriFormatException e)
            {
                throw new Exception(string.Format("Uri {0} {1} is wrong", serviceUrl, urlFragment));
            }
            catch (Exception e)
            {
                Debug.WriteLine(string.Format("Other exception {0}{1} is wrong", serviceUrl, urlFragment));
                throw new PushNetworkException(e);
            }

        }


        private String getAuthorizationHeader()
        {
            try
            {
                return "Basic " + Base64.encodeBytes(Encoding.UTF8.GetBytes((credentialsProvider.GetUser() + ":" + credentialsProvider.GetPassword())));
            }
            catch (/*UnsupportedEncoding*/Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }


    class WnsRegistrationId
    {

        [JsonProperty]
        private String wnsRegistrationId;

        /*[JsonProperty]
        private bool webSocketChannel;*/

        public WnsRegistrationId() { }

        public WnsRegistrationId(String wnsRegistrationId)
        {
            this.wnsRegistrationId = wnsRegistrationId;
            //this.webSocketChannel = webSocketChannel;
        }
    }

    class AttachmentDescriptor
    {
        [JsonProperty]
        private ulong id;

        [JsonProperty]
        private String location;

        public ulong getId()
        {
            return id;
        }

        public String getLocation()
        {
            return location;
        }
    }
}

