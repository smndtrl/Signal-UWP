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
using TextSecure.push;
using TextSecure.util;

namespace Signal.Tasks
{
    public class ReceiveTask : UntypedTaskActivity
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
                TextSecureDirectory directory = TextSecureDirectory.getInstance();
                ContactTokenDetails contactTokenDetails = new ContactTokenDetails();
                contactTokenDetails.setNumber(envelope.getSource());

                directory.setNumber(contactTokenDetails, true);
            }

            if (envelope.isReceipt()) handleReceipt(envelope);
            else handleMessage(envelope, sendExplicitReceipt);
        }

        private void handleMessage(TextSecureEnvelope envelope, bool sendExplicitReceipt)
        {
            throw new NotImplementedException("ReceiveTask handleMessage");
            /* TODO: TASKS enable
            var worker = App.Current.Worker;

            //JobManager jobManager = ApplicationContext.getInstance(context).getJobManager();
            long messageId = DatabaseFactory.getPushDatabase().insert(envelope);

            if (sendExplicitReceipt)
            {
                worker.AddTaskActivities(new DeliveryReceiptTask(envelope.getSource(),
                                                      envelope.getTimestamp(),
                                                      envelope.getRelay()));
            }

            worker.AddTaskActivities(new PushDecryptTask(messageId, envelope.getSource()));
            */
        }

        private void handleReceipt(TextSecureEnvelope envelope)
        {
            Debug.WriteLine(String.Format("Received receipt: (XXXXX, %d)", envelope.getTimestamp()));
            DatabaseFactory.getMessageDatabase().incrementDeliveryReceiptCount(envelope.getSource(),
                                                                                     (long)envelope.getTimestamp());
        }

        private bool isActiveNumber( String e164number)
        {
            bool isActiveNumber;

            try
            {
                isActiveNumber = TextSecureDirectory.getInstance().isActiveNumber(e164number);
            }
            catch (/*NotInDirectory*/Exception e)
            {
                isActiveNumber = false;
            }

            return isActiveNumber;
        }
    }
}
