using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signal.Database;
using TextSecure.recipient;

namespace Signal.Models
{
    public abstract class Record : ObservableObject
    {

        public class IdentityKeyMismatch { }
        public class NetworkFailure { }

        protected long Type;

        public Recipients Recipients { get; internal set; }
        public DateTime DateSent { get; internal set; }
        public DateTime DateReceived { get; internal set; }
        public long ThreadId { get; internal set; }
        public Body BodyI { get; internal set; }

        public Record ()
        {
        }

        public Record(Body body, Recipients recipients, DateTime dateSent,
                             DateTime dateReceived, long threadId, long type)
        {
            this.ThreadId = threadId;
            this.Recipients = recipients;
            this.DateSent = dateSent;
            this.DateReceived = dateReceived;
            this.Type = type;
            this.BodyI = body;
        }

        /*public Body getBody()
        {
            return body;
        }*/

        //public abstract string getDisplayBody();

        /*public Recipients getRecipients()
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
        }*/

       /* public bool isKeyExchange()
        {
            return MessageTypes.isKeyExchangeType(Type);
        }

        public bool isEndSession()
        {
            return MessageTypes.isEndSessionType(Type);
        }

        public bool isGroupUpdate()
        {
            return MessageTypes.isGroupUpdate(Type);
        }

        public bool isGroupQuit()
        {
            return MessageTypes.isGroupQuit(Type);
        }

        public bool isGroupAction()
        {
            return isGroupUpdate() || isGroupQuit();
        }*/

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
