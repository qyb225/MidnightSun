using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App1
{
    public static class AppDatabase
    {
        /// <summary>
        /// 数据库文件所在路径，这里使用 LocalFolder，数据库文件名叫 test.db。
        /// </summary>
        public static string DbPath { set; get; }

        public static SQLiteConnection GetDbConnection(string data)
        {
            DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, data);
            var conn = new SQLiteConnection(new SQLitePlatformWinRT(), DbPath);
            conn.CreateTable<msg>();
            return conn;
        }
    }
}
