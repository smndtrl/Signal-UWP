using libtextsecure;
using libtextsecure.push;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure;
using TextSecure.database;
using TextSecure.push;
using TextSecure.util;

namespace Signal.Tasks
{
    public class SendTask : UntypedTaskActivity
    {
        public override void onAdded()
        {
            throw new NotImplementedException("SendTask onAdded");
        }

        protected override string ExecuteAsync()
        {
            throw new NotImplementedException("SendTask Execure");
        }

        protected TextSecureAddress getPushAddress(String number)
        {
            String e164number = Util.canonicalizeNumber(number);
            String relay = TextSecureDirectory.getInstance().getRelay(e164number);
            return new TextSecureAddress(e164number, relay.Equals("") ? May<string>.NoValue : new May<string>(relay));
        }

        protected TextSecureMessageSender messageSender = new TextSecureMessageSender(TextSecureCommunicationFactory.PUSH_URL, new TextSecurePushTrustStore(), TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), new TextSecureAxolotlStore(),
                                                                                   May<TextSecureMessageSender.EventListener>.NoValue);
    }
}
