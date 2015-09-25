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
using Signal.Model;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace TextSecure.database
{
    public class ThreadDatabase : ThreadDatabaseHelper, IThreadDatabase
    {

        const String TABLE_NAME = "thread";
        public static readonly String ID = "_id";
        public static readonly String DATE = "date";
        public static readonly String MESSAGE_COUNT = "message_count";
        public static readonly String RECIPIENT_IDS = "recipient_ids";
        public static readonly String SNIPPET = "snippet";
        private static readonly String SNIPPET_CHARSET = "snippet_cs";
        public static readonly String READ = "read";
        private static readonly String TYPE = "type";
        private static readonly String ERROR = "error";
        private static readonly String HAS_ATTACHMENT = "has_attachment";
        public static readonly String SNIPPET_TYPE = "snippet_type";

        [Table(TABLE_NAME)]
        public class ThreadTable
        {
            [PrimaryKey, AutoIncrement]
            public long? _id { get; set; } = null;
            public DateTime date { get; set; } = DateTime.MinValue;
            public long message_count { get; set; } = 0;
            public string recipient_ids { get; set; }
            public string snippet { get; set; }
            public long snippet_cs { get; set; } = 0;
            public long read { get; set; } = 1;
            public long type { get; set; } = 1;
            public long error { get; set; } = 1;
            public long snippet_type { get; set; } = 1;
        }

        /*public static final String CREATE_TABLE = "CREATE TABLE " + TABLE_NAME + " (" + ID + " INTEGER PRIMARY KEY, "                             +
    DATE + " INTEGER DEFAULT 0, " + MESSAGE_COUNT + " INTEGER DEFAULT 0, "                         +
    RECIPIENT_IDS + " TEXT, " + SNIPPET + " TEXT, " + SNIPPET_CHARSET + " INTEGER DEFAULT 0, "     +
    READ + " INTEGER DEFAULT 1, " + TYPE + " INTEGER DEFAULT 0, " + ERROR + " INTEGER DEFAULT 0, " +
    SNIPPET_TYPE + " INTEGER DEFAULT 0);";

        public static final String[] CREATE_INDEXS = {
            "CREATE INDEX IF NOT EXISTS thread_recipient_ids_index ON " + TABLE_NAME + " (" + RECIPIENT_IDS + ");",
  };*/
        SQLiteConnection conn;


        public ThreadDatabase(SQLiteConnection connection)
        {
            conn = connection;
            

            conn.CreateTable<ThreadTable>();
/*
#if DEBUG
            if (conn.Table<Thread>().Count() == 0) // populate demo set
            {
                var thread1 = new Thread() { recipient_ids = "Tester", snippet = "Hallo wierdo!", read = 0, message_count = 2 };
                var thread2 = new Thread() { recipient_ids = "Klaus", snippet = "Wohooo!", read = 1, message_count = 0 };
                conn.Insert(thread1);
                conn.Insert(thread2);
            }
#endif*/
        }

        public async Task<int> Count()
        {
            return conn.Table<ThreadTable>().Count();
        }

        

        private async Task<long> createThreadForRecipients(String recipients, int recipientCount, int distributionType) /* done */
        {
            var thread = new ThreadTable()
            {
                date = DateTime.Now,
                recipient_ids = recipients,
                message_count = 0
            };

            if (recipientCount > 1)
            {
                thread.type = distributionType;
            }

            return conn.Insert(thread);
        }

        private async Task UpdateThread(long threadId, long count, string body, long date, long type) /* done */
        {
            var thread = new ThreadTable()
            {
                _id = threadId,
                date = DateTime.Now, // - (uint)date % 1000, // TODO: change
                message_count = count,
                snippet = body,
                snippet_type = type
            };

             conn.Update(thread);

            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.update(TABLE_NAME, contentValues, ID + " = ?", new String[] { threadId + "" });
            notifyConversationListListeners();*/
        }

        public async Task UpdateSnippet(long threadId, String snippet, long date, long type) /* done */
        {
            var thread = new ThreadTable()
            {
                _id = threadId,
                date = DateTime.Now, //(uint)date - (uint)date % 1000,
                snippet = snippet,
                snippet_type = type
            };

            conn.Update(thread);

            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.update(TABLE_NAME, contentValues, ID + " = ?", new String[] { threadId + "" });
            notifyConversationListListeners();*/
        }

         /* private */ public async Task DeleteThread(long threadId) /* done */
        {
            conn.Delete(new ThreadTable() { _id = threadId});
            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.delete(TABLE_NAME, ID_WHERE, new String[] { threadId + "" });
            notifyConversationListListeners();*/
        }

        /* private */ public async Task DeleteThreads(ICollection<long> threadIds) /* done */
        {
            throw new NotImplementedException("ThreadDatabase deleteThreads");
            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            String where = "";

            for (long threadId : threadIds)
            {
                where += ID + " = '" + threadId + "' OR ";
            }

            where = where.substring(0, where.length() - 4);

            db.delete(TABLE_NAME, where, null);
            notifyConversationListListeners();*/
        }

        /* private */ public async Task DeleteAllThreads() /* done */
        {
            conn.Delete(new ThreadTable() { });

            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.delete(TABLE_NAME, null, null);
            notifyConversationListListeners();*/
        }

        public void trimAllThreads(int length/*, ProgressListener listener*/)
        {
            throw new NotImplementedException("ThreadDatabase trimAllThreads");
            /*Cursor cursor = null;
            int threadCount = 0;
            int complete = 0;

            try
            {
                cursor = this.getConversationList();

                if (cursor != null)
                    threadCount = cursor.getCount();

                while (cursor != null && cursor.moveToNext())
                {
                    long threadId = cursor.getLong(cursor.getColumnIndexOrThrow(ID));
                    trimThread(threadId, length);

                    listener.onProgress(++complete, threadCount);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }*/
        }

        public void trimThread(long threadId, int length)
        {
            throw new NotImplementedException("ThreadDatabase trimThread");
            /*Log.w("ThreadDatabase", "Trimming thread: " + threadId + " to: " + length);
            Cursor cursor = null;

            try
            {
                cursor = DatabaseFactory.getMmsSmsDatabase(context).getConversation(threadId);

                if (cursor != null && cursor.getCount() > length)
                {
                    Log.w("ThreadDatabase", "Cursor count is greater than length!");
                    cursor.moveToPosition(cursor.getCount() - length);

                    long lastTweetDate = cursor.getLong(cursor.getColumnIndexOrThrow(MmsSmsColumns.NORMALIZED_DATE_RECEIVED));

                    Log.w("ThreadDatabase", "Cut off tweet date: " + lastTweetDate);

                    DatabaseFactory.getSmsDatabase(context).deleteMessagesInThreadBeforeDate(threadId, lastTweetDate);
                    DatabaseFactory.getMmsDatabase(context).deleteMessagesInThreadBeforeDate(threadId, lastTweetDate);

                    update(threadId);
                    notifyConversationListeners(threadId);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.close();
            }*/
        }

        public async Task SetAllThreadsRead()
        {
            throw new NotImplementedException("ThreadDatabase setAllThreadsRead");
            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            ContentValues contentValues = new ContentValues(1);
            contentValues.put(READ, 1);*/

            var thread = new ThreadTable() { read = 1 }; // TODO: don't know how to update on all values-> create own sql

            //db.update(TABLE_NAME, contentValues, null, null);

            /*DatabaseFactory.getSmsDatabase(context).setAllMessagesRead();
            DatabaseFactory.getMmsDatabase(context).setAllMessagesRead();
            notifyConversationListListeners();*/
        }

        public async Task SetRead(long threadId)
        {
            /*ContentValues contentValues = new ContentValues(1);
            contentValues.put(READ, 1);

            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.update(TABLE_NAME, contentValues, ID_WHERE, new String[] { threadId + "" });

            DatabaseFactory.getSmsDatabase(context).setMessagesRead(threadId);
            DatabaseFactory.getMmsDatabase(context).setMessagesRead(threadId);
            notifyConversationListListeners();*/

            var query = conn.Table<ThreadTable>().Where(t => t._id == threadId);
            var thread = query.First();

            thread.read = 1;

            conn.Update(thread);

            /*DatabaseFactory.getSmsDatabase().setMessagesRead(threadId); TODO: enable again
            DatabaseFactory.getMmsDatabase().setMessagesRead(threadId);*/
        }

        public async Task SetUnread(long threadId)
        {
            /*ContentValues contentValues = new ContentValues(1);
            contentValues.put(READ, 0);*/

            var query = conn.Table<ThreadTable>().Where(t => t._id == threadId);
            var thread = query.First();

            thread.read = 0;

            conn.Update(thread);

            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.update(TABLE_NAME, contentValues, ID_WHERE, new String[] { threadId + "" });*/
            //notifyConversationListListeners();
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
            var query = conn.Table<ThreadTable>().Where(v => true);

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

        public async Task<List<ThreadTable>> getConversationList()
        {
            var query = conn.Table<ThreadTable>().Where(v => true);

            return query.ToList();
            throw new NotImplementedException("ThreadDatabase getConversationList");
            /*SQLiteDatabase db = databaseHelper.getReadableDatabase();
            Cursor cursor = db.query(TABLE_NAME, null, null, null, null, null, DATE + " DESC");
            setNotifyConverationListListeners(cursor);*/
        }


        public async void deleteConversation(long threadId)
        {
            /*DatabaseFactory.getSmsDatabase(context).deleteThread(threadId);
            DatabaseFactory.getMmsDatabase(context).deleteThread(threadId);
            DatabaseFactory.getDraftDatabase(context).clearDrafts(threadId);*/
            await DeleteThread(threadId);
            /*notifyConversationListeners(threadId);
            notifyConversationListListeners();*/
        }


        public void deleteConversations(IList<long> selectedConversations)
        {
            /*DatabaseFactory.getSmsDatabase(context).deleteThreads(selectedConversations);
            DatabaseFactory.getMmsDatabase(context).deleteThreads(selectedConversations);
            DatabaseFactory.getDraftDatabase(context).clearDrafts(selectedConversations);*/
            DeleteThreads(selectedConversations);
            /*notifyConversationListeners(selectedConversations);
            notifyConversationListListeners();*/
        }

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
                var query = conn.Table<ThreadTable>().Where(t => t.recipient_ids == recipientsList);
                var first = query.First();
                //cursor = db.query(TABLE_NAME, new String[] { ID }, where, recipientsArg, null, null, null);

                if (query != null && first != null)
                    return (long)first._id;
                else
                    return -1L;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
               // if (cursor != null)
                //    cursor.close();
            }
        }

        public async Task<long> GetThreadIdFor(Recipients recipients)
        {
            return await GetThreadIdFor(recipients, DistributionTypes.DEFAULT);
        }

        public async Task<long> GetThreadIdFor(Recipients recipients, int distributionType)
        {
            long[] recipientIds = getRecipientIds(recipients);
            String recipientsList = getRecipientsAsString(recipientIds);


            try
            {
                //cursor = db.query(TABLE_NAME, new String[] { ID }, where, recipientsArg, null, null, null);
                var query = conn.Table<ThreadTable>().Where(t => t.recipient_ids == recipientsList);
                var first = query.Count() == 0 ? null : query.First();

                if (query != null && first != null)
                    return (long)first._id;
                else
                    return await createThreadForRecipients(recipientsList, recipientIds.Length, distributionType);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
 
            }
        }

        public async Task<Recipients> getRecipientsForThreadId(long threadId)
        {


            try
            {
                var query = conn.Table<ThreadTable>().Where(t => t._id == threadId);
                var first = query.First();
                var recipientIds = first.recipient_ids;


                if (query != null && first != null)
                {
                    
                    return RecipientFactory.getRecipientsForIds(recipientIds, false);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                //if (cursor != null)
                 //   cursor.close();
            }

            return null;
        }

        public async Task<bool> update(long threadId)
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

            return true;
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
        }


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
