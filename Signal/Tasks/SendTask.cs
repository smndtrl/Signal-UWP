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
    public class SendTask : UntypedTaskActivity
    {
        public override void onAdded()
        {
            throw new NotImplementedException("SendTask onAdded");
        }

        public new void OnCanceled()
        {
            throw new NotImplementedException("SendTask OnCanceled");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("SendTask Execure");
        }

        protected TextSecureAddress getPushAddress(String number)
        {
            String e164number = Utils.canonicalizeNumber(number);
            String relay = DatabaseFactory.getDirectoryDatabase().getRelay(e164number);
            return new TextSecureAddress(e164number, relay == null ? May<string>.NoValue : new May<string>(relay));
        }

        protected TextSecureMessageSender messageSender = new TextSecureMessageSender(TextSecureCommunicationFactory.PUSH_URL, new TextSecurePushTrustStore(), TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), new TextSecureAxolotlStore(),
                                                                                   May<TextSecureMessageSender.EventListener>.NoValue, App.CurrentVersion);
    }
}
