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

namespace Signal.ViewModel
{
    public class MessageViewModel : ViewModelBase, INavigableViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;


        //private Dictionary<long, MessageCollection> Cache = new Dictionary<long, MessageCollection>();

        public MessageViewModel(IDataService service, INavigationService navService)
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
                    Debug.WriteLine($"MessageView got Thread {SelectedThread.ThreadId}");
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
                    Debug.WriteLine("MessageView got Recipients");
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

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                if (_selectedThread == value || value == null)
                {
                    return;
                }

                var oldValue = _selectedThread;
                _selectedThread = value;

                /*if (Cache.ContainsKey(_selectedThread.ThreadId))
                {
                    Debug.WriteLine($"Cache hit for Thread {_selectedThread.ThreadId}");
                    Messages = Cache[_selectedThread.ThreadId];
                }
                else
                {
                    Debug.WriteLine($"Cache miss for Thread {_selectedThread.ThreadId}");*/
                    //var collection = new MessageCollection(_dataService, _selectedThread.ThreadId);
                    //Cache.Add(_selectedThread.ThreadId, collection);
                    //Messages = collection;
                //}


                RaisePropertyChanged(SelectedThreadPropertyName);
            }
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
        
        
            ObservableCollection<Message> _Messages;
        public ObservableCollection<Message> Messages
        {
            get { return _Messages; }
            set
            {
                _Messages = value;
                RaisePropertyChanged("Messages");
            }
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

        public void Activate(object parameter)
        {
            var thread = (Thread)parameter;

            SelectedThread = thread;
            Messages = new MessageCollection(_dataService, thread.ThreadId);
            RaisePropertyChanged("Messages");
            Debug.WriteLine($"MessageView got Thread {SelectedThread.ThreadId}");

        }

        public void Deactivate(object parameter)
        {
            //throw new NotImplementedException();
        }
    }
}
