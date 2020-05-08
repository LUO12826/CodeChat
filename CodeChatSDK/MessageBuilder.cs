using Newtonsoft.Json;
using Pbx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

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
                chatMsg.IsPlainText = false;
            }
            else
            {
                chatMsg = new ChatMessage() { Text = JsonConvert.DeserializeObject<string>(message.Content.ToStringUtf8()),SeqId=message.SeqId };
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
        /// 构建附件消息
        /// </summary>
        /// <param name="attachmentInfo">附件信息</param>
        /// <param name="text">消息</param>
        /// <returns>聊天消息</returns>
        public static ChatMessage BuildAttachmentMessage(UploadedAttachmentInfo attachmentInfo, string text = " ")
        {
            var msg = new ChatMessage();
            msg.Text = text;
            msg.Ent = new List<EntMessage>();
            msg.Fmt = new List<FmtMessage>();
            msg.Ent.Add(new EntMessage()
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
            msg.Fmt.Add(new FmtMessage()
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
                        msg.Fmt.Add(fmt);
                    }
                }
            }
            return msg;
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
            using (MemoryStream stream=new MemoryStream(imageArray))
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

    /// <summary>
    /// 消息基类
    /// </summary>
    public class MessageBase
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }

    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage : MessageBase
    {

        [JsonProperty("txt")]
        public string Text { get; set; }
        [JsonProperty("fmt")]
        public List<FmtMessage> Fmt { get; set; }
        [JsonProperty("ent")]
        public List<EntMessage> Ent { get; set; }
        [JsonIgnore]
        public bool IsPlainText { get; set; }
        [JsonIgnore]
        public string MessageType { get; set; }
        [JsonIgnore]
        public int SeqId { get; set; }

        public ChatMessage()
        {
            Text = "";
            MessageType = "unknown";
        }


        /// <summary>
        /// 获取富文本格式
        /// </summary>
        /// <returns>富文本格式</returns>
        public string GetFormattedText()
        {
            if (Text == null)
            {
                return null;
            }
            var textArray = Text.ToCharArray();
            if (Fmt != null)
            {
                foreach (var fmt in Fmt)
                {
                    if (!string.IsNullOrEmpty(fmt.Tp))
                    {
                        if (fmt.Tp == "BR")
                        {
                            textArray[fmt.At.Value] = '\n';
                        }
                    }
                }
            }
            return new String(textArray);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="tp">话题名</param>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetEntDatas(string tp)
        {
            var ret = new List<EntData>();
            if (Ent != null)
            {
                foreach (var ent in Ent)
                {
                    if (ent.Tp == tp)
                    {
                        ret.Add(ent.Data);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取提到
        /// </summary>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetMentions()
        {
            return GetEntDatas("MN");
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetImages()
        {
            return GetEntDatas("IM");
        }

        /// <summary>
        /// 获取哈希标签
        /// </summary>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetHashTags()
        {
            return GetEntDatas("HT");
        }

        /// <summary>
        /// 获取超链接
        /// </summary>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetLinks()
        {
            return GetEntDatas("LN");
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <returns>实体数据列表</returns>
        public List<EntData> GetGenericAttachment()
        {
            return GetEntDatas("EX");
        }


    }

    /// <summary>
    /// 格式消息
    /// </summary>
    public class FmtMessage : MessageBase
    {
        [JsonProperty("at")]
        public int? At { get; set; }

        [JsonProperty("len")]
        public int? Len { get; set; }

        [JsonProperty("tp")]
        public string Tp { get; set; }


        [JsonProperty("key")]
        public int? Key { get; set; }
    }

    /// <summary>
    /// 实体消息
    /// </summary>
    public class EntMessage : MessageBase
    {
        [JsonProperty("tp")]
        public string Tp { get; set; }
        [JsonProperty("data")]
        public EntData Data { get; set; }
    }

    /// <summary>
    /// 实体数据
    /// </summary>
    public class EntData : MessageBase
    {
        [JsonProperty("mime")]
        public string Mime { get; set; }
        [JsonProperty("val")]
        public dynamic Val { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("ref")]
        public string Ref { get; set; }
        [JsonProperty("width")]
        public int? Width { get; set; }
        [JsonProperty("height")]
        public int? Height { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("size")]
        public int? Size { get; set; }
        [JsonProperty("act")]
        public string Act { get; set; }
    }
}
