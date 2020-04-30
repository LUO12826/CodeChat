using System;
using Windows.UI.Xaml.Data;
using CAC.client.Common;

namespace CAC.client.Converters
{
    /// <summary>
    /// 将日期转化为简短的时间提示字符串。
    /// 主要用于聊天列表的列表项中。
    /// </summary>
    class DateTimeToShortString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((DateTime)value).ToExplicitString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("无法将便捷日期转换回标准日期");
        }

    }
}
