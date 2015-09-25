using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Model
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public string name { get; set; }
        public string number { get; set;  }
        public string label { get; set; }
    }
}
