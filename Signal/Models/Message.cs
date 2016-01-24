using GalaSoft.MvvmLight;
using Signal.Database;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.Models
{
    [Table("Messages")]
    public class Message : ObservableObject
    {

        [PrimaryKey, AutoIncrement]
        public long MessageId { get; set; }
        [ForeignKey(typeof(Thread))]
        public long ThreadId { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime DateReceived { get; set; }
        public long Type { get; set; }
        public bool Read { get; set; }
        public string Body { get; set; }
        public string Address { get; set; }
        public long AddressDeviceId { get; set; }
        public long ReceiptCount { get; set; }
        public string MismatchedIdentities { get; set; }


        public bool IsPush = true;
        public bool IsForcedSms = false;

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

        public override bool Equals(object other)
        {
            if (!(other is Message)) return false;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)MessageId;
        }
    }
}
