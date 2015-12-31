

using GalaSoft.MvvmLight.Messaging;
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
using libaxolotl.util;
using Signal.Database.interfaces;
using Signal.Models;
using Signal.Util;
using Signal.ViewModel.Messages;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace Signal.Database { 
    public class ThreadDatabase : ThreadDatabaseHelper
    {

        /*public static final String CREATE_TABLE = "CREATE TABLE " + TABLE_NAME + " (" + ID + " INTEGER PRIMARY KEY, "                             +
    DATE + " INTEGER DEFAULT 0, " + MESSAGE_COUNT + " INTEGER DEFAULT 0, "                         +
    RECIPIENT_IDS + " TEXT, " + SNIPPET + " TEXT, " + SNIPPET_CHARSET + " INTEGER DEFAULT 0, "     +
    READ + " INTEGER DEFAULT 1, " + TYPE + " INTEGER DEFAULT 0, " + ERROR + " INTEGER DEFAULT 0, " +
    SNIPPET_TYPE + " INTEGER DEFAULT 0);";

        public static final String[] CREATE_INDEXS = {
            "CREATE INDEX IF NOT EXISTS thread_recipient_ids_index ON " + TABLE_NAME + " (" + RECIPIENT_IDS + ");",
  };*/
        SQLiteConnection conn;


        public ThreadDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Thread>();
        }

        public async Task<int> Count()
        {
            return conn.Table<Thread>().Count();
        }

        /*
         * CREATE
         */

        private long CreateForRecipients(String recipients, int recipientCount, int distributionType)
        {
            var reci = RecipientFactory.getRecipientsForIds(recipients, false);

            if (recipientCount > 1)
            {
                return CreateForRecipients(reci, distributionType);
            }

            return CreateForRecipients(reci, DistributionTypes.DEFAULT); // TODO: lala
        }

        private long CreateForRecipients(Recipients recipients, int distributionType)
        {
            var thread = new Thread()
            {
                Date = TimeUtil.GetDateTimeMillis(),
                Count = 0,
                Recipients = recipients,
                Type = distributionType
            };

            var rows = conn.Insert(thread);

            return thread.ThreadId;
        }

        /*
         * GET
         */

        public Thread Get(long threadId)
        {
            return conn.Get<Thread>(threadId);
        }

        public async Task<List<Thread>> GetAllAsync()
        {
            var query = conn.Table<Thread>().Where(v => true).OrderByDescending(v => v.Date);
            return query.ToList();
        }

        public long GetThreadIdForRecipients(Recipients recipients)
        {
            return GetThreadIdForRecipients(recipients, DistributionTypes.DEFAULT);
        }

        public long GetThreadIdForRecipients(Recipients recipients, int distributionType)
        {
            long[] recipientIds = getRecipientIds(recipients);
            String recipientsList = getRecipientsAsString(recipientIds);

            try
            {
                var query = conn.Table<Thread>().Where(t => t.RecipientIds == recipientsList); // use internal property
                var first = query.Count() == 0 ? null : query.First();

                if (query != null && first != null)
                    return (long)first.ThreadId;
                else
                    return CreateForRecipients(recipientsList, recipientIds.Length, distributionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /*
         * UPDATE
         */

        public void Update(long threadId, long count, string snippet, object attachment, DateTime date, long type)
        {
            var thread = Get(threadId);

            thread.Count = count;
            thread.Date = date;
            thread.Snippet = snippet;
            thread.SnippetType = type;
            //thread.SnippetUri

            conn.Update(thread);
        }

        public void UpdateSnippet(long threadId, string snippet, object attachment, DateTime date, long type)
        {
            var thread = Get(threadId);

            thread.Date = date;
            thread.Snippet = snippet;
            thread.SnippetType = type;
            //thread.SnippetUri

            conn.Update(thread);
        }

        /*
         * DELETE
         */

        public long Delete(long threadId)
        {
            return conn.Delete<Thread>(threadId);
        }

        public long DeleteConversation(long threadId)
        {
            DatabaseFactory.getMessageDatabase().DeleteThread(threadId);
            return Delete(threadId);
        }

        /*
         * REFRESH
         */

        public void Refresh(long threadId)
        {
            var lastMessage = DatabaseFactory.getMessageDatabase().GetConversationSnippet(threadId);
            Update(threadId, 0, lastMessage.Body, null, lastMessage.DateSent, lastMessage.Type);

            Messenger.Default.Send(new RefreshThreadMessage() { ThreadId = threadId });

        }





        /*
        private async Task UpdateThread(long threadId, long count, string body, long date, long type)
        {
            var thread = conn.Get<Thread>(threadId);

            thread.Date = DateTime.Now; // - (uint)date % 1000, // TODO: change
            thread.Count = count;
            thread.Snippet = body;
            thread.SnippetType = type;

            conn.Update(thread);

        }*/

        /*public async Task UpdateSnippet(long threadId, String snippet, long date, long type)
        {
            var thread = conn.Get<Thread>(threadId);

            thread.Date = DateTime.Now; //(uint)date - (uint)date % 1000,
            thread.Snippet = snippet;
            thread.SnippetType = type;

            conn.Update(thread);

        }*/


        /* private */
        public async Task DeleteAllThreads() /* done */
        {
            throw new NotImplementedException("ThreadDatabase DeleteAllThreads");
        }

        public void trimAllThreads(int length/*, ProgressListener listener*/)
        {
            throw new NotImplementedException("ThreadDatabase trimAllThreads");
        }

        public void trimThread(long threadId, int length)
        {
            throw new NotImplementedException("ThreadDatabase trimThread");
        }

        public async Task SetAllThreadsRead()
        {
            throw new NotImplementedException("ThreadDatabase setAllThreadsRead");
            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            ContentValues contentValues = new ContentValues(1);
            contentValues.put(READ, 1);*/

            var thread = new Thread() { Read = true }; // TODO: don't know how to update on all values-> create own sql

            //db.update(TABLE_NAME, contentValues, null, null);

            /*DatabaseFactory.getSmsDatabase(context).setAllMessagesRead();
            DatabaseFactory.getMmsDatabase(context).setAllMessagesRead();
            notifyConversationListListeners();*/
        }

        public async Task SetRead(long threadId)
        {
            var thread = conn.Get<Thread>(threadId);

            thread.Read = true;

            conn.Update(thread);
        }

        public async Task SetUnread(long threadId)
        {
            var thread = conn.Get<Thread>(threadId);

            thread.Read = false;

            conn.Update(thread);
        }
        /*
        public void setDistributionType(long threadId, int distributionType)
        {
            ContentValues contentValues = new ContentValues(1);
            contentValues.put(TYPE, distributionType);

            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.update(TABLE_NAME, contentValues, ID_WHERE, new String[] { threadId + "" });
            notifyConversationListListeners();
        }*/

        /*public Cursor getFilteredConversationList(List<String> filter)
        {
            if (filter == null || filter.size() == 0)
                return null;

            List<Long> rawRecipientIds = DatabaseFactory.getAddressDatabase(context).getCanonicalAddressIds(filter);

            if (rawRecipientIds == null || rawRecipientIds.size() == 0)
                return null;

            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            List<List<Long>> partitionedRecipientIds = Util.partition(rawRecipientIds, 900);
            List<Cursor> cursors = new LinkedList<>();

            for (List<Long> recipientIds : partitionedRecipientIds)
            {
                String selection = RECIPIENT_IDS + " = ?";
                String[] selectionArgs = new String[recipientIds.size()];

                for (int i = 0; i < recipientIds.size() - 1; i++)
                    selection += (" OR " + RECIPIENT_IDS + " = ?");

                int i = 0;
                for (long id : recipientIds)
                {
                    selectionArgs[i++] = String.valueOf(id);
                }

                cursors.add(db.query(TABLE_NAME, null, selection, selectionArgs, null, null, DATE + " DESC"));
            }

            Cursor cursor = cursors.size() > 1 ? new MergeCursor(cursors.toArray(new Cursor[cursors.size()])) : cursors.get(0);
            setNotifyConverationListListeners(cursor);
            return cursor;
        }*/

        /*public Cursor getConversationList()
        {
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            Cursor cursor = db.query(TABLE_NAME, null, null, null, null, null, DATE + " DESC");
            setNotifyConverationListListeners(cursor);
            return cursor;
        }*/

        /*public async Task<List<Thread>> getThreads()
        {
            var query = conn.Table<Thread>().Where(v => true);

            List < Thread > list = new List<Thread>();

            foreach (var thread in await query.ToList())
            {
                var t = new Thread()
                {
                    ThreadId = thread._id.Value,
                    Date = thread.date,
                    Count = thread.message_count,
                    Read = thread.read == 1,
                    SnippetType = thread.snippet_type,
                    Recipients = RecipientFactory.getRecipientsForIds(thread.recipient_ids, true),
                    Body = thread.snippet
                };

                list.Add(t);
            }

            return list;
        }*/







        public async void deleteAllConversations()
        {
            /*DatabaseFactory.getSmsDatabase(context).deleteAllThreads();
            DatabaseFactory.getMmsDatabase(context).deleteAllThreads();
            DatabaseFactory.getDraftDatabase(context).clearAllDrafts();*/
            await DeleteAllThreads();
        }

        public async Task<long> getThreadIdIfExistsFor(Recipients recipients)
        {
            long[] recipientIds = getRecipientIds(recipients);
            String recipientsList = getRecipientsAsString(recipientIds);

            try
            {
                var query = conn.Table<Thread>().Where(t => t.Recipients == recipients);
                var first = query.First();
                //cursor = db.query(TABLE_NAME, new String[] { ID }, where, recipientsArg, null, null, null);

                if (query != null && first != null)
                    return (long)first.ThreadId;
                else
                    return -1L;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                // if (cursor != null)
                //    cursor.close();
            }
        }





        public async Task<Recipients> getRecipientsForThreadId(long threadId)
        {


            try
            {
                var thread = conn.GetWithChildren<Thread>(threadId);

                return thread.Recipients;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return null;
        }

        /*public async Task<bool> update(long threadId)
        {

            //throw new NotImplementedException("update");
            MessageDatabase messageDatabase = DatabaseFactory.getMessageDatabase();
            long count = await messageDatabase.getConversationCount(threadId);

            if (count == 0)
            {
                await DeleteThread(threadId);
                //notifyConversationListListeners();
                return true;
            }

            return true;*/
            // TODO: update thread
            /*try
            {
                messageDatabase.getConversationSnippet(threadId)

                reader = messageDatabase.readerFor();
                MessageRecord record;

                if (reader != null && (record = reader.getNext()) != null)
                {
                    long timestamp;

                    if (record.isPush()) timestamp = record.getDateSent();
                    else timestamp = record.getDateReceived();

                    await updateThread(threadId, count, record.getBody().getBody(), timestamp, record.getType());
                    //notifyConversationListListeners();
                    return false;
                }
                else
                {
                    await deleteThread(threadId);
                    //notifyConversationListListeners();
                    return true;
                }
            }*/
            /*finally
            {
                if (reader != null)
                    reader.close();
            }*/
        //}


        /*public static interface ProgressListener
        {
            public void onProgress(int complete, int total);
        }

        public Reader readerFor(Cursor cursor, MasterCipher masterCipher)
        {
            return new Reader(cursor, masterCipher);
        }*/

        public static class DistributionTypes
        {
            public const int DEFAULT = 2;
            public const int BROADCAST = 1;
            public const int CONVERSATION = 2;
        }
        /*
        public class Reader
        {

            private final Cursor       cursor;
        private final MasterCipher masterCipher;

        public Reader(Cursor cursor, MasterCipher masterCipher)
            {
                this.cursor = cursor;
                this.masterCipher = masterCipher;
            }

            public ThreadRecord getNext()
            {
                if (cursor == null || !cursor.moveToNext())
                    return null;

                return getCurrent();
            }

            public ThreadRecord getCurrent()
            {
                long threadId = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.ID));
                String recipientId = cursor.getString(cursor.getColumnIndexOrThrow(ThreadDatabase.RECIPIENT_IDS));
                Recipients recipients = RecipientFactory.getRecipientsForIds(context, recipientId, true);

                DisplayRecord.Body body = getPlaintextBody(cursor);
                long date = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.DATE));
                long count = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.MESSAGE_COUNT));
                long read = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.READ));
                long type = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.SNIPPET_TYPE));
                int distributionType = cursor.getInt(cursor.getColumnIndexOrThrow(ThreadDatabase.TYPE));

                return new ThreadRecord(context, body, recipients, date, count,
                                        read == 1, threadId, type, distributionType);
            }

            private DisplayRecord.Body getPlaintextBody(Cursor cursor)
            {
                try
                {
                    long type = cursor.getLong(cursor.getColumnIndexOrThrow(ThreadDatabase.SNIPPET_TYPE));
                    String body = cursor.getString(cursor.getColumnIndexOrThrow(SNIPPET));

                    if (!TextUtils.isEmpty(body) && masterCipher != null && MmsSmsColumns.Types.isSymmetricEncryption(type))
                    {
                        return new DisplayRecord.Body(masterCipher.decryptBody(body), true);
                    }
                    else if (!TextUtils.isEmpty(body) && masterCipher == null && MmsSmsColumns.Types.isSymmetricEncryption(type))
                    {
                        return new DisplayRecord.Body(body, false);
                    }
                    else
                    {
                        return new DisplayRecord.Body(body, true);
                    }
                }
                catch (InvalidMessageException e)
                {
                    Log.w("ThreadDatabase", e);
                    return new DisplayRecord.Body(context.getString(R.string.ThreadDatabase_error_decrypting_message), true);
                }
            }

            public void close()
            {
                cursor.close();
            }
        }*/
    }
}
