using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using Windows.UI.Input;
using System;

namespace CAC.client.CustomControls
{
    /// <summary>
    /// 可以改变master栏宽度的master-detail视图。
    /// </summary>
    sealed partial class ResizableMasterDetail : UserControl
    {
        private double width;
        private bool dragEnabled = false;

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

        public static readonly DependencyProperty MasterMaxWidthProperty =
            DependencyProperty.Register("MasterMaxWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(400.0));
        public double MasterMaxWidth {
            get { return (double)GetValue(MasterMaxWidthProperty); }
            set { SetValue(MasterMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty MasterMinWidthProperty =
            DependencyProperty.Register("MasterMinWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(80.0));
        public double MasterMinWidth {
            get { return (double)GetValue(MasterMinWidthProperty); }
            set { SetValue(MasterMinWidthProperty, value); }
        }

        public static readonly DependencyProperty MasterThresholdWidthProperty =
            DependencyProperty.Register("MasterThresholdWidth", typeof(double), typeof(ResizableMasterDetail), new PropertyMetadata(200.0));
        public double MasterThresholdWidth {
            get { return (double)GetValue(MasterThresholdWidthProperty); }
            set { SetValue(MasterThresholdWidthProperty, value); }
        }

        public static readonly DependencyProperty MasterDefaultWidthProperty =
            DependencyProperty.Register("MasterDefaultWidth", typeof(double), 
            typeof(ResizableMasterDetail), new PropertyMetadata(250.0, new PropertyChangedCallback(MasterDefaultWidthChanged)));

        public double MasterDefaultWidth {
            get { return (double)GetValue(MasterDefaultWidthProperty); }
            set { SetValue(MasterDefaultWidthProperty, value); }
        }

        private static void MasterDefaultWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ResizableMasterDetail rmd) {
                rmd.width = (double)e.NewValue;
                rmd.MasterColumn.Width = new GridLength((double)e.NewValue);
            }
        }

        #endregion

        public ResizableMasterDetail()
        {
            this.InitializeComponent();
            DataContext = this;
            width = MasterDefaultWidth;
            MasterColumn.Width = new GridLength(width);
            Splitter.AddHandler(Button.PointerPressedEvent, new PointerEventHandler(Splitter_PointerPressed), true);
            Splitter.AddHandler(Button.PointerReleasedEvent, new PointerEventHandler(Splitter_PointerReleased), true);
        }

        private void Splitter_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Button btn = sender as Button;
            PointerPoint point = e.GetCurrentPoint(btn);

            if (point.Properties.IsLeftButtonPressed && dragEnabled) {
                width += point.Position.X;
                if (width < MasterMinWidth)
                    width = MasterMinWidth;
                else if (width > MasterMaxWidth)
                    width = MasterMaxWidth;
                else if (width < MasterThresholdWidth)
                    width = MasterThresholdWidth;
                MasterColumn.Width = new GridLength(width);
            }

        }

        private double valueThreshold(double input, double Threshold)
        {
            if ((input > 0 && input < Threshold) || (input < 0 && input > -Threshold))
                return 0.0;
            return input;
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
            if(!dragEnabled)
                Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }
    }
}
