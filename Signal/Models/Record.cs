using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Signal.Database;
using SQLite.Net.Attributes;
using TextSecure.recipient;

namespace Signal.Models
{
    public abstract class Record : ObservableObject
    {

        /// public long MessageId { get; set; } // In MessageRecord
        public long ThreadId { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }
        public long Type { get; set; }
        public bool Read { get; set; }
        public BodyRecord Body { get; set; }
        public long ReceiptCount { get; set; }

        public Recipients Recipients { get; internal set; }

        public Record ()
        {
        }

        public Record(BodyRecord body, Recipients recipients, DateTime dateSent,
                             DateTime dateReceived, long threadId, long type)
        {
            this.ThreadId = threadId;
            this.Recipients = recipients;
            this.DateSent = dateSent;
            this.DateReceived = dateReceived;
            this.Type = type;
            this.Body = body;
        }

        [Ignore]
        public bool IsFailed => MessageTypes.isFailedMessageType(Type);

        [Ignore]
        public bool IsPending => MessageTypes.isPendingMessageType(Type);

        [Ignore]
        public bool IsOutgoing => MessageTypes.isOutgoingMessageType(Type);

        [Ignore]
        public bool IsKeyExchange => MessageTypes.isKeyExchangeType(Type);

        [Ignore]
        public bool IsEndSession => MessageTypes.isEndSessionType(Type);

        [Ignore]
        public bool IsGroupUpdate => MessageTypes.isGroupUpdate(Type);

        [Ignore]
        public bool IsGroupQuit => MessageTypes.isGroupQuit(Type);

        [Ignore]
        public bool IsGroupAction => IsGroupUpdate || IsGroupQuit;

        [Ignore]
        public bool IsCallLog => MessageTypes.isCallLog(Type);

        [Ignore]
        public bool IsJoined => MessageTypes.isJoinedType(Type);

        [Ignore]
        public bool IsIncomingCall => MessageTypes.isIncomingCall(Type);

        [Ignore]
        public bool IsOutgoingCall => MessageTypes.isOutgoingCall(Type);

        [Ignore]
        public bool IsMissedCall => MessageTypes.isMissedCall(Type);

        [Ignore]
        public bool IsDelivered => ReceiptCount > 0; // TODO

        [Ignore, Obsolete]
        public bool IsPendingInsecureSmsFallback => false; // TODO

        public class BodyRecord
        {
            private readonly string body;
            private readonly bool plaintext;

            public BodyRecord(string body, bool plaintext)
            {
                this.body = body;
                this.plaintext = plaintext;
            }

            public bool isPlaintext()
            {
                return plaintext;
            }

            public string Body => body ?? "";
        }
    }
}
