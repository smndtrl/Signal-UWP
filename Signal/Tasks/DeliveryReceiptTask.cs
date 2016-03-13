using libtextsecure;
using libtextsecure.push;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using TextSecure;
using Signal.Push;
using Signal.Util;
using TextSecure.util;
using Signal.Database;

namespace Signal.Tasks
{
    public class DeliveryReceiptTask : UntypedTaskActivity
    {

        private string destination;
        private long timestamp;
        private string relay;

        protected TextSecureMessageSender messageSender = new TextSecureMessageSender(TextSecureCommunicationFactory.PUSH_URL, new TextSecurePushTrustStore(), TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), new TextSecureAxolotlStore(),
                                                                          May<TextSecureMessageSender.EventListener>.NoValue, App.CurrentVersion);


        public DeliveryReceiptTask(string destination, ulong timestamp, string relay)
        {
            this.destination = destination;
            this.timestamp = (long)timestamp;
            this.relay = relay;
        }

        public override void onAdded()
        {
            //throw new NotImplementedException("SendTask onAdded");
        }

        public new void OnCanceled()
        {
            //throw new NotImplementedException("SendTask OnCanceled");
        }

        protected override string Execute()
        {
            Log.Debug("DeliveryReceiptJob : Sending delivery receipt...");
            TextSecureAddress textSecureAddress = new TextSecureAddress(destination, new May<string>(relay));

            messageSender.sendDeliveryReceipt(textSecureAddress, (ulong)timestamp);

            return "";
        }

    }
}
