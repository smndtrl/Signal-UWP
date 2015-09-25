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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace libtextsecure.websocket
{
    class WebSocketClient
    {
        String uri;
        MessageWebSocket socket;
        DataWriter messageWriter;
        WebSocketConnection conn;

        public WebSocketClient(String uri, WebSocketConnection conn)
        {
            this.uri = uri;
            this.conn = conn;
        }

        public async void connect()
        {
            Uri server = new Uri(uri);
            await socket.Connect(server);
            conn.onConnected();
            socket.MessageReceived += MessageReceived;
            socket.Closed += Closed;
            messageWriter = new DataWriter(socket.OutputStream);
        }

        private void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void disconnect()
        {
            socket.Close(0, "Graceefull disconect");
        }

        public async void sendMessage(byte[] message)
        {
            messageWriter.WriteBytes(message);
            await messageWriter.Store();
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    conn.onMessage(Encoding.UTF8.GetBytes(read));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
