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

using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using Windows.ApplicationModel.Contacts;

namespace TextSecure.contacts
{
    public class ContactAccessor
    {

        public class ContactData
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<string> numbers { get; set; }
        }

        public static String PUSH_COLUMN = "push";

        private static ContactAccessor instance;

        private ContactAccessor()
        {
        }

        public static ContactAccessor getInstance()
        {

            if (instance == null)
                instance = new ContactAccessor();

            return instance;

        }



        /*public CursorLoader getCursorLoaderForContactsWithNumbers(Context context)
        {
            Uri uri = ContactsContract.Contacts.CONTENT_URI;
            String selection = ContactsContract.Contacts.HAS_PHONE_NUMBER + " = 1";

            return new CursorLoader(context, uri, null, selection, null,
                                    ContactsContract.Contacts.DISPLAY_NAME + " ASC");
        }

        public CursorLoader getCursorLoaderForContactGroups(Context context)
        {
            return new CursorLoader(context, ContactsContract.Groups.CONTENT_URI,
                                    null, null, null, ContactsContract.Groups.TITLE + " ASC");
        }

        public Loader<Cursor> getCursorLoaderForContacts(Context context, String filter)
        {
            return new ContactsCursorLoader(context, filter, false);
        }

        public Loader<Cursor> getCursorLoaderForPushContacts(Context context, String filter)
        {
            return new ContactsCursorLoader(context, filter, true);
        }

        public Cursor getCursorForContactsWithNumbers(Context context)
        {
            Uri uri = ContactsContract.Contacts.CONTENT_URI;
            String selection = ContactsContract.Contacts.HAS_PHONE_NUMBER + " = 1";

            return context.getContentResolver().query(uri, null, selection, null,
                                                      ContactsContract.Contacts.DISPLAY_NAME + " ASC");
        }
        */
        public async Task<List<ContactData>> getContactsWithPush()
        {
            //final ContentResolver resolver = context.getContentResolver();
            //final String[] inProjection = new String[] { PhoneLookup._ID, PhoneLookup.DISPLAY_NAME };
            try
            {

                List<String> pushNumbers = await DatabaseFactory.getDirectoryDatabase().getActiveNumbers();
                List<ContactData> lookupData = new List<ContactData>();

                var contactStore = await ContactManager.RequestStoreAsync();



                foreach (String pushNumber in pushNumbers)
                {
                    //Uri uri = Uri.withAppendedPath(PhoneLookup.CONTENT_FILTER_URI, Uri.encode(pushNumber));
                    //Cursor lookupCursor = resolver.query(uri, inProjection, null, null, null);
                    var contacts = await contactStore.FindContactsAsync(pushNumber);


                    if (contacts.First() != null)
                    {
                        ContactData contactData = new ContactData() { id = contacts.First().Id, name = contacts.First().DisplayName, numbers = new List<string>() };
                        contactData.numbers.Add(pushNumber);
                        lookupData.Add(contactData);
                    }
                }

                return lookupData;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw new Exception(e.Message);

                throw new NotImplementedException("ContactAccessor getContactsWithPush");
                //return lookupData;
            }
        }
        /*
        public String getNameFromContact(Context context, Uri uri)
        {
            Cursor cursor = null;

            try
            {
                cursor = context.getContentResolver().query(uri, new String[] { Contacts.DISPLAY_NAME },
                                                            null, null, null);

                if (cursor != null && cursor.moveToFirst())
                    return cursor.getString(0);

            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }

            return null;
        }

        public String getNameForNumber(Context context, String number)
        {
            Uri uri = Uri.withAppendedPath(PhoneLookup.CONTENT_FILTER_URI, Uri.encode(number));
            Cursor cursor = context.getContentResolver().query(uri, null, null, null, null);

            try
            {
                if (cursor != null && cursor.moveToFirst())
                    return cursor.getString(cursor.getColumnIndexOrThrow(PhoneLookup.DISPLAY_NAME));
            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }

            return null;
        }

        public GroupData getGroupData(Context context, Cursor cursor)
        {
            long id = cursor.getLong(cursor.getColumnIndexOrThrow(ContactsContract.Groups._ID));
            String title = cursor.getString(cursor.getColumnIndexOrThrow(ContactsContract.Groups.TITLE));

            return new GroupData(id, title);
        }

        public ContactData getContactData(Context context, Cursor cursor)
        {
            return getContactData(context,
                                  cursor.getString(cursor.getColumnIndexOrThrow(Contacts.DISPLAY_NAME)),
                                  cursor.getLong(cursor.getColumnIndexOrThrow(Contacts._ID)));
        }

        public ContactData getContactData(Context context, Uri uri)
        {
            return getContactData(context, getNameFromContact(context, uri), Long.parseLong(uri.getLastPathSegment()));
        }

        private ContactData getContactData(Context context, String displayName, long id)
        {
            ContactData contactData = new ContactData(id, displayName);
            Cursor numberCursor = null;

            try
            {
                numberCursor = context.getContentResolver().query(Phone.CONTENT_URI, null,
                                                                  Phone.CONTACT_ID + " = ?",
                                                                  new String[] { contactData.id + "" }, null);

                while (numberCursor != null && numberCursor.moveToNext())
                {
                    int type = numberCursor.getInt(numberCursor.getColumnIndexOrThrow(Phone.TYPE));
                    String label = numberCursor.getString(numberCursor.getColumnIndexOrThrow(Phone.LABEL));
                    String number = numberCursor.getString(numberCursor.getColumnIndexOrThrow(Phone.NUMBER));
                    String typeLabel = Phone.getTypeLabel(context.getResources(), type, label).toString();

                    contactData.numbers.add(new NumberData(typeLabel, number));
                }
            }
            finally
            {
                if (numberCursor != null)
                    numberCursor.close();
            }

            return contactData;
        }

        public List<ContactData> getGroupMembership(Context context, long groupId)
        {
            LinkedList<ContactData> contacts = new LinkedList<ContactData>();
            Cursor groupMembership = null;

            try
            {
                String selection = ContactsContract.CommonDataKinds.GroupMembership.GROUP_ROW_ID + " = ? AND " +
                                   ContactsContract.CommonDataKinds.GroupMembership.MIMETYPE + " = ?";
                String[] args = new String[] {groupId+"",
                                       ContactsContract.CommonDataKinds.GroupMembership.CONTENT_ITEM_TYPE};

                groupMembership = context.getContentResolver().query(Data.CONTENT_URI, null, selection, args, null);

                while (groupMembership != null && groupMembership.moveToNext())
                {
                    String displayName = groupMembership.getString(groupMembership.getColumnIndexOrThrow(Data.DISPLAY_NAME));
                    long contactId = groupMembership.getLong(groupMembership.getColumnIndexOrThrow(Data.CONTACT_ID));

                    contacts.add(getContactData(context, displayName, contactId));
                }
            }
            finally
            {
                if (groupMembership != null)
                    groupMembership.close();
            }

            return contacts;
        }

        public List<String> getNumbersForThreadSearchFilter(Context context, String constraint)
        {
            LinkedList<String> numberList = new LinkedList<>();
            Cursor cursor = null;

            try
            {
                cursor = context.getContentResolver().query(Uri.withAppendedPath(Phone.CONTENT_FILTER_URI,
                                                                                 Uri.encode(constraint)),
                                                            null, null, null, null);

                while (cursor != null && cursor.moveToNext())
                {
                    numberList.add(cursor.getString(cursor.getColumnIndexOrThrow(Phone.NUMBER)));
                }

            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }

            GroupDatabase.Reader reader = null;
            GroupRecord record;

            try
            {
                reader = DatabaseFactory.getGroupDatabase(context).getGroupsFilteredByTitle(constraint);

                while ((record = reader.getNext()) != null)
                {
                    numberList.add(record.getEncodedId());
                }
            }
            finally
            {
                if (reader != null)
                    reader.close();
            }

            return numberList;
        }

        public CharSequence phoneTypeToString(Context mContext, int type, CharSequence label)
        {
            return Phone.getTypeLabel(mContext.getResources(), type, label);
        }

        private long getContactIdFromLookupUri(Context context, Uri uri)
        {
            Cursor cursor = null;

            try
            {
                cursor = context.getContentResolver().query(uri,
                                                            new String[] { ContactsContract.Contacts._ID },
                                                            null, null, null);

                if (cursor != null && cursor.moveToFirst())
                {
                    return cursor.getLong(0);
                }
                else
                {
                    return -1;
                }

            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }
        }

        public static class NumberData implements Parcelable
        {

            public static final Parcelable.Creator<NumberData> CREATOR = new Parcelable.Creator<NumberData>() {
      public NumberData createFromParcel(Parcel in) {
                return new NumberData(in);
            }

            public NumberData[] newArray(int size) {
        return new NumberData[size];
      }
    };

    public final String number;
    public final String type;

    public NumberData(String type, String number)
    {
    this.type = type;
    this.number = number;
    }

    public NumberData(Parcel in)
    {
    number = in.readString();
    type = in.readString();
    }

    public int describeContents()
    {
    return 0;
    }

    public void writeToParcel(Parcel dest, int flags)
    {
    dest.writeString(number);
    dest.writeString(type);
    }
    }

    public static class GroupData
    {
    public final long id;
    public final String name;

    public GroupData(long id, String name)
    {
        this.id = id;
        this.name = name;
    }
    }

    public static class ContactData implements Parcelable
    {

    public static final Parcelable.Creator<ContactData> CREATOR = new Parcelable.Creator<ContactData>() {
      public ContactData createFromParcel(Parcel in) {
        return new ContactData(in);
    }

    public ContactData[] newArray(int size) {
        return new ContactData[size];
    }
    };

    public final long id;
    public final String name;
    public final List<NumberData> numbers;

    public ContactData(long id, String name)
    {
    this.id = id;
    this.name = name;
    this.numbers = new LinkedList<NumberData>();
    }

    public ContactData(long id, String name, List<NumberData> numbers)
    {
    this.id = id;
    this.name = name;
    this.numbers = numbers;
    }


    public ContactData(Parcel in)
    {
    id = in.readLong();
    name = in.readString();
    numbers = new LinkedList<NumberData>();
      in.readTypedList(numbers, NumberData.CREATOR);
    }

    public int describeContents()
    {
    return 0;
    }

    public void writeToParcel(Parcel dest, int flags)
    {
    dest.writeLong(id);
    dest.writeString(name);
    dest.writeTypedList(numbers);
    }
    }*/

        /***
         * If the code below looks shitty to you, that's because it was taken
         * directly from the Android source, where shitty code is all you get.
         */

        /*public Cursor getCursorForRecipientFilter(CharSequence constraint,
            ContentResolver mContentResolver)
      {
          final String SORT_ORDER = Contacts.TIMES_CONTACTED + " DESC," +
                                    Contacts.DISPLAY_NAME + "," +
                                    Contacts.Data.IS_SUPER_PRIMARY + " DESC," +
                                    Phone.TYPE;

          final String[] PROJECTION_PHONE = {
              Phone._ID,                  // 0
              Phone.CONTACT_ID,           // 1
              Phone.TYPE,                 // 2
              Phone.NUMBER,               // 3
              Phone.LABEL,                // 4
              Phone.DISPLAY_NAME,         // 5
          };

          String phone = "";
          String cons = null;

          if (constraint != null)
          {
              cons = constraint.toString();

              if (RecipientsAdapter.usefulAsDigits(cons))
              {
                  phone = PhoneNumberUtils.convertKeypadLettersToDigits(cons);
                  if (phone.equals(cons) && !PhoneNumberUtils.isWellFormedSmsAddress(phone))
                  {
                      phone = "";
                  }
                  else
                  {
                      phone = phone.trim();
                  }
              }
          }
          Uri uri = Uri.withAppendedPath(Phone.CONTENT_FILTER_URI, Uri.encode(cons));
          String selection = String.format("%s=%s OR %s=%s OR %s=%s",
                                           Phone.TYPE,
                                           Phone.TYPE_MOBILE,
                                           Phone.TYPE,
                                           Phone.TYPE_WORK_MOBILE,
                                           Phone.TYPE,
                                           Phone.TYPE_MMS);

          Cursor phoneCursor = mContentResolver.query(uri,
                                                      PROJECTION_PHONE,
                                                      null,
                                                      null,
                                                      SORT_ORDER);

          if (phone.length() > 0)
          {
              ArrayList result = new ArrayList();
              result.add(Integer.valueOf(-1));                    // ID
              result.add(Long.valueOf(-1));                       // CONTACT_ID
              result.add(Integer.valueOf(Phone.TYPE_CUSTOM));     // TYPE
              result.add(phone);                                  // NUMBER
              */
        /*
        * The "\u00A0" keeps Phone.getDisplayLabel() from deciding
        * to display the default label ("Home") next to the transformation
        * of the letters into numbers.
        */
        /*result.add("\u00A0");                               // LABEL
        result.add(cons);                                   // NAME

        ArrayList<ArrayList> wrap = new ArrayList<ArrayList>();
        wrap.add(result);

        ArrayListCursor translated = new ArrayListCursor(PROJECTION_PHONE, wrap);

        return new MergeCursor(new Cursor[] { translated, phoneCursor });
    }
    else
    {
        return phoneCursor;
    }*/
    }

}
