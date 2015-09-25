using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSecure.recipient;

namespace Signal.Model
{


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
        public virtual ICollection<Recipient> Recipients { get; set; }
        public string Body { get; set; }
        public long Type { get; set; }
    }
}
