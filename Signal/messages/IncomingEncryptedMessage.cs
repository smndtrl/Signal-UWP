using System;

namespace Signal.Messages
{
    class IncomingEncryptedMessage : IncomingTextMessage
    {
        public IncomingEncryptedMessage(IncomingTextMessage parent, string newBody)
            : base(parent, newBody)
        {
        }

        public new IncomingTextMessage WithMessageBody(string body)
        {
            return new IncomingEncryptedMessage(this, body);
        }

        public new bool IsSecureMessage()
        {
            return true;
        }
    }


}
