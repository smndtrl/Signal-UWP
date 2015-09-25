using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.database.loaders;
using Signal.Model;
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
    public class MessageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;


        private Dictionary<long, MessageCollection> Cache = new Dictionary<long, MessageCollection>();

        public MessageViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

            Messenger.Default.Register<PropertyChangedMessage<Thread>>(
                this,
                message =>
                {
                    SelectedThread = message.NewValue;
                    Debug.WriteLine("MessageView got Thread");
                }
            );

            Messenger.Default.Register<PropertyChangedMessage<Recipients>>(
                this,
                message =>
                {
                    //SelectedThread = message.NewValue;
                    Debug.WriteLine("MessageView got Recipients");
                }
            );

        }

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                if (_selectedThread == value)
                {
                    return;
                }

                var oldValue = _selectedThread;
                _selectedThread = value;

                if (Cache.ContainsKey(_selectedThread.ThreadId))
                {
                    Debug.WriteLine($"Cache hit for Thread {_selectedThread.ThreadId}");
                    Messages = Cache[_selectedThread.ThreadId];
                } else
                {
                    Debug.WriteLine($"Cache miss for Thread {_selectedThread.ThreadId}");
                    var collection = new MessageCollection(_dataService, _selectedThread.ThreadId);
                    Cache.Add(_selectedThread.ThreadId, collection);
                    Messages = collection;
                }

                
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

        private RelayCommand<MessageDatabase.MessageTable> _sendCommand;
        public RelayCommand<MessageDatabase.MessageTable> SendCommand
        {
            get
            {
                return _sendCommand ?? (_sendCommand = new RelayCommand<MessageDatabase.MessageTable>( async
                    p =>
                    {
                        /*var message = new OutgoingEncryptedMessage(SelectedThread.Recipients, MessageText); // TODO:
                        MessageText = "";

                        await MessageSender.send(message, SelectedThread.ThreadId);
                        */
                        Debug.WriteLine($"Sending:");
                    },
                    p => true));
            }
        }
    }
}
