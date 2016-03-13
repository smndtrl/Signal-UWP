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
using System.Diagnostics;
using System.Threading.Tasks;
using Signal.Push;
using libtextsecure;
using Strilanc.Value;
using libtextsecure.push;
using Signal.Database;

namespace TextSecure.util
{
    public class MessageSender
    {

        //private static final String TAG = MessageSender.class.getSimpleName();
        /*
        public static long send(OutgoingTextMessage message,
                                long threadId)
        {
            var database = DatabaseFactory.getMessageDatabase();
            Recipients recipients = message.getRecipients();
            bool keyExchange = message.isKeyExchange();

            long allocatedThreadId;

            if (threadId == -1)
            {
                allocatedThreadId = DatabaseFactory.getThreadDatabase().getThreadIdFor(recipients);
            }
            else
            {
                allocatedThreadId = threadId;
            }

            long messageId = database.insertMessageOutbox(new MasterSecretUnion(masterSecret), allocatedThreadId, message, forceSms, System.currentTimeMillis());

            sendTextMessage(recipients, keyExchange, messageId);

            return allocatedThreadId;
        }

        public static long send(OutgoingMediaMessage message,
                                 long threadId)
        {
            try
            {
                ThreadDatabase threadDatabase = DatabaseFactory.getThreadDatabase();
                var database = DatabaseFactory.getMessageDatabase();

                long allocatedThreadId;

                if (threadId == -1)
                {
                    allocatedThreadId = threadDatabase.getThreadIdFor(message.getRecipients(), message.getDistributionType());
                }
                else
                {
                    allocatedThreadId = threadId;
                }

                Recipients recipients = message.getRecipients();
                long messageId = database.insertMessageOutbox(new MasterSecretUnion(masterSecret), message, allocatedThreadId, forceSms, System.currentTimeMillis());

                sendMediaMessage(recipients, messageId);

                return allocatedThreadId;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return threadId;
            }
        }*/

        /*public static void resendGroupMessage(MessageRecord messageRecord, long filterRecipientId)
        {
            if (!messageRecord.isMms()) throw new AssertionError("Not Group");

            Recipients recipients = DatabaseFactory.getMmsAddressDatabase(context).getRecipientsForId(messageRecord.getId());
            sendGroupPush( recipients, messageRecord.getId(), filterRecipientId);
        }*/
        /*
        public static void resend(MessageRecord messageRecord)
        {
            try
            {
                long messageId = messageRecord.getId();
                bool keyExchange = messageRecord.isKeyExchange();

                if (messageRecord.isMms())
                {
                    Recipients recipients = DatabaseFactory.getMmsAddressDatabase(context).getRecipientsForId(messageId);
                    sendMediaMessage(recipients, messageId);
                }
                else
                {
                    Recipients recipients = messageRecord.getRecipients();
                    sendTextMessage(recipients, keyExchange, messageId);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /*private static void sendMediaMessage(
                                             Recipients recipients, bool forceSms, long messageId)
        {
            if (isSelfSend(recipients))
            {
                sendMediaSelf( messageId);
            }
            else if (isGroupPushSend(recipients))
            {
                sendGroupPush(recipients, messageId, -1);
            }
            else if (isPushMediaSend(recipients))
            {
                sendMediaPush(recipients, messageId);
            }
        }*/
        
        /*private static void sendTextMessage(Recipients recipients, bool keyExchange, long messageId)
        {
            if (isSelfSend(recipients))
            {
                sendTextSelf(messageId);
            }
            else if (isPushTextSend(recipients, keyExchange))
            {
                sendTextPush(recipients, messageId);
            }
        }*/
        /*
        private static void sendTextSelf(long messageId)
        {
            var database = DatabaseFactory.getMessageDatabase();

            database.markAsSent(messageId);
            database.markAsPush(messageId);

            Pair<long, long> messageAndThreadId = database.copyMessageInbox(messageId);
            database.markAsPush(messageAndThreadId.first);
        }

        /*private static void sendMediaSelf(Context context, MasterSecret masterSecret, long messageId)
        {
            MmsDatabase database = DatabaseFactory.getMmsDatabase(context);
            database.markAsSent(messageId, "self-send".getBytes(), 0);
            database.markAsPush(messageId);

            long newMessageId = database.copyMessageInbox(masterSecret, messageId);
            database.markAsPush(newMessageId);
        }*/
        /*
        private static void sendTextPush(Recipients recipients, long messageId)
        {
            JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            jobManager.add(new PushTextSendJob(context, messageId, recipients.getPrimaryRecipient().getNumber()));
        }

        /*private static void sendMediaPush(Context context, Recipients recipients, long messageId)
        {
            JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            jobManager.add(new PushMediaSendJob(context, messageId, recipients.getPrimaryRecipient().getNumber()));
        }

        private static void sendGroupPush(Context context, Recipients recipients, long messageId, long filterRecipientId)
        {
            JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            jobManager.add(new PushGroupSendJob(context, messageId, recipients.getPrimaryRecipient().getNumber(), filterRecipientId));
        }

        private static void sendSms(Context context, Recipients recipients, long messageId)
        {
            JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            jobManager.add(new SmsSendJob(context, messageId, recipients.getPrimaryRecipient().getName()));
        }

        private static void sendMms(Context context, long messageId)
        {
            JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            jobManager.add(new MmsSendJob(context, messageId));
        }*/
        /*
        private static bool isPushTextSend(Recipients recipients, bool keyExchange)
        {
            try
            {
                if (!TextSecurePreferences.isPushRegistered())
                {
                    return false;
                }

                if (keyExchange)
                {
                    return false;
                }

                Recipient recipient = recipients.getPrimaryRecipient();
                String destination = Util.canonicalizeNumber(recipient.getNumber());

                return isPushDestination(destination);
            }
            catch (InvalidNumberException e)
            {
                Debug.WriteLine( e);
                return false;
            }
        }

        private static bool isPushMediaSend(Recipients recipients)
        {
            try
            {
                if (!TextSecurePreferences.isPushRegistered())
                {
                    return false;
                }

                if (recipients.getRecipientsList().Count > 1)
                {
                    return false;
                }

                Recipient recipient = recipients.getPrimaryRecipient();
                String destination = Util.canonicalizeNumber(recipient.getNumber());

                return isPushDestination(destination);
            }
            catch (InvalidNumberException e)
            {
                Debug.WriteLine( e);
                return false;
            }
        }

        private static bool isGroupPushSend(Recipients recipients)
        {
            return GroupUtil.isEncodedGroup(recipients.getPrimaryRecipient().getNumber());
        }*/

       /* private static bool isSelfSend(Recipients recipients)
        {
            try
            {
                if (!TextSecurePreferences.isPushRegistered())
                {
                    return false;
                }

                if (!recipients.isSingleRecipient())
                {
                    return false;
                }

                /*if (recipients.isGroupRecipient())
                {
                    return false;
                }*/
/*
                String e164number = Util.canonicalizeNumber( recipients.getPrimaryRecipient().getNumber());
                return TextSecurePreferences.getLocalNumber().Equals(e164number);
            }
            catch (InvalidNumberException e)
            {
                Log.w("MessageSender", e);
                return false;
            }
        }
        */
        private static async Task<bool> isPushDestination(String destination)
        {
            TextSecureDirectory directory = DatabaseFactory.getDirectoryDatabase();

            try
            {
                return directory.isActiveNumber(destination);
            }
            catch (/*NotInDirectory*/Exception e)
            {
                try
                {
                    TextSecureAccountManager accountManager = TextSecureCommunicationFactory.createManager();
                    May<ContactTokenDetails> registeredUser = await accountManager.getContact(destination);

                    if (!registeredUser.HasValue)
                    {
                        registeredUser = new May<ContactTokenDetails>(new ContactTokenDetails());
                        registeredUser.ForceGetValue().setNumber(destination);
                        directory.setNumber(registeredUser.ForceGetValue(), false);
                        return false;
                    }
                    else
                    {
                        registeredUser.ForceGetValue().setNumber(destination);
                        directory.setNumber(registeredUser.ForceGetValue(), true);
                        return true;
                    }
                }
                catch (Exception e1)
                {
                    Debug.WriteLine(e1);
                    return false;
                }
            }
        }

    }
}
