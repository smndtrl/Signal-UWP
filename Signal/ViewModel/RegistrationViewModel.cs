using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using libaxolotl;
using libaxolotl.state;
using libaxolotl.util;
using libtextsecure.push.exceptions;
using libtextsecure.util;
using Signal.database;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.crypto;
using TextSecure.database;
using TextSecure.push;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.ViewModel
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

        public string CountryCode = "";
        public string PhoneNumber = "";
        private string getNumber()
        {   try
            {
                return PhoneNumberFormatter.formatE164(CountryCode, PhoneNumber);
            } catch (Exception e)
            {
                return "";
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


        private RelayCommand<MessageDatabase.MessageTable> _verifyCommand;
        public RelayCommand<MessageDatabase.MessageTable> VerifyCommand
        {
            get
            {
                return _verifyCommand ?? (_verifyCommand = new RelayCommand<MessageDatabase.MessageTable>(async
                   p =>
                {
                    IsBusy = true;

                    var success = await handleRegistration(VerificationToken);

                    if (!success)
                    {
                        State = (int)RegistrationState.Registered;
                    }

                    State = (int)RegistrationState.Verified;

                    IsBusy = false;
                },
                    p => true));
            }
        }

        private RelayCommand<MessageDatabase.MessageTable> _registerCommand;
        public RelayCommand<MessageDatabase.MessageTable> RegisterCommand
        {
            get
            {
                return _registerCommand ?? (_registerCommand = new RelayCommand<MessageDatabase.MessageTable>(async
                   p =>
                {
                    
                    number = $"+{CountryCode}{PhoneNumber}";
                    Debug.WriteLine($"Register: {number}");



                    password = Util.getSecret(18);
                    signalingKey = Util.getSecret(52);

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
                    p => {
                        PhoneNumberFormatter.isValidNumber(getNumber());
                        return true;
                        }));
            }
        }

        private string password;
        private string signalingKey;
        private string number;

        private async Task<bool> handleRegistration(string receivedSmsVerificationCode)
        {
           
            try
            {


                var registrationId = KeyHelper.generateRegistrationId(false);
                TextSecurePreferences.setLocalRegistrationId((int)registrationId);

                await App.Current.accountManager.verifyAccount(receivedSmsVerificationCode, signalingKey, false, registrationId);
                await PushHelper.getInstance().OpenChannelAndUpload(); // also updates push channel id
                State = (int)RegistrationState.Verified;

                Recipient self = RecipientFactory.getRecipientsFromString(number, false).getPrimaryRecipient();
                IdentityKeyUtil.generateIdentityKeys();
                IdentityKeyPair identityKey = IdentityKeyUtil.getIdentityKeyPair();
                List<PreKeyRecord> records = await PreKeyUtil.generatePreKeys();
                PreKeyRecord lastResort = await PreKeyUtil.generateLastResortKey();
                SignedPreKeyRecord signedPreKey = await PreKeyUtil.generateSignedPreKey(identityKey);

                await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResort, signedPreKey, records);

                DatabaseFactory.getIdentityDatabase().saveIdentity(self.getRecipientId(), identityKey.getPublicKey());
                State = (int)RegistrationState.Generated;

                await DirectoryHelper.refreshDirectory(App.Current.accountManager, TextSecurePreferences.getLocalNumber());


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
            TextSecurePreferences.setPushRegistered(true);
            TextSecurePreferences.setLocalNumber(number);
            TextSecurePreferences.setPushServerPassword(password);
            TextSecurePreferences.setSignalingKey(signalingKey);
            TextSecurePreferences.setSignedPreKeyRegistered(true);
            TextSecurePreferences.setPromptedPushRegistration(true);
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
