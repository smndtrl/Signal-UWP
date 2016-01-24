


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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libaxolotl.util;
using TextSecure.recipient;
using Signal.Models;
using Signal.Messages;
using Signal.Util;

namespace Signal.Database
{
    public class TextMessageDatabase : MessageDatabase
    {
        //private SQLiteConnection conn;

        public TextMessageDatabase(SQLiteConnection conn)
            : base(conn)
        {
            // this.conn = conn;
        }

        private Recipients GetRecipientsFor(string address)
        {
            if (address != null)
            {
                Recipients recipients = RecipientFactory.getRecipientsFromString(address, false);

                if (recipients == null || recipients.IsEmpty)
                {
                    return RecipientFactory.getRecipientsFor(Recipient.getUnknownRecipient(), false);
                }

                return recipients;
            }
            else
            {
                //Log.w(TAG, "getRecipientsFor() address is null");
                return RecipientFactory.getRecipientsFor(Recipient.getUnknownRecipient(), false);
            }
        }

        /* private Message Converter(MessageTable m)
         {
             var recipients = GetRecipientsFor(m.address);
             return new Message(m._id.Value, m.body, recipients, recipients.getPrimaryRecipient(), (int)m.address_device_id, m.date_sent, m.date_received, m.receipt_count, (int)m.type, (int)m.thread_id, m.status/*,m.mismatches); // TODO: ???
         }*/

        public async Task<List<Message>> getMessages(long threadId, long skip = 0, long take = 10)
        {
            var query = conn.Table<Message>().Where(v => v.ThreadId == threadId);

            List<Message> list = new List<Message>();

            /*foreach (var thread in query.ToList())
            {
                var t = Converter(thread);

                list.Add(t);
            }*/

            return query.ToList();
        }

        public async Task<TextMessageRecord> getMessageRecord(long messageId)
        {
            try
            {
                var first = conn.Get<Message>(messageId);

                if (first != null)
                {
                    return new TextMessageRecord(first);
                    //LinkedList<IdentityKeyMismatch> mismatches = getMismatches(first.mismatches);
                    /*Recipients recipients = GetRecipientsFor(first.Address);
                    DisplayRecord.Body body = getBody(first.Body, first.Type);

                    return new SmsMessageRecord(first.MessageId, body, recipients,
                                        recipients.getPrimaryRecipient(),
                                        (int)first.AddressDeviceId,
                                        first.DateSent, first.DateReceived, (int)first.ReceiptCount, first.Type,
                                        first.ThreadId, (int)0, null); // TODO*/
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public Message Get(long messageId)
        {
            return conn.Get<Message>(messageId);
        }

        public void Delete(long messageId)
        {
            conn.Delete<Message>(messageId);
        }

        public async Task<Message> GetAsync(long messageId)
        {
            return conn.Get<Message>(messageId);
        }

        public async Task<Message> getMessage(long messageId)
        {
            try
            {
                var query = conn.Table<Message>().Where(m => m.MessageId == messageId);
                var first = query.Count() != 0 ? query.First() : null;

                if (query != null && first != null)
                    return first;
                else
                    return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                //if (cursor != null)
                //cursor.close();
            }

        }

        /*private LinkedList<IdentityKeyMismatch> getMismatches(String document)
        {
            try
            {
                if (document.Equals(""))
                {
                    return JsonUtil.fromJson<IdentityKeyMismatchList>(document).getList();
                    //return JsonUtils.fromJson(document, IdentityKeyMismatchList.class).getList();
                }
            }
            catch (IOException e)
            {
                //Log.w(TAG, e);
            }

            return new LinkedList<IdentityKeyMismatch>();
        }*/

        /*protected DisplayRecord.Body getBody(string body, long type)
        {
            if (MessageTypes.isSymmetricEncryption(type))
            {
                return new DisplayRecord.Body(body, false);
            }
            else
            {
                return new DisplayRecord.Body(body, true);
            }
        }*/

        #region SmsDatabase

        private void UpdateTypeBitmask(long messageId, long maskOff, long maskOn)
        {
            Log.Debug($"MessageDatabase: Updating ID: {messageId} to base type: {maskOn}");


            var message = conn.Get<Message>(messageId);

            message.Type = (MessageTypes.TOTAL_MASK - maskOff) | maskOn;
            conn.Update(message);

            DatabaseFactory.getThreadDatabase().Refresh(message.ThreadId);

            notifyConversationListeners(message.ThreadId);
            notifyConversationListListeners();
        }

        public long GetThreadIdForMessage(long messageId)
        {
            var message = conn.Get<Message>(messageId);

            if (message != null)
            {
                return message.ThreadId;
            }
            else
            {
                return -1;
            }
        }

        public void MarkAsEndSession(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.KEY_EXCHANGE_MASK, MessageTypes.END_SESSION_BIT);
        }

        public void MarkAsPreKeyBundle(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.KEY_EXCHANGE_MASK, MessageTypes.KEY_EXCHANGE_BIT | MessageTypes.KEY_EXCHANGE_BUNDLE_BIT);
        }

        public void MarkAsInvalidVersionKeyExchange(long id)
        {
            UpdateTypeBitmask(id, 0, MessageTypes.KEY_EXCHANGE_INVALID_VERSION_BIT);
        }

        public void MarkAsSecure(long id)
        {
            UpdateTypeBitmask(id, 0, MessageTypes.SECURE_MESSAGE_BIT);
        }

        public void MarkAsInsecure(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.SECURE_MESSAGE_BIT, 0);
        }

        public void MarkAsPush(long id)
        {
            UpdateTypeBitmask(id, 0, MessageTypes.PUSH_MESSAGE_BIT);
        }

        public void MarkAsForcedSms(long id)
        {
            UpdateTypeBitmask(id, 0, MessageTypes.MESSAGE_FORCE_SMS_BIT);
        }

        public void MarkAsDecryptFailed(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_FAILED_BIT);
        }

        public void MarkAsDecryptDuplicate(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_DUPLICATE_BIT);
        }

        public void MarkAsNoSession(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_NO_SESSION_BIT);
        }

        public void MarkAsDecrypting(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_BIT);
        }

        public void MarkAsLegacyVersion(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.ENCRYPTION_MASK, MessageTypes.ENCRYPTION_REMOTE_LEGACY_BIT);
        }

        public void MarkAsOutbox(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_OUTBOX_TYPE);
        }

        public void MarkAsPendingInsecureSmsFallback(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_PENDING_INSECURE_SMS_FALLBACK);
        }

        public void MarkAsSending(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENDING_TYPE);
        }

        public void MarkAsSent(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENT_TYPE);
        }

        /*public async void markStatus(long messageId, int status)
        {
            Debug.WriteLine($"MessageDatabase Updating ID: {messageId} to status: {status}");
            var message = conn.Get<Message>(messageId);

            message.DeliveryStatus = status;

            conn.Update(message);
        }*/

        public void MarkAsSentFailed(long id)
        {
            UpdateTypeBitmask(id, MessageTypes.BASE_TYPE_MASK, MessageTypes.BASE_SENT_FAILED_TYPE);
        }

        protected Pair<long, long> updateMessageBodyAndType(long messageId, string body, long maskOff, long maskOn)
        {
            var message = Get(messageId);

            message.Body = body;
            message.Type = (MessageTypes.TOTAL_MASK - maskOff) | maskOn;

            conn.Update(message);

            long threadId = GetThreadIdForMessage(messageId);

            DatabaseFactory.getThreadDatabase().Update(threadId, true);
            notifyConversationListeners(threadId);
            notifyConversationListListeners();

            return new Pair<long, long>(messageId, threadId);
        }

        protected long InsertMessageOutbox(long threadId, OutgoingTextMessage message,
                                     long type, bool forceSms, ulong date)
        {
            if (message.IsKeyExchange) type |= MessageTypes.KEY_EXCHANGE_BIT;
            else if (message.IsSecureMessage) type |= MessageTypes.SECURE_MESSAGE_BIT;
            else if (message.IsEndSession) type |= MessageTypes.END_SESSION_BIT;
            if (forceSms) type |= MessageTypes.MESSAGE_FORCE_SMS_BIT;

            var insert = new Message();
            insert.Address = message.Recipients.getPrimaryRecipient().getNumber(); // PhoneNumberUtils.formatNumber(message.Recipients.getPrimaryRecipient().getNumber()); // TODO: 
            insert.ThreadId = threadId;
            insert.Body = message.MessageBody;
            insert.DateReceived = TimeUtil.GetDateTime(date);
            insert.DateSent = TimeUtil.GetDateTime(date);
            insert.Read = true;
            insert.Type = type;

            conn.Insert(insert);

            long messageId = insert.MessageId;

            DatabaseFactory.getThreadDatabase().Update(threadId);
            notifyConversationListeners(threadId);
            //jobManager.add(new TrimThreadJob(context, threadId));

            return messageId;
        }

        #endregion

        #region EncryptingSmsDatabase
        /*private String getAsymmetricEncryptedBody(AsymmetricMasterSecret masterSecret, String body)
        {
            AsymmetricMasterCipher bodyCipher = new AsymmetricMasterCipher(masterSecret);
            return bodyCipher.encryptBody(body);
        }*/

        /*private String getEncryptedBody(MasterSecret masterSecret, String body)
        {
            MasterCipher bodyCipher = new MasterCipher(masterSecret);
            String ciphertext = bodyCipher.encryptBody(body);
            plaintextCache.put(ciphertext, body);

            return ciphertext;
        }*/

        public long InsertMessageOutbox(long threadId,
                                        OutgoingTextMessage message, bool forceSms,
                                        ulong timestamp)
        {
            long type = MessageTypes.BASE_OUTBOX_TYPE;

            /*if (masterSecret.getMasterSecret().isPresent())
            {
                message = message.withBody(getEncryptedBody(masterSecret.getMasterSecret().get(), message.getMessageBody()));
                type |= Types.ENCRYPTION_SYMMETRIC_BIT;
            }
            else {
                message = message.withBody(getAsymmetricEncryptedBody(masterSecret.getAsymmetricMasterSecret().get(), message.getMessageBody()));
                type |= Types.ENCRYPTION_ASYMMETRIC_BIT;
            }*/

            return InsertMessageOutbox(threadId, message, type, forceSms, timestamp);
        }
        /*
        public Pair<Long, Long> insertMessageInbox(@NonNull MasterSecretUnion masterSecret,
                                                   @NonNull IncomingTextMessage message)
        {
            if (masterSecret.getMasterSecret().isPresent())
            {
                return insertMessageInbox(masterSecret.getMasterSecret().get(), message);
            }
            else {
                return insertMessageInbox(masterSecret.getAsymmetricMasterSecret().get(), message);
            }
        }

        private Pair<Long, Long> insertMessageInbox(@NonNull MasterSecret masterSecret,
                                                    @NonNull IncomingTextMessage message)
        {
            long type = Types.BASE_INBOX_TYPE | Types.ENCRYPTION_SYMMETRIC_BIT;

            message = message.withMessageBody(getEncryptedBody(masterSecret, message.getMessageBody()));

            return insertMessageInbox(message, type);
        }

        private Pair<Long, Long> insertMessageInbox(@NonNull AsymmetricMasterSecret masterSecret,
                                                    @NonNull IncomingTextMessage message)
        {
            long type = Types.BASE_INBOX_TYPE | Types.ENCRYPTION_ASYMMETRIC_BIT;

            message = message.withMessageBody(getAsymmetricEncryptedBody(masterSecret, message.getMessageBody()));

            return insertMessageInbox(message, type);
        }

        public Pair<Long, Long> updateBundleMessageBody(MasterSecretUnion masterSecret, long messageId, String body)
        {
            long type = Types.BASE_INBOX_TYPE | Types.SECURE_MESSAGE_BIT;
            String encryptedBody;

            if (masterSecret.getMasterSecret().isPresent())
            {
                encryptedBody = getEncryptedBody(masterSecret.getMasterSecret().get(), body);
                type |= Types.ENCRYPTION_SYMMETRIC_BIT;
            }
            else {
                encryptedBody = getAsymmetricEncryptedBody(masterSecret.getAsymmetricMasterSecret().get(), body);
                type |= Types.ENCRYPTION_ASYMMETRIC_BIT;
            }

            return updateMessageBodyAndType(messageId, encryptedBody, Types.TOTAL_MASK, type);
        }
        */
        public void UpdateMessageBody(long messageId, string body)
        {
            long type = MessageTypes.TOTAL_MASK; // TODO: FIX

            /*if (masterSecret.getMasterSecret().isPresent()) // TODO: FIX
            {
                body = getEncryptedBody(masterSecret.getMasterSecret().get(), body);
                type = Types.ENCRYPTION_SYMMETRIC_BIT;
            }
            else {
                body = getAsymmetricEncryptedBody(masterSecret.getAsymmetricMasterSecret().get(), body);
                type = Types.ENCRYPTION_ASYMMETRIC_BIT;
            }*/

            updateMessageBodyAndType(messageId, body, MessageTypes.ENCRYPTION_MASK, type);
        }
        /*
        public Reader getMessages(MasterSecret masterSecret, int skip, int limit)
        {
            Cursor cursor = super.getMessages(skip, limit);
            return new DecryptingReader(masterSecret, cursor);
        }

        public Reader getOutgoingMessages(MasterSecret masterSecret)
        {
            Cursor cursor = super.getOutgoingMessages();
            return new DecryptingReader(masterSecret, cursor);
        }

        public SmsMessageRecord getMessage(MasterSecret masterSecret, long messageId) throws NoSuchMessageException
        {
            Cursor cursor = super.getMessage(messageId);
            DecryptingReader reader = new DecryptingReader(masterSecret, cursor);
        SmsMessageRecord record = reader.getNext();

        reader.close();

    if (record == null) throw new NoSuchMessageException("No message for ID: " + messageId);
    else                return record;
  }

    public Reader getDecryptInProgressMessages(MasterSecret masterSecret)
    {
        Cursor cursor = super.getDecryptInProgressMessages();
        return new DecryptingReader(masterSecret, cursor);
    }

    public Reader readerFor(MasterSecret masterSecret, Cursor cursor)
    {
        return new DecryptingReader(masterSecret, cursor);
    }
    */
        #endregion
    }
}
