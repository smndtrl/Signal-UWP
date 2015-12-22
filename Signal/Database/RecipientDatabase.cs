using Signal.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.database;
using TextSecure.recipient;

namespace Signal.Database
{
    public class RecipientDatabase
    {

        private SQLiteConnection conn;

        public RecipientDatabase(SQLiteConnection conn)
        {
            this.conn = conn;
            conn.CreateTable<Recipient>();
        }

        public Recipient Get(long recipientId)
        {
            return conn.Get<Recipient>(recipientId);
        }

        public Recipients GetRecipients(long[] recipientIds)
        {
            //var recipients = conn.Table<Recipient>().Where(s => recipientIds.Contains(s.RecipientId)).ToList();

            var recipients = new List<Recipient>();
            foreach (var recipientId in recipientIds)
            {

            }

            return RecipientFactory.getRecipientsFor(recipients, false);
        }

        public Recipients GetRecipients(List<Recipient> recipients)
        {
            // TODO: advanced recipient settings

            return new Recipients(recipients, null);
        }

        public Recipient GetRecipientForNumber(string number)
        {
            var query = conn.Table<Recipient>().Where(r => r.Number == number);

            if (query.Count() != 0) return query.First();

            var recipient = new Recipient()
            {
                Number = number
            };

            conn.Insert(recipient);

            return recipient;
        }

        public Recipient GetSelfRecipient(string number)
        {
            var query = conn.Table<Recipient>().Where(r => r.Number == number);

            if (query.Count() != 0) return query.First();

            var recipient = new Recipient()
            {
                RecipientId = 0,
                Number = number
            };

            conn.Insert(recipient);

            return recipient;
        }

        public long GetRecipientIdForNumber(string number)
        {
            var query = conn.Table<Recipient>().Where(r => r.Number == number);

            if (query.Count() != 0) return query.First().RecipientId;

            var recipient = new Recipient()
            {
                Number = number
            };

            conn.Insert(recipient);

            return recipient.RecipientId;
        }

        public Recipient GetOrCreateRecipient(TextSecureDirectory.Directory d)
        {
            var query = conn.Table<Recipient>().Where(r => r.Number == d.Number);

            if (query.Count() != 0) return query.First();

            var recipient = new Recipient()
            {
                RecipientId = 0,
                Number = d.Number,
                ContactId = d.ContactId,
                Name = d.Name
            };

            conn.Insert(recipient);

            return recipient;
        }

        public Recipients GetOrCreateRecipients(TextSecureDirectory.Directory d)
        {
            var recipients = new List<Recipient>();
            /*foreach (var recipientId in recipientIds)
            {

            }*/

            recipients.Add(GetOrCreateRecipient(d));

            return RecipientFactory.getRecipientsFor(recipients, false);
        }
    }
}
