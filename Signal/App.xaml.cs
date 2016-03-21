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
            RunWebsocket();



            //await DirectoryHelper.refreshDirectory();

            // var task = new EchoActivity("ASDFFDSA");

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
            Worker.Stop();
            pipe.shutdown();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
