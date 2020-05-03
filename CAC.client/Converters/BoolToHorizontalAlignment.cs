using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CAC.client.Converters
{
    /// <summary>
    /// 将布尔值转换为UI元素水平停靠位置。
    /// </summary>
    class BoolToHorizontalAlignment : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //真为靠右，假为靠左。
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var val = (HorizontalAlignment)value;
            return val == HorizontalAlignment.Right ? true : false;
        }
    }
}
