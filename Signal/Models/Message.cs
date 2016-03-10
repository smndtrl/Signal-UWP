using GalaSoft.MvvmLight;
using Signal.Database;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Signal.Util;
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

        [Column("MismatchedIdentities")]
        public string _mismatchedIdentities { get; set; }

        [Ignore]
        public List<IdentityKeyMismatch> MismatchedIdentities
        {
            get {
                return _mismatchedIdentities != null ? JsonConvert.DeserializeObject<List<IdentityKeyMismatch>>(_mismatchedIdentities) : null;
            }
            set
            {
                _mismatchedIdentities = JsonConvert.SerializeObject(value);
                Log.Debug($"Serialized IdentityKeyMismatch to {_mismatchedIdentities}");
                return;
            }
        }


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
