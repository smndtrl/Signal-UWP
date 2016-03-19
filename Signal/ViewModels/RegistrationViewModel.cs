using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Signal.database;
using System.Diagnostics;
using Signal.Resources;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using libaxolotl;
using libaxolotl.state;
using libaxolotl.util;
using libtextsecure.push.exceptions;
using libtextsecure.util;
using Signal.Database;
using Signal.Models;
using Signal.Push;
using Signal.Util;
using TextSecure.crypto;
using TextSecure.util;

namespace Signal.ViewModels
{
    /*public class Country
    {
        public string code { get; set; }
        public string phoneCode { get; set; }
        public string name { get; set; }
    }*/

    public class Country : ObservableObject
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public Country(string Name, string Code)
        {
            this.DisplayName = Name;
            this.Code = Code;
            this.Name = Name.ToLower();
         
        }
    }

    public class RegistrationViewModel : ViewModelBase, INavigableViewModel, IBackAwareViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;


        public RegistrationViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;
        }


        #region Properties
        /*
         * Register Pivot
         */
        private string _countryCode = string.Empty;
        public string CountryCode
        {
            get { return _countryCode; }
            set { Set(ref _countryCode, value); RegisterCommand.RaiseCanExecuteChanged(); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { Set(ref _phoneNumber, value); RegisterCommand.RaiseCanExecuteChanged(); }
        }

        private bool _isRegistering = false;
        public bool IsRegistering
        {
            get { return _isRegistering; }
            set { Set(ref _isRegistering, value); RaisePropertyChanged(); RegisterCommand.RaiseCanExecuteChanged(); }
        }

        /*
         * Verification Pivot
         */
        private string _verificationToken = string.Empty;
        public string VerificationToken
        {
            get { return _verificationToken; }
            set { Set(ref _verificationToken, value); VerifyCommand.RaiseCanExecuteChanged(); }
        }

        private bool _isVerifying = false;
        public bool IsVerifying
        {
            get { return _isVerifying; }
            set { Set(ref _isVerifying, value); RaisePropertyChanged(); VerifyCommand.RaiseCanExecuteChanged(); }

        }
        /*
         * Link Pivot
         */
        private string _qrCode = "asdf";
        public string QrCode
        {
            get { return _qrCode; }
            set { Set(ref _qrCode, value); RaisePropertyChanged(); }
        }

        #endregion


        private ICollection<Country> _unfilteredCountries = new CountryCollection();

        public ICollection<Country> _filteredCountries;

        public ICollection<Country> FilteredCountries
        {
            get { return _filteredCountries ?? _unfilteredCountries; }
            set { Set(ref _filteredCountries, value); }
        }

        private string _countryText = string.Empty;
        public string CountryText
        {
            get { return _countryText; }
            set {

                Set(ref _countryText, value);
                Debug.WriteLine($"Searching for {value}");
                string containsThis = value.ToLower();
                var fr = from fobjs in _unfilteredCountries
                         where fobjs.Name.Contains(containsThis)
                         select fobjs;

                //if (FilteredCountries.Count() == fr.Count()) return;

                FilteredCountries = new ObservableCollection<Country>(fr);
                Debug.WriteLine($"Found {fr.Count()}");

                RaisePropertyChanged("FilteredCountries");
            }
        }




        private string getNumber()
        {
            try
            {
                return PhoneNumberFormatter.formatE164(CountryCode, PhoneNumber.TrimStart('0'));
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        


        

        private RelayCommand _verifyCommand;
        public RelayCommand VerifyCommand
        {
            get
            {
                return _verifyCommand ?? (_verifyCommand = new RelayCommand(async
                   () => {
                       IsVerifying = true;
                       VerifyCommand.RaiseCanExecuteChanged();

                       var success = await handleRegistration(VerificationToken);
                       if (!success)
                       {
                           IsVerifying = false;
                           VerifyCommand.RaiseCanExecuteChanged();

                           return; // TODO:
                       }

                       _navigationService.NavigateTo(ViewModelLocator.MAIN_PAGE_KEY);
                   },
                   () => true || !IsVerifying && VerificationToken.Length == 6));
            }
        }

        private bool _isPushNetworkException = false;
        public bool IsPushNetworkException
        {
            get { return _isPushNetworkException; }
            set { Set(ref _isPushNetworkException, value); RaisePropertyChanged(); }
        }

        private RelayCommand _registerCommand;
        public RelayCommand RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand = new RelayCommand(async
                   () =>
                {
                    IsRegistering = true;
                    RegisterCommand.RaiseCanExecuteChanged();

                    FlipIndex += 1;

                    /*number = getNumber();
                    Debug.WriteLine($"Register: {number}");

                    password = Utils.getSecret(18);
                    signalingKey = Utils.getSecret(52);

                    try
                    {
                        App.Current.accountManager = TextSecureCommunicationFactory.createManager(number, password);
                        await App.Current.accountManager.requestSmsVerificationCode();


                        FlipIndex += 1;
                    }
                    catch (PushNetworkException pne)
                    {
                        
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }*/

                    IsRegistering = false;
                    RegisterCommand.RaiseCanExecuteChanged();
                },
                    () => true || !IsRegistering && (CountryCode != string.Empty) && PhoneNumberFormatter.isValidNumber(getNumber())));
            }
        }


        private int _pivotIndex = 0;
        public int FlipIndex
        {
            get { return _pivotIndex; }
            set
            {
                Set(ref _pivotIndex, value);
            }
        }

        private RelayCommand _getStartedCommand;
        public RelayCommand GetStartedCommand
        {
            get
            {
                return _getStartedCommand ?? (
                    _getStartedCommand = new RelayCommand(
                        () => { FlipIndex += 1; },
                        () => true)
                    );
            }
        }

        private RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs> _countrySelectedCommand;
        public RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs> CountrySelectedCommand
        {
            get
            {
                return _countrySelectedCommand ?? (
                    _countrySelectedCommand = new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(
                        (t) =>
                        {
                            Debug.WriteLine("selected");
                            var country = t.SelectedItem as Country;

                            if (country != null)
                            {
                                CountryCode = "+" + country.Code;
                            }


                        },
                        (t) => { return true; }
                        )
                    );
            }
        }

        /*
         * ReegistrationTypeView
         */

        /*private RelayCommand _navigateRegisterCommand;
        public RelayCommand NavigateRegisterCommand
        {
            get
            {
                return _navigateRegisterCommand ?? (_navigateRegisterCommand = new RelayCommand(
                    () => { _navigationService.NavigateTo(ViewModelLocator.REGISTERING_PAGE_KEY); },
                    () => { return true; }
                    ));
            }
        }*/

        private RelayCommand _navigateLinkCommand;
        public RelayCommand NavigateLinkCommand
        {
            get
            {
                return _navigateLinkCommand ?? (_navigateLinkCommand = new RelayCommand(
                    () => { _navigationService.NavigateTo(ViewModelLocator.PROVISIONING_PAGE_KEY); },
                    () => false
                    ));
            }
        }

        /*
         * ProvisioningView
         */


        private string password;
        private string signalingKey;
        private string number;

        private async Task<bool> handleRegistration(string receivedSmsVerificationCode)
        {

            try
            {


                var registrationId = KeyHelper.generateRegistrationId(false);
                TextSecurePreferences.setLocalRegistrationId((int)registrationId);

                //await App.Current.accountManager.verifyAccountWithCode(receivedSmsVerificationCode, signalingKey, registrationId, false);

                Recipient self = DatabaseFactory.getRecipientDatabase().GetSelfRecipient(number);
                IdentityKeyUtil.generateIdentityKeys();
                IdentityKeyPair identityKey = IdentityKeyUtil.GetIdentityKeyPair();
                List<PreKeyRecord> records = await PreKeyUtil.generatePreKeys();
                PreKeyRecord lastResort = await PreKeyUtil.generateLastResortKey();
                SignedPreKeyRecord signedPreKey = PreKeyUtil.generateSignedPreKey(identityKey);

                //await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResort, signedPreKey, records);

                DatabaseFactory.getIdentityDatabase().SaveIdentity(self.getRecipientId(), identityKey.getPublicKey());


                markAsVerified(number, password, signalingKey);



            }
            catch (RateLimitException ex)
            {
                return false;
            }
            catch (AuthorizationFailedException ex)
            {
                return false;
            }
            catch (PushNetworkException ex)
            {
                return false;
            }
            catch (NonSuccessfulResponseCodeException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
                //throw new Exception(ex.Message);
            }

            return true;

        }

        private void markAsVerified(String number, String password, String signalingKey)
        {
            TextSecurePreferences.setVerifying(false);
            TextSecurePreferences.setPushRegistered(true);
            TextSecurePreferences.setLocalNumber(number);
            TextSecurePreferences.setPushServerPassword(password);
            TextSecurePreferences.setSignalingKey(signalingKey);
            TextSecurePreferences.setSignedPreKeyRegistered(true);
            //TextSecurePreferences.setPromptedPushRegistration(true);
        }


        /*
         * INavigableViewModel
         */

        public void NavigateTo(NavigationEventArgs parameter)
        {
            var prefs = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            var prefs2 = Windows.System.UserProfile.GlobalizationPreferences.HomeGeographicRegion;


            var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
        }

        public void NavigateFrom(NavigationEventArgs parameter)
        {
            _navigationService.RemoveBackEntry();
        }

        /*
         * IBackAwareViewModel
         */
        public void BackRequested(BackRequestedEventArgs args)
        {
            Log.Debug("Reg BackRequested");
            if (FlipIndex >= 1) { FlipIndex -= 1; args.Handled = true; }
            else args.Handled = false;


        }
    }
}
