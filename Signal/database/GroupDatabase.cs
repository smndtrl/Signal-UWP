

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
using SQLite;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;
using TextSecure.util;
using Windows.Security.Cryptography;

namespace TextSecure.database
{
    public class GroupDatabase
    {

        /*public static readonly String DATABASE_UPDATE_ACTION = "org.thoughtcrime.securesms.database.GroupDatabase.UPDATE";

        private static readonly String TAG = GroupDatabase.class.getSimpleName();*/

        private const String TABLE_NAME = "groups";
        private static readonly String ID = "_id";
        private static readonly String GROUP_ID = "group_id";
        private static readonly String TITLE = "title";
        private static readonly String MEMBERS = "members";
        private static readonly String AVATAR = "avatar";
        private static readonly String AVATAR_ID = "avatar_id";
        private static readonly String AVATAR_KEY = "avatar_key";
        private static readonly String AVATAR_CONTENT_TYPE = "avatar_content_type";
        private static readonly String AVATAR_RELAY = "avatar_relay";
        private static readonly String TIMESTAMP = "timestamp";
        private static readonly String ACTIVE = "active";

        [Table(TABLE_NAME)]
        public class Group
        {
            [PrimaryKey, AutoIncrement]
            public int? _id { get; set; } = null;
            [Unique]
            public string group_id { get; set; }
            public string title { get; set; }
            public string members { get; set; }
            public byte[] avatar { get; set; }
            public int avatar_id { get; set; }
            public byte[] avatar_key { get; set; }
            public string avatar_content_type { get; set; }
            public string avatar_relay { get; set; }
            public long timestamp { get; set; }
            public bool active { get; set; } = true;
        }

        public static readonly String CREATE_TABLE =
            "CREATE TABLE " + TABLE_NAME +
                " (" + ID + " INTEGER PRIMARY KEY, " +
                GROUP_ID + " TEXT, " +
                TITLE + " TEXT, " +
                MEMBERS + " TEXT, " +
                AVATAR + " BLOB, " +
                AVATAR_ID + " INTEGER, " +
                AVATAR_KEY + " BLOB, " +
                AVATAR_CONTENT_TYPE + " TEXT, " +
                AVATAR_RELAY + " TEXT, " +
                TIMESTAMP + " INTEGER, " +
                ACTIVE + " INTEGER DEFAULT 1);";

        public static readonly String[] CREATE_INDEXS = {
      "CREATE UNIQUE INDEX IF NOT EXISTS group_id_index ON " + TABLE_NAME + " (" + GROUP_ID + ");",
  };
        SQLiteConnection conn;

        public GroupDatabase(SQLiteConnection conn)
        {
            this.conn = conn;

            conn.CreateTable<Group>();
        }

        public async Task<GroupRecord> getGroup(byte[] groupId)
        {


            var query = conn.Table<Group>().Where(v => v.group_id == GroupUtil.getEncodedId(groupId));

            Reader reader = new Reader(query.ToList());
            GroupRecord record = reader.getNext();

            reader.close();
            return record;
        }

        /*public Reader getGroupsFilteredByTitle(String constraint)
        {
            Cursor cursor = databaseHelper.getReadableDatabase().query(TABLE_NAME, null, TITLE + " LIKE ?",
                                                                       new String[] { "%" + constraint + "%" },
                                                                       null, null, null);

            return new Reader(cursor);
        }*/

        public async Task<Reader> getGroups()
        {
            var query = conn.Table<Group>().Where(v => true);
            //Cursor cursor = databaseHelper.getReadableDatabase().query(TABLE_NAME, null, null, null, null, null, null);
            return new Reader(query.ToList());
        }

        public async Task<Recipients> getGroupMembers(byte[] groupId, bool includeSelf)
        {
            String localNumber = TextSecurePreferences.getLocalNumber();
            List<String> members = await getCurrentMembers(groupId);
            List<Recipient> recipients = new List<Recipient>();

            foreach (String member in members)
            {
                if (!includeSelf && member.Equals(localNumber))
                    continue;

                recipients.AddRange(RecipientFactory.getRecipientsFromString( member, false)
                                                  .getRecipientsList());
            }

            return RecipientFactory.getRecipientsFor(recipients, false);
        }
        /*
        public void create(byte[] groupId, String title, List<String> members,
                           TextSecureAttachmentPointer avatar, String relay)
        {
            ContentValues contentValues = new ContentValues();
            contentValues.put(GROUP_ID, GroupUtil.getEncodedId(groupId));
            contentValues.put(TITLE, title);
            contentValues.put(MEMBERS, Util.join(members, ","));

            if (avatar != null)
            {
                contentValues.put(AVATAR_ID, avatar.getId());
                contentValues.put(AVATAR_KEY, avatar.getKey());
                contentValues.put(AVATAR_CONTENT_TYPE, avatar.getContentType());
            }

            contentValues.put(AVATAR_RELAY, relay);
            contentValues.put(TIMESTAMP, System.currentTimeMillis());
            contentValues.put(ACTIVE, 1);

            databaseHelper.getWritableDatabase().insert(TABLE_NAME, null, contentValues);
        }

        public void update(byte[] groupId, String title, TextSecureAttachmentPointer avatar)
        {
            ContentValues contentValues = new ContentValues();
            if (title != null) contentValues.put(TITLE, title);

            if (avatar != null)
            {
                contentValues.put(AVATAR_ID, avatar.getId());
                contentValues.put(AVATAR_CONTENT_TYPE, avatar.getContentType());
                contentValues.put(AVATAR_KEY, avatar.getKey());
            }

            databaseHelper.getWritableDatabase().update(TABLE_NAME, contentValues,
                                                        GROUP_ID + " = ?",
                                                        new String[] { GroupUtil.getEncodedId(groupId) });

            RecipientFactory.clearCache();
            notifyDatabaseListeners();
        }

        public void updateTitle(byte[] groupId, String title)
        {
            ContentValues contentValues = new ContentValues();
            contentValues.put(TITLE, title);
            databaseHelper.getWritableDatabase().update(TABLE_NAME, contentValues, GROUP_ID + " = ?",
                                                        new String[] { GroupUtil.getEncodedId(groupId) });

            RecipientFactory.clearCache();
            notifyDatabaseListeners();
        }

        public void updateAvatar(byte[] groupId, Bitmap avatar)
        {
            updateAvatar(groupId, BitmapUtil.toByteArray(avatar));
        }

        public void updateAvatar(byte[] groupId, byte[] avatar)
        {
            ContentValues contentValues = new ContentValues();
            contentValues.put(AVATAR, avatar);

            databaseHelper.getWritableDatabase().update(TABLE_NAME, contentValues, GROUP_ID + " = ?",
                                                        new String[] { GroupUtil.getEncodedId(groupId) });

            RecipientFactory.clearCache();
            notifyDatabaseListeners();
        }

        public void updateMembers(byte[] id, List<String> members)
        {
            ContentValues contents = new ContentValues();
            contents.put(MEMBERS, Util.join(members, ","));
            contents.put(ACTIVE, 1);

            databaseHelper.getWritableDatabase().update(TABLE_NAME, contents, GROUP_ID + " = ?",
                                                        new String[] { GroupUtil.getEncodedId(id) });
        }

        public void remove(byte[] id, String source)
        {
            List<String> currentMembers = getCurrentMembers(id);
            currentMembers.remove(source);

            ContentValues contents = new ContentValues();
            contents.put(MEMBERS, Util.join(currentMembers, ","));

            databaseHelper.getWritableDatabase().update(TABLE_NAME, contents, GROUP_ID + " = ?",
                                                        new String[] { GroupUtil.getEncodedId(id) });
        }
        */
        private async Task<List<String>> getCurrentMembers(byte[] id)
        {

            try
            {
                var query = conn.Table<Group>().Where(v => v.group_id == GroupUtil.getEncodedId(id));


                if (query != null)
                {
                    return (query.First()).members.Split(',').ToList();
                }

                return new List<string>();
            }
            finally
            {

            }
        }
        /*
        public bool isActive(byte[] id)
        {
            GroupRecord record = getGroup(id);
            return record != null && record.isActive();
        }

        public void setActive(byte[] id, boolean active)
        {
            SQLiteDatabase database = databaseHelper.getWritableDatabase();
            ContentValues values = new ContentValues();
            values.put(ACTIVE, active ? 1 : 0);
            database.update(TABLE_NAME, values, GROUP_ID + " = ?", new String[] { GroupUtil.getEncodedId(id) });
        }


        public byte[] allocateGroupId()
        {
            try
            {
                byte[] groupId = new byte[16];
                var buffer = CryptographicBuffer.GenerateRandom(16);
                CryptographicBuffer.CopyToByteArray(buffer, out groupId);
                return groupId;
            }
            catch (Exception e)
            {
                throw new Exception(e);
            }
        }

        /*private void notifyDatabaseListeners()
        {
            Intent intent = new Intent(DATABASE_UPDATE_ACTION);
            context.sendBroadcast(intent);
        }*/

        public class Reader
        {

            private readonly IList<Group> groups;
            private IEnumerator<Group> cursor;

            public Reader(IList<Group> groups)
            {
                this.groups = groups;
                this.cursor = groups.GetEnumerator();
            }

            public GroupRecord getNext()
            {
                if (groups.Count() == 0 || !cursor.MoveNext())
                {
                    return null;
                }

                var group = cursor.Current;
                return new GroupRecord(group.group_id,
                                       group.title,
                                       group.members,
                                       group.avatar,
                                       group.avatar_id,
                                       group.avatar_key,
                                       group.avatar_content_type,
                                       group.avatar_relay,
                                       group.active == true);
            }

            public void close()
            {
                
            }
        }

        public class GroupRecord
        {

            private readonly String id;
            private readonly String title;
            private readonly List<String> members;
            private readonly byte[] avatar;
            private readonly long avatarId;
            private readonly byte[] avatarKey;
            private readonly String avatarContentType;
            private readonly String relay;
            private readonly bool active;

            public GroupRecord(String id, String title, String members, byte[] avatar,
                               long avatarId, byte[] avatarKey, String avatarContentType,
                               String relay, bool active)
            {
                this.id = id;
                this.title = title;
                this.members = members.Split(',').ToList();
                this.avatar = avatar;
                this.avatarId = avatarId;
                this.avatarKey = avatarKey;
                this.avatarContentType = avatarContentType;
                this.relay = relay;
                this.active = active;
            }

            public byte[] getId()
            {
                try
                {
                    return GroupUtil.getDecodedId(id);
                }
                catch (IOException ioe)
                {
                    throw new Exception(ioe.Message);
                }
            }

            public String getEncodedId()
            {
                return id;
            }

            public String getTitle()
            {
                return title;
            }

            public List<String> getMembers()
            {
                return members;
            }

            public byte[] getAvatar()
            {
                return avatar;
            }

            public long getAvatarId()
            {
                return avatarId;
            }

            public byte[] getAvatarKey()
            {
                return avatarKey;
            }

            public String getAvatarContentType()
            {
                return avatarContentType;
            }

            public String getRelay()
            {
                return relay;
            }

            public bool isActive()
            {
                return active;
            }
        }
    }
}
