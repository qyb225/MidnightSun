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

        private Models.MomentModelItems selectedItem = default(Models.MomentModelItems);
        public Models.MomentModelItems SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public MomentViewModes() {
            using (var conn = MomentDatabase.GetDbConnection()) {

                var allDB = conn.Table<MomentModelItems>();
                foreach (var item in allDB) {
                    this.allItems.Add(new MomentModelItems() { id = item.id, User = item.User, Details = item.Details,
                        Image = item.Image, BmpImage = new BitmapImage(new Uri("ms-appx://Midnight/" + item.Image)) });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
