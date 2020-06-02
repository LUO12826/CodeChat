
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CAC.client
{
    class GlobalFunctions
    {
        /// <summary>
        /// 尝试更改窗口尺寸。
        /// </summary>
        public static bool TryResizeWindow(int widthIncrement, int heightIncrement)
        {
            var width = Window.Current.Bounds.Width;
            var height = Window.Current.Bounds.Height;
            return ApplicationView.GetForCurrentView().TryResizeView(new Windows.Foundation.Size(width + widthIncrement, height + heightIncrement));
        }


        public static int FindPosInLangList(string lang)
        {
            var list = GlobalConfigs.HighlightLanguageListLower;
            for (int i = 0; i < list.Length; i++) {
                if (list[i] == lang)
                    return i;
            }
            return -1;
        }
    }
}
