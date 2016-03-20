using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Sockets;
using Windows.Web;
using libtextsecure;
using libtextsecure.messages;

namespace SignalTasks
{


    public sealed class WebsocketTask : IBackgroundTask
    {
#if RELEASE // TODO: RELEASE
        private static readonly string PUSH_URL = "http://textsecure.simondieterle.net";
#else
        private static readonly string PUSH_URL = "http://textsecure-staging.simondieterle.net";
#endif  

        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        private volatile bool _cancelRequested = false;
        private BackgroundTaskDeferral _deferral = null;
        private IBackgroundTaskInstance _taskInstance = null;

        MessageWebSocket socket;
        private Timer keepAliveTimer;

        private bool HasRecieved = false;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskInstance.Canceled += OnCanceled;

            _deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            //var messageReceiver = new TextSecureMessageReceiver(PUSH_URL, TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), TextSecurePreferences.getSignalingKey(), "Test User Agent");
            //var connection = new WebSocketConnection(PUSH_URL, TextSecurePreferences.getLocalNumber(), TextSecurePreferences.getPushServerPassword(), "Test User Agent");
            //connection.Connected += OnConnected;
            var username = TextSecurePreferences.getLocalNumber();
            var password = TextSecurePreferences.getPushServerPassword();
            var userAgent = "ASdfA";

            try
            {
                socket = new MessageWebSocket();
                if (userAgent != null) socket.SetRequestHeader("X-Signal-Agent", userAgent);
                socket.MessageReceived += OnMessageReceived;
                socket.Closed += OnClosed;

                var wsUri = PUSH_URL.Replace("https://", "wss://")
                                              .Replace("http://", "ws://") + $"/v1/websocket/?login={username}&password={password}";
                Uri server = new Uri(wsUri);
                await socket.ConnectAsync(server);
                Debug.WriteLine("WebsocketTask connected...");
                keepAliveTimer = new Timer(sendDisconnect, null, TimeSpan.FromSeconds(15), Timeout.InfiniteTimeSpan);


                //messageWriter = new DataWriter(socket.OutputStream);



            }
            catch (Exception e)
            {
                WebErrorStatus status = WebSocketError.GetStatus(e.GetBaseException().HResult);

                switch (status)
                {
                    case WebErrorStatus.CannotConnect:
                    case WebErrorStatus.NotFound:
                    case WebErrorStatus.RequestTimeout:
                        Debug.WriteLine("Cannot connect to the server. Please make sure " +
                            "to run the server setup script before running the sample.");
                        break;

                    case WebErrorStatus.Unknown:
                        throw;

                    default:
                        Debug.WriteLine("Error: " + status);
                        break;
                }

                Debug.WriteLine("fuuuu");
                socket.Close(1000, "None");
            }

            //var pipe = messageReceiver.createMessagePipe();
            // pipe.MessageReceived += OnMessageRecevied;

        }

        private void sendDisconnect(object state)
        {
            socket.Close(1000, "None");
        }

        private void OnClosed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebsocketTask disconnected...");
            var t = args;
            // TODO return
            _deferral.Complete();
        }

        private void OnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            var t = args;
            HasRecieved = false;

            _deferral.Complete();
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // TODO return
            _cancelRequested = true;
            _cancelReason = reason;
        }
    }
}
