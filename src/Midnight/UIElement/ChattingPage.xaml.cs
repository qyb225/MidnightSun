using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Midnight.UIElement {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChattingPage : Page {
        private string choose = "X1";
        private int count = 0;
        private int branchLength;
        private DispatcherTimer Timer;
        private StoryInfo.StoryItem[] aBranch;
        private ViewModels.ChattingViewModels ViewModel;

        public ChattingPage() {
            this.InitializeComponent();
            ViewModel = new ViewModels.ChattingViewModels();
            this.DataContext = ViewModel;
            loadABranchStoryData();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 4);
            Timer.Tick += loadMessage;
            Timer.Start();
        }

        private void loadABranchStoryData() {
            using (var conn = Database.StoryDatabase.GetDbConnection(choose + ".db")) {
                var aBranchStory = conn.Table<StoryInfo.StoryItem>();
                branchLength = aBranchStory.ToArray().Length;
                int i = count = 0;
                if (branchLength <= 0) {
                    Timer.Stop();
                }
                aBranch = new StoryInfo.StoryItem[branchLength];
                foreach (var aStoryItem in aBranchStory) {
                    aBranch[i++] = aStoryItem;
                }
            }
        }

        /*Run every 1 sec*/
        private void loadMessage(object sender, object e) {
            Inputing.Text = "对方正在输入...";
            if (count >= branchLength) {
                loadABranchStoryData();
                Inputing.Text = "";
            } else if (aBranch[count].Next == "choose") {
                ViewModel.AddChattingItem(0, aBranch[count++].Msg);
                Choose0.Visibility = Visibility.Visible;
                Choose0.Content = aBranch[count++].Msg;
                Choose1.Visibility = Visibility.Visible;
                Choose1.Content = aBranch[count++].Msg;
                Inputing.Text = "";
                Timer.Stop();
            } else if (aBranch[count].Next.Length != 0) {
                ViewModel.AddChattingItem(0, aBranch[count].Msg);
                this.DataContext = ViewModel;
                choose = aBranch[count++].Next;
            } else {
                ViewModel.AddChattingItem(0, aBranch[count++].Msg);
                this.DataContext = ViewModel;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(IDPage));
        }

        private void TheChattingItem_SizeChanged(object sender, SizeChangedEventArgs e) {
            ChattingScrollViewer.ChangeView(null, TheChattingItem.ActualHeight, null);
        }

        private void Choose0_Click(object sender, RoutedEventArgs e) {
            choose += "0";
            ViewModel.AddChattingItem(1, Choose0.Content.ToString());
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            this.DataContext = ViewModel;
            Timer.Start();
            Inputing.Text = "对方正在输入...";
        }

        private void Choose1_Click(object sender, RoutedEventArgs e) {
            choose += "1";
            ViewModel.AddChattingItem(1, Choose1.Content.ToString());
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            this.DataContext = ViewModel;
            Timer.Start();
        }
    }
}
