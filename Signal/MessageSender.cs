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

using libtextsecure;
using libtextsecure.push;
using libtextsecure.util;
using Signal;
using Signal.Models;
using Signal.Tasks;
using Strilanc.Value;
using System;
using System.Threading.Tasks;
using Signal.Push;
using TextSecure.recipient;
using TextSecure.util;
using Signal.Util;
using Signal.Database;
using Signal.Messages;

namespace TextSecure
{
    public class MessageSender
    {

        public async static Task<long> send(
                                 OutgoingTextMessage message,
                                 long threadId)
        {
            long type = MessageTypes.BASE_OUTBOX_TYPE;
            //EncryptingSmsDatabase database = DatabaseFactory.getEncryptingSmsDatabase(context);
            MessageDatabase database = DatabaseFactory.getMessageDatabase();
            Recipients recipients = message.Recipients;
            bool keyExchange = message.IsKeyExchange;

            long allocatedThreadId;

            if (threadId == -1)
            {
                allocatedThreadId =  DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients);
            }
            else
            {
                allocatedThreadId = threadId;
            }

            long messageId = await database.insertMessageOutbox(allocatedThreadId, message, type, TimeUtil.GetDateTimeMillis());

            // notify user interface
           // TODO: Remove Messenger.Default.Send(new AddMessageMessage() { ThreadId = allocatedThreadId, MessageId = messageId });


            await sendTextMessage(recipients, keyExchange, messageId);

            return allocatedThreadId;
        }
        /*
        public static long send(
                                 OutgoingMediaMessage message,
                                 long threadId)
        {
            try
            {
                ThreadDatabase threadDatabase = DatabaseFactory.getThreadDatabase();
                MessageDatabase database = DatabaseFactory.getMessageDatabase();

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
                long messageId = database.insertMessageOutbox(message, allocatedThreadId, System.currentTimeMillis());

                sendMediaMessage(recipients, messageId);

                return allocatedThreadId;
            }
            catch (MmsException e)
            {
                Log.w(TAG, e);
                return threadId;
            }
        }

        public static void resendGroupMessage(MessageRecord messageRecord, long filterRecipientId)
        {
            if (!messageRecord.isMms()) throw new AssertionError("Not Group");

            Recipients recipients = DatabaseFactory.getMmsAddressDatabase().getRecipientsForId(messageRecord.getId());
            sendGroupPush(recipients, messageRecord.getId(), filterRecipientId);
        }

        public static void resend(Context context, MasterSecret masterSecret, MessageRecord messageRecord)
        {
            try
            {
                long messageId = messageRecord.getId();
                boolean forceSms = messageRecord.isForcedSms();
                boolean keyExchange = messageRecord.isKeyExchange();

                if (messageRecord.isMms())
                {
                    Recipients recipients = DatabaseFactory.getMmsAddressDatabase(context).getRecipientsForId(messageId);
                    sendMediaMessage(context, masterSecret, recipients, forceSms, messageId);
                }
                else
                {
                    Recipients recipients = messageRecord.getRecipients();
                    sendTextMessage(context, recipients, forceSms, keyExchange, messageId);
                }
            }
            catch (MmsException e)
            {
                Log.w(TAG, e);
            }
        }

        private static void sendMediaMessage(Context context, MasterSecret masterSecret,
                                             Recipients recipients, boolean forceSms, long messageId)
      throws MmsException
        {
    if (!forceSms && isSelfSend(context, recipients)) {
                sendMediaSelf(context, masterSecret, messageId);
            } else if (isGroupPushSend(recipients)) {
                sendGroupPush(context, recipients, messageId, -1);
            } else if (!forceSms && isPushMediaSend(context, recipients)) {
                sendMediaPush(context, recipients, messageId);
            } else {
                sendMms(context, messageId);
            }
        }
        */
        private async static Task<bool> sendTextMessage(Recipients recipients, bool keyExchange, long messageId)
        {
            /*if (isSelfSend(recipients))
            {
                sendTextSelf(messageId);
            }*/
            //else if (await isPushTextSend(recipients, keyExchange))
            //{
            await sendTextPush(recipients, messageId);
            return true;
            //}
            //else
            //{
            // throw new NotImplementedException();
            //sendSms(context, recipients, messageId);
            // }
        }

        /*private static void sendTextSelf(long messageId)
        {
            MessageDatabase database = DatabaseFactory.getMessageDatabase();

            database.markAsSent(messageId);
            database.markAsPush(messageId);

            Pair<long, long> messageAndThreadId = database.copyMessageInbox(messageId);
            database.markAsPush(messageAndThreadId.first());
        }*//*

        private static void sendMediaSelf(Context context, MasterSecret masterSecret, long messageId)
      throws MmsException
        {
            MmsDatabase database = DatabaseFactory.getMmsDatabase(context);
            database.markAsSent(messageId, "self-send".getBytes(), 0);
            database.markAsPush(messageId);

    long newMessageId = database.copyMessageInbox(masterSecret, messageId);
        database.markAsPush(newMessageId);
  }
  */
        private async static Task<bool> sendTextPush(Recipients recipients, long messageId)
        {



            App.Current.Worker.AddTaskActivities(new PushTextSendTask(messageId, recipients.getPrimaryRecipient().getNumber()));
            return true;
        }
        /*
            private static void sendMediaPush(Context context, Recipients recipients, long messageId)
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
            }
            */
        private async static Task<bool> isPushTextSend(Recipients recipients, bool keyExchange)
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
                String destination = Utils.canonicalizeNumber(recipient.getNumber());

                return await isPushDestination(destination);
            }
            catch (InvalidNumberException e)
            {
                //Log.w(TAG, e);
                return false;
            }
        }
        /*
    private static boolean isPushMediaSend(Context context, Recipients recipients)
    {
        try
        {
            if (!TextSecurePreferences.isPushRegistered(context))
            {
                return false;
            }

            if (recipients.getRecipientsList().size() > 1)
            {
                return false;
            }

            Recipient recipient = recipients.getPrimaryRecipient();
            String destination = Util.canonicalizeNumber(context, recipient.getNumber());

            return isPushDestination(context, destination);
        }
        catch (InvalidNumberException e)
        {
            Log.w(TAG, e);
            return false;
        }
    }

    private static boolean isGroupPushSend(Recipients recipients)
    {
        return GroupUtil.isEncodedGroup(recipients.getPrimaryRecipient().getNumber());
    }*/

        private static bool isSelfSend(Recipients recipients)
        {
            try
            {
                if (!TextSecurePreferences.isPushRegistered())
                {
                    return false;
                }

                if (!recipients.IsSingleRecipient)
                {
                    return false;
                }

                if (recipients.IsGroupRecipient)
                {
                    return false;
                }

                String e164number = Utils.canonicalizeNumber(recipients.getPrimaryRecipient().getNumber());
                return TextSecurePreferences.getLocalNumber().Equals(e164number);
            }
            catch (InvalidNumberException e)
            {
                //Log.w("MessageSender", e);
                return false;
            }
        }

        private async static Task<bool> isPushDestination(String destination)
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
                    May<ContactTokenDetails> registeredUser = await App.Current.accountManager.getContact(destination);

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
                    //Log.w(TAG, e1);
                    return false;
                }
            }
        }

    }
}
