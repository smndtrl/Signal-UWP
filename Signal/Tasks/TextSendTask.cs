using libtextsecure;
using libtextsecure.messages;
using libtextsecure.push;
using libtextsecure.util;
using Signal.database.models;
using Signal.Models;
using Signal.Tasks.Library;
using Signal.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.util;

namespace Signal.Tasks
{
    class PushTextSendTask : SendTask
    {
        private long messageId;

        public PushTextSendTask(long messageId, string destination)
        {
            this.messageId = messageId;
        }

        public override void onAdded()
        {
            MessageDatabase database = DatabaseFactory.getMessageDatabase();
            database.markAsSending(messageId);
            database.markAsPush(messageId);

            Debug.WriteLine("Pushsendtask onadded");

            //throw new NotImplementedException();
        }

        protected override string Execute()
        {
            throw new NotImplementedException("TextSendTask Execute happened");
        }

        protected override async Task<string> ExecuteAsync()
        {
            Debug.WriteLine("executeasync");
            TextMessageDatabase database = DatabaseFactory.getTextMessageDatabase();
            var message = await database.getMessageRecord(messageId);
            //var message = await database.Get(messageId);

            try
            {
                deliver(message);
                database.markAsPush(messageId);
                database.markAsSecure(messageId);
                database.markAsSent(messageId);

            } catch(Exception e) {
                Debug.WriteLine($"{GetType()} failure {e.Message}");
            }


            return "";
            //throw new NotImplementedException();
        }

        private void deliver(SmsMessageRecord message)
        {
            try
            {
                TextSecureAddress address = getPushAddress(message.getIndividualRecipient().getNumber());
                TextSecureDataMessage textSecureMessage = TextSecureDataMessage.newBuilder()
                                                                                 .withTimestamp((ulong)TimeUtil.GetUnixTimestampMillis(message.getDateSent()))
                                                                                 .withBody(message.getBody().getBody())
                                                                                 .asEndSessionMessage(message.isEndSession())
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
                //throw new RetryLaterException(e);
            }
        }
    }

}