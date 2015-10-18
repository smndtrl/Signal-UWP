

using Signal.database.models;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace TextSecure.database
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

                if (recipients == null || recipients.isEmpty())
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

            return query.ToList() ;
        }

        public async Task<SmsMessageRecord> getMessageRecord(long messageId)
        {
            try
            {
                var first = conn.Get<Message>(messageId);

                if ( first != null)
                {
                    //LinkedList<IdentityKeyMismatch> mismatches = getMismatches(first.mismatches);
                    Recipients recipients = GetRecipientsFor(first.Address);
                    DisplayRecord.Body body = getBody(first.Body, first.Type);

                    return new SmsMessageRecord(first.MessageId, body, recipients,
                                        recipients.getPrimaryRecipient(),
                                        (int)first.AddressDeviceId,
                                        first.DateSent, first.DateReceived, (int)first.ReceiptCount, first.Type,
                                        first.ThreadId, (int)0, null); // TODO
                }
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

            /*long messageId = cursor.getLong(cursor.getColumnIndexOrThrow(SmsDatabase.ID));
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

            DisplayRecord.Body body = getBody(cursor);*/
        }

        public Message Get(long messageId)
        {
            return conn.Get<Message>(messageId);
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
                var first =  query.Count() != 0 ?  query.First() : null;

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

        protected DisplayRecord.Body getBody(string body, long type)
        {
            if (MessageTypes.isSymmetricEncryption(type))
            {
                return new DisplayRecord.Body(body, false);
            }
            else
            {
                return new DisplayRecord.Body(body, true);
            }
        }
    }
}
