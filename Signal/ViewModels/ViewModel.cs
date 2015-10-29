using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Windows.UI.Xaml;

namespace Signal.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public MainViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        

        private RelayCommand _narrowStateCommand;
        public RelayCommand NarrowStateCommand
        {
            get
            {
                return _narrowStateCommand ?? (_narrowStateCommand = new RelayCommand(
                    () =>
                   {
                       _navigationService.NavigateTo("MessagePageKey");

                   },
                    () => true));
            }
        }

    }
}
