using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChatSDK.Models
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
        /// 消息来自
        /// </summary>
        [JsonIgnore]
        public string From { get; set; }

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
        /// 是否已读
        /// </summary>
        [JsonIgnore]
        public bool IsRead { get; set; }
        
        /// <summary>
        /// 是否含有附件
        /// </summary>
        [JsonIgnore]
        public bool IsAttachment { get; set; }

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
