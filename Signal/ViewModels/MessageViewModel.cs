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
using TextSecure.recipient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Signal.Messages;
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

        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set { Set(ref _selectedThread, value); RaisePropertyChanged("SelectedThread"); }
        }

        private string _messageText = string.Empty;
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                Set(ref _messageText, value);
                SendCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("MessageText");
            }
        }

        private ObservableCollection<MessageRecord> _messages;
        public ObservableCollection<MessageRecord> Messages
        {
            get { return _messages; }
            set { Set(ref _messages, value); RaisePropertyChanged("Messages"); }
        }

        private RelayCommand _scrollCommand;
        public RelayCommand ScrollCommand
        {
            get
            {
                return _scrollCommand ?? (_scrollCommand = new RelayCommand(
                   () =>
                   {

                   },
                    () => true));
            }
        }

        private RelayCommand _sendCommand;
        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand ?? (_sendCommand = new RelayCommand(
                    async () =>
                    {
                       var message = new OutgoingEncryptedMessage(SelectedThread.Recipients, MessageText); // TODO:
                       MessageText = string.Empty;

                       await MessageSender.send(message, SelectedThread.ThreadId);
                    },
                    () => !MessageText.Equals(string.Empty)));
            }
        }

        private RelayCommand _attachCommand;
        public RelayCommand AttachCommand
        {
            get
            {
                return _attachCommand ?? (_attachCommand = new RelayCommand(
                    async () =>
                    {
 
                    },
                    () => false )); // TODO: attachment enable
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

        private RelayCommand _phoneCommand;
        public RelayCommand PhoneCommand
        {
            get
            {
                return _phoneCommand ?? (_phoneCommand = new RelayCommand(
                    () => { return; },
                    () => false)
                    );
            }
        }

        private RelayCommand _resetSessionCommand;
        public RelayCommand ResetSessionCommand
        {
            get
            {
                return _resetSessionCommand ?? (_resetSessionCommand = new RelayCommand(
                    async () =>
                    {
                        var endSessionMessage = new OutgoingEndSessionMessage(new OutgoingTextMessage(this.SelectedThread.Recipients, "TERMINATE"));

                        await MessageSender.send(endSessionMessage, this.SelectedThread.ThreadId);
                    },
                    () => true)
                    );
            }
        }

        public void NavigateTo(NavigationEventArgs args)
        {

            var thread = (Thread)args.Parameter;

            if (thread == null)
            {
                Debug.WriteLine($"{GetType().FullName}: Activate without Thread");
                //_messages = null;
                Messages = new MessageCollection(_dataService, 0);
                return; // TODO:
            }

            SelectedThread = thread;
           // _messages = null;
            Messages = new MessageCollection(_dataService, thread.ThreadId);
            Debug.WriteLine($"{GetType().Name}: Activate with Thread #{thread.ThreadId}");

        }

        public void NavigateFrom(NavigationEventArgs args)
        {
            //throw new NotImplementedException();
        }
    }
}
