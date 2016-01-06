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
using TextSecure.database;
using TextSecure.recipient;
using TextSecure.util;

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

        ObservableCollection<TextSecureDirectory.Directory> _Contacts;
        public ObservableCollection<TextSecureDirectory.Directory> Contacts
        {
            get { return _Contacts ?? (Contacts = new DirectoryCollection(_dataService)); }
            set
            {
                _Contacts = value;
                RaisePropertyChanged("Contacts");
            }
        }

        /*GroupedDirectoryCollection _GroupedContacts;
        public GroupedDirectoryCollection GroupedContacts
        {
            get { return _GroupedContacts ?? (_GroupedContacts = new GroupedDirectoryCollection(_dataService)); }
            set
            {
                _GroupedContacts = value;
                RaisePropertyChanged("GroupedContacts");
            }
        }*/

        /* public ObservableCollection<IGrouping<long, TextSecureDirectory.Directory>> GroupedContacts
         {
             get {
                 var contacts = _dataService.getDictionary();
                 var grouped = from contact in contacts group contact by contact.Registered;
                 foreach (var group in grouped)
                     Debug.WriteLine($"We have {grouped.Count()} groups, {group.Key} with {group.Count()} members.");
                 return new ObservableCollection<IGrouping<long, TextSecureDirectory.Directory>>(grouped);
             }

         }*/


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

        // broadcast
        /*public const string SelectedRecipientsPropertyName = "SelectedRecipients";
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
        }*/

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

        private RelayCommand _multiSelectCommand;
        public RelayCommand MultiSelectCommand
        {
            get
            {
                return _multiSelectCommand ?? (_multiSelectCommand = new RelayCommand(
                   () => { return; },
                   () => true
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
            DirectoryHelper.refreshDirectory();
        }

        private async void addCommandInternal()
        {

            //Recipients recipients = RecipientFactory.getRecipientsFromString(SelectedContact.Number, true);

            Recipients recipients = DatabaseFactory.getRecipientDatabase().GetOrCreateRecipients(SelectedContact); //RecipientFactory.getRecipientsFromContact(SelectedContact);

            var threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients, 0);

            Messenger.Default.Send(new AddThreadMessage() { ThreadId = threadId });

            _navigationService.NavigateTo(ViewModelLocator.MESSAGES_PAGE_KEY, DatabaseFactory.getThreadDatabase().Get(threadId));

            //_navigationService.NavigateTo("MasterDetail");
            // _navigationService.GoBack();
        }

        public void Activate(object parameter)
        {
            //throw new NotImplementedException();
        }

        public void Deactivate(object parameter)
        {
            _navigationService.RemoveBackEntry();
        }
    }
}
