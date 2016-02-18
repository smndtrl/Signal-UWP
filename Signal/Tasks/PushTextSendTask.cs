using libtextsecure;
using libtextsecure.messages;
using libtextsecure.push;
using libtextsecure.util;
using Signal.Database;
using Signal.Models;
using Signal.Tasks.Library;
using Signal.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libtextsecure.crypto;
using Signal.Push;
using Strilanc.Value;
using TextSecure;
using TextSecure.database;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.Tasks
{
    class PushTextSendTask : PushSendTask
    {
        private long messageId;
        protected TextSecureMessageSender messageSender = new TextSecureMessageSender(TextSecureCommunicationFactory.PUSH_URL, new TextSecurePushTrustStore(), TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), new TextSecureAxolotlStore(),
                                                                                  May<TextSecureMessageSender.EventListener>.NoValue, App.CurrentVersion);

        public PushTextSendTask(long messageId, string destination) : base(destination)
        {
            this.messageId = messageId;
        }

        public override void onAdded()
        {
            var textDatabase = DatabaseFactory.getTextMessageDatabase();
            textDatabase.MarkAsSending(messageId);
            textDatabase.MarkAsPush(messageId);
        }

        public new void OnCanceled()
        {
            DatabaseFactory.getTextMessageDatabase().MarkAsSentFailed(messageId);

            /*long threadId = DatabaseFactory.getSmsDatabase(context).getThreadIdForMessage(messageId); // TODO
            Recipients recipients = DatabaseFactory.getThreadDatabase(context).getRecipientsForThreadId(threadId);

            if (threadId != -1 && recipients != null)
            {
                MessageNotifier.notifyMessageDeliveryFailed(context, recipients, threadId);
            }*/
        }

        protected override string Execute()
        {
            throw new NotImplementedException("TextSendTask Execute happened");
        }

        protected override async Task<string> ExecuteAsync()
        {
            TextMessageDatabase database = DatabaseFactory.getTextMessageDatabase();
            var record = await database.getMessageRecord(messageId);
            //var message = await database.Get(messageId);

            try
            {
                deliver(record);
                database.MarkAsPush(messageId);
                database.MarkAsSecure(messageId);
                database.MarkAsSent(messageId);

            }
            catch (UntrustedIdentityException e)
            {
                Log.Debug($"Untrusted Identity Key: {e.IdentityKey} {e.E164Number}");
                var recipients = RecipientFactory.getRecipientsFromString(e.E164Number, false);
                long recipientId = recipients.getPrimaryRecipient().getRecipientId();

                database.AddMismatchedIdentity(record.MessageId, recipientId, e.IdentityKey);
                database.MarkAsSentFailed(record.MessageId);
                database.MarkAsPush(record.MessageId);
            }
            catch (Exception e)
            {
                Log.Error($"Unexpected Exception {e.Source} {e.GetType()} : {e.Message}");
            }


            return "";
            //throw new NotImplementedException();
        }

        private void deliver(TextMessageRecord message)
        {
            try
            {
                TextSecureAddress address = getPushAddress(message.IndividualRecipient.Number);
                TextSecureDataMessage textSecureMessage = TextSecureDataMessage.newBuilder()
                                                                                 .withTimestamp((ulong)TimeUtil.GetUnixTimestampMillis(message.DateSent))
                                                                                 .withBody(message.Body.Body)
                                                                                 .asEndSessionMessage(message.IsEndSession)
                                                                                 .build();

                Debug.WriteLine("TextSendTask deliver");
                messageSender.sendMessage(address, textSecureMessage);
            }
            catch (InvalidNumberException e/*| UnregisteredUserException e*/) {
                //Log.w(TAG, e);
                //throw new InsecureFallbackApprovalException(e);
            } catch (Exception e)
            {
                //Log.w(TAG, e);
               throw new Exception(e.Message);
            }
        }
    }

}