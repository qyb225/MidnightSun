using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Midnight.Models {
    public class NewsModelItems : INotifyPropertyChanged {
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

        private string date;
        [MaxLength(20)]
        public string Date {
            get { return this.date; }
            set {
                if (date != value) {
                    date = value;
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
