using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Signal.database;
using Signal.database.loaders;
using Signal.Database;
using Signal.Models;
using Signal.Resources;
using Signal.Util;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using TextSecure.recipient;
using TextSecure.util;
using Windows.UI.Xaml;

namespace Signal.ViewModels
{
    public class MainViewModel : ViewModelBase, IBackAwareViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;

        public MainViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

            Chats = new ThreadCollection(service);
        }

        private int _pivotIndex = 0;
        public int PivotIndex
        {
            get { return _pivotIndex; }
            set
            {
                Set(ref _pivotIndex, value);
            }
        }


        #region Chats . . .
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



        private RelayCommand<Thread> _deleteChatCommand;
        public RelayCommand<Thread> DeleteChatCommand
        {
            get
            {
                return _deleteChatCommand ?? (_deleteChatCommand = new RelayCommand<Thread>(
                  chat =>
                  {
                      Debug.WriteLine($"Should delete {chat.Recipients.ShortString}");

                      DatabaseFactory.getThreadDatabase().DeleteConversation(chat.ThreadId);
                      Chats.Remove(chat);
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

        #endregion

        #region Directory . . .

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

        private RelayCommand<TextSecureDirectory.Directory> _contactClicked;
        public RelayCommand<TextSecureDirectory.Directory> ContactClicked
        {
            get
            {
                return _contactClicked ?? (_contactClicked = new RelayCommand<TextSecureDirectory.Directory>(
                    (t) =>
                    {
                        Log.Debug($"Selected {t.Name}");
                        SelectedContact = t;
                    },
                    (t) => true));
            }
        }
       
        private RelayCommand _refreshContactsCommand;
        public RelayCommand RefreshContactsCommand
        {
            get
            {
                return _refreshContactsCommand ?? (_refreshContactsCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            refreshCommandInternal();
                        }
                        catch (Exception e)
                        {
                            // TODO:
                        }
                    },
                   () => true
                   ));
            }
        }

        private RelayCommand _testCommand;
        public RelayCommand TestCommand
        {
            get
            {
                return _testCommand ?? (_testCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            TileHelper.Update();
                            //ToastHelper.NewMessage();
                        }
                        catch (Exception e)
                        {
                            // TODO:
                        }
                    },
                   () => true
                   ));
            }
        }

        private async Task refreshCommandInternal()
        {
            await DirectoryHelper.refreshDirectory();
            Contacts.Requery();

        }
        #endregion

        private void OpenChat(TextSecureDirectory.Directory contacts)
        {
            Recipients recipients = DatabaseFactory.getRecipientDatabase().GetOrCreateRecipients(SelectedContact); //RecipientFactory.getRecipientsFromContact(SelectedContact);
            var chatid = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients, 0);
            var chat = DatabaseFactory.getThreadDatabase().Get(chatid);
            OpenChat(chat);
        }

        private void OpenChat(Thread chat)
        {
            /*if (CurrentVisualState != null && CurrentVisualState.Name == "NarrowState") // navigate to detail page
            {
                Debug.WriteLine($"NarrowState -> Thread #{chat.ThreadId}");*/
                _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY, chat);
            /*}
            else if (CurrentVisualState != null && CurrentVisualState.Name == "DefaultState")
            {
                Debug.WriteLine($"WideState -> Thread #{chat.ThreadId}");
                DetailFrame.Navigate(typeof(ThreadPage), chat);
            }*/
        }


        /*
         * IBackAwareViewModel
         */
        public void BackRequested(BackRequestedEventArgs args)
        {
            Log.Debug($"Back from Main");
            if (PivotIndex != 0) { PivotIndex -= 1; args.Handled = true; }
            else args.Handled = false;
        }

        #region Visual State . . .
        private VisualStateGroup _vsg;
        public VisualStateGroup AdaptiveStates
        {
            get
            {
                return _vsg;
            }
            set
            {
                CurrentVisualState = value.CurrentState;
                Set(ref _vsg, value);
            }
        }


        private RelayCommand<VisualStateChangedEventArgs> _VisualStateChanged;
        public RelayCommand<VisualStateChangedEventArgs> VisualStateChanged
        {
            get
            {
                return _VisualStateChanged ?? (_VisualStateChanged = new RelayCommand<VisualStateChangedEventArgs>(
                    (e) =>
                    {
                        var old = e.OldState;
                        var n = e.NewState;

                        Debug.WriteLine($"{GetType().Name}: AdaptiveState:  {old.Name} -> {n.Name}");

                        UpdateForVisualState(e.NewState, e.OldState);

                        CurrentVisualState = n;

                    },
                    (e) => true));
            }
        }

        private VisualState _currentVisualState;
        public VisualState CurrentVisualState
        {
            get { return _currentVisualState; }
            set
            {
                Set(ref _currentVisualState, value);

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
        #endregion
    }
}
