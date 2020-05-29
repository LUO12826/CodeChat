using CodeChatSDK.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeChatSDK.Models
{
    /// <summary>
    /// 话题
    /// </summary>
    public class Topic
    {

        /// <summary>
        /// 话题名
        /// </summary>
        [Key]
        public string Name { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 更新时间戳
        /// </summary>
        public long Updated { get; set; }

        /// <summary>
        /// 已读消息SeqId
        /// </summary>
        public long Read { get; set; }

        /// <summary>
        /// 已接受消息SeqId
        /// </summary>
        public long Recieve { get; set; }

        /// <summary>
        /// 删除事务最新ID
        /// </summary>
        public long Clear { get; set; }

        /// <summary>
        /// 最新消息时间戳
        /// </summary>
        public long LastUsed { get; set; }

        /// <summary>
        /// 本地最小消息SeqId
        /// </summary>
        public int MinLocalSeqId { get; set; }

        /// <summary>
        /// 本地最大消息SeqId
        /// </summary>
        public int MaxLocalSeqId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// 是否归档
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// 私密备注
        /// </summary>
        public string PrivateComment { get; set; }

        /// <summary>
        /// 加载消息数目限制
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 订阅者列表
        /// </summary>
        public List<Subscriber> SubsriberList { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<ChatMessage> MessageList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">话题名</param>
        public Topic(string name)
        {
            Name = name;
            Limit = 24;
            Status = 0;
            IsVisible = true;
            LastUsed = ChatMessageBuilder.GetTimeStamp();
            SubsriberList = new List<Subscriber>();
            MessageList = new List<ChatMessage>();
        }

        public override bool Equals(object obj)
        {
            Topic topic = obj as Topic;
            return topic!=null && Name.Equals(topic.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name}, {PrivateComment}";
        }
    }
}
