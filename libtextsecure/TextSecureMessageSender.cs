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
using libaxolotl.state;
using libtextsecure.messages;
using libtextsecure.crypto;
using libtextsecure.push;
using libtextsecure.push.exceptions;
using libtextsecure.src.api.crypto;
using libtextsecure.util;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using libaxolotl;
using static libtextsecure.push.TextSecureProtos;
using libaxolotl.util;
using libtextsecure.messages.multidevice;
using System.Threading.Tasks;

namespace libtextsecure
{
    /**
 * The main interface for sending TextSecure messages.
 *
 * @author
 */
    public class TextSecureMessageSender
    {

        private static String TAG = "TextSecureMessageSender";

        private readonly PushServiceSocket socket;
        private readonly AxolotlStore store;
        private readonly TextSecureAddress localAddress;
        private readonly May<EventListener> eventListener;
        private readonly string userAgent;

        /**
         * Construct a TextSecureMessageSender.
         *
         * @param url The URL of the TextSecure server.
         * @param trustStore The trust store containing the TextSecure server's signing TLS certificate.
         * @param user The TextSecure username (eg phone number).
         * @param password The TextSecure user's password.
         * @param store The AxolotlStore.
         * @param eventListener An optional event listener, which fires whenever sessions are
         *                      setup or torn down for a recipient.
         */
        public TextSecureMessageSender(String url, TrustStore trustStore,
                                       String user, String password,
                                       AxolotlStore store,
                                       May<EventListener> eventListener, String userAgent)
        {
            this.socket = new PushServiceSocket(url, trustStore, new StaticCredentialsProvider(user, password, null), userAgent);
            this.store = store;
            this.localAddress = new TextSecureAddress(user);
            this.eventListener = eventListener;
        }

        /**
         * Send a delivery receipt for a received message.  It is not necessary to call this
         * when receiving messages through {@link org.whispersystems.textsecure.api.TextSecureMessagePipe}.
         * @param recipient The sender of the received message you're acknowledging.
         * @param messageId The message id of the received message you're acknowledging.
         * @throws IOException
         */
        public void sendDeliveryReceipt(TextSecureAddress recipient, ulong messageId)
        {
            this.socket.sendReceipt(recipient.getNumber(), messageId, recipient.getRelay());
        }

        /**
         * Send a message to a single recipient.
         *
         * @param recipient The message's destination.
         * @param message The message.
         * @throws UntrustedIdentityException
         * @throws IOException
         */
        public async void sendMessage(TextSecureAddress recipient, TextSecureDataMessage message)
        {
            byte[] content = await createMessageContent(message);
            ulong timestamp = message.getTimestamp();
            SendMessageResponse response = await sendMessage(recipient, timestamp, content, true);

            if (response != null && response.getNeedsSync())
            {
                byte[] syncMessage = createMultiDeviceSentTranscriptContent(content, new May<TextSecureAddress>(recipient), timestamp);
                await sendMessage(localAddress, timestamp, syncMessage, false);
            }

            if (message.isEndSession())
            {
                store.DeleteAllSessions(recipient.getNumber());

                if (eventListener.HasValue)
                {
                    eventListener.ForceGetValue().onSecurityEvent(recipient);
                }
            }
        }

        /**
         * Send a message to a group.
         *
         * @param recipients The group members.
         * @param message The group message.
         * @throws IOException
         * @throws EncapsulatedExceptions
         */
        public async void sendMessage(List<TextSecureAddress> recipients, TextSecureDataMessage message)
        {
            byte[] content = await createMessageContent(message);
            ulong timestamp = message.getTimestamp();
            SendMessageResponse response = sendMessage(recipients, timestamp, content, true);

            try
            {
                if (response != null && response.getNeedsSync())
                {
                    byte[] syncMessage = createMultiDeviceSentTranscriptContent(content, May<TextSecureAddress>.NoValue, timestamp);
                    await sendMessage(localAddress, timestamp, syncMessage, false);
                }
            }
            catch (UntrustedIdentityException e)
            {
                throw new EncapsulatedExceptions(e);
            }
        }

        public async void sendMessage(TextSecureSyncMessage message)
        {
            byte[] content;

            if (message.getContacts().HasValue)
            {
                content = await createMultiDeviceContactsContent(message.getContacts().ForceGetValue().asStream());
            }
            else if (message.getGroups().HasValue)
            {
                content = await createMultiDeviceGroupsContent(message.getGroups().ForceGetValue().asStream());
            }
            else
            {
                throw new Exception("Unsupported sync message!");
            }

            await sendMessage(localAddress, KeyHelper.getTime(), content, false);
        }

        private async Task<byte[]> createMessageContent(TextSecureDataMessage message)// throws IOException
        {
            DataMessage.Builder builder = DataMessage.CreateBuilder();
            /*List<AttachmentPointer> pointers = createAttachmentPointers(message.getAttachments());

            if (!pointers.Any()) // TODO:check
            {
                builder.AddRangeAttachments(pointers);
            }*/

            if (message.getBody().HasValue)
            {
                builder.SetBody(message.getBody().ForceGetValue());
            }

            if (message.getGroupInfo().HasValue)
            {
                builder.SetGroup(await createGroupContent(message.getGroupInfo().ForceGetValue()));
            }

            if (message.isEndSession())
            {
                builder.SetFlags((uint)DataMessage.Types.Flags.END_SESSION);
            }

            return builder.Build().ToByteArray();
        }
        private async Task<byte[]> createMultiDeviceContactsContent(TextSecureAttachmentStream contacts)
        {
            Content.Builder container = Content.CreateBuilder();
            SyncMessage.Builder builder = SyncMessage.CreateBuilder();
            builder.SetContacts(SyncMessage.Types.Contacts.CreateBuilder()
                                            .SetBlob(await createAttachmentPointer(contacts)));

            return container.SetSyncMessage(builder).Build().ToByteArray();
        }

        private async Task<byte[]> createMultiDeviceGroupsContent(TextSecureAttachmentStream groups)
        {
            Content.Builder container = Content.CreateBuilder();
            SyncMessage.Builder builder = SyncMessage.CreateBuilder();
            builder.SetGroups(SyncMessage.Types.Groups.CreateBuilder()
                                        .SetBlob(await createAttachmentPointer(groups)));

            return container.SetSyncMessage(builder).Build().ToByteArray();
        }

        private byte[] createMultiDeviceSentTranscriptContent(byte[] content, May<TextSecureAddress> recipient, ulong timestamp)
        {
            try
            {
                Content.Builder container = Content.CreateBuilder();
                SyncMessage.Builder syncMessage = SyncMessage.CreateBuilder();
                SyncMessage.Types.Sent.Builder sentMessage = SyncMessage.Types.Sent.CreateBuilder();

                sentMessage.SetTimestamp(timestamp);
                sentMessage.SetMessage(DataMessage.ParseFrom(content));

                if (recipient.HasValue)
                {
                    sentMessage.SetDestination(recipient.ForceGetValue().getNumber());
                }

                return container.SetSyncMessage(syncMessage.SetSent(sentMessage)).Build().ToByteArray();
            }
            catch (InvalidProtocolBufferException e)
            {
                throw new Exception(e.Message);
            }
        }

        private byte[] createSentTranscriptMessage(byte[] content, May<TextSecureAddress> recipient, ulong timestamp)
        {
            {
                try
                {
                    Content.Builder container = Content.CreateBuilder();
                    SyncMessage.Builder syncMessage = SyncMessage.CreateBuilder();
                    SyncMessage.Types.Sent.Builder sentMessage = SyncMessage.Types.Sent.CreateBuilder();

                    sentMessage.SetTimestamp(timestamp);
                    sentMessage.SetMessage(DataMessage.ParseFrom(content));

                    if (recipient.HasValue)
                    {
                        sentMessage.SetDestination(recipient.ForceGetValue().getNumber());
                    }

                    return container.SetSyncMessage(syncMessage.SetSent(sentMessage)).Build().ToByteArray(); ;
                }
                catch (InvalidProtocolBufferException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        private async Task<GroupContext> createGroupContent(TextSecureGroup group)
        {
            GroupContext.Builder builder = GroupContext.CreateBuilder();
            builder.SetId(ByteString.CopyFrom(group.getGroupId()));

            if (group.getType() != TextSecureGroup.Type.DELIVER)
            {
                if (group.getType() == TextSecureGroup.Type.UPDATE) builder.SetType(GroupContext.Types.Type.UPDATE);
                else if (group.getType() == TextSecureGroup.Type.QUIT) builder.SetType(GroupContext.Types.Type.QUIT);
                else throw new Exception("Unknown type: " + group.getType());

                if (group.getName().HasValue) builder.SetName(group.getName().ForceGetValue());
                if (group.getMembers().HasValue) builder.AddRangeMembers(group.getMembers().ForceGetValue());

                if (group.getAvatar().HasValue && group.getAvatar().ForceGetValue().isStream())
                {
                    AttachmentPointer pointer = await createAttachmentPointer(group.getAvatar().ForceGetValue().asStream());
                    builder.SetAvatar(pointer);
                }
            }
            else
            {
                builder.SetType(GroupContext.Types.Type.DELIVER);
            }

            return builder.Build();
        }

        private SendMessageResponse sendMessage(List<TextSecureAddress> recipients, ulong timestamp, byte[] content, bool legacy)
        {
            IList<UntrustedIdentityException> untrustedIdentities = new List<UntrustedIdentityException>(); // was linkedlist
            IList<UnregisteredUserException> unregisteredUsers = new List<UnregisteredUserException>();
            IList<NetworkFailureException> networkExceptions = new List<NetworkFailureException>();

            SendMessageResponse response = null;

            foreach (TextSecureAddress recipient in recipients)
            {
                try
                {
                    response = sendMessage(recipients, timestamp, content, legacy);
                }
                catch (UntrustedIdentityException e)
                {
                    //Log.w(TAG, e);
                    untrustedIdentities.Add(e);
                }
                catch (UnregisteredUserException e)
                {
                    //Log.w(TAG, e);
                    unregisteredUsers.Add(e);
                }
                catch (PushNetworkException e)
                {
                    //Log.w(TAG, e);
                    networkExceptions.Add(new NetworkFailureException(recipient.getNumber(), e));
                }
            }

            if (!(untrustedIdentities.Count == 0) || !(unregisteredUsers.Count == 0) || !(networkExceptions.Count == 0))
            {
                throw new EncapsulatedExceptions(untrustedIdentities, unregisteredUsers, networkExceptions);
            }

            return response;
        }

        private async Task<SendMessageResponse> sendMessage(TextSecureAddress recipient, ulong timestamp, byte[] content, bool legacy)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    OutgoingPushMessageList messages = await getEncryptedMessages(socket, recipient, timestamp, content, legacy);
                    return await socket.sendMessage(messages);
                }
                catch (MismatchedDevicesException mde)
                {
                    Debug.WriteLine(mde.Message, TAG);
                    handleMismatchedDevices(socket, recipient, mde.getMismatchedDevices());
                }
                catch (StaleDevicesException ste)
                {
                    //Log.w(TAG, ste);
                    handleStaleDevices(recipient, ste.getStaleDevices());
                }
            }

            throw new Exception("Failed to resolve conflicts after 3 attempts!");
        }

        private async Task<IList<AttachmentPointer>> createAttachmentPointers(May<LinkedList<TextSecureAttachment>> attachments)
        {
            IList<AttachmentPointer> pointers = new List<AttachmentPointer>();

            if (!attachments.HasValue || attachments.ForceGetValue().Count == 0)
            {
                Debug.WriteLine("No attachments present...", TAG);
                return pointers;
            }

            foreach (TextSecureAttachment attachment in attachments.ForceGetValue())
            {
                if (attachment.isStream())
                {
                    Debug.WriteLine("Found attachment, creating pointer...", TAG);
                    pointers.Add(await createAttachmentPointer(attachment.asStream()));
                }
            }

            return pointers;
        }

        private async Task<AttachmentPointer> createAttachmentPointer(TextSecureAttachmentStream attachment)
        {
            byte[] attachmentKey = Util.getSecretBytes(64);
            PushAttachmentData attachmentData = new PushAttachmentData(attachment.getContentType(),
                                                                       attachment.getInputStream(),
                                                                       attachment.getLength(),
                                                                       attachmentKey);

            ulong attachmentId = await socket.sendAttachment(attachmentData);

            var builder = AttachmentPointer.CreateBuilder()
                                    .SetContentType(attachment.getContentType())
                                    .SetId(attachmentId)
                                    .SetKey(ByteString.CopyFrom(attachmentKey))
                                    .SetSize((uint)attachment.getLength());

            if (attachment.getPreview().HasValue)
            {
                builder.SetThumbnail(ByteString.CopyFrom(attachment.getPreview().ForceGetValue()));
            }

            return builder.Build();
        }


        private async Task<OutgoingPushMessageList> getEncryptedMessages(PushServiceSocket socket,
                                                   TextSecureAddress recipient,
                                                   ulong timestamp,
                                                   byte[] plaintext,
                                                   bool legacy)
        {
            List<OutgoingPushMessage> messages = new List<OutgoingPushMessage>();

            if (!recipient.Equals(localAddress))
            {
                messages.Add(await getEncryptedMessage(socket, recipient, TextSecureAddress.DEFAULT_DEVICE_ID, plaintext, legacy));
            }

            foreach (uint deviceId in store.GetSubDeviceSessions(recipient.getNumber()))
            {
                messages.Add(await getEncryptedMessage(socket, recipient, deviceId, plaintext, legacy));
            }

            return new OutgoingPushMessageList(recipient.getNumber(), timestamp, recipient.getRelay().HasValue ? recipient.getRelay().ForceGetValue() : null, messages);
        }

        private async Task<OutgoingPushMessage> getEncryptedMessage(PushServiceSocket socket, TextSecureAddress recipient, uint deviceId, byte[] plaintext, bool legacy)
        {
            AxolotlAddress axolotlAddress = new AxolotlAddress(recipient.getNumber(), deviceId);
            TextSecureCipher cipher = new TextSecureCipher(localAddress, store);

            if (!store.ContainsSession(axolotlAddress))
            {
                try
                {
                    List<PreKeyBundle> preKeys = await socket.getPreKeys(recipient, deviceId);

                    foreach (PreKeyBundle preKey in preKeys)
                    {
                        try
                        {
                            AxolotlAddress preKeyAddress = new AxolotlAddress(recipient.getNumber(), preKey.getDeviceId());
                            SessionBuilder sessionBuilder = new SessionBuilder(store, preKeyAddress);
                            sessionBuilder.process(preKey);
                        }
                        catch (libaxolotl.exceptions.UntrustedIdentityException e)
                        {
                            throw new UntrustedIdentityException("Untrusted identity key!", recipient.getNumber(), preKey.getIdentityKey());
                        }
                    }

                    if (eventListener.HasValue)
                    {
                        eventListener.ForceGetValue().onSecurityEvent(recipient);
                    }
                }
                catch (InvalidKeyException e)
                {
                    throw new Exception(e.Message);
                }
            }

            return cipher.encrypt(axolotlAddress, plaintext, legacy);
        }

        private async void handleMismatchedDevices(PushServiceSocket socket, TextSecureAddress recipient,
                                           MismatchedDevices mismatchedDevices)
        {
            try
            {
                foreach (uint extraDeviceId in mismatchedDevices.getExtraDevices())
                {
                    store.DeleteSession(new AxolotlAddress(recipient.getNumber(), extraDeviceId));
                }

                foreach (uint missingDeviceId in mismatchedDevices.getMissingDevices())
                {
                    PreKeyBundle preKey = await socket.getPreKey(recipient, missingDeviceId);

                    try
                    {
                        SessionBuilder sessionBuilder = new SessionBuilder(store, new AxolotlAddress(recipient.getNumber(), missingDeviceId));
                        sessionBuilder.process(preKey);
                    }
                    catch (libaxolotl.exceptions.UntrustedIdentityException e)
                    {
                        throw new UntrustedIdentityException("Untrusted identity key!", recipient.getNumber(), preKey.getIdentityKey());
                    }
                }
            }
            catch (InvalidKeyException e)
            {
                throw new Exception(e.Message);
            }
        }

        private void handleStaleDevices(TextSecureAddress recipient, StaleDevices staleDevices)
        {
            foreach (uint staleDeviceId in staleDevices.getStaleDevices())
            {
                store.DeleteSession(new AxolotlAddress(recipient.getNumber(), staleDeviceId));
            }
        }

        public interface EventListener
        {
            void onSecurityEvent(TextSecureAddress address);
        }

    }
}
