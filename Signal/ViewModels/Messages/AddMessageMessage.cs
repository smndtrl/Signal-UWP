using GalaSoft.MvvmLight.Messaging;
using Signal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel.Messages
{
    public class AddMessageMessage : MessageBase
    {
        public long ThreadId { get; set; }
        public long MessageId { get; set; }

    }
}
