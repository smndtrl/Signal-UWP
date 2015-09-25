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

using libaxolotl.util;
using libtextsecure.push;
using libtextsecure.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static libtextsecure.websocket.WebSocketProtos;

namespace libtextsecure.websocket
{
    public class WebSocketConnection //: WebSocketEventListener
    {


        private static readonly int KEEPALIVE_TIMEOUT_SECONDS = 55;

        private readonly LinkedList<WebSocketRequestMessage> incomingRequests = new LinkedList<WebSocketRequestMessage>();

        private readonly String wsUri;
        private readonly TrustStore trustStore;
        private readonly CredentialsProvider credentialsProvider;

        //private OkHttpClientWrapper client;
        private WebSocketClient client;
        //private KeepAliveSender keepAliveSender;

        public WebSocketConnection(String httpUri, TrustStore trustStore, CredentialsProvider credentialsProvider)
        {
            this.trustStore = trustStore;
             this.credentialsProvider = credentialsProvider;
            this.wsUri = httpUri.Replace("https://", "wss://")
                                              .Replace("http://", "ws://") + "/v1/websocket/?login=%s&password=%s";
        }


        public /*synchronized*/ void connect()
        {
            Debug.WriteLine("WSC connect()...");

            if (client == null)
            {
                client = new WebSocketClient(wsUri/*, trustStore, credentialsProvider*/, this);
                client.connect(/*KEEPALIVE_TIMEOUT_SECONDS + 10, TimeUnit.SECONDS*/);
            }
        }

        public /*synchronized*/ void disconnect()
        {
            Debug.WriteLine("WSC disconnect()...");

            if (client != null)
            {
                client.disconnect();
                client = null;
            }

            /*if (keepAliveSender != null)
            {
                keepAliveSender.shutdown();
                keepAliveSender = null;
            }*/
        }

        public /*synchronized*/ WebSocketRequestMessage readRequest(ulong timeoutMillis)
        {
            if (client == null)
            {
                throw new Exception("Connection closed!");
            }

            ulong startTime = KeyHelper.getTime();

            while (client != null && incomingRequests.Count == 0 && elapsedTime(startTime) < timeoutMillis)
            {
                //Util.wait(this, Math.Max(1, timeoutMillis - elapsedTime(startTime)));
            }

            if (incomingRequests.Count == 0 && client == null) throw new Exception("Connection closed!");
            else if (incomingRequests.Count == 0) throw new TimeoutException("Timeout exceeded");
            else
            {
                WebSocketRequestMessage message = incomingRequests.First();
                incomingRequests.RemoveFirst();
                return message;
            }
        }

        public /*synchronized*/ void sendResponse(WebSocketResponseMessage response)
        {
            if (client == null)
            {
                throw new Exception("Connection closed!");
            }

            WebSocketMessage message = WebSocketMessage.CreateBuilder()
                                               .SetType(WebSocketMessage.Types.Type.RESPONSE)
                                               .SetResponse(response)
                                               .Build();

            client.sendMessage(message.ToByteArray());
        }

        /*private synchronized void sendKeepAlive()
        {
            if (keepAliveSender != null)
            {
                client.sendMessage(WebSocketMessage.CreateBuilder()
                                                   .SetType(WebSocketMessage.Types.Type.REQUEST)
                                                   .SetRequest(WebSocketRequestMessage.CreateBuilder()
                                                                                      .SetId(KeyHelper.getTime())
                                                                                      .SetPath("/v1/keepalive")
                                                                                      .SetVerb("GET")
                                                                                      .Build()).Build()
                                                   .ToByteArray());
            }
        }*/

        public /*synchronized*/ void onMessage(byte[] payload)
        {
            Debug.WriteLine("WSC onMessage()");
            try
            {
                WebSocketMessage message = WebSocketMessage.ParseFrom(payload);

                Debug.WriteLine("Message Type: " + message.Type);

                if (message.Type == WebSocketMessage.Types.Type.REQUEST)
                {
                    incomingRequests.AddLast(message.Request);
                }

                //notifyAll();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public /*synchronized*/ void onClose()
        {
            Debug.WriteLine("onClose()...");

            if (client != null)
            {
                client.disconnect();
                client = null;
                connect();
            }

            /* (keepAliveSender != null)
            {
                keepAliveSender.shutdown();
                keepAliveSender = null;
            }

            notifyAll();*/
        }

        public /*synchronized*/ void onConnected()
        {
            if (client != null /*&& keepAliveSender == null*/)
            {
                Debug.WriteLine("onConnected()");
                /*keepAliveSender = new KeepAliveSender();
                keepAliveSender.start();*/
            }
        }

        private ulong elapsedTime(ulong startTime)
        {
            return KeyHelper.getTime() - startTime;
        }
        /*
        private class KeepAliveSender extends Thread
        {

            private AtomicBoolean stop = new AtomicBoolean(false);

            public void run()
        {
            while (!stop.get())
            {
                try
                {
                    Thread.sleep(TimeUnit.SECONDS.toMillis(KEEPALIVE_TIMEOUT_SECONDS));

                    Log.w(TAG, "Sending keep alive...");
                    sendKeepAlive();
                }
                catch (Throwable e)
                {
                    Log.w(TAG, e);
                }
            }
        }*/

        /*public void shutdown()
        {
            stop.set(true);
        }
    }*/
    }
}
