using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using TextSecure;
using TextSecure.database;
using TextSecure.recipient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Signal.Database;
using Signal.Messages;
using Signal.Resources;
using Signal.Util;
using Signal.Views;

namespace Signal.ViewModels
{
    public class MessageDetailViewModel : ViewModelBase, INavigableViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;

        public MessageDetailViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        private MessageRecord _message;
        public MessageRecord Message
        {
            get { return _message; }
            set { Set(ref _message, value); RaisePropertyChanged("Message"); }
        }

        private Recipient _selectedRecipient;
        public Recipient SelectedRecipient
        {
            get { return _selectedRecipient; }
            set { Set(ref _selectedRecipient, value); RaisePropertyChanged("SelectedRecipient"); Log.Debug($"SelectedRecipient"); VerifyIdentityCommand.RaiseCanExecuteChanged(); ResendCommand.RaiseCanExecuteChanged();}
        }

        private IdentityKeyMismatch GetKeyMismatch(Recipient r)
        {
            var keyMismatches = Message.MismatchedIdentities;
            return keyMismatches.FirstOrDefault(identityMismatch => identityMismatch.RecipientId.Equals(r.RecipientId));
        }

        private RelayCommand _verifyIdentityCommand;
        public RelayCommand VerifyIdentityCommand
        {
            get
            {
                return _verifyIdentityCommand ?? (_verifyIdentityCommand = new RelayCommand(
                    () =>
                    {
                        var t = 0;
                    },
                    () =>
                    {
                        var isMismatch = Message.IsIdentityMismatchFailure;
                        if (isMismatch)
                        {
                            var count = Message.MismatchedIdentities.Count();
                            Log.Debug($"");

                        }
                        return Message.IsIdentityMismatchFailure && (Message.MismatchedIdentities.FirstOrDefault(r => r.RecipientId.Equals(SelectedRecipient.RecipientId)) != null);
                    })); 
            }
        }

        private RelayCommand _resendCommand;
        public RelayCommand ResendCommand
        {
            get
            {
                return _resendCommand ?? (_resendCommand = new RelayCommand(
                    () =>
                    {
                        var t = 0;

                    },
                    () => false));
            }
        }

        public void NavigateTo(NavigationEventArgs args)
        {
            var message = (MessageRecord)args.Parameter;

            if (message == null)
            {
                _navigationService.GoBack();
            }

            Message = message;
        }

        public void NavigateFrom(NavigationEventArgs args)
        {
            _navigationService.RemoveBackEntry();
        }
    }
}
