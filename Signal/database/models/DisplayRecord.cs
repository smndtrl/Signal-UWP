using Signal.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.database.models
{
    public abstract class DisplayRecord
    {

        public class IdentityKeyMismatch { }
        public class NetworkFailure { }

        protected readonly long type;

        private readonly Recipients recipients;
        private readonly DateTime dateSent;
        private readonly DateTime dateReceived;
        private readonly long threadId;
        private readonly Body body;

        public DisplayRecord(Body body, Recipients recipients, DateTime dateSent,
                             DateTime dateReceived, long threadId, long type)
        {
            this.threadId = threadId;
            this.recipients = recipients;
            this.dateSent = dateSent;
            this.dateReceived = dateReceived;
            this.type = type;
            this.body = body;
        }

        public Body getBody()
        {
            return body;
        }

        public abstract string getDisplayBody();

        public Recipients getRecipients()
        {
            return recipients;
        }

        public DateTime getDateSent()
        {
            return dateSent;
        }

        public DateTime getDateReceived()
        {
            return dateReceived;
        }

        public long getThreadId()
        {
            return threadId;
        }

        public bool isKeyExchange()
        {
            return MessageTypes.isKeyExchangeType(type);
        }

        public bool isEndSession()
        {
            return MessageTypes.isEndSessionType(type);
        }

        public bool isGroupUpdate()
        {
            return MessageTypes.isGroupUpdate(type);
        }

        public bool isGroupQuit()
        {
            return MessageTypes.isGroupQuit(type);
        }

        public bool isGroupAction()
        {
            return isGroupUpdate() || isGroupQuit();
        }

        public class Body
        {
            private readonly String body;
            private readonly bool plaintext;

            public Body(String body, bool plaintext)
            {
                this.body = body;
                this.plaintext = plaintext;
            }

            public bool isPlaintext()
            {
                return plaintext;
            }

            public String getBody()
            {
                return body == null ? "" : body;
            }
        }
    }
}
