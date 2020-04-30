using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Windows.Devices.SmartCards;

namespace CAC.client.MessagePage
{
    sealed partial class ChatListControl : UserControl
    {
        public ChatListViewModel VM;
        public ChatListControl()
        {
            VM = MessagePageViewModel.ChatListViewModel;
            this.InitializeComponent();

        }

        //当鼠标进入某个cell时，显示关闭按钮
        private void cellGrid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            (sender as Grid).Children[0].Visibility = Visibility.Visible;
        }
        //当鼠标移出某个cell时，隐藏关闭按钮
        private void cellGrid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            (sender as Grid).Children[0].Visibility = Visibility.Collapsed;
        }
        //点击了移除cell的按钮时
        private void RemoveCellBtn_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Debug.WriteLine((sender as Button).DataContext);
        }

        //点击了某个cell时
        private void ChatsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.DidSelectChat(e.ClickedItem as ChatListBaseItemVM);
        }
    }
}
