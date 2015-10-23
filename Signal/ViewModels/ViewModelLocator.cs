using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Signal.database;
using Signal.ViewModels;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure("MasterDetail", typeof(View));
            //nav.Configure("ContactPageKey", typeof(ContactView));
            nav.Configure("MessagePageKey", typeof(MessageView));
            nav.Configure("RegistrationPageKey", typeof(RegistrationView));
            nav.Configure("VerificationPageKey", typeof(VerificationView));
            nav.Configure("DirectoryPageKey", typeof(DirectoryView));
            nav.Configure("SettingsPageKey", typeof(SettingsView));

            SimpleIoc.Default.Register<INavigationService>(() => nav);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design>();
                //SimpleIoc.Default.Register<IDataService, Sqlite>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, Sqlite>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RegistrationViewModel>();
            SimpleIoc.Default.Register<ThreadViewModel>();
            //SimpleIoc.Default.Register<ContactViewModel>();
            SimpleIoc.Default.Register<MessageViewModel>();
            SimpleIoc.Default.Register<DirectoryViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();

        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public MessageViewModel Message
        {
            get { return ServiceLocator.Current.GetInstance<MessageViewModel>(); }
        }

        public ThreadViewModel Thread
        {
            get { return ServiceLocator.Current.GetInstance<ThreadViewModel>(); }
        }

        /*public ContactViewModel Contact
        {
            get { return ServiceLocator.Current.GetInstance<ContactViewModel>(); }
        }*/

        public RegistrationViewModel Registration
        {
            get { return ServiceLocator.Current.GetInstance<RegistrationViewModel>(); }
        }

        public DirectoryViewModel Directory
        {
            get { return ServiceLocator.Current.GetInstance<DirectoryViewModel>(); }
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
