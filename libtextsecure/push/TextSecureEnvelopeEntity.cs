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
    class TextSecureEnvelopeEntity
    {

        [JsonProperty]
        private uint type;

        [JsonProperty]
        private String relay;

        [JsonProperty]
        private ulong timestamp;

        [JsonProperty]
        private String source;

        [JsonProperty]
        private uint sourceDevice;

        [JsonProperty]
        private byte[] message;

        [JsonProperty]
        private byte[] content;

        public TextSecureEnvelopeEntity() { }

        public uint getType()
        {
            return type;
        }

        public String getRelay()
        {
            return relay;
        }

        public ulong getTimestamp()
        {
            return timestamp;
        }

        public String getSource()
        {
            return source;
        }

        public uint getSourceDevice()
        {
            return sourceDevice;
        }

        public byte[] getMessage()
        {
            return message;
        }
        public byte[] getContent()
        {
            return content;
        }
    }
}
