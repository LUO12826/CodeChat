using CodeChatSDK.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace CodeChatSDK
{
    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage : MessageBase
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [JsonIgnore]
        public long Id { get; set; }

        /// <summary>
        /// 所属话题名
        /// </summary>
        [JsonIgnore]
        [ForeignKey("TopicName")]
        public string TopicName { get; set; }

        /// <summary>
        /// 消息文本
        /// </summary>
        [NotMapped]
        [JsonProperty("txt")]
        public string Text { get; set; }

        /// <summary>
        /// 富文本格式列表
        /// </summary>
        [NotMapped]
        [JsonProperty("fmt")]
        public List<FmtMessage> Fmt { get; set; }

        /// <summary>
        /// 实体数据列表
        /// </summary>
        [NotMapped]
        [JsonProperty("ent")]
        public List<EntMessage> Ent { get; set; }

        /// <summary>
        /// 是否为普通文本
        /// </summary>
        [JsonIgnore]
        public bool IsPlainText { get; set; }

        /// <summary>
        /// 是否为代码
        /// </summary>
        [JsonIgnore]
        public bool IsCode { get; set; }

        /// <summary>
        /// 代码类型
        /// </summary>
        [JsonIgnore]
        public CodeType CodeType { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [JsonIgnore]
        public string MessageType { get; set; }

        /// <summary>
        /// 消息序号
        /// </summary>
        [JsonIgnore]
        public int SeqId { get; set; }
        
        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonIgnore]
        public string Content { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChatMessage()
        {
            Text = "";
            MessageType = "user";
        }


        /// <summary>
        /// 获取富文本内容
        /// </summary>
        /// <returns>富文本内容</returns>
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
        /// 获取实体数据列表
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

        public override bool Equals(object obj)
        {
            ChatMessage message = obj as ChatMessage;
            return message != null && this.SeqId == message.SeqId;
        }

        public override int GetHashCode()
        {
            return (int)SeqId;
        }
    }
}
