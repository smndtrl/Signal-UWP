using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Signal.Database;
using TextSecure.recipient;

namespace Signal.Models
{
    public class MessageRecord : Record
    {


      
        internal int DeliveryStatus;
        internal int ReceiptCount;


        internal Recipient IndividualRecipient;
        internal long RecipientDeviceId;
        public long MessageId { get; internal set; }
        internal List<IdentityKeyMismatch> MismatchedIdentities;

        public static readonly int DELIVERY_STATUS_NONE = 0;
        public static readonly int DELIVERY_STATUS_RECEIVED = 1;
        public static readonly int DELIVERY_STATUS_PENDING = 2;
        public static readonly int DELIVERY_STATUS_FAILED = 3;


        /*[Ignore]
        public bool IsForcedSms => MessageTypes.isForcedSms(Type);*/
        #region MessageRecord
        [Ignore]
        public bool IsStaleKeyExchange => MessageTypes.isStaleKeyExchange(Type);

        [Ignore]
        public bool IsProcessedKeyExchange => MessageTypes.isProcessedKeyExchange(Type);

        [Ignore]
        public bool IsIdentityMismatchFailure => MismatchedIdentities != null; // TODO

        [Ignore]
        public bool IsBundleKeyExchange => MessageTypes.isBundleKeyExchange(Type);

        [Ignore]
        public bool IsIdentityUpdate => MessageTypes.isIdentityUpdate(Type);

        [Ignore]
        public bool IsCorruptedKeyExchange => MessageTypes.isCorruptedKeyExchange(Type);

        [Ignore]
        public bool IsInvalidVersionKeyExchange => MessageTypes.isInvalidVersionKeyExchange(Type);
        #endregion

        #region DisplayRecord

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

        /*[Ignore]
        public bool IsPendingInsecureSmsFallback => MessageTypes.isPendingInsecureSmsFallbackType(Type);*/

        [Ignore]
        public bool IsSecure => true;

        // TODO: keep?
        public bool IsLegacyMessage = true;

        [Ignore]
        public bool IsDelivered => Type >= DELIVERY_STATUS_RECEIVED || ReceiptCount > 0;

        [Ignore]
        public bool IsAsymmetricEncryption { get { return MessageTypes.isAsymmetricEncryption(Type); } }

        #endregion

    }
}
