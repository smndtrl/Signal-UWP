using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Windows.UI.Xaml;

namespace Signal.ViewModels
{
    public class SettingsItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SettingsViewModel : ViewModelBase
    {


        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public SettingsViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

            var settings = new ObservableCollection<SettingsItem>();
            settings.Add(new SettingsItem() { Icon = "Highlight", Name = "Appearance", Description = "Themes and stuff"});

            Settings = settings;

        }


        ObservableCollection<SettingsItem> _Settings;
        public ObservableCollection<SettingsItem> Settings
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
                RaisePropertyChanged("Messages");
            }
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
