﻿using CodeChatSDK.Models;
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

        private Topic instance;

        private IAccountRepository db;

        /// <summary>
        /// 客户端
        /// </summary>
        private Client client;

        public TopicController(IAccountRepository database)
        {
            client = Client.Instance;
            db = database;
        }

        public async Task SetTopic(Topic topic)
        {
            instance = topic;
            MessageController messageController = new MessageController(db.Messages);
            instance.MessageList = await messageController.GetMessages(instance);
            client.Load(instance, instance.Limit);
        }

        /// <summary>
        /// 增加订阅者
        /// </summary>
        /// <param name="subsricber">订阅者对象</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        public async Task<bool> AddMessage(ChatMessage message)
        {

            int newSeqId = message.SeqId;
            int oldSeqId = instance.MessageList.Count == 0 ? 1 : instance.MessageList[0].SeqId;
            message.TopicName = instance.Name;
            if (oldSeqId == 1 || oldSeqId >= newSeqId)
            {
                instance.MessageList.Insert(0, message);
                --instance.MinLocalSeqId;
            }
            else
            {
                instance.MessageList.Add(message);
                ++instance.MaxLocalSeqId;
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
        public async Task<bool> RemoveMessage(ChatMessage message)
        {
            if (!instance.MessageList.Contains(message))
            {
                return false;
            }
            if (message.SeqId == instance.MaxLocalSeqId)
            {
                --instance.MaxLocalSeqId;
            }
            if (message.SeqId == instance.MinLocalSeqId)
            {
                ++instance.MinLocalSeqId;
            }

            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.DeleteMessage();

            instance.Clear = message.SeqId;
            instance.MessageList.Remove(message);
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);

            return true;
        }

        public async void NoteRead(ChatMessage message)
        {
            if (!instance.MessageList.Contains(message))
            {
                return;
            }

            MessageController messageController = new MessageController(db.Messages);
            messageController.SetMessage(message);
            messageController.NoteRead();

            client.NoteMessage(instance,message);

            instance.Read = message.SeqId;
            instance.LastUsed = ChatMessageBuilder.GetTimeStamp();
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
        }

        /// <summary>
        /// 加载历史消息
        /// </summary>
        /// <returns></returns>
        public async void LoadMessage()
        {
            
            int since = instance.MessageList.Count == 0 ? instance.MaxLocalSeqId : instance.MessageList[0].SeqId - 1;
            int before = (since - instance.Limit) >= 0 ? since - instance.Limit : 0;
            if (since < instance.MinLocalSeqId)
            {
                client.Load(instance, since, before);
                instance.MinLocalSeqId = before + 1;
            }
            else if (before < instance.MinLocalSeqId)
            {
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));
                since = instance.MinLocalSeqId;
                client.Load(instance, since, before);
            }
            else
            {
                MessageController messageController = new MessageController(db.Messages);
                List<ChatMessage> messages = await messageController.GetMessages(instance, since, before) as List<ChatMessage>;
                messages.ForEach(async m => await AddMessage(m));
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

        public async Task<List<ChatMessage>> SearchMessage(string condition)
        {
            MessageController messageController = new MessageController(db.Messages);
            return await messageController.SearchMessage(instance, condition);
        }

        public async void UpsertTopic()
        {
            var dbContext = db.Topics.GetRepository();
            await dbContext.UpsertTopic(instance);
        }

        public async void DeleteTopic()
        {
            var dbContext = db.Topics.GetRepository();
            await dbContext.DeleteTopic(instance);
        }

        public async Task<List<Topic>> GetTopics()
        {
            var dbContext = db.Topics.GetRepository();
            return await dbContext.GetAsync() as List<Topic>;
        }

        public async Task<List<Topic>> SearchTopic(string condition)
        {
            var dbContext = db.Topics.GetRepository();
            return await dbContext.GetAsync(condition) as List<Topic>;
        }
    }
}