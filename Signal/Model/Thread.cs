using GalaSoft.MvvmLight;
using Signal.Database.interfaces;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace Signal.Model
{
    /*
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
        */

    [Table("Threads")]
    public class Thread : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public long ThreadId { get; set; }
        public DateTime Date { get; set; }
        public long Count { get; set; }
        private bool _read = false;
        public bool Read
        {
            get { return _read; }
            set
            {
                if (_read == value) return;
                _read = value;
                RaisePropertyChanged("Read");
            }
        }

        public string Snippet { get; set; }
        public long SnippetType { get; set; }
        [/*OneToMany, */Ignore]
        public Recipients Recipients {
            get { return RecipientFactory.getRecipientsForIds(RecipientIds, false); }
            set {
                long[] recipientIds = ThreadDatabaseHelper.getRecipientIds(value);
                String recipientsList = ThreadDatabaseHelper.getRecipientsAsString(recipientIds);
                RecipientIds = recipientsList;
            }
        }
        public string RecipientIds { get; set; }
        public string Body { get; set; }
        public long Type { get; set; }
    }
}
