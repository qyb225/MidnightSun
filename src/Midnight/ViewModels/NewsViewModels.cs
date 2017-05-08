using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Midnight.ViewModels {
    public class NewsViewModels : INotifyPropertyChanged {
        private ObservableCollection<Models.NewsModelItems> allItems = new ObservableCollection<Models.NewsModelItems>();
        public ObservableCollection<Models.NewsModelItems> AllItems {
            get { return this.allItems; }
            set {
                if (allItems != value) {
                    allItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public NewsViewModels() {
            using (var conn = Database.NewsDatabase.GetDbConnection()) {

                var allDB = conn.Table<Models.NewsModelItems>();
                foreach (var item in allDB) {
                    this.allItems.Add(new Models.NewsModelItems() {
                        id = item.id,
                        Details = item.Details,
                        Date = item.Date
                    });
                }
            }
        }

        public void AddNewsItem(string news) {
            Models.NewsModelItems theNew = new Models.NewsModelItems() {
                Details = news,
                Date = DateTime.Now.ToString("yyyy-MM-dd")
            };
            this.allItems.Add(theNew);
            using (var conn = Database.NewsDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.NewsModelItems>();
                conn.Insert(theNew);
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
