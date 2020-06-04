using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using CodeChatSDK.Utils;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Controllers
{
    public class TopicController
    {
        /// <summary>
        /// 话题对象
        /// </summary>
        private Topic instance;

        /// <summary>
        /// 数据库
        /// </summary>
        private IAccountRepository db;

        /// <summary>
        /// 客户端
        /// </summary>
        private Client client;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">数据库</param>
        public TopicController(IAccountRepository database)
        {
            client = Client.Instance;
            db = database;
        }

        /// <summary>
        /// 设置话题对象
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        public async Task SetTopic(Topic topic)
        {
            instance = topic;
            MessageController messageController = new MessageController(db.Messages);
            List<ChatMessage> messages = await messageController.GetMessages(instance,instance.Limit);
            messages.ForEach(async m => await AddMessage(m));

            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            instance.SubsriberList = await subscriberController.GetSubscribers(instance);
        }
        
        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="subsricber">订阅者对象</param>
        /// <returns>结果</returns>
        public async Task<bool> AddSubscriber(Subscriber subscriber)
        {
            if (instance.SubsriberList.Contains(subscriber))
            {
                return false;
            }
            subscriber.TopicName = instance.Name;
            instance.SubsriberList.Add(subscriber);

            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.UpsertSubscriber();

            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
            return true;
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveSubscriber(Subscriber subscriber)
        {
            if (!instance.SubsriberList.Contains(subscriber))
            {
                return false;
            }
            instance.SubsriberList.Remove(subscriber);

            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.DeleteSubscriber();

            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public async void SendMessage(ChatMessage message)
        {
            //话题更新时间戳
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
            client.Send(instance, message, false);
        }

        /// <summary>
        /// 新增消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>结果</returns>
        public async Task<bool> AddMessage(ChatMessage message)
        {

            int newSeqId = message.SeqId;
            int oldSeqId = instance.MessageList.Count == 0 ? instance.MinLocalSeqId : instance.MessageList[0].SeqId;
            message.TopicName = instance.Name;
            if (oldSeqId == 0)
            {
                instance.MessageList.Add(message);
                instance.MinLocalSeqId = newSeqId;
                instance.MaxLocalSeqId = newSeqId;
            }else if (oldSeqId >= newSeqId)
            {
                instance.MessageList.Insert(0, message);
                instance.MinLocalSeqId = newSeqId;
            }
            else
            {
                instance.MessageList.Add(message);
                instance.MaxLocalSeqId = newSeqId;
            }

            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.UpsertMessage();

            //话题更新时间戳
            instance.Recieve = message.SeqId;
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
            return true;
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveMessage(ChatMessage message)
        {
            if (!instance.MessageList.Contains(message))
            {
                return false;
            }
            if (message.SeqId == instance.MaxLocalSeqId)
            {
                int size = instance.MessageList.Count;
                if(size <= 1)
                {
                    instance.MaxLocalSeqId = 0;
                }
                else
                {
                    instance.MaxLocalSeqId = instance.MessageList[size - 2].SeqId;
                }
                
            }
            if (message.SeqId == instance.MinLocalSeqId)
            {
                int size = instance.MessageList.Count;
                if (size <= 1)
                {
                    instance.MinLocalSeqId = 0;
                }
                else
                {
                    instance.MinLocalSeqId = instance.MessageList[1].SeqId;
                }
                
            }
            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.DeleteMessage();

            instance.Clear = message.SeqId;
            instance.MessageList.Remove(message);
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            client.RemoveMessage(instance,message);
            return true;
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>结果</returns>
        public async Task<bool> NoteRead(ChatMessage message)
        {
            if (!instance.MessageList.Contains(message))
            {
                return false;
            }

            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.NoteRead();

            client.NoteMessage(instance,message);

            instance.Read = message.SeqId;
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            return true;
        }

        /// <summary>
        /// 加载历史消息
        /// </summary>
        /// <returns></returns>
        public async void LoadMessage()
        {
            
            int before = instance.MessageList.Count == 0 ? instance.MinLocalSeqId : instance.MessageList[0].SeqId;
            int since = (before - instance.Limit) > 0 ? before - instance.Limit : 0;
            if (since >= instance.MinLocalSeqId)
            {
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));
            }
            else if (before > instance.MinLocalSeqId)
            {
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));
                before = instance.MinLocalSeqId;
                client.Load(instance, since, before);
            }
            else
            {
                client.Load(instance, since, before);
            }

            //话题更新时间戳
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
        }

        /// <summary>
        /// 设置备注
        /// </summary>
        /// <param name="comment">备注</param>
        /// <returns></returns>
        public async Task SetPrivateComment(string comment)
        {
            client.SetPrivateComment(instance, comment);

            //话题更新时间戳
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            instance.PrivateComment = comment;
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
        }

        /// <summary>
        /// 插入或更新话题
        /// </summary>
        public async void UpsertTopic()
        {
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
        }

        /// <summary>
        /// 删除话题
        /// </summary>
        public async void DeleteTopic()
        {
            var dbContext = db.Topics.GetRepository();
            await dbContext.DeleteTopic(instance);
        }

        /// <summary>
        /// 获取话题
        /// </summary>
        /// <returns>话题列表</returns>
        public async Task<List<Topic>> GetTopics()
        {
            var dbContext = db.Topics.GetRepository();
            return await dbContext.GetAsync() as List<Topic>;
        }

        /// <summary>
        /// 本地搜索话题
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns>结果列表</returns>
        public async Task<List<Topic>> SearchTopic(string condition)
        {
            var dbContext = db.Topics.GetRepository();
            return await dbContext.GetAsync(condition) as List<Topic>;
        }

        /// <summary>
        /// 本地搜索话题分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>搜索结果</returns>
        public List<Topic> SearchTopic(string condition,int pageIndex,int pageSize,ref int pageCount)
        {
            var dbContext = db.Topics.GetRepository();
            return dbContext.GetSync(condition,pageIndex,pageSize,ref pageCount) as List<Topic>;
        }
    }
}
