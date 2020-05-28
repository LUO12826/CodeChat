using CodeChatSDK.EventHandler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.UI.Xaml.Media.Animation;

namespace CodeChatSDK
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
        public ObservableCollection<Subscriber> SubsriberList { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public ObservableCollection<ChatMessage> MessageList { get; set; }

        /// <summary>
        /// 客户端单例
        /// </summary>
        private Client client;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">话题名</param>
        public Topic(string name)
        {
            Name = name;
            Limit = 50;
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            client = Client.Instance;
            var db = AccountContext.Instance;
            List<Subscriber> subscribers = db.Subscribers.Where(s => s.TopicName == this.Name).ToList();
            List<ChatMessage> messages = db.Messages.Where(m => m.TopicName == this.Name).OrderBy(m=>m.SeqId).Take(Limit).ToList();
            messages.ForEach(m => { 
                MessageBuilder.ParseContent(m);
                MessageBuilder.ParseCode(m);
                });
            SubsriberList = new ObservableCollection<Subscriber>(subscribers);
            MessageList = new ObservableCollection<ChatMessage>(messages);
        }

        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="subsricber">订阅者对象</param>
        /// <returns></returns>
        public bool AddSubscriber(Subscriber subscriber)
        {
            if (SubsriberList.Contains(subscriber))
            {
                return false;
            }
            subscriber.TopicName = Name;
            var db = AccountContext.Instance;
            SubsriberList.Add(subscriber);
            db.Subscribers.Add(subscriber);
            db.SaveChanges();
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
            var db = AccountContext.Instance;
            db.Subscribers.Remove(subscriber);
            db.SaveChanges();
            return true;
        } 

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public async Task SendMessage(ChatMessage message)
        {
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            await client.Send(this, message,false);
        }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="message">消息</param>
        public bool AddMessage(ChatMessage message)
        {
            
            int newSeqId = message.SeqId;
            int oldSeqId = MessageList.Count == 0 ? 1 : MessageList[0].SeqId;
            message.TopicName = Name;
            if (oldSeqId == 1 || oldSeqId >= newSeqId)
            {
                MessageList.Insert(0, message);
                --MinLocalSeqId;
            }
            else
            {
                MessageList.Add(message);
                ++MaxLocalSeqId;
            }
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            var db = AccountContext.Instance;
            db.Messages.Add(message);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="message">消息</param>
        public async Task<bool> RemoveMessage(ChatMessage message)
        {
            if (!MessageList.Contains(message))
            {
                return false;
            }
            if (message.SeqId == MaxLocalSeqId)
            {
                --MaxLocalSeqId;
            }
            if (message.SeqId == MinLocalSeqId)
            {
                ++MinLocalSeqId;
            }
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            await client.RemoveMessage(this,message);
            var db = AccountContext.Instance;
            MessageList.Remove(message);
            db.Messages.Remove(message);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 加载历史消息
        /// </summary>
        /// <returns></returns>
        public async Task LoadMessage()
        {
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            int since = MessageList.Count == 0 ? MaxLocalSeqId : MessageList[0].SeqId-1;
            int before = (since - Limit)>=0?since-Limit:0;
            if (since < MinLocalSeqId)
            {
                await client.Load(this, since, before);
                MinLocalSeqId = before + 1;
            }
            else if (before < MinLocalSeqId)
            {
                var db = AccountContext.Instance;
                List<ChatMessage> messages = db.Messages.Where(m => m.SeqId >= since && m.SeqId < before).ToList();
                messages.ForEach(m => AddMessage(m));
                since = MinLocalSeqId;
                await client.Load(this, since, before);
            }
            else
            {
                var db = AccountContext.Instance;
                List<ChatMessage> messages = db.Messages.Where(m => m.SeqId >= since && m.SeqId < before).ToList();
                messages.ForEach(m => AddMessage(m));
            }
        }

        /// <summary>
        /// 设置备注
        /// </summary>
        /// <param name="comment">备注</param>
        /// <returns></returns>
        public async Task SetPrivateComment(string comment)
        {
            //话题更新时间戳
            LastUsed = GetTimeStamp();
            await client.SetPrivateComment(this, comment);
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
            return Name+"\t"+MinLocalSeqId+"\t"+MaxLocalSeqId+"\t"+Weight;
        }

        private long GetTimeStamp()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(timeSpan.TotalMilliseconds);
        }
    }
}
