namespace Signal.Messages
{
    class IncomingEndSessionMessage : IncomingTextMessage
    {
        public IncomingEndSessionMessage(IncomingTextMessage parent)
            : this(parent, parent.getMessageBody())
        {
        }

        public IncomingEndSessionMessage(IncomingTextMessage parent, string newBody)
            : base(parent, newBody)
        {

        }

        public new IncomingEndSessionMessage withMessageBody(string messageBody)
        {
            return new IncomingEndSessionMessage(this, messageBody);
        }

        public new bool IsEndSession => true;
    }
}
