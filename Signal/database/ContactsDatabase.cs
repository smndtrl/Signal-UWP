

using Signal.Models;
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
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.contacts;

namespace TextSecure.database
{
    /**
 * Database to supply all types of contacts that TextSecure needs to know about
 *
 * @author Jake McGinty
 */
    public class ContactsDatabase
    {



        /* private static final String   FILTER_SELECTION   = NAME_COLUMN + " LIKE ? OR " + NUMBER_COLUMN + " LIKE ?";
   private static final String   CONTACT_LIST_SORT  = NAME_COLUMN + " COLLATE NOCASE ASC";
   private static final String[] ANDROID_PROJECTION = new String[]{ID_COLUMN,
                                                                   NAME_COLUMN,
                                                                   NUMBER_TYPE_COLUMN,
                                                                   LABEL_COLUMN,
                                                                   NUMBER_COLUMN
     };

     private static final String[] CONTACTS_PROJECTION = new String[]{ID_COLUMN,
                                                                    NAME_COLUMN,
                                                                    NUMBER_TYPE_COLUMN,
                                                                    LABEL_COLUMN,
                                                                    NUMBER_COLUMN,
                                                                    TYPE_COLUMN
    };*/

        /*public const uint NORMAL_TYPE = 0;
        public const uint PUSH_TYPE = 1;
        public const uint GROUP_TYPE = 2;

        SQLiteConnection conn;

        public ContactsDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Contact>();
        }

        public int Count()
        {
            return conn.Table<Contact>().Count();
        }

        public void close()
        {
            //dbHelper.close();
        }

        public async Task<List<Contact>> getContacts()
        {

            loadPushUsers();

            var list = conn.Table<Contact>().Where(t => true).ToList();
            return list;
        }*/

        /*public Cursor query(String filter, boolean pushOnly)
        {
            // FIXME: This doesn't make sense to me.  You pass in pushOnly, but then
            // conditionally check to see whether other contacts should be included
            // in the query method itself? I don't think this method should have any
            // understanding of that stuff.
            final boolean      includeAndroidContacts = !pushOnly && TextSecurePreferences.isSmsEnabled(context);
            final Cursor       localCursor = queryLocalDb(filter);
            final Cursor       androidCursor;
            final MatrixCursor newNumberCursor;

            if (includeAndroidContacts)
            {
                androidCursor = queryAndroidDb(filter);
            }
            else
            {
                androidCursor = null;
            }

            if (!TextUtils.isEmpty(filter) && NumberUtil.isValidSmsOrEmail(filter))
            {
                newNumberCursor = new MatrixCursor(CONTACTS_PROJECTION, 1);
                newNumberCursor.addRow(new Object[]{-1L, context.getString(R.string.contact_selection_list__unknown_contact),
                             ContactsContract.CommonDataKinds.Phone.TYPE_CUSTOM, "\u21e2", filter, NORMAL_TYPE});
            }
            else
            {
                newNumberCursor = null;
            }

            List<Cursor> cursors = new ArrayList<Cursor>();
            if (localCursor != null) cursors.add(localCursor);
            if (androidCursor != null) cursors.add(androidCursor);
            if (newNumberCursor != null) cursors.add(newNumberCursor);

            switch (cursors.size())
            {
                case 0: return null;
                case 1: return cursors.get(0);
                default: return new MergeCursor(cursors.toArray(new Cursor[] { }));
            }
        }*/
        /*
        private Cursor queryAndroidDb(String filter)
        {
            final Uri baseUri;
            if (!TextUtils.isEmpty(filter))
            {
                baseUri = Uri.withAppendedPath(ContactsContract.CommonDataKinds.Phone.CONTENT_FILTER_URI,
                                               Uri.encode(filter));
            }
            else
            {
                baseUri = ContactsContract.CommonDataKinds.Phone.CONTENT_URI;
            }
            Cursor cursor = context.getContentResolver().query(baseUri, ANDROID_PROJECTION, null, null, CONTACT_LIST_SORT);
            return cursor == null ? null : new TypedCursorWrapper(cursor);
        }

        private Cursor queryLocalDb(String filter)
        {
            final String   selection;
            final String[] selectionArgs;
            final String   fuzzyFilter = "%" + filter + "%";
            if (!TextUtils.isEmpty(filter))
            {
                selection = FILTER_SELECTION;
                selectionArgs = new String[] { fuzzyFilter, fuzzyFilter };
            }
            else
            {
                selection = null;
                selectionArgs = null;
            }
            return queryLocalDb(selection, selectionArgs, null);
        }

        private Cursor queryLocalDb(String selection, String[] selectionArgs, String[] columns)
        {
            SQLiteDatabase localDb = dbHelper.getReadableDatabase();
            final Cursor localCursor;
            if (localDb != null) localCursor = localDb.query(TABLE_NAME, columns, selection, selectionArgs, null, null, CONTACT_LIST_SORT);
            else localCursor = null;
            if (localCursor != null && !localCursor.moveToFirst())
            {
                localCursor.close();
                return null;
            }
            return localCursor;
        }*/
        /*
        private static class DatabaseOpenHelper extends SQLiteOpenHelper
        {

            private final Context context;
            private SQLiteDatabase mDatabase;

            private static final String TABLE_CREATE =
        "CREATE TABLE " + TABLE_NAME + " (" +
            ID_COLUMN          + " INTEGER PRIMARY KEY, " +
            NAME_COLUMN        + " TEXT, " +
            NUMBER_TYPE_COLUMN + " INTEGER, " +
            LABEL_COLUMN       + " TEXT, " +
            NUMBER_COLUMN      + " TEXT, " +
            TYPE_COLUMN        + " INTEGER);";

            DatabaseOpenHelper(Context context) {
                super(context, null, null, 1);
                this.context = context;
            }

            @Override
          public void onCreate(SQLiteDatabase db)
        {
            Log.d(TAG, "onCreate called for contacts database.");
            mDatabase = db;
            mDatabase.execSQL(TABLE_CREATE);
            if (TextSecurePreferences.isPushRegistered(context))
            {
                try
                {
                    loadPushUsers();
                }
                catch (IOException ioe)
                {
                    Log.e(TAG, "Issue when trying to load push users into memory db.", ioe);
                }
            }
        }

        @Override
      public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            Log.w(TAG, "Upgrading database from version " + oldVersion + " to "
                + newVersion + ", which will destroy all old data");
            db.execSQL("DROP TABLE IF EXISTS " + TABLE_NAME);
            onCreate(db);
        }
        */
        /*private async void loadPushUsers()// throws IOException
        {
            Debug.WriteLine("populating push users into virtual db.");
            List<ContactAccessor.ContactData> pushUsers = await ContactAccessor.getInstance().getContactsWithPush();
            foreach (ContactAccessor.ContactData user in pushUsers)
            {
                /*var contact = new ContactTable()
                {
                    _id = user.id,
                    name = user.name,
                    number_type = 0,
                    number = user.numbers[0],
                    label = "",
                    type = PUSH_TYPE,

                };*

                var contact = new Contact()
                {
                    label = "label",
                    number = user.numbers[0],
                    name = user.name,
                    SystemId = user.id
                };

                Debug.WriteLine($"Found push {contact.name} with {contact.number}");

                try
                {
                    var list = conn.Table<Contact>().Where(t => t.number == contact.number).ToList();
                    if (list.Count == 0) conn.Insert(contact);
                }
                catch (SQLiteException e) { } //assumes duplicate key

                /*ContentValues values = new ContentValues();
                values.put(ID_COLUMN, user.id);
                values.put(NAME_COLUMN, user.name);
                values.put(NUMBER_TYPE_COLUMN, ContactsContract.CommonDataKinds.Phone.TYPE_CUSTOM);
                values.put(LABEL_COLUMN, (String)null);
                values.put(NUMBER_COLUMN, user.numbers.get(0).number);
                values.put(TYPE_COLUMN, PUSH_TYPE);
                mDatabase.insertWithOnConflict(TABLE_NAME, null, values, SQLiteDatabase.CONFLICT_IGNORE);*
            }
            Debug.WriteLine("finished populating push users.");
        }*/
    }/*

    private static class TypedCursorWrapper extends CursorWrapper
    {

        private final int pushColumnIndex;

        public TypedCursorWrapper(Cursor cursor) {
            super(cursor);
            pushColumnIndex = cursor.getColumnCount();
        }

        @Override
      public int getColumnCount() {
            return super.getColumnCount() + 1;
        }

        @Override
      public int getColumnIndex(String columnName) {
            if (TYPE_COLUMN.equals(columnName)) return super.getColumnCount();
            else return super.getColumnIndex(columnName);
        }

        @Override
      public int getColumnIndexOrThrow(String columnName) throws IllegalArgumentException
        {
            if (TYPE_COLUMN.equals(columnName)) return super.getColumnCount();
            else return super.getColumnIndexOrThrow(columnName);
        }

        @Override
      public String getColumnName(int columnIndex) {
            if (columnIndex == pushColumnIndex) return TYPE_COLUMN;
            else return super.getColumnName(columnIndex);
        }

        @Override
      public String[] getColumnNames() {
            final String[] columns = new String[super.getColumnCount() + 1];
            System.arraycopy(super.getColumnNames(), 0, columns, 0, super.getColumnCount());
            columns[pushColumnIndex] = TYPE_COLUMN;
            return columns;
        }

        @Override
      public int getInt(int columnIndex) {
            if (columnIndex == pushColumnIndex) return NORMAL_TYPE;
            else return super.getInt(columnIndex);
        }
    }
    }*/
}
