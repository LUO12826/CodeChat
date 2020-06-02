
using CodeChatSDK.Models;
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

        public static DateTime TimestampToDateTime(long ts)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(ts).ToLocalTime();
        }

        /// <summary>
        /// 将sdk中的代码类型枚举转换为小写字符串
        /// </summary>
        public static string SDKcodeTypeToString(CodeType type)
        {
            switch (type) {
                case CodeType.C:
                    return "cplusplus";
                case CodeType.CPP:
                    return "cplusplus";
                case CodeType.CSHARP:
                    return "csharp";
                case CodeType.CSS:
                    return "css";
                case CodeType.HTML:
                    return "html";
                case CodeType.JAVA:
                    return "java";
                case CodeType.JAVASRCIPT:
                    return "javascript";
                case CodeType.NULL:
                    return "plaintext";
                case CodeType.PYTHON:
                    return "python";
                case CodeType.SWIFT:
                    return "plaintext";
                case CodeType.XML:
                    return "xml";
                default:
                    return "plaintext";
            }
        }

        /// <summary>
        /// 将小写字符串转换为sdk中的代码类型枚举。
        /// </summary>
        public static CodeType StringToSDKcodeType(string lang)
        {
            switch (lang) {
                case "cplusplus":
                    return CodeType.CPP;
                case "csharp":
                    return CodeType.CSHARP;
                case "css":
                    return CodeType.CSS;
                case "html":
                    return CodeType.HTML;
                case "java":
                    return CodeType.JAVA;
                case "javascript":
                    return CodeType.JAVASRCIPT;
                case "plaintext":
                    return CodeType.NULL;
                case "python":
                    return CodeType.PYTHON;
                case "xml":
                    return CodeType.XML;
                default:
                    return CodeType.NULL;
            }
        }
    }
}
