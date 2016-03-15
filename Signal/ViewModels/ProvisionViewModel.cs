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
using Windows.Media.Capture;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Signal.Views;
using Signal.Resources;

namespace Signal.ViewModels
{
    public class ProvisionViewModel : ViewModelBase, INavigableViewModel
    {
        private readonly INavigationServiceSignal _navigationService;
        private readonly IDataService _dataService;

        public ProvisionViewModel(IDataService service, INavigationServiceSignal navService)
        {
            _dataService = service;
            _navigationService = navService;
        }

        private async Task<DeviceInformation> GetCamera()
        {
            var allCameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return allCameras.FirstOrDefault();
        }

        public async void NavigateFrom(NavigationEventArgs parameter)
        {
            var cam = await GetCamera();

            if (cam != null) HasCamera = true;
        }

        public void NavigateTo(NavigationEventArgs parameter)
        {
            //throw new NotImplementedException();
        }

        private CaptureElement _captureElement;
        public CaptureElement CaptureElement
        {
            get
            {
                return _captureElement ?? new CaptureElement();
            }
            set
            {
                Set(ref _captureElement, value);
                RaisePropertyChanged();
            }
        }

        /*
         * ProvisioningView
         */

        private RelayCommand _scanCodeCommand;
        public RelayCommand ScanCodeCommand
        {
            get
            {
                return _scanCodeCommand ?? (_scanCodeCommand = new RelayCommand(
                    async () => {
                        //await InitializeCameraAsync();
                    },
                    () => { return HasCamera; }
                    ));
            }
        }

        private bool _hasCamera;
        public bool HasCamera {
            get
            {
                return _hasCamera;
            }
            set
            {
                Set(ref _hasCamera, value);
                ScanCodeCommand.RaiseCanExecuteChanged();
            }
        }

        /*public RelayCommand _navigateProvisionCommand { get; private set; }
        public RelayCommand NavigateProvisionCommand
        {
            get
            {
                return _navigateProvisionCommand ?? new RelayCommand(
                    () => { _navigationService.NavigateTo(ViewModelLocator.PROVISIONING_PAGE_KEY); },
                    () => { return true; }
                    );
            }
        }*/


        /*private string password;
        private string signalingKey;
        private string number;

        private async Task<bool> handleRegistration(string receivedSmsVerificationCode)
        {
           
            try
            {


                var registrationId = KeyHelper.generateRegistrationId(false);
                TextSecurePreferences.setLocalRegistrationId((int)registrationId);

                await App.Current.accountManager.verifyAccountWithCode(receivedSmsVerificationCode, signalingKey, registrationId, false);
                await PushHelper.getInstance().OpenChannelAndUpload(); // also updates push channel id
                State = (int)RegistrationState.Verified;

                Recipient self = RecipientFactory.getRecipientsFromString(number, false).getPrimaryRecipient();
                IdentityKeyUtil.generateIdentityKeys();
                IdentityKeyPair identityKey = IdentityKeyUtil.GetIdentityKeyPair();
                List<PreKeyRecord> records = await PreKeyUtil.generatePreKeys();
                PreKeyRecord lastResort = await PreKeyUtil.generateLastResortKey();
                SignedPreKeyRecord signedPreKey = await PreKeyUtil.generateSignedPreKey(identityKey);

                await App.Current.accountManager.setPreKeys(identityKey.getPublicKey(), lastResort, signedPreKey, records);

                DatabaseFactory.getIdentityDatabase().SaveIdentity(self.getRecipientId(), identityKey.getPublicKey());
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
            TextSecurePreferences.setPushRegistered(false);
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
