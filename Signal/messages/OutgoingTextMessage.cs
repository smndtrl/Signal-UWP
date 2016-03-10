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
    public class OutgoingTextMessage
    {
        public OutgoingTextMessage(Recipients recipients, string message)
        {
            this.Recipients = recipients;
            this.MessageBody = message;
        }

        protected OutgoingTextMessage(OutgoingTextMessage message, string body)
        {
            this.Recipients = message.Recipients;
            this.MessageBody = body;
        }

        public string MessageBody { get; }

        public Recipients Recipients { get; }

        public virtual bool IsKeyExchange => false;

        public virtual bool IsSecureMessage => false;

        public virtual bool IsEndSession => false;

        public virtual bool IsPreKeyBundle => false;
        /*
        public static OutgoingTextMessage from(MessageDatabase.Message record)
        {
            if (record.isSecure())
            {
                return new OutgoingEncryptedMessage(record.getRecipients(), record.getBody().getBody());
            }
            else if (record.isKeyExchange())
            {
                return new OutgoingKeyExchangeMessage(record.getRecipients(), record.getBody().getBody());
            }
            else if (record.isEndSession())
            {
                return new OutgoingEndSessionMessage(new OutgoingTextMessage(record.getRecipients(), record.getBody().getBody()));
            }
            else
            {
                return new OutgoingTextMessage(record.getRecipients(), record.getBody().getBody());
            }
        }
        */
        public virtual OutgoingTextMessage withBody(String body)
        {
            return new OutgoingTextMessage(this, body);
        }
    }
}
