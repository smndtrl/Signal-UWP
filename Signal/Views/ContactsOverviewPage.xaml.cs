/** 
 * Copyright (C) 2015 smndtrl
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using libtextsecure.push;
using Signal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TextSecure.database;
using Signal.Push;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TextSecure.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactsOverviewPage : Page
    {
        private IList<Contact> contactsList;
        private IDictionary<string, SimplifiedContact> SimplifiedContacts = new Dictionary<string, SimplifiedContact>();

        public ContactsOverviewPage()
        {
            this.InitializeComponent();

        }

        private void loadContacts()
        {
            var directory = DatabaseFactory.getDirectoryDatabase();

            //directory.getActiveNumbers,

            var list = SimplifiedContacts.Values;
            var result = from act in list group act by act.ContactType into grp orderby grp.Key select grp;
            combinedContacts.Source = result;
        }

        public class ListGroupStyleSelector : GroupStyleSelector
        {
            protected override GroupStyle SelectGroupStyleCore(object group, uint level)
            {
                return (GroupStyle)App.Current.Resources["listViewGroupStyle"];
            }
        }

        public class ContactType
        {
            public ContactType()
            {
                Contacts = new ObservableCollection<SimplifiedContact>();
            }

            public string Name { get; set; }
            public ObservableCollection<SimplifiedContact> Contacts { get; private set; }
        }

        public class SimplifiedContact
        {
            public string Name { get; set; }
            public string ContactType { get; set; }
        }
    }

}
