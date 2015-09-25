using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.database.loaders;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;

namespace Signal.ViewModel
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

            //NavigateCommand = new RelayCommand<ThreadDatabase.Thread>(ShowThread);
        }

        ObservableCollection<Thread> _Threads;
        public ObservableCollection<Thread> Threads
        {
            get { return _Threads ?? (Threads = new ThreadCollection(_dataService)); }
            set
            {
                _Threads = value;
                RaisePropertyChanged("Threads");
            }
        }

        private RelayCommand<MessageDatabase.MessageTable> _addCommand;
        public RelayCommand<MessageDatabase.MessageTable> AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand<MessageDatabase.MessageTable>(
                   p =>
                {
                    _navigationService.NavigateTo("DirectoryPageKey");
                },
                    p => true));
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
    }
}
