using libtextsecure;
using Signal.Database;
using Signal.Tasks;
using Signal.Tasks.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TextSecure;
using TextSecure.push;
using TextSecure.util;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Signal
{


    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static new App Current
        {
            get { return Application.Current as App; }
        }

        public TaskWorker Worker { get; private set;  }

        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static Microsoft.ApplicationInsights.TelemetryClient TelemetryClient;
        //public static Microsoft.WindowsAzure.MobileServiceClient client = new Microsoft.WindowsAzure.MobileSerivces.MobileServiceClient("http://textsecure.azure-mobile.net/")


        private PushNotificationChannel channel;
        public TextSecureAccountManager accountManager;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            TelemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();

            this.InitializeComponent();
            this.Suspending += OnSuspending;

            using (var db = new SignalContext())
            {
                Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
                //db.Database.ApplyMigrations();
                db.Database.EnsureCreated();
            }

            //var culture = new CultureInfo("en-US");
            /* Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
             CultureInfo.DefaultThreadCurrentCulture = culture;
             CultureInfo.DefaultThreadCurrentUICulture = culture;*/


            /* 
            new TextSecureAccountManager(URL, TRUST_STORE,
                                                                        USERNAME, PASSWORD);

     */

            App.Current.DebugSettings.IsBindingTracingEnabled = true;
            App.Current.DebugSettings.BindingFailed += (s, e) =>
            {
                Debug.WriteLine("binding failed");
            };




        }

        /*private IBackgroundTaskRegistration GetRegisteredTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (task.Name == "UpdateChannel")
                {
                    return task;
                }
            }
            return null;
        }

        private async Task<bool> init()
        {
            if (channel == null)
            {
                var response = await PushHelper.getInstance().OpenChannelAndUpload();
                channel = response.Channel;

                channel.PushNotificationReceived += OnPushNotificationReceived;
            }

            if (GetRegisteredTask() == null)
            {
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                MaintenanceTrigger trigger = new MaintenanceTrigger(24*60, false);
                taskBuilder.SetTrigger(trigger);
                taskBuilder.TaskEntryPoint = "MaintenanceTask";
                taskBuilder.Name = "UpdateChannel";

                SystemCondition internetCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                taskBuilder.AddCondition(internetCondition);

                try
                {
                    

                    taskBuilder.Register();
                    //rootPage.NotifyUser("Task registered", NotifyType.StatusMessage);
                }
                catch (Exception ex)
                {
                    //rootPage.NotifyUser("Error registering task: " + ex.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                //rootPage.NotifyUser("Task already registered", NotifyType.ErrorMessage);
            }

            //var response = await PushHelper.getInstance().OpenChannelAndUpload();

            return true;
        }*/

        private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
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
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            accountManager = TextSecureCommunicationFactory.createManager();

            TaskHelper.getInstance().RegisterPushReceiver();

            // re request a channel
            if (TextSecurePreferences.isPushRegistered() == true)
            {
                if (channel == null)
                {
                    //Debug.WriteLine("requesting new channel");
                    var response = await PushHelper.getInstance().OpenChannelAndUpload();

                    channel = response.Channel;
                   // Debug.WriteLine($"Save Channel URI: {channel.Uri}");
                    //channel.PushNotificationReceived += OnPushNotificationReceived;
                }
            }

            Worker = new TaskWorker();
            //Worker.Start();

           // var task = new EchoActivity("ASDFFDSA");

            //manager.AddTaskActivities(task);


            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                //rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];


                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                    ExtendedSplash extendedSplash = new ExtendedSplash();
                    rootFrame.Content = extendedSplash;
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            /*if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }*/
            // Ensure the current window is active
            Window.Current.Activate();
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
