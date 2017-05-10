using Midnight.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Midnight.Selector {
    public class MessageItemDataTemplateSelector : DataTemplateSelector {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) {
            if (item is ChattingItems) {
                /*
                 * 1 means yourself
                 */
                if ((item as ChattingItems).Sender == 1) {
                    return App.Current.Resources["SelfMessageDataTemplate"] as DataTemplate;
                } else if ((item as ChattingItems).Sender == 0) {
                    return App.Current.Resources["MessageDataTemplate"] as DataTemplate;
                } else if ((item as ChattingItems).Sender == 2) {
                    return App.Current.Resources["OnlineDataTemplate"] as DataTemplate;
                } else if ((item as ChattingItems).Sender == 3) {
                    return App.Current.Resources["OfflineDataTemplate"] as DataTemplate;
                } else if ((item as ChattingItems).Sender == 4) {
                    return App.Current.Resources["SendMomentDataTemplate"] as DataTemplate;
                } else if ((item as ChattingItems).Sender == 5) {
                    return App.Current.Resources["TimeDataTemplate"] as DataTemplate;
                }
            }

            return base.SelectTemplateCore(item);
        }
    }
}
