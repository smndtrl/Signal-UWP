using GalaSoft.MvvmLight;
using Signal.database;
using Signal.database.loaders;
using Signal.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel
{
    public class ContactViewModel : ViewModelBase
    {
        public ContactViewModel(IDataService service)
        {
            Contacts = new ContactCollection(service);
        }

        ObservableCollection<Contact> _Contacts;

        public ObservableCollection<Contact> Contacts
        {
            get { return _Contacts; }
            set
            {
                _Contacts = value;
                RaisePropertyChanged("Contacts");
            }
        }
    }
}
