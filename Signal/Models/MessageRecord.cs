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

        internal Recipient IndividualRecipient;
        public long RecipientDeviceId { get; internal set; }
        public long MessageId { get; internal set; }

        internal List<IdentityKeyMismatch> MismatchedIdentities;
        //internal List<NetworkFailure> NetworkFailures;

        [Ignore, Obsolete]
        public bool IsSecure => MessageTypes.isSecureType(Type); // TODO

        [Ignore, Obsolete]
        public bool IsLegacyMessage => MessageTypes.isLegacyType(Type); // TODO

        [Ignore]
        public bool IsAsymmetricEncryption => MessageTypes.isAsymmetricEncryption(Type);

        [Ignore, Obsolete]
        public bool IsPush => true;

        [Ignore, Obsolete]
        public bool IsForcedSms => false;

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


    }
}
