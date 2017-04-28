using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Midnight.Database {
    public static class StoryDatabase {
        public static string DbPath { set; get; }

        public static SQLiteConnection GetDbConnection(string data) {
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, data);
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
            conn.CreateTable<StoryInfo.StoryItem>();
            return conn;
        }
    }
}
