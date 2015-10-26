using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Signal.database;
using Signal.Models;
using Signal.Resources;
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
            settings.Add(new SettingsItem() { Icon = "Highlight", Name = "System", Description = "Display, notifications, apps, power"});
            settings.Add(new SettingsItem() { Icon = "Highlight", Name = "Devices", Description = "Bluetooth, printers, mouse" });

            Settings = settings;

        }

        private bool? _darkTheme;
        public bool? DarkTheme
        {
            get
            {
                Debug.WriteLine($"DarkTheme get");
                return _darkTheme;
            }
            set
            {
                Debug.WriteLine($"DarkTheme is now {value}");
                Set(ref _darkTheme, value);
                
                if (value == true)
                {
                    /*var frame = App.Frame as ThemeAwareFrame;
                        frame.AppTheme = ElementTheme.Dark;*/
                    (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                }
            }
        }

        private bool? _lightTheme;
        public bool? LightTheme
        {
            get
            {
                Debug.WriteLine($"LightTheme get");
                return _lightTheme;
            }
            set
            {
                Set(ref _lightTheme, value);
                Debug.WriteLine($"LightTheme is now {value}");

                if (value == true)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        /*var frame = App.Frame as ThemeAwareFrame;
                        frame.AppTheme = ElementTheme.Light;
                        */
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
                    });
                }
            }
        }

        public List<KeyValuePair<string, string>> TestItems = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string,string>("TestASD", "asdvalue"),
             new KeyValuePair<string,string>("TestASD2", "asdvalue2"),
             new KeyValuePair<string,string>("TestASD3", "asdvalue3"),
        };

        private string _testResult = "prevalue";
        public string TestResult
        {
            get { return _testResult; }
            set
            {
                Debug.WriteLine($"SettingsViewModel set TestResult to {value}");
                Set(ref _testResult, value);
                RaisePropertyChanged("TestResult");
            }
        }

        private string _selectedItem = "prevalue";
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Debug.WriteLine($"SettingsViewModel set TestResult to {value}");
                TestResult = value;
                Set(ref _testResult, value);
                RaisePropertyChanged("TestResult");
            }
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
