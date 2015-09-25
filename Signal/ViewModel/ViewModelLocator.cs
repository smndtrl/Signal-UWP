using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Signal.database;
using Signal.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.Views;

namespace Signal.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure("MasterDetail", typeof(View));
            nav.Configure("ContactPageKey", typeof(ContactView));
            nav.Configure("MessagePageKey", typeof(MessageView));
            nav.Configure("RegistrationPageKey", typeof(RegistrationView));
            nav.Configure("VerificationPageKey", typeof(VerificationView));
            nav.Configure("DirectoryPageKey", typeof(ThreadAddView));

            SimpleIoc.Default.Register<INavigationService>(() => nav);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design>();
            } else
            {
                SimpleIoc.Default.Register<IDataService, Real>();
            }

            SimpleIoc.Default.Register<ViewModel>();
            SimpleIoc.Default.Register<RegistrationViewModel>();
            SimpleIoc.Default.Register<ThreadViewModel>();
            SimpleIoc.Default.Register<ContactViewModel>();
            SimpleIoc.Default.Register<MessageViewModel>();
            SimpleIoc.Default.Register<DirectoryViewModel>();

        }

        public ViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<ViewModel>(); }
        }

        public MessageViewModel Message
        {
            get { return ServiceLocator.Current.GetInstance<MessageViewModel>(); }
        }

        public ThreadViewModel Thread
        {
            get { return ServiceLocator.Current.GetInstance<ThreadViewModel>(); }
        }

        public ContactViewModel Contact
        {
            get { return ServiceLocator.Current.GetInstance<ContactViewModel>(); }
        }

        public RegistrationViewModel Registration
        {
            get { return ServiceLocator.Current.GetInstance<RegistrationViewModel>(); }
        }

        public DirectoryViewModel Directory
        {
            get { return ServiceLocator.Current.GetInstance<DirectoryViewModel>(); }
        }
    }
}
