
using CAC.client.MessagePage;
using CodeChatSDK.Models;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CAC.client
{
    /// <summary>
    /// 全局方法。
    /// </summary>
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

        /// <summary>
        /// 找到一种语言在语言列表中的位置。
        /// </summary>
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

        /// <summary>
        /// 在一个字符串数组中找
        /// </summary>
        public static int FindPosInImageMineList(string[] list, string name)
        {
            if (list == null)
                return -1;
            for(int i = 0; i < list.Length; i++) {
                if (list[i] == name) {
                    return i;
                }
            }
            return -1;
        }

        public static string MessageToLatestString(MessageItemBaseVM msg)
        {
            string latestMsg = "";
            if (msg is TextMessageVM t) {
                latestMsg = t.Text;
            }
            else if (msg is CodeMessageVM c) {
                latestMsg = "[代码]";
            }
            else if (msg is FileMessageVM f) {
                latestMsg = f.FileName;
            }
            else if (msg is ImageMessageVM i) {
                latestMsg = "[图片]";
            }
            return latestMsg;
        }
    }
}
