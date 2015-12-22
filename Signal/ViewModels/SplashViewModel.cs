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
using TextSecure.util;
using Windows.UI.Xaml;

namespace Signal.ViewModels
{
    public class SplashViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public SplashViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        private RelayCommand _loaded;
        public RelayCommand Loaded
        {
            get
            {
                return _loaded ?? (_loaded = new RelayCommand(
                    async () => 
                   {
                       await Task.Delay(200);

                       if (TextSecurePreferences.getLocalNumber() == string.Empty)
                       {
                           Debug.WriteLine("First start, registering");
                           _navigationService.NavigateTo(ViewModelLocator.REGISTERING_PAGE_KEY);
                           //rootFrame.Navigate(typeof(RegistrationTypeView));
                           //Window.Current.Content = rootFrame;
                           return;
                       }

                       _navigationService.NavigateTo(ViewModelLocator.THREADS_PAGE_KEY);


                   },
                    () => true));
            }
        }

    }
}
