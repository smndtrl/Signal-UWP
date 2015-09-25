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
using libtextsecure.util;
using libtextsecure.websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static libtextsecure.websocket.WebSocketProtos;

namespace libtextsecure
{
    /**
 * A TextSecureMessagePipe represents a dedicated connection
 * to the TextSecure server, which the server can push messages
 * down.
 */
    public class TextSecureMessagePipe
    {

        private readonly WebSocketConnection websocket;
        private readonly CredentialsProvider credentialsProvider;

        public TextSecureMessagePipe(WebSocketConnection websocket, CredentialsProvider credentialsProvider)
        {
            this.websocket = websocket;
            this.credentialsProvider = credentialsProvider;

            this.websocket.connect();
        }

        /**
         * A blocking call that reads a message off the pipe.  When this
         * call returns, the message has been acknowledged and will not
         * be retransmitted.
         *
         * @param timeout The timeout to wait for.
         * @param unit The timeout time unit.
         * @return A new message.
         *
         * @throws InvalidVersionException
         * @throws IOException
         * @throws TimeoutException
         */
        public TextSecureEnvelope read(ulong timeout/*, TimeUnit unit*/)
        //throws InvalidVersionException, IOException, TimeoutException
        {
            return read(timeout/*, unit*/, new NullMessagePipeCallback());
        }

        /**
         * A blocking call that reads a message off the pipe (see {@link #read(long, java.util.concurrent.TimeUnit)}
         *
         * Unlike {@link #read(long, java.util.concurrent.TimeUnit)}, this method allows you
         * to specify a callback that will be called before the received message is acknowledged.
         * This allows you to write the received message to durable storage before acknowledging
         * receipt of it to the server.
         *
         * @param timeout The timeout to wait for.
         * @param unit The timeout time unit.
         * @param callback A callback that will be called before the message receipt is
         *                 acknowledged to the server.
         * @return The message read (same as the message sent through the callback).
         * @throws TimeoutException
         * @throws IOException
         * @throws InvalidVersionException
         */
        public TextSecureEnvelope read(ulong timeout/*, TimeUnit unit*/, MessagePipeCallback callback)
        //throws TimeoutException, IOException, InvalidVersionException
        {
            while (true)
            {
                WebSocketRequestMessage request = websocket.readRequest(/*unit.toMillis(timeout)*/100000);
                WebSocketResponseMessage response = createWebSocketResponse(request);

                try
                {
                    if (isTextSecureEnvelope(request))
                    {
                        TextSecureEnvelope envelope = new TextSecureEnvelope(request.Body.ToByteArray(),
                                                                             credentialsProvider.GetSignalingKey());

                        callback.onMessage(envelope);
                        return envelope;
                    }
                }
                finally
                {
                    websocket.sendResponse(response);
                }
            }
        }

        /**
         * Close this connection to the server.
         */
        public void shutdown()
        {
            websocket.disconnect();
        }

        private bool isTextSecureEnvelope(WebSocketRequestMessage message)
        {
            return "PUT".Equals(message.Verb) && "/api/v1/message".Equals(message.Path);
        }

        private WebSocketResponseMessage createWebSocketResponse(WebSocketRequestMessage request)
        {
            if (isTextSecureEnvelope(request))
            {
                return WebSocketResponseMessage.CreateBuilder()
                                               .SetId(request.Id)
                                               .SetStatus(200)
                                               .SetMessage("OK")
                                               .Build();
            }
            else
            {
                return WebSocketResponseMessage.CreateBuilder()
                                               .SetId(request.Id)
                                               .SetStatus(400)
                                               .SetMessage("Unknown")
                                               .Build();
            }
        }
      
        /**
         * For receiving a callback when a new message has been
         * received.
         */
        public interface MessagePipeCallback
        {
            void onMessage(TextSecureEnvelope envelope);
        }

        private class NullMessagePipeCallback : MessagePipeCallback
        {

            public void onMessage(TextSecureEnvelope envelope) { }
        }

    }
}
