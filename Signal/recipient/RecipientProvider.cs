

using Signal.Model;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.util;
using Windows.ApplicationModel.Contacts;

namespace TextSecure.recipient
{
    public class RecipientProvider
    {

        private static readonly Dictionary<long, Recipient> recipientCache = new LRUCache<long, Recipient>(1000);
        private static readonly Dictionary<RecipientIds, Recipients> recipientsCache = new LRUCache<RecipientIds, Recipients>(1000);
        //private static readonly ExecutorService asyncRecipientResolver = Util.newSingleThreadedLifoExecutor();

        /**private static final String[] CALLER_ID_PROJECTION = new String[] {
          PhoneLookup.DISPLAY_NAME,
          PhoneLookup.LOOKUP_KEY,
          PhoneLookup._ID,
          PhoneLookup.NUMBER
          };*/

        internal Recipient getRecipient(long recipientId, bool asynchronous)
        {
            Recipient cachedRecipient;
            recipientCache.TryGetValue(recipientId, out cachedRecipient);

            if (cachedRecipient != null) return cachedRecipient;
            //else if (asynchronous) return gethronousRecipient(recipientId);
            else return getSynchronousRecipient(recipientId);
        }

        internal Recipients getRecipients(long[] recipientIds, bool asynchronous)
        {
            Recipients cachedRecipients;
            recipientsCache.TryGetValue(new RecipientIds(recipientIds), out cachedRecipients);

            if (cachedRecipients != null) return cachedRecipients;

            List<Recipient> recipientList = new List<Recipient>();

            foreach (long recipientId in recipientIds)
            {
                recipientList.Add(getRecipient(recipientId, asynchronous));
            }

            if (asynchronous && false) cachedRecipients = null;//cachedRecipients = new Recipients(recipientList, getRecipientsPreferences(recipientIds)); // TODO fix
            else cachedRecipients = new Recipients(recipientList, null/*getRecipientsPreferencesSync(recipientIds)*/);

            recipientsCache.Add(new RecipientIds(recipientIds), cachedRecipients);
            return cachedRecipients;
        }

        private Recipient getSynchronousRecipient(long recipientId)
        {
            Debug.WriteLine("RecipientProvider", "Cache miss [SYNC]!");

            Recipient recipient;
            RecipientDetails details;
            String number = DatabaseFactory.getCanonicalAddressDatabase().getAddressFromId(recipientId);
            bool isGroupRecipient = GroupUtil.isEncodedGroup(number);

            if (isGroupRecipient) details = getGroupRecipientDetails(number).Result;
            else details = getRecipientDetails(number);

            if (details != null)
            {
                recipient = new Recipient(details.name, details.number, recipientId);
            }
            else
            {
                /*final Drawable defaultPhoto = isGroupRecipient
                                                     ? ContactPhotoFactory.getDefaultGroupPhoto(context)
                                                     : ContactPhotoFactory.getDefaultContactPhoto(context, null);*/

                recipient = new Recipient(null, number, recipientId/*, null, defaultPhoto*/);
            }

            recipientCache.Add(recipientId, recipient);
            return recipient;
        }

        /*private Recipient gethronousRecipient(final Context context, final long recipientId)
        {
            Log.w("RecipientProvider", "Cache miss [ASYNC]!");

            final String number = CanonicalAddressDatabase.getInstance(context).getAddressFromId(recipientId);
            final boolean isGroupRecipient = GroupUtil.isEncodedGroup(number);

            Callable<RecipientDetails> task = new Callable<RecipientDetails>() {
          @Override
          public RecipientDetails call() throws Exception
        {
            if (isGroupRecipient) return getGroupRecipientDetails(context, number);
            else                  return getRecipientDetails(context, number);
        }
    };

    ListenableFutureTask<RecipientDetails> future = new ListenableFutureTask<>(task);

    asyncRecipientResolver.submit(future);

        Drawable contactPhoto;

        if (isGroupRecipient) {
          contactPhoto        = ContactPhotoFactory.getDefaultGroupPhoto(context);
        } else {
          contactPhoto        = ContactPhotoFactory.getLoadingPhoto(context);
        }

        Recipient recipient = new Recipient(number, contactPhoto, recipientId, future);
    recipientCache.put(recipientId, recipient);

        return recipient;
      }
      */
        void clearCache()
        {
            recipientCache.Clear();
            recipientsCache.Clear();
        }

        private RecipientDetails getRecipientDetails(String number)
        {
            /*Uri uri = Uri.withAppendedPath(PhoneLookup.CONTENT_FILTER_URI, Uri.encode(number));
            Cursor cursor = context.getContentResolver().query(uri, CALLER_ID_PROJECTION,
                                                               null, null, null);*/

           /* ContactStore contactStore = ContactManager.RequestStore().GetResults();
            IReadOnlyList<Contact> contacts = contactStore.FindContacts(number).GetResults();


            try
            {
                if (contacts != null && contacts.Count == 1)
                {
                    var contact = contacts.First();
                    String name = contact.DisplayName; // cursor.getString(3).equals(cursor.getString(0)) ? null : cursor.getString(0);
                    return new RecipientDetails(name, contact.Id);
                }
            }
            finally
            {
            }*/

            return new RecipientDetails(null, number);
        }

        private async Task<RecipientDetails> getGroupRecipientDetails(String groupId)
        {
            try
            {
                GroupDatabase.GroupRecord record = await DatabaseFactory.getGroupDatabase()
                                                                   .getGroup(GroupUtil.getDecodedId(groupId));

                if (record != null)
                {
                    //Drawable avatar = ContactPhotoFactory.getGroupContactPhoto(context, record.getAvatar());
                    return new RecipientDetails(record.getTitle(), groupId/*, null, avatar*/);
                }

                return null;
            }
            catch (Exception e)
            {
                //Log.w("RecipientProvider", e);
                return null;
            }
        }

        /*private @Nullable RecipientsPreferences getRecipientsPreferencesSync(long[] recipientIds)
        {
            return DatabaseFactory.getRecipientPreferenceDatabase(context)
                                  .getRecipientsPreferences(recipientIds)
                                  .orNull();
        }*/

        /*private ListenableFutureTask<RecipientsPreferences> getRecipientsPreferences(final Context context, final long[] recipientIds)
        {
            ListenableFutureTask<RecipientsPreferences> task = new ListenableFutureTask<>(new Callable<RecipientsPreferences>() {
      @Override
      public RecipientsPreferences call() throws Exception
        {
        return getRecipientsPreferencesSync(context, recipientIds);
        }
    });

    asyncRecipientResolver.execute(task);

    return task;
  }*/



    }
    public class RecipientDetails
    {
        public String name;
        public String number;
        //public static Drawable avatar;
        //public static Uri      contactUri;

        public RecipientDetails(String name, String number/*, Uri contactUri, Drawable avatar*/)
        {
            this.name = name;
            this.number = number;
            //this.avatar = avatar;
            //this.contactUri = contactUri;
        }
    }

    public class RecipientIds
    {
        private long[] ids;

        public RecipientIds(long[] ids)
        {
            this.ids = ids;
        }

        public override bool Equals(Object other)
        {
            if (other == null || !(other is RecipientIds)) return false;
            return Array.Equals(this.ids, ((RecipientIds)other).ids);
        }

        public override int GetHashCode()
        {
            return ids.GetHashCode();
        }
    }
}
