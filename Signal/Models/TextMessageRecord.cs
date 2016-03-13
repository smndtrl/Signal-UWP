using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signal.Database;
using Signal.Util;
using TextSecure.recipient;

namespace Signal.Models
{
    public class TextMessageRecord : MessageRecord
    {
        public TextMessageRecord()
        {
            
        }

        /*public TextMessageRecord(long messageId,
            long threadId,
            DateTime dateSent,
            DateTime dateReceived,
            long type,
            bool read,
            BodyRecord body,
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
            this.Body = body;
        }*/

        public TextMessageRecord(Message message)
        {
            this.MessageId = message.MessageId;
            this.Body = getBody(message);
            Recipients recipients = getRecipientsFor(message.Address);
            this.Recipients = recipients;
            this.IndividualRecipient = recipients.PrimaryRecipient;
            this.DateSent = message.DateSent;
            this.DateReceived = message.DateReceived;
            this.ReceiptCount = message.ReceiptCount;
            this.Type = message.Type;
            this.ThreadId = message.ThreadId;
            this.MismatchedIdentities = message.MismatchedIdentities;
        }

        private Recipients getRecipientsFor(String address)
        {
            if (address != null)
            {
                Recipients recipients = RecipientFactory.getRecipientsFromString(address, true);

                if (recipients == null || recipients.IsEmpty)
                {
                    return RecipientFactory.getRecipientsFor(Recipient.getUnknownRecipient(), true);
                }

                return recipients;
            }
            else {
                Log.Warn("getRecipientsFor() address is null");
                return RecipientFactory.getRecipientsFor(Recipient.getUnknownRecipient(), true);
            }
        }


        protected BodyRecord getBody(Message m)
        {
            long type = m.Type;
            String body = m.Body;

            if (MessageTypes.isSymmetricEncryption(type))
            {
                return new BodyRecord(body, false);
            }
            else {
                return new BodyRecord(body, true);
            }
        }
    }
}
