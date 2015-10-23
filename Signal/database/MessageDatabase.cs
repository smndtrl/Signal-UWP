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
using TextSecure.messages;
using TextSecure.recipient;
using TextSecure.util;

namespace TextSecure.database
{
    public class MessageDatabase : MessageTypes
    {


        protected SQLiteConnection conn;

        public MessageDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Message>();
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

        private async Task updateTypeBitmask(long messageId, long maskOff, long maskOn)
        {
            Debug.WriteLine($"MessageDatabase: Updating ID: {messageId} to base type: {maskOn}");

            var message = conn.Get<Message>(messageId);
            message.PropertyChanged += (s, e) => { Debug.WriteLine("property changed"); };

            message.Type = (MessageTypes.TOTAL_MASK - maskOff) | maskOn;
            Debug.WriteLine("before save");
            conn.Update(message);
            Debug.WriteLine("after save");

            DatabaseFactory.getThreadDatabase().Refresh(message.ThreadId);
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

        public async Task<int> getMessageCountForThread(long threadId)
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

        public void markAsEndSession(long id)
        {
            updateTypeBitmask(id, MessageTypes.KEY_EXCHANGE_MASK, MessageTypes.END_SESSION_BIT);
        }

        public void markAsPreKeyBundle(long id)
        {
            updateTypeBitmask(id, MessageTypes.KEY_EXCHANGE_MASK, MessageTypes.KEY_EXCHANGE_BIT | MessageTypes.KEY_EXCHANGE_BUNDLE_BIT);
        }

        public void markAsInvalidVersionKeyExchange(long id)
        {
            updateTypeBitmask(id, 0, MessageTypes.KEY_EXCHANGE_INVALID_VERSION_BIT);
        }

        public void markAsSecure(long id)
        {
            updateTypeBitmask(id, 0, MessageTypes.SECURE_MESSAGE_BIT);
        }

        public void markAsInsecure(long id)
        {
            updateTypeBitmask(id, MessageTypes.SECURE_MESSAGE_BIT, 0);
        }

        public void markAsPush(long id)
        {
            updateTypeBitmask(id, 0, MessageTypes.PUSH_MESSAGE_BIT);
        }

        public void markAsForcedSms(long id)
        {
            updateTypeBitmask(id, 0, MessageTypes.MESSAGE_FORCE_SMS_BIT);
        }

        public void markAsDecryptFailed(long id)
        {
            updateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_FAILED_BIT);
        }

        public void markAsDecryptDuplicate(long id)
        {
            updateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_DUPLICATE_BIT);
        }

        public void markAsNoSession(long id)
        {
            updateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_NO_SESSION_BIT);
        }

        public void markAsDecrypting(long id)
        {
            updateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_BIT);
        }

        public void markAsLegacyVersion(long id)
        {
            updateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_LEGACY_BIT);
        }

        public void markAsOutbox(long id)
        {
            updateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_OUTBOX_TYPE);
        }

        public void markAsPendingInsecureSmsFallback(long id)
        {
            updateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_PENDING_INSECURE_SMS_FALLBACK);
        }

        public void markAsSending(long id)
        {
            updateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENDING_TYPE);
        }

        public void markAsSent(long id)
        {
            updateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENT_TYPE);
        }

        /*public async void markStatus(long messageId, int status)
        {
            Debug.WriteLine($"MessageDatabase Updating ID: {messageId} to status: {status}");
            var message = conn.Get<Message>(messageId);

            message.DeliveryStatus = status;

            conn.Update(message);
        }*/

        public void markAsSentFailed(long id)
        {
            updateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENT_FAILED_TYPE);
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
            //notifyConversationListeners(threadId);
            //notifyConversationListListeners();

            return new Pair<long, long>(messageId, threadId);
        }

        public Pair<long, long> copyMessageInbox(long messageId)
        {
            Reader reader = readerFor(getMessage(messageId));
            SmsMessageRecord record = reader.getNext();

            ContentValues contentValues = new ContentValues();
            contentValues.put(TYPE, (record.getType() & ~MessageTypes.BASE_TYPE_MASK) | MessageTypes.BASE_INBOX_TYPE);
            contentValues.put(ADDRESS, record.getIndividualRecipient().getNumber());
            contentValues.put(ADDRESS_DEVICE_ID, record.getRecipientDeviceId());
            contentValues.put(DATE_RECEIVED, System.currentTimeMillis());
            contentValues.put(DATE_SENT, record.getDateSent());
            contentValues.put(PROTOCOL, 31337);
            contentValues.put(READ, 0);
            contentValues.put(BODY, record.getBody().getBody());
            contentValues.put(THREAD_ID, record.getThreadId());

            SQLiteDatabase db = databaseHelper.getWritableDatabase();
            long newMessageId = db.insert(TABLE_NAME, null, contentValues);

            DatabaseFactory.getThreadDatabase(context).update(record.getThreadId());
            notifyConversationListeners(record.getThreadId());

            jobManager.add(new TrimThreadJob(context, record.getThreadId()));
            reader.close();

            return new Pair<>(newMessageId, record.getThreadId());
        }
        */
        protected Pair<long, long> insertMessageInbox(IncomingTextMessage message, long type)
        {
            if (message.isPreKeyBundle())
            {
                type |= MessageTypes.KEY_EXCHANGE_BIT | MessageTypes.KEY_EXCHANGE_BUNDLE_BIT;
            }
            else if (message.isSecureMessage())
            {
                type |= MessageTypes.SECURE_MESSAGE_BIT;
            }
            /*else if (message.isGroup()) TODO: GROUP enable
            {
                type |= MessageTypes.SECURE_MESSAGE_BIT;
                if (((IncomingGroupMessage)message).isUpdate()) type |= MessageTypes.GROUP_UPDATE_BIT;
                else if (((IncomingGroupMessage)message).isQuit()) type |= MessageTypes.GROUP_QUIT_BIT;
            }*/
            else if (message.isEndSession())
            {
                type |= MessageTypes.SECURE_MESSAGE_BIT;
                type |= MessageTypes.END_SESSION_BIT;
            }

            if (message.isPush()) type |= MessageTypes.PUSH_MESSAGE_BIT;

            Recipients recipients;

            if (message.getSender() != null)
            {
                recipients = RecipientFactory.getRecipientsFromString(message.getSender(), true);
            }
            else
            {
                //Log.w(TAG, "Sender is null, returning unknown recipient");
                recipients = new Recipients(Recipient.getUnknownRecipient());
            }

            Recipients groupRecipients;

            if (message.getGroupId() == null)
            {
                groupRecipients = null;
            }
            else
            {
                groupRecipients = RecipientFactory.getRecipientsFromString(message.getGroupId(), true);
            }

            bool unread = /*org.thoughtcrime.securesms.util.Util.isDefaultSmsProvider() ||*/
                                    message.isSecureMessage() || message.isPreKeyBundle();

            long threadId;

            if (groupRecipients == null) threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(recipients); // TODO CHECK
            else threadId = DatabaseFactory.getThreadDatabase().GetThreadIdForRecipients(groupRecipients);

            /*ContentValues values = new ContentValues(6);
            values.put(ADDRESS, message.getSender());
            values.put(ADDRESS_DEVICE_ID, message.getSenderDeviceId());
            values.put(DATE_RECEIVED, System.currentTimeMillis());
            values.put(DATE_SENT, message.getSentTimestampMillis());
            values.put(PROTOCOL, message.getProtocol());
            values.put(READ, unread ? 0 : 1);

            if (!TextUtils.isEmpty(message.getPseudoSubject()))
                values.put(SUBJECT, message.getPseudoSubject());

            values.put(REPLY_PATH_PRESENT, message.isReplyPathPresent());
            values.put(SERVICE_CENTER, message.getServiceCenterAddress());
            values.put(BODY, message.getMessageBody());
            values.put(TYPE, type);
            values.put(THREAD_ID, threadId);*/

            var insert = new Message()
            {
                Address = message.getSender(),
                AddressDeviceId = message.getSenderDeviceId(),
                DateReceived = TimeUtil.GetDateTimeMillis(), // force precision to millis not to ticks
                DateSent = TimeUtil.GetDateTime(message.getSentTimestampMillis()),
                Read = !unread,
                Body = message.getMessageBody(),
                Type = type,
                ThreadId = threadId
            };

            long rows = conn.Insert(insert);

            long messageId = insert.MessageId;

            if (unread)
            {
                DatabaseFactory.getThreadDatabase().SetUnread(threadId);
            }

            // TODO: var id = DatabaseFactory.getThreadDatabase().update(threadId).Result;
            //notifyConversationListeners(threadId);
            //jobManager.add(new TrimThreadJob(context, threadId)); // TODO

            return new Pair<long, long>(messageId, threadId);
        }

        public Pair<long, long> insertMessageInbox(IncomingTextMessage message)
        {
            return insertMessageInbox(message, MessageTypes.BASE_INBOX_TYPE);
        }
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
        public async Task<long> insertMessageOutbox(long threadId, OutgoingTextMessage message,
                                     long type, DateTime date)
        {
            if (message.isKeyExchange()) type |= MessageTypes.KEY_EXCHANGE_BIT;
            else if (message.isSecureMessage()) type |= MessageTypes.SECURE_MESSAGE_BIT;
            else if (message.isEndSession()) type |= MessageTypes.END_SESSION_BIT;
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
                Address = message.getRecipients().getPrimaryRecipient().getNumber(),
                ThreadId = threadId,
                Body = message.getMessageBody(),
                DateReceived = date,
                DateSent = date,
                Read = false,
                Type = type
            };

            //SQLiteDatabase db = databaseHelper.getWritableDatabase();
            //long messageId = db.insert(TABLE_NAME, ADDRESS, contentValues);
            long messageId = conn.Insert(insert);
            // TODO: await DatabaseFactory.getThreadDatabase().update(threadId);
            //notifyConversationListeners(threadId);
            //jobManager.add(new TrimThreadJob(context, threadId));

            return insert.MessageId;
        }




        public async Task<int> getConversationCount(long threadId)
        {
            /*int count = DatabaseFactory.getTextMessagingDatabase().getMessageCountForThread(threadId);
            count += DatabaseFactory.getMediaMessagingDatabase().getMessageCountForThread(threadId);*/


            return await getMessageCountForThread(threadId);
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
    }
}
