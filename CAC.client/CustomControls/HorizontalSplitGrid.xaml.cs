
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;



namespace CAC.client.CustomControls
{
    /// <summary>
    /// 将内容一分为二的网格。用于代码编辑器打开的情形：左边是聊天会话，右边是代码编辑器。
    /// </summary>
    sealed partial class HorizontalSplitGrid : UserControl
    {
        private double width;
        private bool dragEnabled = false;

        public bool Expanded { get; private set; }

        public HorizontalSplitGrid()
        {
            this.InitializeComponent();
            DataContext = this;
            width = 0;
            Splitter.IsEnabled = false;
            Detailcolumn.Width = new GridLength(width);
            Splitter.AddHandler(Button.PointerPressedEvent, new PointerEventHandler(Splitter_PointerPressed), true);
            Splitter.AddHandler(Button.PointerReleasedEvent, new PointerEventHandler(Splitter_PointerReleased), true);
        }

        #region 依赖属性

        public static readonly DependencyProperty MasterContentProperty =
            DependencyProperty.Register("MasterContent", typeof(object), typeof(ResizableMasterDetail), new PropertyMetadata(new ContentControl()));
        public object MasterContent {
            get { return (object)GetValue(MasterContentProperty); }
            set { SetValue(MasterContentProperty, value); }
        }

        public static readonly DependencyProperty DetailContentProperty =
            DependencyProperty.Register("DetailContent", typeof(object), typeof(ResizableMasterDetail), new PropertyMetadata(new ContentControl()));
        public object DetailContent {
            get { return (object)GetValue(DetailContentProperty); }
            set { SetValue(DetailContentProperty, value); }
        }

        public static readonly DependencyProperty SplitterWidthProperty =
            DependencyProperty.Register("SplitterWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(2.0));
        public double SplitterWidth {
            get { return (double)GetValue(SplitterWidthProperty); }
            set { SetValue(SplitterWidthProperty, value); }
        }

        public static readonly DependencyProperty DetailMinWidthProperty =
            DependencyProperty.Register("DetailMinWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(300.0));
        public double DetailMinWidth {
            get { return (double)GetValue(DetailMinWidthProperty); }
            set { SetValue(DetailMinWidthProperty, value); }
        }

        public static readonly DependencyProperty MasterMinWidthProperty =
            DependencyProperty.Register("MasterMinWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(400.0));
        public double MasterMinWidth {
            get { return (double)GetValue(MasterMinWidthProperty); }
            set { SetValue(MasterMinWidthProperty, value); }
        }

        #endregion

        public void Expan(int width)
        {
            this.Expanded = true;
            this.width = width;
            Detailcolumn.Width = new GridLength(width);
            SplitterColumn.Width = new GridLength(SplitterWidth);
            Splitter.IsEnabled = true;
        }

        public void Collapse()
        {
            this.Expanded = true;
            width = 0;
            SplitterColumn.Width = new GridLength(0);
            Detailcolumn.Width = new GridLength(width);
            Splitter.IsEnabled = false;
        }

        private void Splitter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            PointerPoint point = e.GetCurrentPoint(btn);
        
            if (point.Properties.IsLeftButtonPressed && dragEnabled) {
                width -= point.Position.X;
                if (width <= DetailMinWidth)
                    width = DetailMinWidth;
                else if (this.ActualWidth - width <= MasterMinWidth)
                    width = this.ActualWidth - MasterMinWidth;
                Detailcolumn.Width = new GridLength(width);
            }

        }

        private void Splitter_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            dragEnabled = true;
        }

        private void Splitter_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            dragEnabled = false;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private void Splitter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeWestEast, 1);
        }

        private void Splitter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!dragEnabled)
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}
