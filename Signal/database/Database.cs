using GalaSoft.MvvmLight.Messaging;
using Signal.ViewModel.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Database
{
    public class Database : MessageTypes
    {
        protected void notifyConversationListeners(long threadId)
        {
            Messenger.Default.Send(new RefreshThreadMessage() { ThreadId = threadId });
        }
    }
}
