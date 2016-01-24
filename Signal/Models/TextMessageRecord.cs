using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signal.Database;
using TextSecure.recipient;

namespace Signal.Models
{
    public class TextMessageRecord : MessageRecord
    {
        public TextMessageRecord(long messageId,
            long threadId,
            DateTime dateSent,
            DateTime dateReceived,
            long type,
            bool read,
            Body body,
            Recipients recipients,
            Recipient individualRecipient,
            long recipientDeviceId,
            long receiptCount,
            int status, List<IdentityKeyMismatch> mismatches)
        {
            this.MessageId = messageId;
            this.ThreadId = threadId;
            this.DateSent = dateSent;
            this.DateReceived = dateReceived;
            this.Type = type;
            //this.Read = read;
            this.BodyI = body;
        }

        public TextMessageRecord(Message message)
        {
            
        }
    }
}
