using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libtextsecure.push;

namespace Signal.Messages
{
    internal class IncomingGroupMessage : IncomingTextMessage
    {
        private readonly TextSecureProtos.GroupContext _groupContext;

        public IncomingGroupMessage(IncomingTextMessage b, TextSecureProtos.GroupContext groupContext, string body) : base(b, body)
        {
            this._groupContext = groupContext;
        }

        public IncomingGroupMessage withMessageBody(string body)
        {
            return new IncomingGroupMessage(this, _groupContext, body);
        }

        public new bool IsGroup => true;

        public bool IsUpdate => _groupContext.Type.Equals(TextSecureProtos.GroupContext.Types.Type.UPDATE);

        public bool IsQuit => _groupContext.Type.Equals(TextSecureProtos.GroupContext.Types.Type.QUIT);
    }
}
