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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.ViewModel
{
    public class DirectoryViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public DirectoryViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        ObservableCollection<Contact> _Contacts;
        public ObservableCollection<Contact> Contacts
        {
            get { return _Contacts ?? (Contacts = new DirectoryCollection(_dataService)); }
            set
            {
                _Contacts = value;
                RaisePropertyChanged("Contacts");
            }
        }


        Contact _selectedContact;
        public Contact SelectedContact
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

        // broadcast
        public const string SelectedRecipientsPropertyName = "SelectedRecipients";
        private Recipients _selectedRecipients = null;
        public Recipients SelectedRecipients
        {
            get { return _selectedRecipients; }
            set
            {
                if (_selectedRecipients == value)
                {
                    return;
                }

                var oldValue = _selectedRecipients;
                _selectedRecipients = value;
                RaisePropertyChanged(SelectedRecipientsPropertyName, oldValue, _selectedRecipients, true);
            }
        }

        // commands
        private RelayCommand<Contact> _addCommand;
        public RelayCommand<Contact> AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand<Contact>(
                   p => { addCommandInternal(); },
                   p => SelectedContact != null
                   ));
            }
        }

        private void addCommandInternal()
        {

            Recipients recipients = RecipientFactory.getRecipientsFromString(SelectedContact.number, true);

            SelectedRecipients = recipients;

            //_navigationService.NavigateTo("MasterDetail");
            _navigationService.GoBack();
        }
    }
}
