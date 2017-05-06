using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

    /*
     * 一些坑：
     * 1. 需要数据库不停记录choose(故事线) 和 count
     * 2. 一切的消息收发都在该页面内进行，也就是说，如果离开了该页面（去看朋友圈），则消息收发无法进行，与现实不符，要推翻目前写法。
     */

    public sealed partial class ChattingPage : Page, INotifyPropertyChanged {
        private string choose; //s
        private int count; //s
        private int branchLength;
        private DispatcherTimer Timer;
        private StoryInfo.StoryItem[] aBranch;
        private DateTime runTime; //s
        private DispatcherTimer delayTimer;
        public ViewModels.ChattingViewModels ViewModel { set; get; }
        public ViewModels.MomentViewModes MomentViewModels { get; set; }
        
        private string lastMsg;
        public string LastMessage {
            get { return lastMsg; }
            set {
                if (lastMsg != value) {
                    lastMsg = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int unRead;
        public int UnRead {
            get { return unRead; }
            set {
                if (unRead != value) {
                    unRead = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ChattingPage() {
            this.InitializeComponent();
            ViewModel = new ViewModels.ChattingViewModels();
            this.MomentViewModels = new ViewModels.MomentViewModes();
            LastMessage = chattingItemHandle(ViewModel.AllItems.Last().Msg);
            this.DataContext = ViewModel;
            
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 4);
            Timer.Tick += loadMessage;

            delayTimer = new DispatcherTimer();
            delayTimer.Interval = new TimeSpan(0, 0, 5);
            delayTimer.Tick += ifBegin;
            delayTimer.Start();

            loadProgress();

            loadABranchStoryData();
        }

        private void loadProgress() {
            using (var conn = Process.ProcessDatabase.GetDbConnection()) {
                var processInfo = conn.Table<Process.Process>();
                var last = processInfo.Last();
                choose = last.Choose;
                count = last.Count;
                runTime = new DateTime(last.Year, last.Month, last.Day, last.Hour, last.Min, last.Sec);
            }
        }

        private void saveProcess() {
            using (var conn = Process.ProcessDatabase.GetDbConnection()) {
                var processInfo = conn.Table<Process.Process>();
                conn.Insert(new Process.Process() {
                    Choose = choose,
                    Count = count,
                    Year = runTime.Year,
                    Month = runTime.Month,
                    Day = runTime.Day,
                    Hour = runTime.Hour,
                    Min = runTime.Minute,
                    Sec = runTime.Second
                });
            }
        }

        private void ifBegin(object sender, object e) {
            if (DateTime.Now >= runTime) {
                Timer.Start();
                delayTimer.Stop();
            }
        }

        private string chattingItemHandle(string str) {
            if (PersonInfo.Visibility == Visibility.Visible) {
                ++UnRead;
                MsgCircle.Visibility = Visibility.Visible;
                NumInfo.Visibility = Visibility.Visible;
            }
            if (str.Length > 12) {
                str = str.Substring(0, 9);
                str += "...";
            }
            return str;
        }

        /*init*/
        private void loadABranchStoryData() {
            using (var conn = Database.StoryDatabase.GetDbConnection(choose + ".db")) {
                var aBranchStory = conn.Table<StoryInfo.StoryItem>();
                branchLength = aBranchStory.ToArray().Length;
                int i = 0;
                if (branchLength <= 0) {
                    Timer.Stop();
                    delayTimer.Stop();
                }
                aBranch = new StoryInfo.StoryItem[branchLength];
                foreach (var aStoryItem in aBranchStory) {
                    aBranch[i++] = aStoryItem;
                }
            }
        }

        /*Run every 2 sec*/
        private void loadMessage(object sender, object e) {
            Inputing.Text = "对方正在输入...";
            if (count >= branchLength) {
                count = 0;
                loadABranchStoryData();
                Inputing.Text = "";
            }
            /*问题的标志，需要调取下面两个作为回答选项*/
            else if (aBranch[count].Next == "choose") {
                saveProcess();
                Choose0.Visibility = Visibility.Visible;
                Choose0.Content = aBranch[count++].Msg;
                Choose1.Visibility = Visibility.Visible;
                Choose1.Content = aBranch[count++].Msg;
                Inputing.Text = "";
                Timer.Stop();
            }
            /*需要延迟*/
            else if (aBranch[count].Next == "delay") {
                Inputing.Text = "";
                int delayLong = int.Parse(aBranch[count++].Msg);
                runTime = DateTime.Now.AddMinutes(delayLong);
                saveProcess();
                Timer.Stop();
                delayTimer.Start();
            }
            /*发朋友圈的图片地址*/
            else if (aBranch[count].Next.Length > 6) {
                this.MomentViewModels.AddMomentItem(aBranch[count].Msg, aBranch[count].Next);
                ++count;
                saveProcess();
            }
            /*不是choose，只可能是跳转database的标志，赋值choose为next即可*/
            else if (aBranch[count].Next.Length != 0) {
                ViewModel.AddChattingItem(0, aBranch[count].Msg);
                LastMessage = chattingItemHandle(aBranch[count].Msg);
                this.DataContext = ViewModel;
                choose = aBranch[count++].Next;
                saveProcess();
            }
            /*普通消息，直接展示*/
            else {
                ViewModel.AddChattingItem(0, aBranch[count].Msg);
                LastMessage = chattingItemHandle(aBranch[count++].Msg);
                this.DataContext = ViewModel;
                saveProcess();
            }
        }

        private void TheChattingItem_SizeChanged(object sender, SizeChangedEventArgs e) {
            ChattingScrollViewer.ChangeView(null, TheChattingItem.ActualHeight, null);
        }

        private void Choose0_Click(object sender, RoutedEventArgs e) {
            /*进入0支线*/
            choose += "0";
            /*将选项的文字作为"你"说的话，添加到页面的viewmodel里*/ 
            ViewModel.AddChattingItem(1, Choose0.Content.ToString());
            saveProcess();
            LastMessage = chattingItemHandle(Choose0.Content.ToString());
            /*隐藏两个按钮*/
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            /*更新dataContext*/
            this.DataContext = ViewModel;
            Timer.Start();
            Inputing.Text = "对方正在输入...";
        }

        private void Choose1_Click(object sender, RoutedEventArgs e) {
            choose += "1";
            ViewModel.AddChattingItem(1, Choose1.Content.ToString());
            saveProcess();
            LastMessage = chattingItemHandle(Choose1.Content.ToString());
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            this.DataContext = ViewModel;
            Timer.Start();
            Inputing.Text = "对方正在输入...";
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            ChattingWindow.Visibility = Visibility.Visible;
            PersonInfo.Visibility = Visibility.Collapsed;
            MsgCircle.Visibility = Visibility.Collapsed;
            NumInfo.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            ChattingWindow.Visibility = Visibility.Visible;
            PersonInfo.Visibility = Visibility.Collapsed;
            MsgCircle.Visibility = Visibility.Collapsed;
            NumInfo.Visibility = Visibility.Collapsed;
        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e) {
            ChattingWindow.Visibility = Visibility.Collapsed;


            PersonInfo.Visibility = Visibility.Visible;
            UnRead = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
