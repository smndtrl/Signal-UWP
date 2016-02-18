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
using Signal.Models;
using Signal.Util;
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
using libaxolotl;
using Signal.Messages;
using TextSecure.recipient;
using TextSecure.util;

namespace Signal.Database
{
    public class MessageDatabase : Database
    {


        protected SQLiteConnection conn;

        public MessageDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Message>();
        }


        public Message GetConversationSnippet(long threadId)
        {
            return conn.Table<Message>().Where(m => m.ThreadId == threadId).OrderByDescending(m => m.DateReceived).Take(1).First();
        }











        /*
         * Thread related
         */

        public void DeleteThread(long threadId)
        {
            conn.Table<Message>().Delete(message => message.ThreadId == threadId);
        }

        public async Task<int> Count()
        {
            return conn.Table<Message>().Count();
        }

        public async Task<List<Message>> getMessageList(long thread_id)
        {
            var query = conn.Table<Message>().Where(v => true);

            return query.ToList();
            /*SQLiteDatabase db = databaseHelper.getReadableDatabase();
            Cursor cursor = db.query(TABLE_NAME, null, null, null, null, null, DATE + " DESC");
            setNotifyConverationListListeners(cursor);*/
        }

        

        /*public async Task Test(long messageId)
        {
            Debug.WriteLine($"MessageDatabase: Testing message: {messageId}");

            var message1 = conn.Get<Message>(messageId);
            message1.Type += 1;
            conn.Update(message1);
            message1.PropertyChanged += (s, e) => { Debug.WriteLine("message 1 property changed"); };


            var message2 = conn.Get<Message>(messageId);
            message2.Type -= 1;
            conn.Update(message2);
            message1.PropertyChanged += (s, e) => { Debug.WriteLine("message 2 property changed"); };


            DatabaseFactory.getThreadDatabase().Refresh(message.ThreadId);
        }*/

        public async Task<long> getThreadIdForMessage(long messageId)
        {

            var message = conn.Get<Message>(messageId);

            if (message != null)
                return message.ThreadId;
            else
                return -1;

        }

        public async Task<int> getMessageCount()
        {

            try
            {
                var query = conn.Table<Message>().Where(t => true);

                if (query != null) return query.Count();
                else return 0;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }
        }

        public int getMessagesCount(long threadId)
        {

            try
            {
                var query = conn.Table<Message>().Where(t => t.ThreadId == threadId);

                if (query != null) return query.Count();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }

            return 0;
        }

        public int getMessageCountForThread(long threadId)
        {
            return getMessagesCount(threadId);
        }

        

        public void incrementDeliveryReceiptCount(String address, long timestamp)
        {
            var date = TimeUtil.GetDateTime(timestamp);
            var query = conn.Table<Message>().Where(m =>
                m.DateSent == date
            );

            if (query.Count() > 0)
            {
                var message = query.First();

                if (MessageTypes.isOutgoingMessageType(message.Type))
                {
                    String theirAddress = Utils.canonicalizeNumber(address);
                    String ourAddress = Utils.canonicalizeNumber(message.Address);

                    if (ourAddress.Equals(theirAddress))
                    {
                        message.ReceiptCount += 1;

                        conn.Update(message);
                    }
                }
            }
        }

        public async void setMessagesRead(long threadId)
        {
            var message = conn.Get<Message>(threadId);

            message.Read = true;

            conn.Update(message);
        }

        public void setAllMessagesRead()
        {
            /*SQLiteDatabase database = databaseHelper.getWritableDatabase();
            ContentValues contentValues = new ContentValues();
            contentValues.put(READ, 1);

            database.update(TABLE_NAME, contentValues, null, null);*/
            throw new NotImplementedException("MessageDatabase setallMessagesRead");
        }

        /*protected Pair<long, long> updateMessageBodyAndType(long messageId, String body, long maskOff, long maskOn)
        {
            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            db.execSQL("UPDATE " + TABLE_NAME + " SET " + BODY + " = ?, " +
                           TYPE + " = (" + TYPE + " & " + (MessageTypes.TOTAL_MASK - maskOff) + " | " + maskOn + ") " +
                           "WHERE " + ID + " = ?",
                       new String[] { body, messageId + "" });

            long threadId = getThreadIdForMessage(messageId);

            DatabaseFactory.getThreadDatabase().update(threadId);
            notifyConversationListeners(threadId);
            //notifyConversationListListeners();

            return new Pair<long, long>(messageId, threadId);
        }
        */
        
        
        
        /*
        protected long insertMessageOutbox(long threadId, OutgoingTextMessage message,
                                           long type, boolean forceSms, long date)
        {
            if (message.isKeyExchange()) type |= MessageTypes.KEY_EXCHANGE_BIT;
            else if (message.isSecureMessage()) type |= MessageTypes.SECURE_MESSAGE_BIT;
            else if (message.isEndSession()) type |= MessageTypes.END_SESSION_BIT;
            if (forceSms) type |= MessageTypes.MESSAGE_FORCE_SMS_BIT;

            ContentValues contentValues = new ContentValues(6);
            contentValues.put(ADDRESS, PhoneNumberUtils.formatNumber(message.getRecipients().getPrimaryRecipient().getNumber()));
            contentValues.put(THREAD_ID, threadId);
            contentValues.put(BODY, message.getMessageBody());
            contentValues.put(DATE_RECEIVED, date);
            contentValues.put(DATE_SENT, date);
            contentValues.put(READ, 1);
            contentValues.put(TYPE, type);

            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            long messageId = db.insert(TABLE_NAME, ADDRESS, contentValues);

            DatabaseFactory.getThreadDatabase(context).update(threadId);
            notifyConversationListeners(threadId);
            jobManager.add(new TrimThreadJob(context, threadId));

            return messageId;
        }

        Cursor getMessages(int skip, int limit)
        {
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            return db.query(TABLE_NAME, MESSAGE_PROJECTION, null, null, null, null, ID, skip + "," + limit);
        }

        Cursor getOutgoingMessages()
        {
            String outgoingSelection = TYPE + " & " + MessageTypes.BASE_TYPE_MASK + " = " + MessageTypes.BASE_OUTBOX_TYPE;
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            return db.query(TABLE_NAME, MESSAGE_PROJECTION, outgoingSelection, null, null, null, null);
        }

        public Cursor getDecryptInProgressMessages()
        {
            String where = TYPE + " & " + (MessageTypes.ENCRYPTION_REMOTE_BIT | MessageTypes.ENCRYPTION_ASYMMETRIC_BIT) + " != 0";
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            return db.query(TABLE_NAME, MESSAGE_PROJECTION, where, null, null, null, null);
        }

        public Cursor getEncryptedRogueMessages(Recipient recipient)
        {
            String selection = TYPE + " & " + MessageTypes.ENCRYPTION_REMOTE_NO_SESSION_BIT + " != 0" +
                                " AND PHONE_NUMBERS_EQUAL(" + ADDRESS + ", ?)";
            String[] args = { recipient.getNumber() };
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            return db.query(TABLE_NAME, MESSAGE_PROJECTION, selection, args, null, null, null);
        }

        public Cursor getMessage(long messageId)
        {
            SQLiteDatabase db = databaseHelper.getReadableDatabase();
            Cursor cursor = db.query(TABLE_NAME, MESSAGE_PROJECTION, ID_WHERE, new String[] { messageId + "" },
                                             null, null, null);
            setNotifyConverationListeners(cursor, getThreadIdForMessage(messageId));
            return cursor;
        }

        public boolean deleteMessage(long messageId)
        {
            Log.w("MessageDatabase", "Deleting: " + messageId);
            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            long threadId = getThreadIdForMessage(messageId);
            db.delete(TABLE_NAME, ID_WHERE, new String[] { messageId + "" });
            boolean threadDeleted = DatabaseFactory.getThreadDatabase(context).update(threadId);
            notifyConversationListeners(threadId);
            return threadDeleted;
        }*/

        /*package */
        async void deleteThread(long threadId)
        {
            throw new NotImplementedException("MessageDatabase deleteThread");

            //conn.Delete(new Message() { thread_id = threadId });
        }

        /*package*/
        void deleteMessagesInThreadBeforeDate(long threadId, long date)
        {
            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            String where = THREAD_ID + " = ? AND (CASE " + TYPE;

            for (long outgoingType : MessageTypes.OUTGOING_MESSAGE_MessageTypes)
            {
                where += " WHEN " + outgoingType + " THEN " + DATE_SENT + " < " + date;
            }

            where += (" ELSE " + DATE_RECEIVED + " < " + date + " END)");

            db.delete(TABLE_NAME, where, new String[] { threadId + "" });*/
            throw new NotImplementedException("MessageDatabase deleteMessagesInThreadBeforeDate");
        }

        /*package*/
        void deleteThreads(IList<long> threadIds)
        {
            throw new NotImplementedException("MessageDatabase deleteThreads");

            /*foreach (var threadId in threadIds)
            {
                conn.Delete(new Message() { thread_id = threadId });
            }*/

            /*SQLiteDatabase db = databaseHelper.getWritableDatabase();
            String where = "";

            for (long threadId : threadIds)
            {
                where += THREAD_ID + " = '" + threadId + "' OR ";
            }

            where = where.substring(0, where.length() - 4);

            db.delete(TABLE_NAME, where, null);*/
        }

        /*package */
        async void deleteAllThreads()
        {
            throw new NotImplementedException("MessageDatabase deleteAllThreads");
        }

        /*
         * Inbox/Outbox
         */
        public long InsertMessageOutbox(long threadId, OutgoingTextMessage message,
                                     long type, DateTime date)
        {
            if (message.IsKeyExchange) type |= MessageTypes.KEY_EXCHANGE_BIT;
            else if (message.IsSecureMessage) type |= MessageTypes.SECURE_MESSAGE_BIT;
            else if (message.IsEndSession) type |= MessageTypes.END_SESSION_BIT;
            //if (forceSms) type |= MessageTypes.MESSAGE_FORCE_SMS_BIT;

            /*ContentValues contentValues = new ContentValues(6);
            contentValues.put(ADDRESS, PhoneNumberUtils.formatNumber(message.getRecipients().getPrimaryRecipient().getNumber()));
            contentValues.put(THREAD_ID, threadId);
            contentValues.put(BODY, message.getMessageBody());
            contentValues.put(DATE_RECEIVED, date);
            contentValues.put(DATE_SENT, date);
            contentValues.put(READ, 1);
            contentValues.put(TYPE, type);*/

            var insert = new Message()
            {
                //address = PhoneNumberUtils.formatNumber(message.getRecipients().getPrimaryRecipient().getNumber()),
                Address = message.Recipients.getPrimaryRecipient().getNumber(),
                ThreadId = threadId,
                Body = message.MessageBody,
                DateReceived = TimeUtil.GetDateTimeMillis(),
                DateSent = date,
                Read = true,
                Type = type
            };

            // TODO: ReceiptCount https://github.com/WhisperSystems/Signal-Android/blob/e9b53cc164d7ae2d838cc211dbd88b7fd4f5669e/src/org/thoughtcrime/securesms/database/SmsDatabase.java

            long messageId = conn.Insert(insert);

            DatabaseFactory.getThreadDatabase().Update(threadId);
            notifyConversationListeners(threadId);

            return insert.MessageId;
        }




        public async Task<int> getConversationCount(long threadId)
        {
            /*int count = DatabaseFactory.getTextMessagingDatabase().getMessageCountForThread(threadId);
            count += DatabaseFactory.getMediaMessagingDatabase().getMessageCountForThread(threadId);*/


            return getMessageCountForThread(threadId);
        }




















        /*package*/
        /*SQLiteDatabase beginTransaction()
        {
            SQLiteDatabase database = databaseHelper.getWritableDatabase();
            database.beginTransaction();
            return database;
        }*/

        /*package*/
        /*void endTransaction(SQLiteDatabase database)
        {
            database.setTransactionSuccessful();
            database.endTransaction();
        }*/

        /*package*/
        /*SQLiteStatement createInsertStatement(SQLiteDatabase database)
        {
            return database.compileStatement("INSERT INTO " + TABLE_NAME + " (" + ADDRESS + ", " +
                                                                              PERSON + ", " +
                                                                              DATE_SENT + ", " +
                                                                              DATE_RECEIVED + ", " +
                                                                              PROTOCOL + ", " +
                                                                              READ + ", " +
                                                                              STATUS + ", " +
                                                                              TYPE + ", " +
                                                                              REPLY_PATH_PRESENT + ", " +
                                                                              SUBJECT + ", " +
                                                                              BODY + ", " +
                                                                              SERVICE_CENTER +
                                                                              ", " + THREAD_ID + ") " +
                                             " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
        }
        */
        public static class Status
        {
            public static readonly int STATUS_NONE = -1;
            public static readonly int STATUS_COMPLETE = 0;
            public static readonly int STATUS_PENDING = 0x20;
            public static readonly int STATUS_FAILED = 0x40;
        }
        /*
        public Reader readerFor(Cursor cursor)
        {
            return new Reader(cursor);
        }*/

        /*public class Reader
        {

            private readonly Cursor cursor;

            public Reader(Cursor cursor)
            {
                this.cursor = cursor;
            }

            public SmsMessageRecord getNext()
            {
                if (cursor == null || !cursor.moveToNext())
                    return null;

                return getCurrent();
            }

            public int getCount()
            {
                if (cursor == null) return 0;
                else return cursor.getCount();
            }

            public SmsMessageRecord getCurrent()
            {
                long messageId = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.ID));
                String address = cursor.getString(cursor.getColumnIndexOrThrow(SmsDatabase.ADDRESS));
                int addressDeviceId = cursor.getInt(cursor.getColumnIndexOrThrow(SmsDatabase.ADDRESS_DEVICE_ID));
                long type = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.TYPE));
                long dateReceived = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.NORMALIZED_DATE_RECEIVED));
                long dateSent = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.NORMALIZED_DATE_SENT));
                long threadId = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.THREAD_ID));
                int status = cursor.getInt(cursor.getColumnIndexOrThrow(SmsDatabase.STATUS));
                int receiptCount = cursor.getInt(cursor.getColumnIndexOrThrow(SmsDatabase.RECEIPT_COUNT));
                String mismatchDocument = cursor.getString(cursor.getColumnIndexOrThrow(SmsDatabase.MISMATCHED_IDENTITIES));

                List<IdentityKeyMismatch> mismatches = getMismatches(mismatchDocument);
                Recipients recipients = getRecipientsFor(address);
                DisplayRecord.Body body = getBody(cursor);

                return new SmsMessageRecord(context, messageId, body, recipients,
                                            recipients.getPrimaryRecipient(),
                                            addressDeviceId,
                                            dateSent, dateReceived, receiptCount, type,
                                            threadId, status, mismatches);
            }

            private Recipients getRecipientsFor(String address)
            {
                if (address != null)
                {
                    Recipients recipients = RecipientFactory.getRecipientsFromString(context, address, false);

                    if (recipients == null || recipients.isEmpty())
                    {
                        return new Recipients(Recipient.getUnknownRecipient(context));
                    }

                    return recipients;
                }
                else
                {
                    Log.w(TAG, "getRecipientsFor() address is null");
                    return new Recipients(Recipient.getUnknownRecipient(context));
                }
            }

            private List<IdentityKeyMismatch> getMismatches(String document)
            {
                try
                {
                    if (!TextUtils.isEmpty(document))
                    {
                        return JsonUtils.fromJson(document, IdentityKeyMismatchList.class).getList();
        }
    } catch (IOException e) {
        Log.w(TAG, e);
      }

      return new LinkedList<>();
    }

    protected DisplayRecord.Body getBody(Cursor cursor)
{
    long type = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.TYPE));
    String body = cursor.getString(cursor.getColumnIndexOrThrow(SmsDatabase.BODY));

    if (MessageTypes.isSymmetricEncryption(type))
    {
        return new DisplayRecord.Body(body, false);
    }
    else
    {
        return new DisplayRecord.Body(body, true);
    }
}

public void close()
{
    cursor.close();
}
  }
}*/

        public void SetMismatchedIdentity(long messageId, long recipientId, IdentityKey identityKey)
        {
            List<IdentityKeyMismatch> items = new List<IdentityKeyMismatch>()
            {
                new IdentityKeyMismatch(recipientId, identityKey)
            };

            /*IdentityKeyMismatchList document = new IdentityKeyMismatchList(items);

            SQLiteDatabase database = databaseHelper.getWritableDatabase();
            database.beginTransaction();

            try {
              setDocument(database, messageId, MISMATCHED_IDENTITIES, document);

            database.setTransactionSuccessful();
            } catch (IOException ioe) {
              Log.w(TAG, ioe);
            } finally {
              database.endTransaction();
            }*/
        }

        public void AddMismatchedIdentity(long messageId, long recipientId, IdentityKey identityKey)
        {
            /*try
            {
                addToDocument(messageId, MISMATCHED_IDENTITIES,
                              new DisplayRecord.IdentityKeyMismatch(recipientId, identityKey),
                              IdentityKeyMismatchList.class);
    } catch (IOException e) {
      Log.w(TAG, e);
    }*/
        }

        public void RemoveMismatchedIdentity(long messageId, long recipientId, IdentityKey identityKey)
        {
            /*try
            {
                removeFromDocument(messageId, MISMATCHED_IDENTITIES,
                                   new DisplayRecord.IdentityKeyMismatch(recipientId, identityKey),
                                   IdentityKeyMismatchList.class);
    } catch (IOException e) {
      Log.w(TAG, e);
    }
        }*/
        }
    }
}
