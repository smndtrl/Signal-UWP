using libtextsecure;
using libtextsecure.messages;
using libtextsecure.push;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure;
using TextSecure.database;
using Signal.Push;
using TextSecure.util;
using Signal.Database;
using Signal.Util;

namespace Signal.Tasks
{
    public class PushReceivedTask : UntypedTaskActivity
    {
        public override void onAdded()
        {
            throw new NotImplementedException("ReceiveTask onAdded");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("ReceiveTask Execute");
        }

        public void handle(TextSecureEnvelope envelope, bool sendExplicitReceipt)
        {
            if (!isActiveNumber(envelope.getSource()))
            {
                TextSecureDirectory directory = DatabaseFactory.getDirectoryDatabase();
                ContactTokenDetails contactTokenDetails = new ContactTokenDetails();
                contactTokenDetails.setNumber(envelope.getSource());

                directory.setNumber(contactTokenDetails, true);
            }

            if (envelope.isReceipt()) handleReceipt(envelope);
            else handleMessage(envelope, sendExplicitReceipt);
        }

        private void handleMessage(TextSecureEnvelope envelope, bool sendExplicitReceipt)
        {
            var worker = App.Current.Worker;
            long messageId = DatabaseFactory.getPushDatabase().Insert(envelope);

            if (sendExplicitReceipt)
            {
             /*   worker.AddTaskActivities(new DeliveryReceiptTask(envelope.getSource(),
                                                      envelope.getTimestamp(),
                                                      envelope.getRelay()));*/
            }

            worker.AddTaskActivities(new PushDecryptTask(messageId, envelope.getSource()));
            
        }

        private void handleReceipt(TextSecureEnvelope envelope)
        {
            Log.Debug($"Received receipt: (XXXXX, {envelope.getTimestamp()})");
            DatabaseFactory.getMessageDatabase().incrementDeliveryReceiptCount(envelope.getSource(),
                                                                                     (long)envelope.getTimestamp());
        }

        private bool isActiveNumber( String e164number)
        {
            bool isActiveNumber;

            try
            {
                isActiveNumber = DatabaseFactory.getDirectoryDatabase().isActiveNumber(e164number);
            }
            catch (/*NotInDirectory*/Exception e)
            {
                isActiveNumber = false;
            }

            return isActiveNumber;
        }
    }
}
