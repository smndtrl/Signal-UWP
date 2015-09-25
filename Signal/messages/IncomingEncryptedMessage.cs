using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.messages
{
    class IncomingEncryptedMessage : IncomingTextMessage
    {
        public IncomingEncryptedMessage(IncomingTextMessage parent, String newBody)
            : base(parent, newBody)
        {
        }

        public new IncomingTextMessage withMessageBody(String body)
        {
            return new IncomingEncryptedMessage(this, body);
        }

        public new bool isSecureMessage()
        {
            return true;
        }
    }


}
