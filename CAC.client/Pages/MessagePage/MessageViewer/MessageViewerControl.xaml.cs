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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace CAC.client.MessagePage
{

    /*
     * 
     *本来计划用官方推荐的数据虚拟化方法（实现ISupportIncrementalLoading的集合）
     * 来实现增量加载。但是发现这种方法只会在往下滚动时触发加载，但消息浏览器需要
     * 的是往上滚动时触发加载，所以还是得靠监听滚动位置的方式引发数据加载。
     */

    sealed partial class MessageViewer : UserControl
    {

        private ScrollViewer _scrollViewer;
        private bool scrollViewerLoaded = false;
        public string testa { get; set; } = CAC.client.GlobalConfigs.testB64Avator;

        public static readonly DependencyProperty VMProperty =
            DependencyProperty.Register("VM", typeof(MessageViewerViewModel), typeof(MessageViewer), new PropertyMetadata(null, VMChanged));
        public MessageViewerViewModel VM {
            get { return (MessageViewerViewModel)GetValue(VMProperty); }
            set { SetValue(VMProperty, value); }
        }

        //当viewModel变化时。现在viewModel被设计为不变的（一个view始终对应一个vm，保存多个view），因此用不到了
        private static void VMChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MessageViewer viewer) {
                
                //这些本来是为保存滚动偏移设计的，现在不需要了。
                var VM = e.OldValue as MessageViewerViewModel;
                var newVM = e.NewValue as MessageViewerViewModel;

            }
        }

        public MessageViewer()
        {
            this.InitializeComponent();
            this.MessageViewerList.DataContext = this;
            VM = new MessageViewerViewModel();
        }

        private void willChangeVM(ChatListBaseItemVM chat)
        {
            if (!scrollViewerLoaded)
                return;
            VM.VerticalScrollOffset = _scrollViewer.VerticalOffset;
        }

        private void MessageViewerList_Loaded(object sender, RoutedEventArgs e)
        {
            TryLoadScrollViewer();
        }

        //各种原因引起ScrollViewer内容改变都会调用这个方法
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
        }


        //尝试加载scrollViewer。
        //暂时不需要
        //原本尝试通过保存滚动偏移的方式恢复滚动位置，后来发现有诸多问题。
        public void TryLoadScrollViewer()
        {
            if (!scrollViewerLoaded) {
                var border = VisualTreeHelper.GetChild(MessageViewerList, 0);
                if (border == null)
                    return;
                var scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
                if (scrollViewer == null)
                    return;
                _scrollViewer = scrollViewer;
                _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;
                scrollViewerLoaded = true;
            }
        }


        public double GetVerticalOffset()
        {
            if (_scrollViewer == null)
                return 0.0;
            return _scrollViewer.VerticalOffset;
        }

        public void SetVerticalOffset(double offset)
        {
            //改变滚动位置
            _scrollViewer?.ChangeView(null, offset, null, false);
        }
    }
}
