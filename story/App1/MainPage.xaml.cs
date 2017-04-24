using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private string choose = "X1";
        private string output = "";

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string c = txtAddName.Text;
            if (c == "1")
            {
                output += "\n                                Yes\n\n";
            }
            else
            {
                output += "\n                                No\n\n";
            }
            choose += c;
        }

        private void loadData()
        {
            using (var conn = AppDatabase.GetDbConnection(choose + ".db"))
            {
                var dbPerson = conn.Table<msg>();
                foreach (var person in dbPerson)
                {
                    output = output + person.Msg + "\n";
                }
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            loadData();
            _out.Text = output;
            string next = "";
            using (var conn = AppDatabase.GetDbConnection(choose + ".db"))
            {
                var dbPerson = conn.Table<msg>();
                next = dbPerson.Last().Next;
                if (next.Length == 0)
                {
                    txtAddName.Text = "choose 1 or 0";
                }
                else
                {
                    txtAddName.Text = "Click Load To Continue";
                    choose = next;
                }
            }
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            /*using (var conn = AppDatabase.GetDbConnection("X3.db"))
            {

                var addPerson = new msg() { Msg = "Hey! I will give everyone a free Mi-band!", Next = "" };
                var count = conn.Insert(addPerson);
            }*/
        }
    }
}
