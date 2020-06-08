using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CAC.client.Converters
{
    /// <summary>
    /// 将布尔值转换为UI元素可见性。
    /// </summary>
    class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = (Visibility)value;
            return val == Visibility.Visible ? true : false;
        }
    }

    /// <summary>
    /// 将布尔值转换为UI元素可见性。反转
    /// </summary>
    class BoolToVisibilityInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = (Visibility)value;
            return val == Visibility.Visible ? false : true;
        }
    }
}
