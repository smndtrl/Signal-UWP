

using Google.ProtocolBuffers;
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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Web;
using static libtextsecure.websocket.WebSocketProtos;

namespace SignalTasks.Background
{
    public sealed class WebSocketConnection //: WebSocketEventListener
    {


        private static readonly int KEEPALIVE_TIMEOUT_SECONDS = 55;

        private readonly LinkedList<WebSocketRequestMessage> incomingRequests = new LinkedList<WebSocketRequestMessage>();

        private readonly String wsUri;
        private readonly string username;
        private readonly string password;
        private readonly string userAgent;


        private Timer keepAliveTimer;

        MessageWebSocket socket;
        DataWriter messageWriter;

        //public event EventHandler Connected;
        //public event EventHandler Closed;
        //public event TypedEventHandler<WebSocketConnection, WebSocketRequestMessage> MessageReceived;

        public WebSocketConnection(String httpUri, string username, string password, string userAgent)
        {
            this.username = username;
            this.password = password;
            this.wsUri = httpUri.Replace("https://", "wss://")
                                              .Replace("http://", "ws://") + $"/v1/websocket/?login={username}&password={password}";
            this.userAgent = userAgent;
        }


        public async void connect()
        {
            Debug.WriteLine("WSC connect()...");

            if (socket == null)
            {
                socket = new MessageWebSocket();
                if (userAgent != null) socket.SetRequestHeader("X-Signal-Agent", userAgent);
                socket.MessageReceived += OnMessageReceived;
                socket.Closed += OnClosed;

                try
                {
                    Uri server = new Uri(wsUri);
                    await socket.ConnectAsync(server);
                    //Connected(this, EventArgs.Empty);
                    //keepAliveTimer = new Timer(sendKeepAlive, null, TimeSpan.FromSeconds(KEEPALIVE_TIMEOUT_SECONDS), TimeSpan.FromSeconds(KEEPALIVE_TIMEOUT_SECONDS));

                    
                    messageWriter = new DataWriter(socket.OutputStream);
                }
                catch (Exception e)
                {
                    WebErrorStatus status = WebSocketError.GetStatus(e.GetBaseException().HResult);

                    switch (status)
                    {
                        case WebErrorStatus.CannotConnect:
                        case WebErrorStatus.NotFound:
                        case WebErrorStatus.RequestTimeout:
                            Debug.WriteLine("Cannot connect to the server. Please make sure " +
                                "to run the server setup script before running the sample.");
                            break;

                        case WebErrorStatus.Unknown:
                            throw;

                        default:
                            Debug.WriteLine("Error: " + status);
                            break;
                    }
                }
                Debug.WriteLine("WSC connected...");
            }
        }

        public void disconnect()
        {
            Debug.WriteLine("WSC disconnect()...");

            if (socket != null)
            {
                socket.Close(1000, "None");
                
                socket = null;
            }

            /*if (keepAliveSender != null)
            {
                keepAliveSender.shutdown();
                keepAliveSender = null;
            }*/
        }

        /*public  WebSocketRequestMessage readRequest(ulong timeoutMillis)
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
        }*/

        /*public async void sendMessage(WebSocketMessage message)
        {
            if (socket == null)
            {
                throw new Exception("Connection closed!");
            }

            messageWriter.WriteBytes(message.ToByteArray());
            await messageWriter.StoreAsync();
        }

        public async void sendResponse(WebSocketResponseMessage response)
        {
            if (socket == null)
            {
                throw new Exception("Connection closed!");
            }

            WebSocketMessage message = WebSocketMessage.CreateBuilder()
                                               .SetType(WebSocketMessage.Types.Type.RESPONSE)
                                               .SetResponse(response)
                                               .Build();

            messageWriter.WriteBytes(message.ToByteArray());
            await messageWriter.StoreAsync();
        }*/

        /*private void sendKeepAlive(object state)
        {
            Debug.WriteLine("keepAlive");
                sendMessage(WebSocketMessage.CreateBuilder()
                                                   .SetType(WebSocketMessage.Types.Type.REQUEST)
                                                   .SetRequest(WebSocketRequestMessage.CreateBuilder()
                                                                                      .SetId(KeyHelper.getTime())
                                                                                      .SetPath("/v1/keepalive")
                                                                                      .SetVerb("GET")
                                                                                      .Build()).Build());
 
        }*/

        private ulong elapsedTime(ulong startTime)
        {
            return KeyHelper.getTime() - startTime;
        } 

        /*public void shutdown()
        {
            stop.set(true);
        }
    }*/

        private void OnClosed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WSC disconnected...");
        }

        private void OnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    byte[] read = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(read);
                    try
                    {
                        WebSocketMessage message = WebSocketMessage.ParseFrom(read);

                        Debug.WriteLine("Message Type: " + message.Type);

                        if (message.Type == WebSocketMessage.Types.Type.REQUEST)
                        {
                            incomingRequests.AddFirst(message.Request);
                            //MessageReceived(this, message.Request);
                        }

                        
                    }
                    catch (InvalidProtocolBufferException e)
                    {
                        Debug.WriteLine(e.Message);
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
