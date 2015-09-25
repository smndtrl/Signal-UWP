using SQLite;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Tasks.Library
{
    public class TaskStorage
    {

        public const String TABLE_NAME = "queue";
        public static readonly String ID = "_id";
        public static readonly String ITEM = "item";


        [Table(TABLE_NAME)]
        public class Contact
        {
            [PrimaryKey]
            public string _id { get; set; }
            public string item { get; set; }
        }

        private SQLiteConnection conn;

        public TaskStorage()
        {
            conn = new SQLiteConnection(Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "task.db"));
        }

        public void store(TaskActivity task)
        {

        }

        public void remove(long id)
        {

        }
    }
}
