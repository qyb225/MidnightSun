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
    public sealed partial class BadEnd : Page {
        public BadEnd() {
            this.InitializeComponent();
            string[] failMsg;
            failMsg = new string[4] { "这得是多厉害才能提出这些建议啊", "子曰：“失败是成功他妈”",
                "能不能认真一点，女主对你很失望啊", "对力量一无所知，你果然无所畏惧……" };
            Laugh.Text = GetRandom(failMsg);
        }

        private void Again_Click(object sender, RoutedEventArgs e) {
            this.Frame.Navigate(typeof(ChattingPage));
        }

        private string GetRandom(string[] arr) {
            Random ran = new Random();
            int n = ran.Next(arr.Length - 1);
            return arr[n];
        }
    }
}
