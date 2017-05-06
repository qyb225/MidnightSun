using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midnight.Process {
    public class Process {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { set; get; }

        [MaxLength(5)]
        public string Choose { set; get; }

        public int Count { set; get; }

        public int Year { set; get; }

        public int Month { set; get; }

        public int Day { set; get; }

        public int Hour { set; get; }

        public int Min { set; get; }

        public int Sec { set; get; }
    }
}
