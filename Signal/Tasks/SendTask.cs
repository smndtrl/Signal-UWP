using libtextsecure;
using libtextsecure.push;
using Signal.Tasks.Library;
using Strilanc.Value;
using System;
using TextSecure;
using Signal.Push;
using Signal.Util;
using TextSecure.util;
using Signal.Database;

namespace Signal.Tasks
{
    public class SendTask : UntypedTaskActivity
    {
        public override void onAdded()
        {
            throw new NotImplementedException("SendTask onAdded");
        }

        public new void OnCanceled()
        {
            throw new NotImplementedException("SendTask OnCanceled");
        }

        protected override string Execute()
        {
            throw new NotImplementedException("SendTask Execure");
        }

    }
}
