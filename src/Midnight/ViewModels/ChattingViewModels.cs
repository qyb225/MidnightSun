using Midnight.Database;
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
                foreach (var item in allDB) {
                    this.allItems.Add(new Models.ChattingItems() {
                        id = item.id,
                        Sender = item.Sender,
                        Msg = item.Msg
                    });
                }
            }
        }

        private Models.ChattingItems selectedItem = default(Models.ChattingItems);
        public Models.ChattingItems SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
