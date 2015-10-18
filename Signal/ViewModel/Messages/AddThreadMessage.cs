using GalaSoft.MvvmLight.Messaging;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel.Messages
{
    public class AddThreadMessage : MessageBase
    {
        public long ThreadId { get; set; }
    }
}
