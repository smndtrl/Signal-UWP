using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.Model
{

    /*
            public const string TABLE_NAME = "messages";
        public const string ID = "_id";
        public const string NORMALIZED_DATE_SENT = "date_sent";
        public const string NORMALIZED_DATE_RECEIVED = "date_received";
        public const string THREAD_ID = "thread_id";
        public const string READ = "read";
        public const string BODY = "body";
        public const string STATUS = "status";
        public const string ADDRESS = "address";
        public const string ADDRESS_DEVICE_ID = "address_device_id";
        public const string RECEIPT_COUNT = "delivery_receipt_count";
        public const string MISMATCHED_IDENTITIES = "mismatched_identities";
        public const String TYPE = "type";

        [Table(TABLE_NAME)]
        public class MessageTable
        {
            [PrimaryKey, AutoIncrement]
            public long? _id { get; set; } = null;
            public long thread_id { get; set; }
            public string address { get; set; }
            public long address_device_id { get; set; }

            public DateTime date_received { get; set; }
            public DateTime date_sent { get; set; }
            public long read { get; set; } = 0;
            public long status { get; set; } = -1;
            public long type { get; set; }
            public long receipt_count { get; set; } = 0;
            public string body { get; set; }
            public string mismatches { get; set; }
        }
        */
    [Table("Messages")]
    public class Message : ObservableObject
    {
        public static readonly int DELIVERY_STATUS_NONE = 0;
        public static readonly int DELIVERY_STATUS_RECEIVED = 1;
        public static readonly int DELIVERY_STATUS_PENDING = 2;
        public static readonly int DELIVERY_STATUS_FAILED = 3;

        [PrimaryKey, AutoIncrement]
        public long MessageId { get; set; }
        [ForeignKey(typeof(Thread))]
        public long ThreadId { get; set; }

        [Ignore]
        public bool IsFailed { get { return MessageTypes.isFailedMessageType(_type); } }
        [Ignore]
        public bool IsOutgoing { get { return MessageTypes.isOutgoingMessageType(_type); } }
        [Ignore]
        public bool IsPending { get { return MessageTypes.isPendingMessageType(_type); } }
        [Ignore]
        public bool IsSecure { get { return true; } } // TODO: keep?
        public bool IsLegacyMessage = true;
        [Ignore]
        public bool IsAsymmetricEncryption { get { return MessageTypes.isAsymmetricEncryption(_type); } }

        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }

        private long _deliveryStatus;
        /*public long DeliveryStatus
        {
            get { return _deliveryStatus; }
            set
            {
                if (_deliveryStatus == value) return;
                _deliveryStatus = value;
                RaisePropertyChanged("MessageDeliveryStatus");
            }
        }*/

        [Ignore]
        public bool IsDelivered { get { return _deliveryStatus == DELIVERY_STATUS_RECEIVED || ReceiptCount > 0; } }
        public bool IsPush = true;
        public bool IsForcedSms = false;
        [Ignore]
        public bool IsStaleKeyExchange { get; }
        public bool IsProcessedKeyExchange { get; }
        [Ignore]
        //public bool IsIdentityMismatch { get; } // TODO: add in
        public bool IsBundleKeyExchange { get { return MessageTypes.isBundleKeyExchange(_type); } }
        [Ignore]
        public bool IsIdentityUpdate { get { return MessageTypes.isIdentityUpdate(_type); } }
        [Ignore]
        public bool IsInvalidVersionKeyExchange { get { return MessageTypes.isInvalidVersionKeyExchange(_type); } }

        /*[Ignore]
        public Recipient IndividualRecipient { get; set; }
        [Ignore]
        public long RecipientDeviceId { get; set; }*/


        private long _type;
        public long Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        private bool _read = false;
        public bool Read
        {
            get { return _read; }
            set
            {
                if (_read == value) return;
                _read = value;
                RaisePropertyChanged("Read");
            }
        }

        public string Body { get; set; }
        public string Address { get; set; }
        public long AddressDeviceId { get; set; }
        public long ReceiptCount { get; set; }

        public Message() { }

       /* public Message(long id, string body, Recipients recipients, 
            Recipient individualRecipient, int recipientDeviceId, 
            DateTime dateSend, DateTime dateReceived, long threadId,
            int deliveryStatus, int recieptCount, long type)
        {
            MessageId = id;
            Body = body;
            IndividualRecipient = individualRecipient;
            RecipientDeviceId = recipientDeviceId;
            DeliveryStatus = deliveryStatus;
            ThreadId = threadId;
            Type = type;

            DateSent = dateSend;
            DateReceived = dateReceived;
            ReceiptCount = recieptCount;

    
        }*/

        public override bool Equals(Object other)
        {
            if (other == null) return false;
            if (!(other is Message)) return false;

            //DjbECPublicKey that = (DjbECPublicKey)other;
            return false;//Enumerable.SequenceEqual(this.publicKey, that.publicKey);  // TODO: change
        }


        public override int GetHashCode()
        {
            return (int)MessageId;
        }
    }
}
