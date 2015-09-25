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
using libaxolotl.protocol;
using libaxolotl.state;
using libtextsecure.messages;
using libtextsecure.messages.multidevice;
using libtextsecure.push;
using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static libtextsecure.push.TextSecureProtos;

namespace libtextsecure.crypto
{
    /**
     * This is used to decrypt received {@link org.whispersystems.textsecure.api.messages.TextSecureEnvelope}s.
     *
     * @author
     */
    public class TextSecureCipher
    {

        private readonly AxolotlStore axolotlStore;
        private readonly TextSecureAddress localAddress;

        public TextSecureCipher(TextSecureAddress localAddress, AxolotlStore axolotlStore)
        {
            this.axolotlStore = axolotlStore;
            this.localAddress = localAddress;
        }

        public OutgoingPushMessage encrypt(AxolotlAddress destination, byte[] unpaddedMessage, bool legacy)
        {
            SessionCipher sessionCipher = new SessionCipher(axolotlStore, destination);
            PushTransportDetails transportDetails = new PushTransportDetails(sessionCipher.getSessionVersion());
            CiphertextMessage message = sessionCipher.encrypt(transportDetails.getPaddedMessageBody(unpaddedMessage));
            uint remoteRegistrationId = sessionCipher.getRemoteRegistrationId();
            String body = Base64.encodeBytes(message.serialize());

            uint type;

            switch (message.getType())
            {
                case CiphertextMessage.PREKEY_TYPE: type = (uint)Envelope.Types.Type.PREKEY_BUNDLE; break; // todo check
                case CiphertextMessage.WHISPER_TYPE: type = (uint)Envelope.Types.Type.CIPHERTEXT; break; // todo check
                default: throw new Exception("Bad type: " + message.getType());
            }

            return new OutgoingPushMessage(type, destination.getDeviceId(), remoteRegistrationId, legacy ? body : null, legacy ? null : body);
        }



        /**
         * Decrypt a received {@link org.whispersystems.textsecure.api.messages.TextSecureEnvelope}
         *
         * @param envelope The received TextSecureEnvelope
         * @return a decrypted TextSecureMessage
         * @throws InvalidVersionException
         * @throws InvalidMessageException
         * @throws InvalidKeyException
         * @throws DuplicateMessageException
         * @throws InvalidKeyIdException
         * @throws UntrustedIdentityException
         * @throws LegacyMessageException
         * @throws NoSessionException
         */
        public TextSecureContent decrypt(TextSecureEnvelope envelope)
        {
            try
            {
                TextSecureContent content = new TextSecureContent();

                if (envelope.hasLegacyMessage())
                {
                    DataMessage message = DataMessage.ParseFrom(decrypt(envelope, envelope.getLegacyMessage()));
                    content = new TextSecureContent(createTextSecureMessage(envelope, message));
                }
                else if (envelope.hasContent())
                {
                    Content message = Content.ParseFrom(decrypt(envelope, envelope.getContent()));

                    if (message.HasDataMessage)
                    {
                        content = new TextSecureContent(createTextSecureMessage(envelope, message.DataMessage));
                    }
                    else if (message.HasSyncMessage && localAddress.getNumber().Equals(envelope.getSource()))
                    {
                        content = new TextSecureContent(createSynchronizeMessage(envelope, message.SyncMessage));
                    }
                }

                return content;
            }
            catch (InvalidProtocolBufferException e)
            {
                throw new InvalidMessageException(e);
            }
        }

        private byte[] decrypt(TextSecureEnvelope envelope, byte[] ciphertext)

        {
            AxolotlAddress sourceAddress = new AxolotlAddress(envelope.getSource(), envelope.getSourceDevice());
            SessionCipher sessionCipher = new SessionCipher(axolotlStore, sourceAddress);

            byte[] paddedMessage;

            if (envelope.isPreKeyWhisperMessage())
            {
                paddedMessage = sessionCipher.decrypt(new PreKeyWhisperMessage(ciphertext));
            }
            else if (envelope.isWhisperMessage())
            {
                paddedMessage = sessionCipher.decrypt(new WhisperMessage(ciphertext));
            }
            else
            {
                throw new InvalidMessageException("Unknown type: " + envelope.getType());
            }

            PushTransportDetails transportDetails = new PushTransportDetails(sessionCipher.getSessionVersion());
            return transportDetails.getStrippedPaddingMessageBody(paddedMessage);
        }

        private TextSecureDataMessage createTextSecureMessage(TextSecureEnvelope envelope, DataMessage content)
        {
            TextSecureGroup groupInfo = createGroupInfo(envelope, content);
            LinkedList<TextSecureAttachment> attachments = new LinkedList<TextSecureAttachment>();
            bool endSession = ((content.Flags & (uint)DataMessage.Types.Flags.END_SESSION) != 0);

            foreach (AttachmentPointer pointer in content.AttachmentsList)
            {
                attachments.AddLast(new TextSecureAttachmentPointer(pointer.Id,
                                                                pointer.ContentType,
                                                                pointer.Key.ToByteArray(),
                                                                envelope.getRelay()));
            }

            return new TextSecureDataMessage(envelope.getTimestamp(), groupInfo, attachments,
                                             content.Body, endSession);
        }

        private TextSecureSyncMessage createSynchronizeMessage(TextSecureEnvelope envelope, SyncMessage content)
        {
            if (content.HasSent)
            {
                SyncMessage.Types.Sent sentContent = content.Sent;
                return TextSecureSyncMessage.forSentTranscript(new SentTranscriptMessage(sentContent.Destination,
                                                                           sentContent.Timestamp,
                                                                           createTextSecureMessage(envelope, sentContent.Message)));
            }

            if (content.HasRequest)
            {
                return TextSecureSyncMessage.forRequest(new RequestMessage(content.Request));
            }

            return TextSecureSyncMessage.empty();
        }

        private TextSecureGroup createGroupInfo(TextSecureEnvelope envelope, DataMessage content)
        {
            if (!content.HasGroup) return null;

            TextSecureGroup.Type type;

            switch (content.Group.Type)
            {
                case GroupContext.Types.Type.DELIVER: type = TextSecureGroup.Type.DELIVER; break;
                case GroupContext.Types.Type.UPDATE: type = TextSecureGroup.Type.UPDATE; break;
                case GroupContext.Types.Type.QUIT: type = TextSecureGroup.Type.QUIT; break;
                default: type = TextSecureGroup.Type.UNKNOWN; break;
            }

            if (content.Group.Type != GroupContext.Types.Type.DELIVER)
            {
                String name = null;
                IList<String> members = null;
                TextSecureAttachmentPointer avatar = null;

                if (content.Group.HasName)
                {
                    name = content.Group.Name;
                }

                if (content.Group.MembersCount > 0)
                {
                    members = content.Group.MembersList;
                }

                if (content.Group.HasAvatar)
                {
                    avatar = new TextSecureAttachmentPointer(content.Group.Avatar.Id,
                                                             content.Group.Avatar.ContentType,
                                                             content.Group.Avatar.Key.ToByteArray(),
                                                             envelope.getRelay());
                }

                return new TextSecureGroup(type, content.Group.Id.ToByteArray(), name, members, avatar);
            }

            return new TextSecureGroup(content.Group.Id.ToByteArray());
        }


    }
}
