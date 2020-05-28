using CodeChatSDK.Message;
using Newtonsoft.Json;
using Pbx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK
{
    /// <summary>
    /// 消息构造器
    /// </summary>
    public class MessageBuilder
    {
        /// <summary>
        /// 解析服务器信息
        /// </summary>
        /// <param name="message">服务器信息</param>
        /// <returns>聊天消息</returns>
        public static ChatMessage Parse(ServerData message)
        {
            ChatMessage chatMsg;
            if (message.Head.ContainsKey("mime"))
            {
                chatMsg = JsonConvert.DeserializeObject<ChatMessage>(message.Content.ToStringUtf8()
                        , new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                chatMsg.Content = message.Content.ToStringUtf8();
                chatMsg.SeqId = message.SeqId;
                chatMsg.TopicName = message.Topic;
                chatMsg.IsPlainText = false;
                ParseCode(chatMsg);
            }
            else
            {
                chatMsg = new ChatMessage() { Text = JsonConvert.DeserializeObject<string>(message.Content.ToStringUtf8()) };
                chatMsg.Content = message.Content.ToStringUtf8();
                chatMsg.SeqId = message.SeqId;
                chatMsg.TopicName = message.Topic;
                chatMsg.IsPlainText = true;
            }

            if (message.Topic.StartsWith("usr"))
            {
                chatMsg.MessageType = "user";
            }
            else if (message.Topic.StartsWith("grp"))
            {
                chatMsg.MessageType = "group";
            }
            return chatMsg;
        }

        /// <summary>
        /// 解析消息Content
        /// </summary>
        /// <param name="message"></param>
        public static void ParseContent(ChatMessage message)
        {
            if (message.Content == null)
            {
                return;
            }

            if (message.IsPlainText == true)
            {
                message.Text = JsonConvert.DeserializeObject<string>(message.Content);
            }
            else
            {
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message.Content
                        , new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                message.Ent = chatMessage.Ent;
                message.Fmt = chatMessage.Fmt;
                message.Text = chatMessage.Text;
            }
        }

        /// <summary>
        /// 解析代码
        /// </summary>
        /// <param name="message"></param>
        public static void ParseCode(ChatMessage message)
        {
            if (message.Text == null || message.Fmt == null)
            {
                return;
            }
            foreach (var fmt in message.Fmt)
            {
                if (!string.IsNullOrEmpty(fmt.Tp))
                {
                    if (fmt.Tp == "CO")
                    {
                        string code = message.Text.Substring(fmt.At.Value, fmt.Len.Value);
                        string type = message.Text.Substring(fmt.At.Value + fmt.Len.Value);
                        CodeType codeType = (CodeType)Enum.Parse(typeof(CodeType), type);
                        message.Text = code;
                        message.IsCode = true;
                        message.CodeType = codeType;
                    }
                }
            }
        }

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
        /// 将Base64转为图像
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns>Bitmap</returns>
        static public Bitmap ConvertBase64ToImage(string base64String)
        {
            byte[] imageArray = Convert.FromBase64String(base64String);
            Bitmap image = null;
            using (MemoryStream stream = new MemoryStream(imageArray))
            {
                image = new Bitmap(stream);
            }
            return image;
        }


        /// <summary>
        /// 将图像转换为Base64
        /// </summary>
        /// <param name="image">转化图像</param>
        /// <param name="format">图像格式</param>
        /// <returns>Base64字符串</returns>
        static public async Task<string> ConvertImageToBase64(Bitmap image, ImageFormat format)
        {
            string base64String = "";
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, format);
                byte[] array = new byte[stream.Length];
                stream.Position = 0;
                await stream.ReadAsync(array, 0, (int)stream.Length);
                base64String = Convert.ToBase64String(array);
            }
            return base64String;
        }
    }
}
