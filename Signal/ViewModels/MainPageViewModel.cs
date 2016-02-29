using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.database.loaders;
using Signal.Database;
using Signal.Models;
using Signal.Resources;
using Signal.Util;
using Signal.ViewModel.Messages;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;
using TextSecure.util;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region contacts

        DirectoryCollection _Contacts;
        public DirectoryCollection Contacts
        {
            get { return _Contacts ?? (Contacts = new DirectoryCollection(_dataService)); }
            set
            {
                _Contacts = value;
                RaisePropertyChanged("Contacts");
            }
        }

        TextSecureDirectory.Directory _selectedContact;
        public TextSecureDirectory.Directory SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                Debug.WriteLine("SelectedContact");
                //AddCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedContact");

                OpenChat(value);
            }
        }

        private void OpenChat(TextSecureDirectory.Directory contacts)
        {
            Recipients recipients = DatabaseFactory.getRecipientDatabase().GetOrCreateRecipients(SelectedContact); //RecipientFactory.getRecipientsFromContact(SelectedContact);
            var chatid = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients, 0);
            var chat = DatabaseFactory.getThreadDatabase().Get(chatid);
            OpenChat(chat);
        }

        private void OpenChat(Thread chat)
        {
            if (State != null && State.Name == "NarrowState") // navigate to detail page
            {
                Debug.WriteLine($"NarrowState -> Thread #{chat.ThreadId}");
                _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY, chat);
            }
            else if (State != null && State.Name == "DefaultState")
            {
                Debug.WriteLine($"WideState -> Thread #{chat.ThreadId}");
                DetailFrame.Navigate(typeof(MessageDetailPage), chat);
            }
        }

        // commands
        //private RelayCommand<TextSecureDirectory.Directory> _addCommand;
        //public RelayCommand<TextSecureDirectory.Directory> AddCommand
        //{
        //    get
        //    {
        //        return _addCommand ?? (_addCommand = new RelayCommand<TextSecureDirectory.Directory>(
        //           p => { addCommandInternal(); },
        //           p => SelectedContact != null
        //           ));
        //    }
        //}

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new RelayCommand(
                   () => {
                       refreshCommandInternal(); },
                   () => true
                   ));
            }
        }

        private async Task refreshCommandInternal()
        {
            await DirectoryHelper.refreshDirectory();
            Contacts.Requery();

        }

        private async void addCommandInternal()
        {
            Recipients recipients = DatabaseFactory.getRecipientDatabase().GetOrCreateRecipients(SelectedContact); //RecipientFactory.getRecipientsFromContact(SelectedContact);
            var threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients, 0);

            _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY, DatabaseFactory.getThreadDatabase().Get(threadId));
        }
        #endregion

        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;

        public MainPageViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

            Chats = new ThreadCollection(service);
        }

        public Frame DetailFrame;

        private VisualStateGroup _vsg;
        public VisualStateGroup AdaptiveStates {
            get
            {
                return _vsg;
            }
            set
            {
                State = value.CurrentState;
                Set(ref _vsg, value);
            }
        }

        private Thread _selectedChat = null;
        public Thread SelectedChat
        {
            get { return _selectedChat; }
            set
            {
                OpenChat(value);
                Set(ref _selectedChat, value);
            }
        }

        ThreadCollection _chats;
        public ThreadCollection Chats
        {
            get { return _chats ?? (Chats = new ThreadCollection(_dataService)); }
            set { Set(ref _chats, value); }
        }

        //private RelayCommand _addCommand;
        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return _addCommand ?? (_addCommand = new RelayCommand(
        //          () =>
        //        {
        //            _navigationService.NavigateTo(ViewModelLocator.DIRECTORY_PAGE_KEY);
                   
        //        },
        //            () => true));
        //    }
        //}


        private RelayCommand<Thread> _deleteCommand;
        public RelayCommand<Thread> DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new RelayCommand<Thread>(
                  thread =>
                  {
                      Debug.WriteLine($"Should delete {thread.Recipients.ShortString}");

                      DatabaseFactory.getThreadDatabase().DeleteConversation(thread.ThreadId);
                      Chats.Remove(thread);
                      //deleteConversations(selectedConversations);

                      /*var menuFlyoutItem = sender as MenuFlyoutItem;
                      if (menuFlyoutItem != null)
                      {
                          var thread = menuFlyoutItem.DataContext as Thread;
                          if (thread != null)
                          {
                              Debug.WriteLine($"Should delete {thread.Recipients.ShortString}");

                          }
                      }*/


                  },
                   obj => true));
            }
        }

        private RelayCommand _flyoutCommand;
        public RelayCommand FlyoutCommand
        {
            get
            {
                return _flyoutCommand ?? (_flyoutCommand = new RelayCommand(
                  () =>
                  {

                  },
                   () => true));
            }
        }

        private RelayCommand _loaded;
        public RelayCommand Loaded
        {
            get
            {
                return _loaded ?? (_loaded = new RelayCommand(
                  () =>
                  {
                      //if (_selectedThread != null) SelectedThread = _selectedThread;

                  },
                   () => true));
            }
        }



        /*
         * VISUAL STATE
         */

        private RelayCommand<VisualStateChangedEventArgs> _stateChanged;
        public RelayCommand<VisualStateChangedEventArgs> StateChanged
        {
            get
            {
                return _stateChanged ?? (_stateChanged = new RelayCommand<VisualStateChangedEventArgs>(
                    (e) =>
                    {
                        var old = e.OldState;
                        var n = e.NewState;

                        Debug.WriteLine($"{GetType().Name}: AdaptiveState:  {old.Name} -> {n.Name}");

                        UpdateForVisualState(e.NewState, e.OldState);

                        State = n;

                    },
                    (e) => true));
            }
        }

        private VisualState _state;
        public VisualState State
        {
            get { return _state; }
            set
            {
                Set(ref _state, value);

            }
        }

        public void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState.Name == "NarrowState";

            if (isNarrow && oldState != null && oldState.Name == "DefaultState" && SelectedChat != null)
            {
                NarrowStateCommand.Execute(null);
                // Resize down to the detail item. Don't play a transition.
                //Frame.Navigate(typeof(ConversationView), _lastSelectedItem.number, new SuppressNavigationTransitionInfo());
            }


            /*EntranceNavigationTransitionInfo.SetIsTargetElement(masterFrame, isNarrow);
            if (detailFrame != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(detailFrame, !isNarrow);
            }*/
        }



        private RelayCommand _narrowStateCommand;
        public RelayCommand NarrowStateCommand
        {
            get
            {
                return _narrowStateCommand ?? (_narrowStateCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY);

                    },
                    () => true));
            }
        }

        private RelayCommand<Thread> _chatClicked;
        public RelayCommand<Thread> ChatClicked
        {
            get
            {
                return _chatClicked ?? (_chatClicked = new RelayCommand<Thread>(
                    (t) =>
                    {
                        SelectedChat = t;
                    },
                    (t) => true));
            }
        }

        private RelayCommand<TextSecureDirectory.Directory> _contactClicked;
        public RelayCommand<TextSecureDirectory.Directory> ContactClicked
        {
            get
            {
                return _contactClicked ?? (_contactClicked = new RelayCommand<TextSecureDirectory.Directory>(
                    (t) =>
                    {
                        SelectedContact = t;
                    },
                    (t) => true));
            }
        }
    }
}
