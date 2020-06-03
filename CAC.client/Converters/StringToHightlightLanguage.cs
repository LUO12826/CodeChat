using System;
using RichTextControls;
using Windows.UI.Xaml.Data;

namespace CAC.client.Converters
{
    /// <summary>
    /// 将字符串转为代码高亮类型的枚举
    /// </summary>
    class StringToHightlightLanguage: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return HighlightLanguage.PlainText;

            string lang = ((string)value).ToLower();
            switch (lang) {
                case "plaintext":
                    return HighlightLanguage.PlainText;

                case "cplusplus":
                    return HighlightLanguage.CPlusPlus;

                case "csharp":
                    return HighlightLanguage.CSharp;

                case "java":
                    return HighlightLanguage.Java;

                case "javascript":
                    return HighlightLanguage.JavaScript;

                case "css":
                    return HighlightLanguage.CSS;

                case "json":
                    return HighlightLanguage.JSON;

                case "php":
                    return HighlightLanguage.PHP;

                case "python":
                    return HighlightLanguage.Python;

                case "ruby":
                    return HighlightLanguage.Ruby;

                case "sql":
                    return HighlightLanguage.SQL;

                case "xml":
                    return HighlightLanguage.XML;

                default:
                    return HighlightLanguage.PlainText;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
