using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using libaxolotl;
using libaxolotl.state;
using libaxolotl.util;
using libtextsecure.push.exceptions;
using libtextsecure.util;
using Signal.database;
using Signal.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.crypto;
using TextSecure.database;
using Signal.Push;
using TextSecure.recipient;
using Signal.Util;
using TextSecure.util;
using System.Windows.Input;
using Signal.Resources;
using Windows.UI.Xaml;
using System.Globalization;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Reflection;
using Signal.Database;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

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

    public class RegistrationViewModel : ViewModelBase, INavigableViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;


        public RegistrationViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;

            /*XmlSerializer ser = new XmlSerializer(typeof(Country));

            string path = Path.Combine(Path.GetDirectoryName(Assembly.Load.GetExecutingAssembly().Location), );
            var reader = new StreamReader(Application.Start);
            var test = ser.Deserializeread()*/


        }

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

        private string getNumber()
        {
            try
            {
                return PhoneNumberFormatter.formatE164(CountryCode, PhoneNumber);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public string VerificationToken = "";


        private bool _isVerifying = false;
        public bool IsVerifying
        {
            get { return _isVerifying; }
            set { Set(ref _isVerifying, value); RaisePropertyChanged("IsVerifying"); Debug.WriteLine($"Veriying {_isVerifying}"); }

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

                       _navigationService.NavigateTo(ViewModelLocator.THREADS_PAGE_KEY);
                   },
                   () => !_isVerifying));
            }
        }

        private bool _isRegistering = false;
        public bool IsRegistering
        {
            get { return _isRegistering; }
            set { Set(ref _isRegistering, value); RaisePropertyChanged("IsRegistering"); Debug.WriteLine($"Registering {_isRegistering}"); }

        }

        private RelayCommand _testCommand;
        public RelayCommand TestCommand
        {
            get
            {
                return _testCommand ?? (_testCommand = new RelayCommand(
                    () =>
                    {
                        CountryCode = "+49";
                            Debug.WriteLine("TestCommand");
                    },
                    () => {
                        return true;
                    }
                    ));
            }
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

                    number = getNumber();
                    Debug.WriteLine($"Register: {number}");



                    password = Utils.getSecret(18);
                    signalingKey = Utils.getSecret(52);

                    try
                    {
                        App.Current.accountManager = TextSecureCommunicationFactory.createManager(number, password);
                        await App.Current.accountManager.requestSmsVerificationCode();


                        FlipIndex += 1;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }

                    IsRegistering = false;
                    RegisterCommand.RaiseCanExecuteChanged();

                },
                    () =>
                    {
                        return !IsRegistering && (CountryCode != string.Empty) && PhoneNumberFormatter.isValidNumber(getNumber());
                    }));
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
                        () => { Debug.WriteLine($"getstarted"); FlipIndex += 1; },
                        () => { return true; }
                        )
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

        private RelayCommand _navigateRegisterCommand;
        public RelayCommand NavigateRegisterCommand
        {
            get
            {
                return _navigateRegisterCommand ?? new RelayCommand(
                    () => { _navigationService.NavigateTo(ViewModelLocator.REGISTERING_PAGE_KEY); },
                    () => { return true; }
                    );
            }
        }

        public RelayCommand _navigateProvisionCommand { get; private set; }
        public RelayCommand NavigateProvisionCommand
        {
            get
            {
                return _navigateProvisionCommand ?? new RelayCommand(
                    () => { _navigationService.NavigateTo(ViewModelLocator.PROVISIONING_PAGE_KEY); },
                    () => { return true; }
                    );
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

                await App.Current.accountManager.verifyAccountWithCode(receivedSmsVerificationCode, signalingKey, registrationId, false);
                //await PushHelper.getInstance().OpenChannelAndUpload(); // also updates push channel id

                Recipient self = DatabaseFactory.getRecipientDatabase().GetSelfRecipient(number);
                IdentityKeyUtil.generateIdentityKeys();
                IdentityKeyPair identityKey = IdentityKeyUtil.GetIdentityKeyPair();
                List<PreKeyRecord> records = await PreKeyUtil.generatePreKeys();
                PreKeyRecord lastResort = await PreKeyUtil.generateLastResortKey();
                SignedPreKeyRecord signedPreKey = await PreKeyUtil.generateSignedPreKey(identityKey);

                await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResort, signedPreKey, records);

                DatabaseFactory.getIdentityDatabase().SaveIdentity(self.getRecipientId(), identityKey.getPublicKey());

                //await DirectoryHelper.refreshDirectory(App.Current.accountManager, TextSecurePreferences.getLocalNumber());


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
            TextSecurePreferences.setPushRegistered(false);
            TextSecurePreferences.setLocalNumber(number);
            TextSecurePreferences.setPushServerPassword(password);
            TextSecurePreferences.setSignalingKey(signalingKey);
            TextSecurePreferences.setSignedPreKeyRegistered(true);
            //TextSecurePreferences.setPromptedPushRegistration(true);
        }


        /*
         * INavigableViewModel
         */

        public void Activate(object parameter)
        {
            var prefs = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            var prefs2 = Windows.System.UserProfile.GlobalizationPreferences.HomeGeographicRegion;


            var ci = new CultureInfo(Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
        }

        public void Deactivate(object parameter)
        {
            _navigationService.RemoveBackEntry();
        }
    }
}
