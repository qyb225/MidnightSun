# MidnightSun

> This is a story about murder, betrayal
> 
> This is also a story about love
> 
> This is a journey under the midnight sun

---

Do you ever think about the authenticity of the world?

<img src="./image/game.png" width = "555" height = "387" alt="" />


If the whole world is just a lie, what would you do?

Help her and find all the secrets

<img src="./image/lady0.webp" width = "600" height = "350" alt="" />

----

## 部分技术实现：

### 数据库部分：

剧情数据库table：
**X1.db**

id | msg | Flag
 ------|------|----
1 |  | **online**
2 |  | **time**
3 | Hello！ | 
4 | Are you OK？| 
5 | Yes, I'm OK | **choose0 / choose1**
6 | No, I'm not OK | 
7 | TA好帅! | **moment**
8 | | **offline**
9 | 专家研究认为打代码不会导致脱发 | **news**
10 | 60 | **delay**
11 | | **X2.db**



分支跳转示意图：

<img src="./image/database.png" alt="" />

---

每次加载，并都会存入**process 进度**数据库db文件

id | databse | num | next
 ------|------|----|----
 1 | X1 | 5 | 2017-06-15/21:00:20
 
 ----
 
（1） 如果是聊天信息，通过聊天界面的viewModel更新并存入**chattingInfo（聊天记录）** 数据库
*更新照片时重写所有sender为player的avatarPath*
 
 id | sender |  msg | avatarPath
  ------|------|----|----
  1 | robot | 你好 | ms-appdata:///local/robot.jpg
  2 | player | 你是谁？ | ms-appdata:///local/20170615030220.jpg
  3 | time | 06-15/16:33 | 
  4 | online | 对方已上线 |
  5 | offline | 对方已下线 |


  ---

（2） 如果是朋友圈更新信息，朋友圈的viewModel更新并存入**朋友圈数据库**

 id | article |  image 
  ------|------|----
  1 | TA好帅 | ms-appdata:///local/ta.jpg


  ---
  
 （3） 如果是新闻信息，新闻viewModel更新，存入**新闻数据库**
 
  id | news |  time
  ------|------|----
  1 | 专家研究认为打代码不会导致脱发 | 2017-06-15/22:00

  ---

  ### 界面与后台

  技术难点之聊天窗口多种样式的消息如何正确选择并展示

```
<ScrollViewer x:Name="ChattingScrollViewer"
                Grid.Row="1"
                VerticalScrollBarVisibility="Hidden"
                HorizontalScrollBarVisibility="Disabled">
    <ListView IsItemClickEnabled="False"
                SelectionMode="None"
                x:Name="TheChattingItem"
                SizeChanged="TheChattingItem_SizeChanged"
                ItemTemplateSelector="{StaticResource MessageItemDataTemplateSelector}"
                ItemsSource="{Binding AllItems}"
                HorizontalAlignment="Stretch">
        <ListView.ItemContainerStyle>
            <Style TargetType="ListViewItem">
                <Setter Property="HorizontalContentAlignment" Value="Stretch"/>
            </Style>
        </ListView.ItemContainerStyle>
    </ListView>
</ScrollViewer>
```

这是聊天窗口界面的最中间的核心部分。可以看到我们使用了ListView。但是我们没有直接将他绑定一个用户控件，为什么不这样做是有原因的。因为直接绑定一种DataTemplate虽然简单，但是不能实现我们这里复杂的情况。比如说对方的消息，和自己发出的消息，虽然在同一个ListView里，他们都是item但是他们要表现的样式是差别很大的，从位置到颜色等等。也不是没有想过最简单的写法就是隐藏和可视化不同的部件，达到好像看起来不同的消息有不同的效果。但是这样一来代码变得复杂，而且不利于进一步需求的增加，比如要是现在我要进一步支持图片类型的消息。这样就很难继续修改了。
然后经过在网上寻找解决方案，最后找到ItemTemplateSelector的方法解决。
首先再listview中间添加一个属性ItemTemplateSelector="{StaticResource MessageItemDataTemplateSelector}"，这里面指向一个静态资源。
在app里面可以找到这个静态资源
```
<selector:MessageItemDataTemplateSelector x:Key="MessageItemDataTemplateSelector"></selector:MessageItemDataTemplateSelector>
```
其实他指向的是下面这一段代码
这是一个选择器，将会根据数据中的标签选择不同的模板。
```
namespace Midnight.Selector {
    public class MessageItemDataTemplateSelector : DataTemplateSelector {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) {
            if (item is ChattingItems) {
                /*
                 * 标签
                 * 0 代表对方发出的消息
                 * 1 代表自己发出的消息
                 * 2 代表系统发出的消息的上线提示
                 * 3 代表系统发出的消息的下线提示
                 * 4 代表系统发出的朋友圈提醒消息
                 * 5 代表系统发出de时间消息
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
```