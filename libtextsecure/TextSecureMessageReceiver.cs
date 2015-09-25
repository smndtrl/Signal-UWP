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

using libtextsecure.messages;
using libtextsecure.push;
using libtextsecure.util;
using libtextsecure.websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace libtextsecure
{
    /**
 * The primary interface for receiving TextSecure messages.
 *
 * @author
 */
    public class TextSecureMessageReceiver
    {

        private readonly PushServiceSocket socket;
        private readonly TrustStore trustStore;
        private readonly String url;
        private readonly CredentialsProvider credentialsProvider;

        /**
         * Construct a TextSecureMessageReceiver.
         *
         * @param url The URL of the TextSecure server.
         * @param trustStore The {@link org.whispersystems.textsecure.api.push.TrustStore} containing
         *                   the server's TLS signing certificate.
         * @param user The TextSecure user's username (eg. phone number).
         * @param password The TextSecure user's password.
         * @param signalingKey The 52 byte signaling key assigned to this user at registration.
         */
        public TextSecureMessageReceiver(String url, TrustStore trustStore,
                                         String user, String password, String signalingKey)
            : this(url, trustStore, new StaticCredentialsProvider(user, password, signalingKey))
        {

        }

        /**
         * Construct a TextSecureMessageReceiver.
         *
         * @param url The URL of the TextSecure server.
         * @param trustStore The {@link org.whispersystems.textsecure.api.push.TrustStore} containing
         *                   the server's TLS signing certificate.
         * @param credentials The TextSecure user's credentials.
         */
        public TextSecureMessageReceiver(String url, TrustStore trustStore, CredentialsProvider credentials)
        {
            this.url = url;
            this.trustStore = trustStore;
            this.credentialsProvider = credentials;
            this.socket = new PushServiceSocket(url, trustStore, credentials);
        }

        /**
         * Retrieves a TextSecure attachment.
         *
         * @param pointer The {@link org.whispersystems.textsecure.api.messages.TextSecureAttachmentPointer}
         *                received in a {@link TextSecureDataMessage}.
         * @param destination The download destination for this attachment.
         *
         * @return An InputStream that streams the plaintext attachment contents.
         * @throws IOException
         * @throws InvalidMessageException
         */
        /*public IInputStream retrieveAttachment(TextSecureAttachmentPointer pointer, File destination)
        {
            socket.retrieveAttachment(pointer.getRelay().orNull(), pointer.getId(), destination);
            return new AttachmentCipherInputStream(destination, pointer.getKey());
        }*/

        /**
         * Creates a pipe for receiving TextSecure messages.
         *
         * Callers must call {@link TextSecureMessagePipe#shutdown()} when finished with the pipe.
         *
         * @return A TextSecureMessagePipe for receiving TextSecure messages.
         */
        public TextSecureMessagePipe createMessagePipe()
        {
            WebSocketConnection webSocket = new WebSocketConnection(url, trustStore, credentialsProvider);
            return new TextSecureMessagePipe(webSocket, credentialsProvider);
        }

        public async Task<List<TextSecureEnvelope>> retrieveMessages()
        {
            return await retrieveMessages(new NullMessageReceivedCallback());
        }

        public async Task<List<TextSecureEnvelope>> retrieveMessages(MessageReceivedCallback callback)
        {
            List<TextSecureEnvelope> results = new List<TextSecureEnvelope>();
            List<TextSecureEnvelopeEntity> entities = await socket.getMessages();

            foreach (TextSecureEnvelopeEntity entity in entities)
            {
                TextSecureEnvelope envelope = new TextSecureEnvelope(entity.getType(), entity.getSource(),
                                                                      entity.getSourceDevice(), entity.getRelay(),
                                                                      entity.getTimestamp(), entity.getMessage(),
                                                                      entity.getContent());

                callback.onMessage(envelope);
                results.Add(envelope);

                socket.acknowledgeMessage(entity.getSource(), entity.getTimestamp());
            }

            return results;
        }


        public interface MessageReceivedCallback
        {
            void onMessage(TextSecureEnvelope envelope);
        }

        public class NullMessageReceivedCallback : MessageReceivedCallback
        {
            public void onMessage(TextSecureEnvelope envelope) { }
        }

    }
}
