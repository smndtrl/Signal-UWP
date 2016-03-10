/** 
 * Copyright (C) 2015 smndtrl
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Database
{
    public abstract class MessageTypes
    {
        public const long TOTAL_MASK = 0xFFFFFFFF;

        // Base Types
        public const long BASE_TYPE_MASK = 0x1F;

        protected const long INCOMING_CALL_TYPE = 1;
        protected const long OUTGOING_CALL_TYPE = 2;
        protected const long MISSED_CALL_TYPE = 3;
        protected const long JOINED_TYPE = 4;

        public const long BASE_INBOX_TYPE = 20;
        public const long BASE_OUTBOX_TYPE = 21;
        public const long BASE_SENDING_TYPE = 22;
        public const long BASE_SENT_TYPE = 23;
        public const long BASE_SENT_FAILED_TYPE = 24;
        public const long BASE_PENDING_SECURE_SMS_FALLBACK = 25;
        public const long BASE_PENDING_INSECURE_SMS_FALLBACK = 26;
        public const long BASE_DRAFT_TYPE = 27;

        public static long[] OUTGOING_MESSAGE_TYPES = {BASE_OUTBOX_TYPE, BASE_SENT_TYPE,
                                                            BASE_SENDING_TYPE, BASE_SENT_FAILED_TYPE,
                                                            BASE_PENDING_SECURE_SMS_FALLBACK,
                                                            BASE_PENDING_INSECURE_SMS_FALLBACK};

        // Message attributes
        public const long MESSAGE_ATTRIBUTE_MASK = 0xE0;
        public const long MESSAGE_FORCE_SMS_BIT = 0x40;

        // Key Exchange Information
        public const long KEY_EXCHANGE_MASK = 0xFF00;
        public const long KEY_EXCHANGE_BIT = 0x8000;
        public const long KEY_EXCHANGE_STALE_BIT = 0x4000;
        public const long KEY_EXCHANGE_PROCESSED_BIT = 0x2000;
        public const long KEY_EXCHANGE_CORRUPTED_BIT = 0x1000;
        public const long KEY_EXCHANGE_INVALID_VERSION_BIT = 0x800;
        public const long KEY_EXCHANGE_BUNDLE_BIT = 0x400;
        public const long KEY_EXCHANGE_IDENTITY_UPDATE_BIT = 0x200;

        // Secure Message Information
        public const long SECURE_MESSAGE_BIT = 0x800000;
        public const long END_SESSION_BIT = 0x400000;
        public const long PUSH_MESSAGE_BIT = 0x200000;

        // Group Message Information
        public const long GROUP_UPDATE_BIT = 0x10000;
        public const long GROUP_QUIT_BIT = 0x20000;

        // Encrypted Storage Information
        public const long ENCRYPTION_MASK = 0xFF000000;
        public const long ENCRYPTION_SYMMETRIC_BIT = 0x80000000;
        public const long ENCRYPTION_ASYMMETRIC_BIT = 0x40000000;
        public const long ENCRYPTION_REMOTE_BIT = 0x20000000;
        public const long ENCRYPTION_REMOTE_FAILED_BIT = 0x10000000;
        public const long ENCRYPTION_REMOTE_NO_SESSION_BIT = 0x08000000;
        public const long ENCRYPTION_REMOTE_DUPLICATE_BIT = 0x04000000;
        public const long ENCRYPTION_REMOTE_LEGACY_BIT = 0x02000000;

        public static bool isDraftMessageType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_DRAFT_TYPE;
        }

        public static bool isFailedMessageType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_SENT_FAILED_TYPE;
        }

        public static bool isOutgoingMessageType(long type)
        {
            foreach (long outgoingType in OUTGOING_MESSAGE_TYPES)
            {
                if ((type & BASE_TYPE_MASK) == outgoingType)
                    return true;
            }

            return false;
        }

        public static bool isForcedSms(long type)
        {
            return (type & MESSAGE_FORCE_SMS_BIT) != 0;
        }

        public static bool isPendingMessageType(long type)
        {
            return
                (type & BASE_TYPE_MASK) == BASE_OUTBOX_TYPE ||
                    (type & BASE_TYPE_MASK) == BASE_SENDING_TYPE;
        }

        public static bool isPendingSmsFallbackType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_PENDING_INSECURE_SMS_FALLBACK ||
                   (type & BASE_TYPE_MASK) == BASE_PENDING_SECURE_SMS_FALLBACK;
        }

        public static bool isPendingSecureSmsFallbackType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_PENDING_SECURE_SMS_FALLBACK;
        }

        public static bool isPendingInsecureSmsFallbackType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_PENDING_INSECURE_SMS_FALLBACK;
        }

        public static bool isInboxType(long type)
        {
            return (type & BASE_TYPE_MASK) == BASE_INBOX_TYPE;
        }

        public static bool isJoinedType(long type)
        {
            return (type & BASE_TYPE_MASK) == JOINED_TYPE;
        }

        public static bool isSecureType(long type)
        {
            return (type & SECURE_MESSAGE_BIT) != 0;
        }

        public static bool isPushType(long type)
        {
            return (type & PUSH_MESSAGE_BIT) != 0;
        }

        public static bool isEndSessionType(long type)
        {
            return (type & END_SESSION_BIT) != 0;
        }

        public static bool isKeyExchangeType(long type)
        {
            return (type & KEY_EXCHANGE_BIT) != 0;
        }

        public static bool isStaleKeyExchange(long type)
        {
            return (type & KEY_EXCHANGE_STALE_BIT) != 0;
        }

        public static bool isProcessedKeyExchange(long type)
        {
            return (type & KEY_EXCHANGE_PROCESSED_BIT) != 0;
        }

        public static bool isCorruptedKeyExchange(long type)
        {
            return (type & KEY_EXCHANGE_CORRUPTED_BIT) != 0;
        }

        public static bool isInvalidVersionKeyExchange(long type)
        {
            return (type & KEY_EXCHANGE_INVALID_VERSION_BIT) != 0;
        }

        public static bool isBundleKeyExchange(long type)
        {
            return (type & KEY_EXCHANGE_BUNDLE_BIT) != 0;
        }

        public static bool isIdentityUpdate(long type)
        {
            return (type & KEY_EXCHANGE_IDENTITY_UPDATE_BIT) != 0;
        }

        public static bool isCallLog(long type)
        {
            return type == INCOMING_CALL_TYPE || type == OUTGOING_CALL_TYPE || type == MISSED_CALL_TYPE;
        }

        public static bool isIncomingCall(long type)
        {
            return type == INCOMING_CALL_TYPE;
        }

        public static bool isOutgoingCall(long type)
        {
            return type == OUTGOING_CALL_TYPE;
        }

        public static bool isMissedCall(long type)
        {
            return type == MISSED_CALL_TYPE;
        }

        public static bool isGroupUpdate(long type)
        {
            return (type & GROUP_UPDATE_BIT) != 0;
        }

        public static bool isGroupQuit(long type)
        {
            return (type & GROUP_QUIT_BIT) != 0;
        }

        public static bool isSymmetricEncryption(long type)
        {
            return (type & ENCRYPTION_SYMMETRIC_BIT) != 0;
        }

        public static bool isAsymmetricEncryption(long type)
        {
            return (type & ENCRYPTION_ASYMMETRIC_BIT) != 0;
        }

        public static bool isFailedDecryptType(long type)
        {
            return (type & ENCRYPTION_REMOTE_FAILED_BIT) != 0;
        }

        public static bool isDuplicateMessageType(long type)
        {
            return (type & ENCRYPTION_REMOTE_DUPLICATE_BIT) != 0;
        }

        public static bool isDecryptInProgressType(long type)
        {
            return
                (type & ENCRYPTION_REMOTE_BIT) != 0 ||
                (type & ENCRYPTION_ASYMMETRIC_BIT) != 0;
        }

        public static bool isNoRemoteSessionType(long type)
        {
            return (type & ENCRYPTION_REMOTE_NO_SESSION_BIT) != 0;
        }

        public static bool isLegacyType(long type)
        {
            return (type & ENCRYPTION_REMOTE_LEGACY_BIT) != 0;
        }

        public static long translateFromSystemBaseType(long theirType)
        {
            //    public static final int NONE_TYPE           = 0;
            //    public static final int INBOX_TYPE          = 1;
            //    public static final int SENT_TYPE           = 2;
            //    public static final int SENT_PENDING        = 4;
            //    public static final int FAILED_TYPE         = 5;

            switch ((int)theirType)
            {
                case 1: return BASE_INBOX_TYPE;
                case 2: return BASE_SENT_TYPE;
                case 3: return BASE_DRAFT_TYPE;
                case 4: return BASE_OUTBOX_TYPE;
                case 5: return BASE_SENT_FAILED_TYPE;
                case 6: return BASE_OUTBOX_TYPE;
            }

            return BASE_INBOX_TYPE;
        }

        public static int translateToSystemBaseType(long type)
        {
            if (isInboxType(type)) return 1;
            else if (isOutgoingMessageType(type)) return 2;
            else if (isFailedMessageType(type)) return 5;

            return 1;
        }


        //
        //
        //
        //    public static final int NONE_TYPE           = 0;
        //    public static final int INBOX_TYPE          = 1;
        //    public static final int SENT_TYPE           = 2;
        //    public static final int SENT_PENDING        = 4;
        //    public static final int FAILED_TYPE         = 5;
        //
        //    public static final int OUTBOX_TYPE = 43;  // Messages are stored local encrypted and need delivery.
        //
        //
        //    public static final int ENCRYPTING_TYPE      = 42;  // Messages are stored local encrypted and need async encryption and delivery.
        //    public static final int SECURE_SENT_TYPE     = 44;  // Messages were sent with async encryption.
        //    public static final int SECURE_RECEIVED_TYPE = 45;  // Messages were received with async decryption.
        //    public static final int FAILED_DECRYPT_TYPE  = 46;  // Messages were received with async encryption and failed to decrypt.
        //    public static final int DECRYPTING_TYPE      = 47;  // Messages are in the process of being asymmetricaly decrypted.
        //    public static final int NO_SESSION_TYPE      = 48;  // Messages were received with async encryption but there is no session yet.
        //
        //    public static final int OUTGOING_KEY_EXCHANGE_TYPE  = 49;
        //    public static final int INCOMING_KEY_EXCHANGE_TYPE  = 50;
        //    public static final int STALE_KEY_EXCHANGE_TYPE     = 51;
        //    public static final int PROCESSED_KEY_EXCHANGE_TYPE = 52;
        //
        //    public static final int[] OUTGOING_MESSAGE_TYPES = {SENT_TYPE, SENT_PENDING, ENCRYPTING_TYPE,
        //                                                        OUTBOX_TYPE, SECURE_SENT_TYPE,
        //                                                        FAILED_TYPE, OUTGOING_KEY_EXCHANGE_TYPE};
        //
        //    public static bool isFailedMessageType(long type) {
        //      return type == FAILED_TYPE;
        //    }
        //
        //    public static bool isOutgoingMessageType(long type) {
        //      for (int outgoingType : OUTGOING_MESSAGE_TYPES) {
        //        if (type == outgoingType)
        //          return true;
        //      }
        //
        //      return false;
        //    }
        //
        //    public static bool isPendingMessageType(long type) {
        //      return type == SENT_PENDING || type == ENCRYPTING_TYPE || type == OUTBOX_TYPE;
        //    }
        //
        //    public static bool isSecureType(long type) {
        //      return
        //          type == SECURE_SENT_TYPE     || type == ENCRYPTING_TYPE ||
        //          type == SECURE_RECEIVED_TYPE || type == DECRYPTING_TYPE;
        //    }
        //
        //    public static bool isKeyExchangeType(long type) {
        //      return type == OUTGOING_KEY_EXCHANGE_TYPE || type == INCOMING_KEY_EXCHANGE_TYPE;
        //    }
    }
}
