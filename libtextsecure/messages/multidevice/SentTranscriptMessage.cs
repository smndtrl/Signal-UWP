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

using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libtextsecure.messages.multidevice
{
    public class SentTranscriptMessage
    {

        private readonly May<String> destination;
        private readonly ulong timestamp;
        private readonly TextSecureDataMessage message;

        public SentTranscriptMessage(String destination, ulong timestamp, TextSecureDataMessage message)
        {
            this.destination = new May<string>(destination);
            this.timestamp = timestamp;
            this.message = message;
        }

        public SentTranscriptMessage(ulong timestamp, TextSecureDataMessage message)
        {
            this.destination = May<string>.NoValue;
            this.timestamp = timestamp;
            this.message = message;
        }

        public May<String> getDestination()
        {
            return destination;
        }

        public ulong getTimestamp()
        {
            return timestamp;
        }

        public TextSecureDataMessage getMessage()
        {
            return message;
        }
    }
}