using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Midnight.UIElement {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class ChattingPage : Page, INotifyPropertyChanged {
        private string choose;
        private int count;
        private int branchLength;
        private DispatcherTimer Timer;
        private StoryInfo.StoryItem[] aBranch;
        private DateTime runTime;
        private DispatcherTimer delayTimer;
        private double speedUp = 1.0;
        private bool ifInit = true;
        public ViewModels.ChattingViewModels ChattingViewModel { set; get; }
        public ViewModels.MomentViewModes MomentViewModels { get; set; }
        public ViewModels.NewsViewModels NewsViewModel { set; get; }
        private string pageNickNamePath;

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

        private string lastNews;
        public string LastNews {
            get { return lastNews; }
            set {
                if (lastNews != value) {
                    lastNews = value;
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

        private int unReadNews;
        public int UnReadNews {
            get { return unReadNews; }
            set {
                if (unReadNews != value) {
                    unReadNews = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ChattingPage() {
            this.InitializeComponent();
            ChattingViewModel = new ViewModels.ChattingViewModels();
            this.MomentViewModels = new ViewModels.MomentViewModes();
            NewsViewModel = new ViewModels.NewsViewModels();
            pageNickNamePath = "ms-appx://Midnight/Assets/IDPage/Test/tx.jpg";
            if (ChattingViewModel.AllItems.Count == 0) {
                LastMessage = "";
            } else {
                LastMessage = "";
                for (int i = ChattingViewModel.AllItems.Count - 1; i >= 0; --i) {
                    if (ChattingViewModel.AllItems.ElementAt(i).Sender == 0 || ChattingViewModel.AllItems.ElementAt(i).Sender == 1) {
                        LastMessage = chattingItemHandle(ChattingViewModel.AllItems.ElementAt(i).Msg);
                        pageNickNamePath = ChattingViewModel.AllItems.ElementAt(i).NickPath;
                        break;
                    }
                }
            }
            NickPic.Source = new BitmapImage(new Uri(pageNickNamePath));

            if (NewsViewModel.AllItems.Count == 0) {
                LastNews = "";
            } else {
                LastNews = chattingItemHandle(NewsViewModel.AllItems.Last().Details);
            }

            this.DataContext = ChattingViewModel;
            this.speedUp = 1.0;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 7);
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
                if (processInfo.Count() > 0) {
                    var last = processInfo.Last();
                    choose = last.Choose;
                    count = last.Count;
                    runTime = new DateTime(last.Year, last.Month, last.Day, last.Hour, last.Min, last.Sec);
                } else {
                    choose = "X1";
                    count = 0;
                    runTime = new DateTime(1000, 1, 1, 0, 0, 0);
                }
                
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
                /*时间显示 or 上线通知  问题：会被初始化*/
            }
        }

        private string chattingItemHandle(string str) {
            if (NewsInfo.Visibility == Visibility.Collapsed) {
                if (UnReadNews > 0) {
                    NewsCircle.Visibility = Visibility.Visible;
                    NumInfoNews.Visibility = Visibility.Visible;
                }
            } else {
                UnReadNews = 0;
            }

            if (ChattingWindow.Visibility == Visibility.Collapsed) {
                if (UnRead > 0) {
                    MsgCircle.Visibility = Visibility.Visible;
                    NumInfo.Visibility = Visibility.Visible;
                }
            } else {
                UnRead = 0;
            }
            if (str.Length > 11) {
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

        /*Run every 6 sec*/
        private void loadMessage(object sender, object e) {
            if (count >= branchLength) {
                count = 0;
                loadABranchStoryData();
                Inputing.Text = "";
            }
            else {
                if (aBranch[count].Next == "time") {
                    string timeShow = DateTime.Now.ToString("MM月dd日 HH:mm");
                    ChattingViewModel.AddChattingItem(5, timeShow, pageNickNamePath);
                    ++count;
                    this.DataContext = ChattingViewModel;
                    saveProcess();
                }
                Inputing.Text = "对方正在输入...";
                if (aBranch[count].Next == "on") {
                    Inputing.Text = "";
                    ChattingViewModel.AddChattingItem(2, "对方已上线", pageNickNamePath);
                    ++count;
                    this.DataContext = ChattingViewModel;
                    saveProcess();
                } else if (aBranch[count].Next == "off") {
                    Inputing.Text = "";
                    ChattingViewModel.AddChattingItem(3, "对方已下线", pageNickNamePath);
                    ++count;
                    this.DataContext = ChattingViewModel;
                    saveProcess();
                }
                /*问题的标志，需要调取下面两个作为回答选项*/
                else if (aBranch[count].Next == "choose") {
                    saveProcess();
                    ifInit = false;
                    Choose0.Visibility = Visibility.Visible;
                    Choose0Text.Text = aBranch[count++].Msg;
                    Choose1.Visibility = Visibility.Visible;
                    Choose1Text.Text = aBranch[count++].Msg;
                    Inputing.Text = "";
                    Timer.Stop();
                }
                /*直接进行*/
                else if (aBranch[count].Next == "choose0") {
                    saveProcess();
                    ifInit = true;
                    Choose0.Visibility = Visibility.Visible;
                    Choose0Text.Text = aBranch[count++].Msg;
                    Choose1.Visibility = Visibility.Visible;
                    Choose1Text.Text = aBranch[count++].Msg;
                    Inputing.Text = "";
                    Timer.Stop();
                }
                /*需要延迟*/
                else if (aBranch[count].Next == "delay") {
                    Inputing.Text = "";
                    int delayLong = (int) (speedUp * int.Parse(aBranch[count++].Msg));
                    runTime = DateTime.Now.AddMinutes(delayLong);
                    saveProcess();
                    Timer.Stop();
                    delayTimer.Start();
                }
                /*新闻*/
                else if (aBranch[count].Next == "news") {
                    Inputing.Text = "";
                    ++UnReadNews;
                    LastNews = chattingItemHandle(aBranch[count].Msg);
                    NewsViewModel.AddNewsItem(aBranch[count++].Msg);
                    saveProcess();
                }
                /*Bad end*/
                else if (aBranch[count].Next == "die") {
                    choose = "X1";
                    count = 0;
                    using (var conn = Process.ProcessDatabase.GetDbConnection()) {
                        var processInfo = conn.Table<Process.Process>();
                        foreach (var item in processInfo) {
                            conn.Delete(item);
                        }
                    }
                    this.ChattingViewModel.Clear();
                    this.NewsViewModel.Clear();
                    this.MomentViewModels.Clear();
                    Timer.Stop();
                    this.Frame.Navigate(typeof(BadEnd));
                }
                /*发朋友圈的图片地址*/
                else if (aBranch[count].Next.Length > 6) {
                    this.MomentViewModels.AddMomentItem(aBranch[count].Msg, aBranch[count].Next);
                    ChattingViewModel.AddChattingItem(4, "朋友圈已更新", pageNickNamePath);
                    this.DataContext = ChattingViewModel;
                    ++count;
                    saveProcess();
                }
                /*不是choose，只可能是跳转database的标志，赋值choose为next即可*/
                else if (aBranch[count].Next.Length != 0) {
                    ChattingViewModel.AddChattingItem(0, aBranch[count].Msg, pageNickNamePath);
                    ++UnRead;
                    LastMessage = chattingItemHandle(aBranch[count].Msg);
                    this.DataContext = ChattingViewModel;
                    choose = aBranch[count++].Next;
                    saveProcess();
                }
                /*普通消息，直接展示*/
                else {
                    ChattingViewModel.AddChattingItem(0, aBranch[count].Msg, pageNickNamePath);
                    ++UnRead;
                    LastMessage = chattingItemHandle(aBranch[count++].Msg);
                    this.DataContext = ChattingViewModel;
                    saveProcess();
                }
            }
            if (runTime < DateTime.Now) {
                runTime = DateTime.Now;
            }
        }

        private void TheChattingItem_SizeChanged(object sender, SizeChangedEventArgs e) {
            ChattingScrollViewer.ChangeView(null, TheChattingItem.ActualHeight, null);
        }

        private void Choose0_Click(object sender, RoutedEventArgs e) {
            /*进入0支线*/
            if (!ifInit) {
                choose += "0";
            }
            /*将选项的文字作为"你"说的话，添加到页面的viewmodel里*/
            ChattingViewModel.AddChattingItem(1, Choose0Text.Text, pageNickNamePath);
            saveProcess();
            LastMessage = chattingItemHandle(Choose0Text.Text);
            /*隐藏两个按钮*/
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            /*更新dataContext*/
            this.DataContext = ChattingViewModel;
            Timer.Start();
            Inputing.Text = "对方正在输入...";
        }

        private void Choose1_Click(object sender, RoutedEventArgs e) {
            if (!ifInit) {
                choose += "1";
            }
            ChattingViewModel.AddChattingItem(1, Choose1Text.Text, pageNickNamePath);
            saveProcess();
            LastMessage = chattingItemHandle(Choose1Text.Text);
            Choose0.Visibility = Visibility.Collapsed;
            Choose1.Visibility = Visibility.Collapsed;
            this.DataContext = ChattingViewModel;
            Timer.Start();
            Inputing.Text = "对方正在输入...";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            NewsInfo.Visibility = Visibility.Collapsed;
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (Chatting.IsSelected) {
                ChattingWindow.Visibility = Visibility.Visible;
                PersonInfo.Visibility = Visibility.Collapsed;
                MsgCircle.Visibility = Visibility.Collapsed;
                NumInfo.Visibility = Visibility.Collapsed;
                NewsInfo.Visibility = Visibility.Collapsed;
                UnRead = 0;
            } else {
                ChattingWindow.Visibility = Visibility.Collapsed;
                PersonInfo.Visibility = Visibility.Collapsed;
                NewsInfo.Visibility = Visibility.Visible;
                NewsCircle.Visibility = Visibility.Collapsed;
                NumInfoNews.Visibility = Visibility.Collapsed;
                UnReadNews = 0;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
            if (Chatting.IsSelected) {
                ChattingWindow.Visibility = Visibility.Visible;
                PersonInfo.Visibility = Visibility.Collapsed;
                MsgCircle.Visibility = Visibility.Collapsed;
                NumInfo.Visibility = Visibility.Collapsed;
                NewsInfo.Visibility = Visibility.Collapsed;
                UnRead = 0;
            } else {
                ChattingWindow.Visibility = Visibility.Collapsed;
                PersonInfo.Visibility = Visibility.Collapsed;
                NewsInfo.Visibility = Visibility.Visible;
                NewsCircle.Visibility = Visibility.Collapsed;
                NumInfoNews.Visibility = Visibility.Collapsed;
                UnReadNews = 0;
            }
        }

        private void NewsList_SizeChanged(object sender, SizeChangedEventArgs e) {
            newsScroll.ChangeView(null, NewsList.ActualHeight, null);
        }

        private void Speed_Toggled(object sender, RoutedEventArgs e) {
            if (Speed.IsOn) {
                speedUp = 0.3;
            } else {
                speedUp = 1.0;
            }
        }

        private static async Task SaveWriteableBitmapImageFile(WriteableBitmap image, StorageFile file) {
            //BitmapEncoder 存放格式
            Guid bitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            string filename = file.Name;
            if (filename.EndsWith("jpg")) {
                bitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            } else if (filename.EndsWith("png")) {
                bitmapEncoderGuid = BitmapEncoder.PngEncoderId;
            }
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None)) {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(bitmapEncoderGuid, stream);
                Stream pixelStream = image.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                          (uint)image.PixelWidth, (uint)image.PixelHeight, 96.0, 96.0, pixels);
                await encoder.FlushAsync();
            }
        }

        private async void NickNameBtn_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null) {
                IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);


                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(readStream);
                WriteableBitmap writeableImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                writeableImage.SetSource(readStream);

                StorageFolder folder;
                folder = ApplicationData.Current.LocalFolder;

                string fileName = DateTimeOffset.UtcNow.ToString();
                fileName = fileName.Replace(":", "").Replace("/", "").Replace("+", "").Replace(" ", "");

                StorageFile saving = await folder.CreateFileAsync(fileName + file.FileType.ToString(), CreationCollisionOption.ReplaceExisting);
                await SaveWriteableBitmapImageFile(writeableImage, saving);

                pageNickNamePath = "ms-appdata:///local/" + fileName + file.FileType.ToString();
                BitmapImage bitMap = new BitmapImage(new Uri(pageNickNamePath));
                NickPic.Source = bitMap;
                ChattingViewModel.UpdateNick(pageNickNamePath);
                this.DataContext = ChattingViewModel;
            }
        }
    }
}
