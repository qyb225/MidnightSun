using Midnight.Database;
using Midnight.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Midnight.ViewModels {
    public class MomentViewModes : INotifyPropertyChanged {
        private ObservableCollection<Models.MomentModelItems> allItems = new ObservableCollection<Models.MomentModelItems>();
        public ObservableCollection<Models.MomentModelItems> AllItems {
            get { return this.allItems; }
            set {
                if (allItems != value) {
                    allItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MomentViewModes() {
            using (var conn = MomentDatabase.GetDbConnection()) {

                var allDB = conn.Table<MomentModelItems>();
                int size = allDB.ToArray().Length;
                for (int i = size - 1; i >= 0; --i) {
                    var item = allDB.ElementAt(i);
                    this.allItems.Add(new MomentModelItems() {
                        id = item.id,
                        Details = item.Details,
                        Image = item.Image,
                        BmpImage = new BitmapImage(new Uri("ms-appx://Midnight/" + item.Image))
                    });
                }
            }
        }

        public void AddMomentItem(Models.MomentModelItems x) {
            this.allItems.Insert(0, x);
            using (var conn = MomentDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.MomentModelItems>();
                conn.Insert(x);
            }
            NotifyPropertyChanged();
        }

        public void Clear() {
            this.allItems.Clear();
            using (var conn = MomentDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.MomentModelItems>();
                foreach (var item in Database) {
                    conn.Delete(item);
                }
            }
        }

        public void AddMomentItem(string detail, string img) {
            Models.MomentModelItems theNew = new Models.MomentModelItems() { Details = detail,
                Image = img,
                BmpImage = new BitmapImage(new Uri("ms-appx://Midnight/" + img))
            };
            this.allItems.Insert(0, theNew);
            using (var conn = MomentDatabase.GetDbConnection()) {
                var Database = conn.Table<Models.MomentModelItems>();
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
