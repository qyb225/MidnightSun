using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class msg
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { set; get; }

        [MaxLength(5)]
        public string Msg { get; set; }

        [MaxLength(5)]
        public string Next { get; set; }
    }
}
