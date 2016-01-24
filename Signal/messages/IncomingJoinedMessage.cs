using libtextsecure.messages;
using Signal.Util;
using Strilanc.Value;

namespace Signal.Messages
{
    internal class IncomingJoinedMessage : IncomingTextMessage
    {
        public IncomingJoinedMessage(string sender) : base(sender, 1, (ulong)TimeUtil.GetUnixTimestampMillis(), null, May<TextSecureGroup>.NoValue)
        {
        }

        public bool IsJoined => true;

        public bool IsSecureMessage => true;
    }
}
