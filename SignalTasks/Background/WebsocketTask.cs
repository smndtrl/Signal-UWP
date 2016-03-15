using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using libtextsecure;
using libtextsecure.util;
using libtextsecure.messages;

namespace SignalTasks.Background
{


    class WebsocketTask : IBackgroundTask
    {
#if RELEASE // TODO: RELEASE
        public static readonly string PUSH_URL = "http://textsecure.simondieterle.net";
#else
        public static readonly string PUSH_URL = "http://textsecure-staging.simondieterle.net";
#endif  

        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        private volatile bool _cancelRequested = false;
        private BackgroundTaskDeferral _deferral = null;
        private IBackgroundTaskInstance _taskInstance = null;


        public void Run(IBackgroundTaskInstance taskInstance)
        {

            var messageReceiver = new TextSecureMessageReceiver(PUSH_URL,
                                         TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), TextSecurePreferences.getSignalingKey(),
                                        "Test User Agent");
            var pipe = messageReceiver.createMessagePipe();
            pipe.MessageReceived += OnMessageRecevied;

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;
            
            throw new NotImplementedException();
        }

        private void OnMessageRecevied(TextSecureMessagePipe sender, TextSecureEnvelope args)
        {
            throw new NotImplementedException();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _cancelRequested = true;
            _cancelReason = reason;
        }
    }
}
