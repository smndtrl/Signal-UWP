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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libtextsecure.push
{
    public class OutgoingPushMessage
    {

        [JsonProperty]
        private uint type;
        [JsonProperty]
        private uint destinationDeviceId;
        [JsonProperty]
        private uint destinationRegistrationId;
        [JsonProperty]
        private String body;
        [JsonProperty]
        private String content;

        public OutgoingPushMessage(uint type,
                                   uint destinationDeviceId,
                                   uint destinationRegistrationId,
                                   String legacyMessage, String content)
        {
            this.type = type;
            this.destinationDeviceId = destinationDeviceId;
            this.destinationRegistrationId = destinationRegistrationId;
            this.body = legacyMessage;
            this.content = content;
        }

        public uint getDestinationDeviceId()
        {
            return destinationDeviceId;
        }

        public String getBody()
        {
            return body;
        }

        public uint getType()
        {
            return type;
        }

        public uint getDestinationRegistrationId()
        {
            return destinationRegistrationId;
        }
    }
}
