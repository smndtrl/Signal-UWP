using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.Model
{
    public class Message : ObservableObject
    {
        public static readonly int DELIVERY_STATUS_NONE = 0;
        public static readonly int DELIVERY_STATUS_RECEIVED = 1;
        public static readonly int DELIVERY_STATUS_PENDING = 2;
        public static readonly int DELIVERY_STATUS_FAILED = 3;

        public long MessageId { get; set; }
        public long ThreadId { get; set; }

        public bool IsFailed { get { return MessageTypes.isFailedMessageType(_type); } }
        public bool IsOutgoing { get { return MessageTypes.isOutgoingMessageType(_type); } }
        public bool IsPending { get { return MessageTypes.isPendingMessageType(_type); } }
        public bool IsSecure { get { return true; } } // TODO: keep?
        public bool IsLegacyMessage = true;
        public bool IsAsymmetricEncryption { get { return MessageTypes.isAsymmetricEncryption(_type); } }

        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }

        private int _deliveryStatus;
        public int DeliveryStatus
        {
            get { return _deliveryStatus; }
            set
            {
                if (_deliveryStatus == value) return;
                _deliveryStatus = value;
                RaisePropertyChanged("MessageDeliveryStatus");
            }
        }

        public bool IsDelivered { get { return _deliveryStatus == DELIVERY_STATUS_RECEIVED /*|| _recipientCount > 0*/; } }
        public bool IsPush = true;
        public bool IsForcedSms = false;
        public bool IsStaleKeyExchange { get; }
        public bool IsProcessedKeyExchange { get; }
        //public bool IsIdentityMismatch { get; } // TODO: add in
        public bool IsBundleKeyExchange { get { return MessageTypes.isBundleKeyExchange(_type); } }
        public bool IsIdentityUpdate { get { return MessageTypes.isIdentityUpdate(_type); } }
        public bool IsInvalidVersionKeyExchange { get { return MessageTypes.isInvalidVersionKeyExchange(_type); } }

        public Recipient IndividualRecipient { get; set; }
        public long RecipientDeviceId { get; set; }

        private long _type;
        public long Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
                _type = value;
                RaisePropertyChanged("MessageType");
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
                RaisePropertyChanged("MessageRead");
            }
        }

        public string Body { get; set; }

        public Message(long id, string body, Recipients recipients, 
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
            //ReceiptCount = recieptCount;


        }

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
