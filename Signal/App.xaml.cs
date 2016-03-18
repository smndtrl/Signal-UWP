using Bezysoftware.Navigation.BackButton;
using libtextsecure;
using Signal.Push;
using Signal.Tasks;
using Signal.Tasks.Library;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using libtextsecure.messages;
using Signal.Views;
using GalaSoft.MvvmLight.Threading;
using Signal.Util;

namespace Signal
{


    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : SignalApp
    {
        public static new App Current => Application.Current as App;

        public static string CurrentVersion => $"TextSecure for Windows {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}-{Package.Current.Id.Version.Revision}";

        /*private static Frame _frame;
         
        public static Frame Frame
        {
            get { return _frame; }
        }*/

        public TaskWorker Worker { get; private set; }


        private PushNotificationChannel channel;
        public TextSecureAccountManager accountManager;
        private TextSecureMessageReceiver messageReceiver;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {

            this.InitializeComponent();

        }


        /*private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {

            string typeString = String.Empty;
            string notificationContent = String.Empty;
            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    typeString = "Badge";
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;
                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    typeString = "Tile";
                    break;
                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    typeString = "Toast";
                    // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts: 
                    // if your application is already on the screen, there's no need to display a toast from push notifications. 
                    e.Cancel = true;
                    break;
                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    typeString = "Raw";
                    break;
            }

            Debug.WriteLine($"Notification recieved: {typeString}");
        }*/





        protected override void InitializeUi()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
            }

            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            BackButtonManager.RegisterFrame(rootFrame, true, true, true);
        }

        protected override async void OnNormalLaunch(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;




            DispatcherHelper.Initialize();

            accountManager = TextSecureCommunicationFactory.createManager();


            Worker = new TaskWorker();
            Worker.Start();

            //await DirectoryHelper.refreshDirectory();

            // var task = new EchoActivity("ASDFFDSA");
            var websocketTask = new WebsocketTask();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var messageReceiver = TextSecureCommunicationFactory.createReceiver();
                    var pipe = messageReceiver.createMessagePipe();
                    pipe.MessageReceived += OnMessageRecevied;
                }
                catch (Exception ex) { Debug.WriteLine("Failed asd:" + ex.Message); }

            });
            //Worker.AddTaskActivities(websocketTask);
            //App.Current.Worker.AddTaskActivities(new RefreshPreKeysTask());


            if (rootFrame != null && rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            Window.Current.Activate();

        }


        protected override void OnFirstLaunch(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(ExtendedSplash), e.Arguments);
            }

            Window.Current.Activate();

        }

        private void OnMessageRecevied(TextSecureMessagePipe sender, TextSecureEnvelope envelope)
        {
            Log.Debug("Push message recieved");
            var task = new PushContentReceiveTask();
            task.handle(envelope, false);
            //throw new NotImplementedException("OnMessageReceived");
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
