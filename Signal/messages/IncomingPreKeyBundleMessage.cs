using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Messages
{
    class IncomingPreKeyBundleMessage : IncomingTextMessage
    {
        public IncomingPreKeyBundleMessage(IncomingTextMessage b, string newBody) : base(b, newBody)
        {
        }

        public IncomingPreKeyBundleMessage withMessageBody(string messageBody)
        {
            return new IncomingPreKeyBundleMessage(this, messageBody);
        }

        public bool IsPreKeyBundle => true;
    }
}
