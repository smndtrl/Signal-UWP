using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.database.loaders;
using Signal.Models;
using Signal.ViewModel.Messages;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.ViewModels
{
    public class ThreadViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public ThreadViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;
            Threads = new ThreadCollection(service);

            Messenger.Default.Register<RefreshThreadMessage>(
                this,
                async message =>  
                {

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Threads.Refresh();
                        Debug.WriteLine("Thread Refresh");

                    });
                    
                   
                }
            );
        }

        public Frame DetailFrame;

        public const string SelectedThreadPropertyName = "SelectedThread";
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
            set
            {
                _Threads = value;
                RaisePropertyChanged("Threads");
            }
        }

        private RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(
                  () =>
                {
                    _navigationService.NavigateTo("DirectoryPageKey");
                   
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
                      Debug.WriteLine($"Show flyout");

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
                      Debug.WriteLine($"Loaded");

                  },
                   () => true));
            }
        }



        /* private void ShowThread(ThreadDatabase.Thread thread)
         {
             if (!SimpleIoc.Default.ContainsCreated<MessageViewModel>("Thread-" + thread._id))
             {
                 SimpleIoc.Default.Register(() => new MessageViewModel(_dataService, _navigationService) { ThreadId = thread._id.Value }, "Thread-" + thread._id, true);
             }

             _navigationService.NavigateTo("Thread-" + thread._id, "test");              
         } 

         public RelayCommand<ThreadDatabase.Thread> NavigateCommand { get; private set; }*/

        /*
         * VISUAL STATE
         */

        private RelayCommand<VisualStateChangedEventArgs> _stateChanged;
        public RelayCommand<VisualStateChangedEventArgs> StateChanged
        {
            get
            {
                return _stateChanged ?? (_stateChanged = new RelayCommand<VisualStateChangedEventArgs>(
                    (t) =>
                    {
                        State = t.NewState;

                    },
                    (t) => true));
            }
        }

        private VisualState _state;
        public VisualState State
        {
            get { return _state; }
            set
            {
                if (_state != null)
                {
                    Debug.WriteLine($"AdaptiveState:  {_state.Name} -> {value.Name}");
                } else
                {
                    Debug.WriteLine($"AdaptiveState: default -> {value.Name}");
                }

                UpdateForVisualState(value, _state);
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
                        Debug.WriteLine("Thread clicked");
                        SelectedThread = t;

                    },
                    (t) => true));
            }
        }
    }
}
