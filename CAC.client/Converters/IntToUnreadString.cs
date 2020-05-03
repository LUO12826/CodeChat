using System;
using Windows.UI.Xaml.Data;

namespace CAC.client.Converters
{
    /// <summary>
    /// 将int转换为未读计数。
    /// </summary>
    class IntToUnreadString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int temp = (int)value;
            return temp > 99 ? "99+" : temp.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
