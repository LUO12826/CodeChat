using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CodeChatSDK
{
    /// <summary>
    /// 话题
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 话题名
        /// </summary>
        public string Name { get; private set; }

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
        /// 创建时间戳
        /// </summary>
        public long Created { get; set; }

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
        /// 已执行删除事务ID
        /// </summary>
        public int MaxDeleteId { get; set; }

        /// <summary>
        /// 最新消息时间戳
        /// </summary>
        public long LastUsed { get; set; }

        /// <summary>
        /// 本地最小消息SeqId
        /// </summary>
        public long MinLocalSeqId { get; set; }

        /// <summary>
        /// 本地最大消息SeqId
        /// </summary>
        public long MaxLocalSeqId { get; set; }

        /// <summary>
        /// 下个可用SeqId
        /// </summary>
        public long NextSeqId { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; private set; }

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
        /// 订阅者列表
        /// </summary>
        public ObservableCollection<Subscriber> SubsriberList { get; private set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public ObservableCollection<ChatMessage> MessageList { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">话题名</param>
        public Topic(string name)
        {
            Name = name;
            SubsriberList = new ObservableCollection<Subscriber>();
            MessageList = new ObservableCollection<ChatMessage>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">话题名</param>
        /// <param name="type">类型</param>
        /// <param name="tags">标签</param>
        /// <param name="weight">权重</param>
        /// <param name="isArchived">是否归档</param>
        public Topic(string name,string type,string tags,int weight,bool isArchived)
        {
            Name = name;
            Type = type;
            Tags = tags;
            Weight = weight;
            IsArchived = isArchived;
            SubsriberList = new ObservableCollection<Subscriber>();
            MessageList = new ObservableCollection<ChatMessage>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">话题名</param>
        /// <param name="type">类型</param>
        /// <param name="tags">标签</param>
        /// <param name="weight">权重</param>
        /// <param name="isArchived">是否归档</param>
        public Topic(string name, string type, string tags, int weight, int isArchived)
        {
            Name = name;
            Type = type;
            Tags = tags;
            Weight = weight;
            IsArchived = isArchived == 0 ? false:true ;
            SubsriberList = new ObservableCollection<Subscriber>();
            MessageList = new ObservableCollection<ChatMessage>();
        }

        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="subsricber">订阅者对象</param>
        /// <returns></returns>
        public bool AddSubscriber(Subscriber subsricber)
        {
            if (SubsriberList.Contains(subsricber))
            {
                return false;
            }
            SubsriberList.Add(subsricber);
            return true;
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns></returns>
        public bool RemoveSubscriber(Subscriber subscriber)
        {
            if (!SubsriberList.Contains(subscriber))
            {
                return false;
            }
            SubsriberList.Remove(subscriber);
            return true;
        } 

        /// <summary>
        /// 新增历史消息
        /// </summary>
        /// <param name="message">消息</param>
        public void AddFirstMessage(ChatMessage message)
        {
            MessageList.Insert(0, message);
        }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="message">消息</param>
        public void AddMessage(ChatMessage message)
        {
            MessageList.Add(message);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="message">消息</param>
        public void RemoveMessage(ChatMessage message)
        {
            MessageList.Remove(message);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Topic topic = obj as Topic;
            return topic != null && this.Name.Equals(topic.Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
