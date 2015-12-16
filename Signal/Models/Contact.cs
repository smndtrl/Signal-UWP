using GalaSoft.MvvmLight;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Models
{
    /*
        public const String TABLE_NAME = "contacts";
        public static readonly String ID = "_id";
        public static readonly String NAME = "name";
        public static readonly String NUMBER_TYPE = "number_type";
        public static readonly String NUMBER = "number";
        public static readonly String LABEL = "label";
        public static readonly String TYPE = "type";

        [Table(TABLE_NAME)]
        public class ContactTable
        {
            [PrimaryKey]
            public string _id { get; set; }
            public string name { get; set; }
            public uint number_type { get; set; }
            public string number { get; set; }
            public string label { get; set; }
            public uint type { get; set; }
        }
        */

    [Table("Contacts")]
    public class Contact : ObservableObject
    {
        [PrimaryKey,AutoIncrement]
        public long ContactId { get; set; }
        public string Name { get; set; }
        [Indexed(Name = "number", Unique = true)]
        public string Number { get; set;  }
        public string label { get; set; }
        public string SystemId { get; set; }
    }
}
