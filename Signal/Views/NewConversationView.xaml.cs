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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TextSecure.database;
using TextSecure.database.loaders;
using TextSecure.recipient;
using TextSecure.util;
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
    public sealed partial class NewConversationView : Page
    {

        //private ContactsLoader<ContactsDatabase.Contact> contacts;

        public NewConversationView()
        {
            this.InitializeComponent();

            Debug.WriteLine("loaded new conversation view");
            //contacts = new ContactsLoader<ContactsDatabase.Contact>();
            //contactsCollection.Source = contacts;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //contacts = new ContactsLoader<ContactsDatabase.Contact>();
            //contactsCollection.Source = contacts;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
            /*Debug.WriteLine("Button clicked");
            if (identities != null)
            {
                identities.CollectionChanged -= idchange;
            }
            identities = new DirectoryLoader<TextSecureDirectory.Directory>();
            identities.CollectionChanged += idchange;
            identitiesCVS.Source = identities;*/
        }

        private void ContactsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine($"{sender.ToString()} sends {e.ToString()}");
            ContactsDatabase.ContactTable contact = (ContactsDatabase.ContactTable)e.ClickedItem;
            //ContactsDatabase.Contact contact = (ContactsDatabase.Contact)sender;
            Recipients recipients = RecipientFactory.getRecipientsFromString(contact.number, true);

            var rootFrame = new Frame();
            //rootFrame.Navigate(typeof(NarrowMessageView), new Payload() { threadId = -1, recipients = recipients.getIds() });
            Window.Current.Content = rootFrame;
        }

        private async void RefreshDictionary_Click(object sender, RoutedEventArgs e)
        {
            await DirectoryHelper.refreshDirectory();
            //contacts.Reload();
        }
    }
}
