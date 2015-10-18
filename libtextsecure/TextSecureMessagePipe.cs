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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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

        public event TypedEventHandler<TextSecureMessagePipe, TextSecureEnvelope> MessageReceived;

        public TextSecureMessagePipe(WebSocketConnection websocket, CredentialsProvider credentialsProvider)
        {
            this.websocket = websocket;

            this.websocket.MessageReceived += OnMessageReceived;
            this.credentialsProvider = credentialsProvider;

            this.websocket.connect();
        }

        private void OnMessageReceived(WebSocketConnection sender, WebSocketRequestMessage request)
        {
            WebSocketResponseMessage response = createWebSocketResponse(request);

            Debug.WriteLine($"Verb: {request.Verb}, Path {request.Path}, Body({request.Body.Length}): {request.Body}, Id: {request.Id}");

            try
            {
                if (isTextSecureEnvelope(request))
                {
                    TextSecureEnvelope envelope = new TextSecureEnvelope(request.Body.ToByteArray(),
                                                                         credentialsProvider.GetSignalingKey());

                    MessageReceived(this, envelope);
                }
            }
            //catch (Exception e) { } // ignore for now
            finally
            {
                websocket.sendResponse(response);
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
        /*public interface MessagePipeCallback
        {
            void onMessage(TextSecureEnvelope envelope);
        }

        private class NullMessagePipeCallback : MessagePipeCallback
        {

            public void onMessage(TextSecureEnvelope envelope) { }
        }*/

    }
}
