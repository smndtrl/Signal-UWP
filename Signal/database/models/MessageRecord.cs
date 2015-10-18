using Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.database.models
{
    public abstract class MessageRecord : DisplayRecord
    {

        public static readonly int DELIVERY_STATUS_NONE = 0;
        public static readonly int DELIVERY_STATUS_RECEIVED = 1;
        public static readonly int DELIVERY_STATUS_PENDING = 2;
        public static readonly int DELIVERY_STATUS_FAILED = 3;

        private static readonly int MAX_DISPLAY_LENGTH = 2000;

        private readonly Recipient individualRecipient;
        private readonly int recipientDeviceId;
        private readonly long id;
        private readonly int deliveryStatus;
        private readonly int receiptCount;
        public readonly LinkedList<IdentityKeyMismatch> mismatches;
        public readonly LinkedList<NetworkFailure> networkFailures;

        public MessageRecord(long id, DisplayRecord.Body body, Recipients recipients,
                      Recipient individualRecipient, int recipientDeviceId,
                      DateTime dateSent, DateTime dateReceived, long threadId,
                      int deliveryStatus, int receiptCount, long type,
                      LinkedList<IdentityKeyMismatch> mismatches,
                      LinkedList<NetworkFailure> networkFailures)
        : base(body, recipients, dateSent, dateReceived, threadId, type)
        {
            this.id = id;
            this.individualRecipient = individualRecipient;
            this.recipientDeviceId = recipientDeviceId;
            this.deliveryStatus = deliveryStatus;
            this.receiptCount = receiptCount;
            this.mismatches = mismatches;
            this.networkFailures = networkFailures;
        }

        public abstract bool isMms();
        public abstract bool isMmsNotification();

        public bool isFailed()
        {
            return
                MessageTypes.isFailedMessageType(type) ||
                MessageTypes.isPendingSecureSmsFallbackType(type) ||
                getDeliveryStatus() == DELIVERY_STATUS_FAILED;
        }

        public bool isOutgoing()
        {
            return MessageTypes.isOutgoingMessageType(type);
        }

        public bool isPending()
        {
            return MessageTypes.isPendingMessageType(type);
        }

        public bool isSecure()
        {
            return MessageTypes.isSecureType(type);
        }

        public bool isLegacyMessage()
        {
            return MessageTypes.isLegacyType(type);
        }

        public bool isAsymmetricEncryption()
        {
            return MessageTypes.isAsymmetricEncryption(type);
        }

        public override string getDisplayBody()
        {
            return "";
            /*if (isGroupUpdate() && isOutgoing())
            {
                return emphasisAdded(context.getString(R.string.MessageRecord_updated_group));
            }
            else if (isGroupUpdate())
            {
                return emphasisAdded(GroupUtil.getDescription(context, getBody().getBody()));
            }
            else if (isGroupQuit() && isOutgoing())
            {
                return emphasisAdded(context.getString(R.string.MessageRecord_left_group));
            }
            else if (isGroupQuit())
            {
                return emphasisAdded(context.getString(R.string.ConversationItem_group_action_left, getIndividualRecipient().toShortString()));
            }
            else if (getBody().getBody().length() > MAX_DISPLAY_LENGTH)
            {
                return new SpannableString(getBody().getBody().substring(0, MAX_DISPLAY_LENGTH));
            }

            return new SpannableString(getBody().getBody());*/
        }

        public long getId()
        {
            return id;
        }

        public int getDeliveryStatus()
        {
            return deliveryStatus;
        }

        public bool isDelivered()
        {
            return getDeliveryStatus() == DELIVERY_STATUS_RECEIVED || receiptCount > 0;
        }

        public bool isPush()
        {
            return MessageTypes.isPushType(type) && !MessageTypes.isForcedSms(type);
        }

        public bool isForcedSms()
        {
            return MessageTypes.isForcedSms(type);
        }

        public bool isStaleKeyExchange()
        {
            return MessageTypes.isStaleKeyExchange(type);
        }

        public bool isProcessedKeyExchange()
        {
            return MessageTypes.isProcessedKeyExchange(type);
        }

        public bool isPendingInsecureSmsFallback()
        {
            return MessageTypes.isPendingInsecureSmsFallbackType(type);
        }

        public bool isIdentityMismatchFailure()
        {
            return mismatches != null && !(mismatches.Count == 0);
        }

        public bool isBundleKeyExchange()
        {
            return MessageTypes.isBundleKeyExchange(type);
        }

        public bool isIdentityUpdate()
        {
            return MessageTypes.isIdentityUpdate(type);
        }

        public bool isCorruptedKeyExchange()
        {
            return MessageTypes.isCorruptedKeyExchange(type);
        }

        public bool isInvalidVersionKeyExchange()
        {
            return MessageTypes.isInvalidVersionKeyExchange(type);
        }

        public Recipient getIndividualRecipient()
        {
            return individualRecipient;
        }

        public int getRecipientDeviceId()
        {
            return recipientDeviceId;
        }

        public long getType()
        {
            return type;
        }

        public LinkedList<IdentityKeyMismatch> getIdentityKeyMismatches()
        {
            return mismatches;
        }

        public LinkedList<NetworkFailure> getNetworkFailures()
        {
            return networkFailures;
        }

        public bool hasNetworkFailures()
        {
            return networkFailures != null && !(networkFailures.Count == 0);
        }

        /*protected SpannableString emphasisAdded(String sequence)
        {
            SpannableString spannable = new SpannableString(sequence);
            spannable.setSpan(new RelativeSizeSpan(0.9f), 0, sequence.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
            spannable.setSpan(new StyleSpan(android.graphics.Typeface.ITALIC), 0, sequence.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

            return spannable;
        }*/

        /*public bool equals(Object other)
        {
            return other != null &&
                   other instanceof MessageRecord             &&
                   ((MessageRecord)other).getId() == getId() &&
                   ((MessageRecord)other).isMms() == isMms();
        }*/

        public int hashCode()
        {
            return (int)getId();
        }

    }
}
