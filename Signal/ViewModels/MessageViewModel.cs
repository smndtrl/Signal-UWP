using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.database.loaders;
using Signal.Models;
using Signal.ViewModel.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure;
using TextSecure.database;
using TextSecure.messages;
using TextSecure.recipient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Signal.Resources;
using Signal.Views;

namespace Signal.ViewModels
{
    public class MessageViewModel : ViewModelBase, INavigableViewModel, IAmbientColor
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;


        //private Dictionary<long, MessageCollection> Cache = new Dictionary<long, MessageCollection>();

        

        public MessageViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;


            /*Messenger.Default.Register<PropertyChangedMessage<Thread>>(
                this,
                message =>
                {
                    SelectedThread = message.NewValue;
                    Messages = new MessageCollection(_dataService, SelectedThread.ThreadId);
                    RaisePropertyChanged("Messages");
                    Debug.WriteLine($"MessageDetailPage got Thread {SelectedThread.ThreadId}");
                }
            );*/



            /*Messenger.Default.Register<RefreshThreadMessage>(
            this,
            async message =>
            {

                Debug.WriteLine($"(MessageViewModel)Refreshing message Collection for Thread {message.ThreadId}");
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //Clear();
                });
                
            }
        );*/

            /*Messenger.Default.Register<PropertyChangedMessage<Recipients>>(
                this,
                message =>
                {
                    //SelectedThread = message.NewValue;
                    Debug.WriteLine("MessageDetailPage got Recipients");
                }
            );*/

            /*Messenger.Default.Register<AddMessageMessage>(
               this,
               msg =>
               {
                   Debug.WriteLine($"New Message for Thread {msg.ThreadId}: #{msg.MessageId}");
                   var message = DatabaseFactory.getTextMessageDatabase().Get(msg.MessageId);
                   Cache[msg.ThreadId].Add(message);
               }
           );*/

        }

        private RelayCommand _loaded;
        public RelayCommand Loaded
        {
            get
            {
                return _loaded ?? (_loaded = new RelayCommand(
                   () =>
                        {
                            Window.Current.SizeChanged += Window_SizeChanged;
                        },
                    () => true));
            }
        }

        private RelayCommand _unloaded;
        public RelayCommand Unloaded
        {
            get
            {
                return _unloaded ?? (_unloaded = new RelayCommand(
                   () =>
                    {
                        Window.Current.SizeChanged -= Window_SizeChanged;
                    },
                    () => true));
            }
        }

        private bool ShouldGoToWideState()
        {
            return Window.Current.Bounds.Width >= 720;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (ShouldGoToWideState())
            {
                // Make sure we are no longer listening to window change events.
                Window.Current.SizeChanged -= Window_SizeChanged;

                // We shouldn't see this page since we are in "wide master-detail" mode.
                NavigateBackForWideState(useTransition: false);
            }
        }

        void NavigateBackForWideState(bool useTransition)
        {
            _navigationService.GoBack();
        }




        private bool _isBackEnabled = true;
        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }
            set { Set(ref _isBackEnabled, value); }
        }

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set { Set(ref _selectedThread, value); }
        }

        string _messageText { get; set; } = "";
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                Debug.WriteLine("changed");
                _messageText = value;
                RaisePropertyChanged("MessageText");
            }
        }

        /*MessageCollection _Messages;
        public MessageCollection Messages
        {
            get { return _Messages; }
            set
            {
                _Messages = value;
                RaisePropertyChanged("Messages");
            }
        }*/


        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set { Set(ref _messages, value); }
        }

        private RelayCommand _scrollCommand;
        public RelayCommand ScrollCommand
        {
            get
            {
                return _scrollCommand ?? (_scrollCommand = new RelayCommand(
                   () =>
                   {
                       //DatabaseFactory.getTextMessageDatabase().Test(message.MessageId);

                       Debug.WriteLine($"Marked as sent:");
                   },
                    () => true));
            }
        }

        private RelayCommand _sendCommand;
        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand ?? (_sendCommand = new RelayCommand(async
                   () =>
                   {
                       var message = new OutgoingEncryptedMessage(SelectedThread.Recipients, MessageText); // TODO:
                       MessageText = "";

                       await MessageSender.send(message, SelectedThread.ThreadId);

                       Debug.WriteLine($"Sending:");
                   },
                    () => true));
            }
        }

        /*
         * Messages
         */

        public RelayCommand<Message> DeleteCommand;

        private RelayCommand<Message> _updateCommand;

        public event EventHandler AmbientColorChanged;

        public RelayCommand<Message> UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand<Message>(
                   message =>
                {
                    //DatabaseFactory.getTextMessageDatabase().Test(message.MessageId);

                    Debug.WriteLine($"Marked as sent:");
                },
                    message => true));
            }
        }

        public SolidColorBrush AmbientColorBrush = new SolidColorBrush((Color)Application.Current.Resources["SignalBlue"]);

        private Color _ambientColor = (Color)Application.Current.Resources["SignalBlue"];
        public Color AmbientColor
        {
            get
            {
                return _ambientColor;
            }

            set
            {
                Set(ref _ambientColor, value);
                if (AmbientColorChanged != null)
                {
                    AmbientColorChanged(this, new EventArgs());
                }
            }
        }

        public void Activate(object parameter)
        {

            var args = parameter as NavigationEventArgs;

            var thread = (Thread)args.Parameter;

            if (thread == null)
            {
                Debug.WriteLine($"{GetType().FullName}: Activate without Thread");
                Messages = new MessageCollection(_dataService, 0);
                return; // TODO:
            }

            SelectedThread = thread;
            Messages = new MessageCollection(_dataService, thread.ThreadId);
            RaisePropertyChanged("Messages");
            Debug.WriteLine($"{GetType().Name}: Activate with Thread #{thread.ThreadId}");

        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }
    }
}
