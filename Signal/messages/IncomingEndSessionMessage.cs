using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSecure.messages
{
    class IncomingEndSessionMessage : IncomingTextMessage
    {

        public IncomingEndSessionMessage(IncomingTextMessage parent)
            : this(parent, parent.getMessageBody())
        {
        }

        public IncomingEndSessionMessage(IncomingTextMessage parent, String newBody)
            : base(parent, newBody)
        {

        }

        public new IncomingEndSessionMessage withMessageBody(String messageBody)
        {
            return new IncomingEndSessionMessage(this, messageBody);
        }


        public new bool isEndSession()
        {
            return true;
        }
    }
}
