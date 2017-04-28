using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Midnight.Models {
    public class MomentModelItems : INotifyPropertyChanged {
        [PrimaryKey]
        [AutoIncrement]
        public int id { set; get; }

        private string details;
        [MaxLength(100)]
        public string Details {
            get { return this.details; }
            set {
                if (details != value) {
                    details = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string image;
        [MaxLength(20)]
        public string Image {
            get { return image; }
            set {
                if (image != value) {
                    image = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private BitmapImage bmpImage;
        [Ignore]
        public BitmapImage BmpImage {
            get { return bmpImage; }
            set {
                if (bmpImage != value) {
                    bmpImage = value;
                    NotifyPropertyChanged();
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
