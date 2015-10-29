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
using TextSecure.recipient;

namespace Signal.ViewModels
{
    public class DirectoryViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;

        public DirectoryViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

            var test = Contacts;

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

        GroupedDirectoryCollection _GroupedContacts;
        public GroupedDirectoryCollection GroupedContacts
        {
            get { return _GroupedContacts ?? (_GroupedContacts = new GroupedDirectoryCollection(_dataService)); }
            set
            {
                _GroupedContacts = value;
                RaisePropertyChanged("GroupedContacts");
            }
        }

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

        private async void addCommandInternal()
        {

            Recipients recipients = RecipientFactory.getRecipientsFromString(SelectedContact.Number, true);

            var threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients, 0);

            Messenger.Default.Send(new AddThreadMessage() { ThreadId = threadId });

            //_navigationService.NavigateTo("MasterDetail");
            _navigationService.GoBack();
        }
    }
}
