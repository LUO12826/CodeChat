using Newtonsoft.Json;
using Pbx;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    public class MessageBuilder
    {
        /// <summary>
        /// 解析服务器信息
        /// </summary>
        /// <param name="message">服务器信息</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
    }

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

        public List<EntData> GetMentions()
        {
            return GetEntDatas("MN");
        }

        public List<EntData> GetImages()
        {
            return GetEntDatas("IM");
        }

        public List<EntData> GetHashTags()
        {
            return GetEntDatas("HT");
        }

        public List<EntData> GetLinks()
        {
            return GetEntDatas("LN");
        }

        public List<EntData> GetGenericAttachment()
        {
            return GetEntDatas("EX");
        }


    }

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

    public class EntMessage : MessageBase
    {
        [JsonProperty("tp")]
        public string Tp { get; set; }
        [JsonProperty("data")]
        public EntData Data { get; set; }
    }

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
