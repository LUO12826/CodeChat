using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CAC.client.CustomControls
{
    /// <summary>
    /// 最右侧导航栏。没做成一个通用性很强的组件。
    /// </summary>
    sealed partial class Navigator : UserControl
    {
        public event EventHandler<object> OnNavigationChanged;

        public static readonly DependencyProperty AvatarProperty =
            DependencyProperty.Register("Avatar", typeof(string), typeof(Navigator), new PropertyMetadata(""));
        public string Avatar {
            get { return (string)GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        public static readonly DependencyProperty UnreadCountProperty =
            DependencyProperty.Register("UnreadCount", typeof(int), typeof(Navigator), new PropertyMetadata(0));
        public int UnreadCount {
            get { return (int)GetValue(UnreadCountProperty); }
            set { SetValue(UnreadCountProperty, value); }
        }


        public Navigator()
        {
            this.InitializeComponent();
            this.DataContext = this;
            OnNavigationChanged += (a, b) => { };
        }

        private void naviItemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (sender == naviItemList) {
                additionItemList.SelectedItem = null;
            }
            else if (sender == additionItemList) {
                naviItemList.SelectedItem = null;
            }
            else {

            }
            OnNavigationChanged(this, (e.ClickedItem as Grid).Tag);
        }

        private void avatar_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            naviItemList.SelectedItem = null;
            additionItemList.SelectedItem = null;
            OnNavigationChanged(this, (sender as ImageEx).Tag);
        }
    }

    class NavigatorItem
    {
        public string Symbol { get; set; }
        public int FontSize { get; set; }
        public string Tag { get; set; }
    }
}
