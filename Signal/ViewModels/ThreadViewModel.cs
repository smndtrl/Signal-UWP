using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
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
using TextSecure.database;
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

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                Debug.WriteLine("Setting Thread");
                if (_selectedThread == value)
                {
                    return;
                }

                var oldValue = _selectedThread;
                _selectedThread = value;
                //RaisePropertyChanged(SelectedThreadPropertyName, oldValue, _selectedThread, true);

                //Uri eventDetailPageUri = new Uri(string.Format("{0}?key={1}", "MessagePageKey", _selectedThread.ThreadId), UriKind.Relative);
                _navigationService.NavigateTo("MessagePageKey", _selectedThread);
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



        /* private void ShowThread(ThreadDatabase.Thread thread)
         {
             if (!SimpleIoc.Default.ContainsCreated<MessageViewModel>("Thread-" + thread._id))
             {
                 SimpleIoc.Default.Register(() => new MessageViewModel(_dataService, _navigationService) { ThreadId = thread._id.Value }, "Thread-" + thread._id, true);
             }

             _navigationService.NavigateTo("Thread-" + thread._id, "test");              
         } 

         public RelayCommand<ThreadDatabase.Thread> NavigateCommand { get; private set; }*/

        public static bool CanGoBack = false;
    }
}
