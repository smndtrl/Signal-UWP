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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.ViewModels
{
    public class ThreadViewModel : ViewModelBase
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
        private ListViewSelectionMode _selectionMode = ListViewSelectionMode.Single;

        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set { Set(ref _selectionMode, value); RaisePropertyChanged("SelectionMode"); }
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

        private bool _multiSelect = false;
        public bool? MultiSelect
        {
            get { return _multiSelect; }
            set
            {
                if (!value.HasValue) return;

                Set(ref _multiSelect, value.Value);
                if (value.Value)
                {
                    Log.Debug("Setting multi");
                    SelectionMode = ListViewSelectionMode.Multiple;
                }
                else
                {
                    SelectionMode = ListViewSelectionMode.Single;
                }
            }
        }

        private RelayCommand<object> _multiSelectCommand;
        public RelayCommand<object> MultiSelectCommand
        {
            get
            {
                return _multiSelectCommand ?? (_multiSelectCommand = new RelayCommand<object>(
                    (t) =>
                    {
                        if (MultiSelect.HasValue && MultiSelect.Value)
                        {
                            MultiSelect = false;
                        }
                        else
                        {
                            MultiSelect = true;
                        }

                    },
                   (t) => true
                   ));
            }
        }

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new RelayCommand(
                   () => { refreshCommandInternal(); },
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

        public ThreadViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

            Threads = new ThreadCollection(service);
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

        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set
            {

                if (State != null && State.Name == "NarrowState") // navigate to detail page
                {
                    Debug.WriteLine($"NarrowState -> Thread #{value.ThreadId}");
                    _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY, value);
                } else if (State != null && State.Name == "DefaultState")
                {
                    Debug.WriteLine($"WideState -> Thread #{value.ThreadId}");
                    DetailFrame.Navigate(typeof(MessageDetailPage), value);
                }


                Set(ref _selectedThread, value);
            }
        }

        ThreadCollection _Threads;
        public ThreadCollection Threads
        {
            get { return _Threads ?? (Threads = new ThreadCollection(_dataService)); }
            set { Set(ref _Threads, value); }
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(
                  () =>
                {
                    _navigationService.NavigateTo(ViewModelLocator.DIRECTORY_PAGE_KEY);
                   
                },
                    () => true));
            }
        }

        private RelayCommand _addCommand2;
        public RelayCommand CommandTest
        {
            get
            {
                return _addCommand2 ?? (_addCommand2 = new RelayCommand(
                  () =>
                  {
                      Debug.WriteLine("CommandTest executed");

                  },
                    () => true));
            }
        }


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
                      Threads.Remove(thread);
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

            if (isNarrow && oldState != null && oldState.Name == "DefaultState" && SelectedThread != null)
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

        private RelayCommand<Thread> _threadClicked;
        public RelayCommand<Thread> ThreadClicked
        {
            get
            {
                return _threadClicked ?? (_threadClicked = new RelayCommand<Thread>(
                    (t) =>
                    {
                        SelectedThread = t;
                    },
                    (t) => true));
            }
        }
    }
}
