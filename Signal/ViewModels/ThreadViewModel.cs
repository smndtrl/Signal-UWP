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
using Signal.ViewModel.Messages;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.ViewModels
{
    public class ThreadViewModel : ViewModelBase
    {
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
                    DetailFrame.Navigate(typeof(ThreadDetailPage), value);
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
