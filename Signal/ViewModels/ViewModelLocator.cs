using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Signal.database;
using Signal.Resources;
using Signal.ViewModels;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModels
{
    public class ViewModelLocator
    {
        public static string REGISTERTYPE_PAGE_KEY = "RegisterTypePageKey";
        public static string REGISTERING_PAGE_KEY = "RegisterPageKey";
        public static string PROVISIONING_PAGE_KEY = "ProvisionPageKey";

        public static string SPLASH_PAGE_KEY = "SplashPageKey";
        public static string SETTINGS_PAGE_KEY = "SettingsPageKey";

        public static string MAIN_PAGE_KEY = "MainPageKey";
        public static string MESSAGES_PAGE_KEY = "ThreadDetailPageKey";
        public static string MESSAGEDETAIL_PAGE_KEY = "MessageDetailPageKey";


        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new SignalNavigationService();
            nav.Configure(MAIN_PAGE_KEY, typeof(MainPage));
            nav.Configure(MESSAGES_PAGE_KEY, typeof(ThreadPage));
            nav.Configure(MESSAGEDETAIL_PAGE_KEY, typeof(MessageDetailsPage));


            //nav.Configure(REGISTERTYPE_PAGE_KEY, typeof(RegistrationTypeView));
            nav.Configure(REGISTERING_PAGE_KEY, typeof(RegistrationView));
            //nav.Configure(PROVISIONING_PAGE_KEY, typeof(ProvisioningView));
            
            nav.Configure(SETTINGS_PAGE_KEY, typeof(SettingsViewTest));

            SimpleIoc.Default.Register<INavigationServiceSignal>(() => nav);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, Sqlite>();
            }


            SimpleIoc.Default.Register<SplashViewModel>();
            SimpleIoc.Default.Register<RegistrationViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            //SimpleIoc.Default.Register<ContactViewModel>();
            SimpleIoc.Default.Register<MessageViewModel>();
            SimpleIoc.Default.Register<MessageDetailViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<ProvisionViewModel>();

        }

        public SplashViewModel Splash
        {
            get { return ServiceLocator.Current.GetInstance<SplashViewModel>(); }
        }


        public MessageViewModel Message
        {
            get { return ServiceLocator.Current.GetInstance<MessageViewModel>(); }
        }

        public MessageDetailViewModel MessageDetail
        {
            get { return ServiceLocator.Current.GetInstance<MessageDetailViewModel>(); }
        }

        public MainViewModel Thread
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /*public ContactViewModel Contact
        {
            get { return ServiceLocator.Current.GetInstance<ContactViewModel>(); }
        }*/

        public RegistrationViewModel Registration
        {
            get { return ServiceLocator.Current.GetInstance<RegistrationViewModel>(); }
        }

        public ProvisionViewModel Provision
        {
            get { return ServiceLocator.Current.GetInstance<ProvisionViewModel>(); }
        }
        
        public SettingsViewModel Settings
        {
            get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); }
        }

        public static ViewModelBase GetViewModel<T>(string key) where T : ViewModelBase
        {
            return ServiceLocator.Current.GetInstance<T>(key);
        }
    }
}
