
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CAC.client
{
    class GlobalFunctions
    {
        /// <summary>
        /// 尝试更改窗口尺寸。
        /// </summary>
        public static void TryResizeWindow(int widthIncrement, int heightIncrement)
        {
            var width = Window.Current.Bounds.Width;
            var height = Window.Current.Bounds.Height;
            ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(width + widthIncrement, height + heightIncrement));
        }
    }
}
