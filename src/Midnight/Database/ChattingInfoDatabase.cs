﻿using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Midnight.Database {
    public static class ChattingInfoDatabase {
        public readonly static string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ChattingInfo.db");
        
        public static SQLiteConnection GetDbConnection() {
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
            conn.CreateTable<Models.ChattingItems>();
            return conn;
        }
    }
}
