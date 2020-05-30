using CodeChatSDK.Models;
using Newtonsoft.Json;
using Pbx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;

namespace CodeChatSDK.Utils
{
    /// <summary>
    /// 消息构造器
    /// </summary>
    public class ChatMessageBuilder
    {
        /// <summary>
        /// 构建附件消息
        /// </summary>
        /// <param name="attachmentInfo">附件信息</param>
        /// <param name="text">消息</param>
        /// <returns>聊天消息</returns>
        public static ChatMessage BuildAttachmentMessage(UploadedAttachmentInfo attachmentInfo, string text = " ")
        {
            var message = new ChatMessage();
            message.Text = text;
            message.Ent = new List<EntMessage>();
            message.Fmt = new List<FmtMessage>();
            message.Ent.Add(new EntMessage()
            {
                Tp = "EX",
                Data = new EntData()
                {
                    Mime = attachmentInfo.Mime,
                    Name = attachmentInfo.FileName,
                    Ref = attachmentInfo.RelativeUrl,
                    Size = int.Parse(attachmentInfo.Size.ToString()),

                }
            });
            message.Fmt.Add(new FmtMessage()
            {
                At = text.Length,
                Len = 1,
                Key = 0,
            });

            if (text.Contains("\n"))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        FmtMessage fmt = new FmtMessage() { At = i, Tp = "BR", Len = 1 };
                        message.Fmt.Add(fmt);
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// 构建代码消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ChatMessage BuildCodeMessage(CodeType type, string text = "")
        {
            var message = new ChatMessage();
            int baseLen = 0;
            message.Text = text;
            message.Ent = new List<EntMessage>();
            message.Fmt = new List<FmtMessage>();
            if (text.Contains("\r"))
            {
                for (int i = 0; i < text.Length - 1; i++)
                {
                    if (text[i] == '\r')
                    {
                        FmtMessage fmtMessage = new FmtMessage() { At = baseLen + i, Tp = "BR", Len = 1 };
                        message.Fmt.Add(fmtMessage);
                    }
                }
            }

            var leftLen = baseLen + (text.Length - text.TrimStart().Length);
            var subLen = text.Length - text.TrimEnd().Length;
            var validLen = message.Text.Length - leftLen - subLen;

            FmtMessage fmt = new FmtMessage() { Tp = "CO", At = leftLen, Len = validLen };
            message.Fmt.Add(fmt);

            message.Text += type.ToString();

            return message;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns>时间戳</returns>
        public static long GetTimeStamp()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(timeSpan.TotalMilliseconds);
        }
    }
}
