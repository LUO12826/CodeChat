using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using CodeChatSDK.SDKClient;
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

            //初始化消息列表
            MessageController messageController = new MessageController(db.Messages);
            List<ChatMessage> messages = await messageController.GetMessages(instance,instance.Limit);
            messages.ForEach(async m => await AddMessage(m));

            //初始化话题订阅者列表
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            instance.SubsriberList = await subscriberController.GetSubscribers(instance);
        }
        
        /// <summary>
        /// 增加订阅者（群组话题使用）
        /// </summary>
        /// <param name="subsricber">订阅者对象</param>
        /// <returns>结果</returns>
        public async Task<bool> AddSubscriber(Subscriber subscriber)
        {
            //判断订阅者是否位于订阅者列表中
            if (instance.SubsriberList.Contains(subscriber))
            {
                //重复添加返回假
                return false;
            }
            subscriber.TopicName = instance.Name;
            instance.SubsriberList.Add(subscriber);

            //调用订阅者控制器更新订阅者
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.UpsertSubscriber();

            //更新话题时间戳
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            //添加成功返回真
            return true;
        }

        /// <summary>
        /// 移除订阅者（群组话题使用）
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveSubscriber(Subscriber subscriber)
        {
            //判断订阅者是否位于订阅者列表中
            if (!instance.SubsriberList.Contains(subscriber))
            {
                //订阅者不存在返回假
                return false;
            }
            instance.SubsriberList.Remove(subscriber);

            //调用订阅者控制器删除订阅者
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.DeleteSubscriber();

            //更新话题时间戳
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            //删除成功返回真
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
            //判断消息是否已存在
            //if (instance.MessageList.Contains(message))
            //{
                //已存在则添加失败返回假
             //   return false;
            //}

            int newSeqId = message.SeqId;
            int oldSeqId = instance.MessageList.Count == 0 ? instance.MinLocalSeqId : instance.MessageList[0].SeqId;
            message.TopicName = instance.Name;

            //判断是否为初次添加话题
            if (oldSeqId == 0)
            {
                instance.MessageList.Add(message);

                //更新本地消息序号
                instance.MinLocalSeqId = newSeqId;
                instance.MaxLocalSeqId = newSeqId;

            //判断是否为历史消息
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

            //调用消息控制器添加消息
            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.UpsertMessage();

            //话题更新时间戳
            instance.Recieve = message.SeqId;
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            //添加成功返回真
            return true;
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveMessage(ChatMessage message)
        {
            //判断消息是否位于消息列表中
            if (!instance.MessageList.Contains(message))
            {
                //消息不存在返回假
                return false;
            }

            //判断是否删除最新消息
            if (message.SeqId == instance.MaxLocalSeqId)
            {
                int size = instance.MessageList.Count;

                //判断是否删除完后消息列表为空
                if(size <= 1)
                {
                    //为空本地最大序号置为0
                    instance.MaxLocalSeqId = 0;
                }
                else
                {
                    //不空更新本地最大序号
                    instance.MaxLocalSeqId = instance.MessageList[size - 2].SeqId;
                }
                
            }

            //判断是否删除最先消息
            if (message.SeqId == instance.MinLocalSeqId)
            {
                int size = instance.MessageList.Count;

                //判断是否删除完后消息列表为空
                if (size <= 1)
                {
                    //为空本地最小序号置为0
                    instance.MinLocalSeqId = 0;
                }
                else
                {
                    //不空更新本地最小序号
                    instance.MinLocalSeqId = instance.MessageList[1].SeqId;
                }
                
            }

            //调用消息控制器删除消息
            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.DeleteMessage();

            //更新话题时间戳
            instance.Clear = message.SeqId;
            instance.MessageList.Remove(message);
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            //向服务器发送删除消息请求
            client.RemoveMessage(instance,message);

            //删除成功返回真
            return true;
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>结果</returns>
        public async Task<bool> NoteRead(ChatMessage message)
        {
            //判断消息是否位于消息列表中
            if (!instance.MessageList.Contains(message))
            {
                //消息不存在返回假
                return false;
            }

            //调用消息控制器标记消息为已读
            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.NoteRead();

            //向服务器发送已读消息请求
            client.NoteMessage(instance,message);

            //更新话题时间戳
            instance.Read = message.SeqId;
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            //标记成功返回真
            return true;
        }

        /// <summary>
        /// 加载历史消息
        /// </summary>
        /// <returns></returns>
        public async void LoadMessage()
        {
            //获得起始序号与截止序号
            int before = instance.MessageList.Count == 0 ? instance.MinLocalSeqId : instance.MessageList[0].SeqId;
            int since = (before - instance.Limit) > 0 ? before - instance.Limit : 0;

            //判断范围
            if (since >= instance.MinLocalSeqId)
            {
                //全部消息从数据库中加载
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));
            }
            else if (before > instance.MinLocalSeqId)
            {
                //从数据加载部分消息
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));

                //更新截止序号
                before = instance.MinLocalSeqId;

                //剩余消息请求服务器
                client.Load(instance, since, before);
            }
            else
            {
                //全部消息请求服务器
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
