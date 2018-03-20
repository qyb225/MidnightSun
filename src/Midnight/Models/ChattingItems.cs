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
    public class ChattingItems : INotifyPropertyChanged {
        [PrimaryKey]
        [AutoIncrement]
        public int id { set; get; }

        private int sender;
        public int Sender {
            get { return sender; }
            set {
                if (sender != value) {
                    sender = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string msg;
        [MaxLength(20)]
        public string Msg {
            get { return msg; }
            set {
                if (msg != value) {
                    msg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string nickPath;
        [MaxLength(20)]
        public string NickPath {
            get { return nickPath; }
            set {
                if (nickPath != value) {
                    nickPath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        
        private BitmapImage nickImage;
        [Ignore]
        public BitmapImage NickImage {
            get { return nickImage; }
            set {
                if (nickImage != value) {
                    nickImage = value;
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
