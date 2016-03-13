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
using TextSecure.recipient;

namespace Signal.Messages
{
    public class OutgoingKeyExchangeMessage : OutgoingTextMessage
    {
        public OutgoingKeyExchangeMessage(Recipients recipients, string message)
            : base(recipients, message)
        {
        }

        private OutgoingKeyExchangeMessage(OutgoingKeyExchangeMessage message, string body)
            : base(message, body)
        {
        }

        public override bool IsKeyExchange { get; } = true;

        public override OutgoingTextMessage withBody(string body)
        {
            return new OutgoingKeyExchangeMessage(this, body);
        }
    }
}
