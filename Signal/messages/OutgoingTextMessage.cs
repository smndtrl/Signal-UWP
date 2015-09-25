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
using TextSecure.database;
using TextSecure.recipient;

namespace TextSecure.messages
{
    public class OutgoingTextMessage
    {

        private readonly Recipients recipients;
        private readonly String message;

        public OutgoingTextMessage(Recipients recipients, String message)
        {
            this.recipients = recipients;
            this.message = message;
        }

        protected OutgoingTextMessage(OutgoingTextMessage message, String body)
        {
            this.recipients = message.getRecipients();
            this.message = body;
        }

        public String getMessageBody()
        {
            return message;
        }

        public Recipients getRecipients()
        {
            return recipients;
        }

        public virtual bool isKeyExchange()
        {
            return false;
        }

        public virtual bool isSecureMessage()
        {
            return false;
        }

        public virtual bool isEndSession()
        {
            return false;
        }

        public virtual bool isPreKeyBundle()
        {
            return false;
        }
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
