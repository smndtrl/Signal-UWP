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

namespace Signal.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDataService _dataService;



        public RegistrationViewModel(IDataService service, INavigationService navService)
        {
            _dataService = service;
            _navigationService = navService;

        }

        public const string SelectedThreadPropertyName = "SelectedThread";
        private Thread _selectedThread = null;
        public Thread SelectedThread
        {
            get { return _selectedThread; }
            set
            {
                Debug.WriteLine("Setting Thread");
                if (_selectedThread == value)
                {
                    return;
                }

                var oldValue = _selectedThread;
                _selectedThread = value;
                RaisePropertyChanged(SelectedThreadPropertyName, oldValue, _selectedThread, true);
            }
        }

        public const string IsBusyPropertyName = "IsBusy";
        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                {
                    return;
                }

                var oldValue = _isBusy;
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName, oldValue, _isBusy, true);
            }
        }

        public string CountryCode = string.Empty;
        public string PhoneNumber = string.Empty;
        private string getNumber()
        {   try
            {
                return PhoneNumberFormatter.formatE164(CountryCode, PhoneNumber);
            } catch (Exception e)
            {
                return string.Empty;
            }
        }
        public string VerificationToken = "";

        public enum RegistrationState { None, Registering, Registered, Verifying, Verified,  Generated, Sat };

        public const string StatePropertyName = "State";
        private int _state = (int)RegistrationState.None;
        public int State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                {
                    return;
                }

                var oldValue = _state;
                _state = value;
                Debug.WriteLine($"State: {_state}");
                RaisePropertyChanged(StatePropertyName, oldValue, _state, true);
            }
        }


        private RelayCommand _verifyCommand;
        public RelayCommand VerifyCommand
        {
            get
            {
                return _verifyCommand ?? (_verifyCommand = new RelayCommand(async
                   () =>
                {

                    FlipIndex += 1; return; // TODO:

                    IsBusy = true;

                    var success = await handleRegistration(VerificationToken);

                    if (!success)
                    {
                        State = (int)RegistrationState.Registered;
                    }

                    State = (int)RegistrationState.Verified;

                    IsBusy = false;
                },
                    () => true));
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

                    FlipIndex += 1; return; // TODO:
                    
                    number = $"+{CountryCode}{PhoneNumber}";
                    Debug.WriteLine($"Register: {number}");



                    password = Utils.getSecret(18);
                    signalingKey = Utils.getSecret(52);

                    State = (int)RegistrationState.Registering;
                    IsBusy = true;
                    try {
                        await Task.Delay(2000);

                        App.Current.accountManager = TextSecureCommunicationFactory.createManager(number, password);
                        App.Current.accountManager.requestSmsVerificationCode();
                        State = (int)RegistrationState.Registered;

                        _navigationService.NavigateTo("VerificationPageKey");

                    } catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        State = (int)RegistrationState.None;
                    }

                    IsBusy = false;

                },
                    () => {
                        PhoneNumberFormatter.isValidNumber(getNumber());
                        return true;
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

        private bool _flipEnabled = true;
        public bool FlipEnabled
        {
            get { return _flipEnabled; }
            set
            {
                Set(ref _flipEnabled, value);
            }
        }

        private RelayCommand _getStartedCommand;
        public RelayCommand GetStartedCommand
        {
            get
            {
                return _getStartedCommand ?? new RelayCommand(
                    () => { Debug.WriteLine($"getstarted");  FlipIndex += 1; },
                    () => { return true; }
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
                State = (int)RegistrationState.Verified;

                Recipient self = DatabaseFactory.getRecipientDatabase().GetSelfRecipient(number);
                IdentityKeyUtil.generateIdentityKeys();
                IdentityKeyPair identityKey = IdentityKeyUtil.GetIdentityKeyPair();
                List<PreKeyRecord> records = await PreKeyUtil.generatePreKeys();
                PreKeyRecord lastResort = await PreKeyUtil.generateLastResortKey();
                SignedPreKeyRecord signedPreKey = await PreKeyUtil.generateSignedPreKey(identityKey);

                await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResort, signedPreKey, records);

                DatabaseFactory.getIdentityDatabase().SaveIdentity(self.getRecipientId(), identityKey.getPublicKey());
                State = (int)RegistrationState.Generated;

                //await DirectoryHelper.refreshDirectory(App.Current.accountManager, TextSecurePreferences.getLocalNumber());


                markAsVerified(number, password, signalingKey);

                _navigationService.NavigateTo("MasterDetail");

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

        /*private string generateRandomSignalingKey()
        {
            IBuffer randBuffer = CryptographicBuffer.GenerateRandom(52);
            byte[] value;
            CryptographicBuffer.CopyToByteArray(randBuffer, out value);

            string signalingKey = Base64.encodeBytes(value);
            ApplicationData.Current.LocalSettings.Values["prefs_install_signalingKey"] = signalingKey;
            return signalingKey.Replace("=", "");
        }*/
    }
}
