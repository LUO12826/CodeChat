using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CAC.client.Common;
using CAC.client;
using System.Threading.Tasks;
using System.Threading;

namespace CAC.client.MessagePage
{

    /*
     * 
     *本来计划用官方推荐的数据虚拟化方法（实现ISupportIncrementalLoading的集合）
     * 来实现增量加载。但是发现这种方法只会在往下滚动时触发加载，但消息浏览器需要
     * 的是往上滚动时触发加载，所以还是得靠监听滚动位置的方式引发数据加载。
     */

    sealed partial class MessageViewer : UserControl, INotifyPropertyChanged
    {
        public IncrementalCollection<MessageItemBaseVM> Messages;
        private ScrollViewer _scrollViewer;

        //测试，统一控制cell的颜色
        private Brush _BubbleBgColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255 ,244, 244, 244));
        public Brush BubbleBgColor {
            get => _BubbleBgColor;
            set {
                _BubbleBgColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BubbleBgColor)));
            }
        }

        private async Task<Tuple<List<MessageItemBaseVM>, bool>> loadMoreItems(uint itemsNum) {
            var a = await Task.Run(() => {
                Thread.Sleep(2000);
                var b = new List<MessageItemBaseVM>();
                for(int i = 0; i < itemsNum; i++) {

                    b.Add(new TextMessageVM() {
                        NickName = "bbb",
                        Text = "新加项目",
                        Base64Avatar = GlobalConfigs.testB64Avator
                    });
                }
                return b;
            });

            return new Tuple<List<MessageItemBaseVM>, bool>(a, false);
        }

        public MessageViewer()
        {
            this.InitializeComponent();
            this.MessageViewerList.DataContext = this;

            Messages = new IncrementalCollection<MessageItemBaseVM>(loadMoreItems);
            for(int i = 0; i < 40; i++) {
                Messages.Add(new TextMessageVM() {
                    Base64Avatar = GlobalConfigs.testB64Avator,
                    Text = "aaaaaaaaaaaaaaaa",
                    NickName = "aaa",
                    SendByMe = i % 2 == 0 ? true : false
                });
            }
            var b = new ImageMessageVM() {
                Base64Avatar = GlobalConfigs.testB64Avator,
                ImageBase64 = GlobalConfigs.testB64Avator,
                NickName = "aaa",
                SendByMe = false
            };
            Messages.Add(b);
        }

        public event PropertyChangedEventHandler PropertyChanged;


        //ListView加载完成的回调
        private void MessageViewerList_Loaded(object sender, RoutedEventArgs e)
        {
            //获取ListView的ScrollViewer
            _scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(MessageViewerList, 0), 0) as ScrollViewer;
            _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
        }

        //各种原因引起ScrollViewer内容改变都会调用这个方法
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Debug.WriteLine((sender as ScrollViewer).VerticalOffset);
            Debug.WriteLine("-----------");
            Debug.WriteLine((sender as ScrollViewer).ScrollableHeight);
            Debug.WriteLine("-----------");
            Debug.WriteLine((sender as ScrollViewer).ActualHeight);

        }
    }
}
