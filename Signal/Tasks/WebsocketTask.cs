using libtextsecure;
using libtextsecure.messages;
using libtextsecure.push;
using libtextsecure.util;
using Signal.Push;
using Signal.Tasks.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Signal.Push;
using TextSecure.util;

namespace Signal.Tasks
{
    class WebsocketTask : UntypedTaskActivity
    {

        public WebsocketTask()
        {
        }

        public override void onAdded()
        {

        }

        protected override string Execute()
        {
            throw new NotImplementedException("TextSendTask Execute happened");
        }

        protected override async Task<string> ExecuteAsync()
        {
            /*var messageReceiver = TextSecureCommunicationFactory.createReceiver();
            var pipe = messageReceiver.createMessagePipe();
            var callback = new MessageReceiverCallback();
            try
            {
                pipe.read(60, callback);
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }*/

            return "";
        }


    }

}