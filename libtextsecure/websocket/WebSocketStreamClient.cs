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

namespace libtextsecure.websocket
{
    class WebSocketStreamClient
    {
        String uri;
        StreamWebSocket socket;

        public WebSocketStreamClient(String uri)
        {
            this.uri = uri;
        }

        public async void connect()
        {
            Uri server = new Uri(uri);
            await socket.Connect(server);
        }

        public void disconnect()
        {
            socket.Close(0, "Graceefull disconect");
        }

        public void sendMessage(byte[] message)
        {
            //socket.
        }
    }
}
