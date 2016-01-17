using GalaSoft.MvvmLight.Messaging;
using Signal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel.Messages
{
    public class RefreshThreadMessage : MessageBase
    {
        public long ThreadId { get; set; }
    }

    public class RefreshThreadListMessage : MessageBase
    {
    }

    public class RefreshDirectoryListMessage : MessageBase
    {
    }
}
