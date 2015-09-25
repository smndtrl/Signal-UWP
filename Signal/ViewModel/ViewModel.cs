using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;

namespace Signal.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public ViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread;  }
            set {
                Debug.WriteLine("Setting Thread");
                if (_selectedThread == value)
                {
                    return;
                }

                var oldValue = _selectedThread;
                _selectedThread = value;
                RaisePropertyChanged(SelectedThreadPropertyName, oldValue, _selectedThread, true);
            }
        }
    }
}
