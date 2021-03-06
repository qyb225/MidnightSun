﻿using Midnight.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Midnight.ViewModels {
    public class ChattingViewModels : INotifyPropertyChanged {
        private ObservableCollection<Models.ChattingItems> allItems = new ObservableCollection<Models.ChattingItems>();
        public ObservableCollection<Models.ChattingItems> AllItems {
            get { return this.allItems; }
            set {
                if (allItems != value) {
                    allItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ChattingViewModels() {
            using (var conn = ChattingInfoDatabase.GetDbConnection()) {

                var allDB = conn.Table<Models.ChattingItems>();
                string lastNick = "ms-appx://Midnight/Assets/IDPage/Test/tx.jpg";
                if (allDB.Count() > 0) {
                    lastNick = allDB.Last().NickPath;
                }
                foreach (var item in allDB) {
                    this.allItems.Add(new Models.ChattingItems() {
                        id = item.id,
                        Sender = item.Sender,
                        Msg = item.Msg,
                        NickPath = lastNick,
                        NickImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(lastNick))
                    });
                }
            }
        }

        public void AddChattingItem(Models.ChattingItems x) {
            this.allItems.Add(x);
            using (var conn = ChattingInfoDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.ChattingItems>();
                conn.Insert(x);
            }
            NotifyPropertyChanged();
        }

        public void AddChattingItem(int sender, string message, string nickPath) {
            Models.ChattingItems theNew = new Models.ChattingItems() {
                Sender = sender, Msg = message,
                NickPath = nickPath,
                NickImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(nickPath))
            };
            this.allItems.Add(theNew);
            using (var conn = ChattingInfoDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.ChattingItems>();
                conn.Insert(theNew);
            }
            NotifyPropertyChanged();
        }

        public void Clear() {
            this.allItems.Clear();
            using (var conn = ChattingInfoDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.ChattingItems>();
                foreach (var item in Database) {
                    conn.Delete(item);
                }
            }
            NotifyPropertyChanged();
        }

        public void UpdateNick(string uri) {
            foreach (var item in AllItems) {
                item.NickPath = uri;
                item.NickImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(uri));
                using (var conn = ChattingInfoDatabase.GetDbConnection()) {
                    var Database = conn.Table<Models.ChattingItems>();
                    conn.InsertOrReplace(item);
                }
            }
            NotifyPropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
