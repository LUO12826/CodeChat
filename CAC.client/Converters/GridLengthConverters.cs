using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;

namespace CAC.client.Converters
{
    /// <summary>
    /// 从double到Grid的GridLength的转换
    /// </summary>
    class DoubleToGridLength : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new GridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return  ((GridLength)value).Value;
        }
    }

    /// <summary>
    /// 从Grid的GridLength到double的转换
    /// </summary>
    class GridLengthToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((GridLength)value).Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new GridLength((double)value);
        }
    }
}
