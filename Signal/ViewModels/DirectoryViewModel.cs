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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using TextSecure.database;
using TextSecure.recipient;
using TextSecure.util;
using Windows.UI.Xaml.Navigation;
using Signal.Util;

namespace Signal.ViewModels
{
    public class DirectoryViewModel : ViewModelBase, INavigableViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;

        public DirectoryViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

            var test = Contacts;

        }

        private bool _isBackEnabled = true;
        public bool IsBackEnabled
        {
            get
            {
                return _isBackEnabled;
            }
            set
            {
                _isBackEnabled = value;
                RaisePropertyChanged();
            }
        }

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

        TextSecureDirectory.Directory _selectedContact;
        public TextSecureDirectory.Directory SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                Debug.WriteLine("SelectedContact");
                AddCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedContact");
            }
        }

        // commands
        private RelayCommand<TextSecureDirectory.Directory> _addCommand;
        public RelayCommand<TextSecureDirectory.Directory> AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand<TextSecureDirectory.Directory>( 
                   p => { addCommandInternal(); },
                   p => SelectedContact != null
                   ));
            }
        }

        private ListViewSelectionMode _selectionMode = ListViewSelectionMode.Single;

        public ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set { Set(ref _selectionMode, value); RaisePropertyChanged("SelectionMode"); }
        }

        private bool _multiSelect =  false;
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
                        if(MultiSelect.HasValue && MultiSelect.Value)
                        {
                            MultiSelect = false;
                        } else
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

        public void NavigateTo(NavigationEventArgs parameter)
        {
        }

        public void NavigateFrom(NavigationEventArgs parameter)
        {
            _navigationService.RemoveBackEntry();
        }
    }
}
