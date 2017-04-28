using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midnight.StoryInfo {
    public class StoryItem {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { set; get; }

        [MaxLength(5)]
        public string Msg { get; set; }

        [MaxLength(5)]
        public string Next { get; set; }
    }
}
