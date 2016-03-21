using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using libtextsecure;
using libtextsecure.messages;
using Signal.Push;
using Signal.Tasks;
using Signal.Tasks.Library;
using Signal.Util;
using TextSecure.util;

namespace Signal
{
    public abstract class SignalApp : Application
    {

        public TaskHelper TaskHelper = TaskHelper.getInstance();
        public TaskWorker Worker { get; private set; }

        protected TextSecureMessagePipe pipe;

        public static string CurrentVersion => $"TextSecure for Windows {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}-{Package.Current.Id.Version.Revision}";

        public SignalApp()
        {
            this.Suspending += OnSuspending;
        }



        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
                this.DebugSettings.IsBindingTracingEnabled = true;
                this.DebugSettings.BindingFailed += (s, e) =>
                {
                    Debug.WriteLine("binding failed");
                };
            }
#endif

            switch (args.PreviousExecutionState)
            {
                case ApplicationExecutionState.Terminated:
                case ApplicationExecutionState.ClosedByUser:
                    break; // load saved application data and refresh content
                case ApplicationExecutionState.NotRunning: // normal launch
                    await TaskHelper.RegisterAll();
                    InitializeUi();
                    if (IsFirstLaunch()) OnFirstLaunch(args);
                    else OnNormalLaunch(args);
                    break;
                case ApplicationExecutionState.Running:
                // activated through secondary tile or activiation contracts and extensions
                case ApplicationExecutionState.Suspended:
                    // activated through secondary tile of activation contracts and extensions after has been suspended
                    break;
                default:
                    Log.Warn("Unexpected PreviousExecutionState");
                    break;
            }

            Worker = new TaskWorker();
            Worker.Start();
            

        }

        protected abstract void InitializeUi();


        protected abstract void OnFirstLaunch(LaunchActivatedEventArgs args);


        protected abstract void OnNormalLaunch(LaunchActivatedEventArgs args);

        protected abstract void OnSuspending(object sender, SuspendingEventArgs e);


        private bool IsFirstLaunch()
        {
            return TextSecurePreferences.getLocalNumber() == String.Empty;
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            switch (args.PreviousExecutionState)
            {
                case ApplicationExecutionState.Terminated: // restore session data
                    break;
                case ApplicationExecutionState.ClosedByUser: // start with default data
                case ApplicationExecutionState.NotRunning:
                    break; // normal launch
                default:
                    Log.Debug("Activated without anything to do.");
                    break;
            }
            //base.OnActivated(args);
        }

        protected void RunWebsocket()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var messageReceiver = TextSecureCommunicationFactory.createReceiver();
                    pipe = messageReceiver.createMessagePipe();
                    pipe.MessageReceived += OnMessageRecevied;
                }
                catch (Exception ex) { Debug.WriteLine("Failed asd:" + ex.Message); }

            });
        }

        private void OnMessageRecevied(TextSecureMessagePipe sender, TextSecureEnvelope envelope)
        {
            Log.Debug("Push message recieved");
            var task = new PushContentReceiveTask();
            task.handle(envelope, false);
            //throw new NotImplementedException("OnMessageReceived");
        }

        /*
                protected override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                // TODO: Handle URI activation
                // The received URI is eventArgs.Uri.AbsoluteUri

                var uri = eventArgs.Uri.Query;

                try
                {
                    var test = eventArgs.Uri;
                    WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(uri);
                    var param = decoder.ToDictionary(x => x.Name, x => x.Value);

                    var uuid = param["uuid"];
                    var pubKey = param["pub_key"];

                    Debug.WriteLine($"UUID: {uuid}, PubKey: {pubKey}");

                    if (uuid.Equals(string.Empty) || pubKey.Equals(string.Empty))
                    {

                    }
                } catch (Exception e)
                {
                    Debug.WriteLine($"Error while parsing protocol uri tsdevice://{uri}");
                }



            }
        }
        */
    }
}
