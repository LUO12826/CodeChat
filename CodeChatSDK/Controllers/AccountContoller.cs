using CodeChatSDK.Controllers;
using CodeChatSDK.EventHandler;
using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using Google.Protobuf;
using Pbx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace CodeChatSDK
{
    public class AccountController
    {
        /// <summary>
        /// 用户单例
        /// </summary>
        private static Account instance;

        /// <summary>
        /// 用户单例
        /// </summary>
        public static Account Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Account();
                }
                return instance;
            }
        }

        private List<Subscriber> searchSubscriberResult;

        /// <summary>
        /// 用户数据库
        /// </summary>
        private IAccountRepository db;

        /// <summary>
        /// 客户端
        /// </summary>
        private Client client;

        public AccountController()
        {
            client = Client.Instance;
            searchSubscriberResult = new List<Subscriber>();
            client.AddMessageEvent += AddMessage;
            client.AddSubscriberEvent += AddSubscriber;
            client.AddTopicEvent += AddTopic;
            client.LoginSuccessEvent += LoginSuccess;
            client.LoginFailedEvent += LoginFailed;
            client.RegisterFailedEvent += RegisterFailed;
            client.SetAccountEvent += SetAccountInformation;
            client.SubscriberStateChangedEvent += SubscriberStateChanged;
        }

        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public void SetSecret(string username, string password)
        {
            instance.Username = username;
            instance.Password = password;
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <returns>Secret</returns>
        public ByteString GetSecret()
        {
            return ByteString.CopyFromUtf8(instance.Username + ":" + instance.Password);
        }

        /// <summary>
        /// 设置用户数据库
        /// </summary>
        public async Task SetDatabase(IAccountRepository database)
        {
            db = database;
            SubscriberController subscriberController = new SubscriberController(database.Subscribers);
            TopicController topicController=new TopicController(database);
            instance.SubscriberList = await subscriberController.GetSubscribers();
            instance.TopicList = await topicController.GetTopics();
            
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public void Login()
        {
            ByteString secret = GetSecret();
            client.LogIn(secret);
            client.Start();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public void Register()
        {
            ByteString secret = GetSecret();
            client.Register(secret, instance.FormattedName, instance.Email);
            client.Start();
        }

        public void SendVerificationCode(string code)
        {
            client.SendVerificationCode(GetSecret(), code);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        public void ForgetPassword()
        {
            client.ForgetPassword(instance.Email);
            client.Start();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public void ChangePassword(string newPassword)
        {
            instance.Password = newPassword;
            ByteString secret = GetSecret();
            client.ChangePassword(secret);
        }

        /// <summary>
        /// 设置显示名称
        /// </summary>
        /// <param name="newFormattedName">新显示名称</param>
        /// <returns></returns>
        public void SetFormattedName(string newFormattedName)
        {
            instance.FormattedName = newFormattedName;
            client.SetDescription(new Topic("me"), newFormattedName);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public void AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            instance.Tags.Add(tag);
            client.SetTags(new Topic("me"), instance.Tags);
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || !instance.Tags.Contains(tag))
            {
                return;
            }
            instance.Tags.Remove(tag);
            client.SetTags(new Topic("me"), instance.Tags);
        }

        /// <summary>
        /// 通过话题位置获取话题
        /// </summary>
        /// <param name="position">话题位置</param>
        /// <returns></returns>
        public Topic GetTopicAt(int position)
        {
            client.SubscribeTopic(instance.TopicList[position]);
            return position < 0 ? null : instance.TopicList[position];
        }

        /// <summary>
        /// 通过话题名获取话题
        /// </summary>
        /// <param name="name">话题名</param>
        /// <returns></returns>
        public Topic GetTopicByName(string name)
        {
            int index = instance.TopicList.IndexOf(new Topic(name));
            if (index == -1)
            {
                return null;
            }
            return instance.TopicList[index];
        }

        public async Task<TopicController> GetTopicControllerByName(string name)
        {
            int index = instance.TopicList.IndexOf(new Topic(name));
            if (index == -1)
            {
                return null;
            }
            client.SubscribeTopic(instance.TopicList[index]);
            TopicController topicController = new TopicController(db);
            await topicController.SetTopic(instance.TopicList[index]);
            return topicController;
        }

        public async Task<TopicController> GetTopicController(Topic topic)
        {
            if (topic == null)
            {
                return null;
            }

            client.SubscribeTopic(topic);
            TopicController topicController = new TopicController(db);
            await topicController.SetTopic(topic);
            return topicController;
        }

        /// <summary>
        /// 订阅话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public async Task<bool> AddTopic(Topic topic)
        {
            if (instance.TopicList.Contains(topic) || topic.Name.Equals("me"))
            {
                return false;
            }
            instance.TopicList.Add(topic);
            await db.Topics.UpsertTopic(topic);
            return true;
        }

        /// <summary>
        /// 通过话题位置移除话题
        /// </summary>
        /// <param name="position">话题位置</param>
        /// <returns></returns>
        public async Task<bool> RemoveTopicAt(int position)
        {
            if (position < 0 || position > instance.TopicList.Count)
            {
                return false;
            }
            Topic topic = instance.TopicList[position];
            topic.IsArchived = true;
            await db.Topics.UpsertTopic(topic);
            instance.TopicList.Remove(topic);
            client.RemoveTopic(topic);
            return true;
        }

        /// <summary>
        /// 移除话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public async Task<bool> RemoveTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic))
            {
                return false;
            }

            instance.TopicList.Remove(topic);
            topic.IsArchived = true;
            await db.Topics.UpsertTopic(topic);
            instance.TopicList.Remove(topic);
            client.RemoveTopic(topic);
            return true;
        }

        public bool MoveTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic))
            {
                return false;
            }
            int oldPosition = instance.TopicList.IndexOf(topic);
            instance.TopicList.RemoveAt(oldPosition);
            instance.TopicList.Insert(0, topic);
            db.Topics.UpsertTopic(topic);
            return true;
        }

        /// <summary>
        /// 置顶话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool PinTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic) || topic.Weight > 0)
            {
                return false;
            }
            int oldPosition = instance.TopicList.IndexOf(topic);
            topic.Weight = instance.TopicList[0].Weight + 1;
            instance.TopicList.RemoveAt(oldPosition);
            instance.TopicList.Insert(0, topic);
            db.Topics.UpsertTopic(topic);
            return true;
        }

        /// <summary>
        /// 取消置顶话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool UnpinTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic) || topic.Weight == 0)
            {
                return false;
            }
            topic.Weight = 0;
            db.Topics.UpsertTopic(topic);
            return true;
        }

        /// <summary>
        /// 刷新话题列表
        /// </summary>
        /// <returns></returns>
        public void RefreshTopicList()
        {
            client.RefreshTopicList();
        }

        public async Task<List<Topic>> SearchTopic(string condition)
        {
            TopicController topicController = new TopicController(db);
            return await topicController.SearchTopic(condition);
        }

        public SubscriberController GetSubscriberController(Subscriber subscriber)
        {
            if (subscriber == null)
            {
                return null;
            }

            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            return subscriberController;
        }

        /// <summary>
        /// 新增订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <param name="isTemporary">是否临时保存</param>
        /// <returns></returns>
        public async Task<bool> AddSubscriber(Subscriber subscriber,bool isTemporary=false)
        {
            if (instance.SubscriberList.Contains(subscriber) || subscriber.TopicName == "fnd")
            {
                return false;
            }
            if (subscriber.TopicName == "me")
            {
                instance.FormattedName = subscriber.Username;
                instance.Avatar = subscriber.PhotoData;
                return true;
            }
            if (isTemporary == true)
            {
                if (searchSubscriberResult.Contains(subscriber))
                {
                    return false;
                }
                searchSubscriberResult.Add(subscriber);
                return true;
            }

            Topic newTopic = new Topic(subscriber.TopicName);
            instance.TopicList.Add(newTopic);
            await db.Topics.UpsertTopic(newTopic);
            instance.SubscriberList.Add(subscriber);
            await db.Subscribers.UpsertSubscriber(subscriber);

            client.SubscribeTopic(newTopic);
            return true;
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns></returns>
        public async Task<bool> RemoveSubscriber(Subscriber subscriber)
        {
            if (!instance.SubscriberList.Contains(subscriber))
            {
                return false;
            }

            Topic removedTopic = GetTopicByName(subscriber.TopicName);
            instance.SubscriberList.Remove(subscriber);
            await db.Subscribers.DeleteSubscriber(subscriber);
            instance.TopicList.Remove(removedTopic);
            await db.Topics.DeleteTopic(removedTopic);

            return true;
        }

        /// <summary>
        /// 通过订阅者位置移除订阅者
        /// </summary>
        /// <param name="position">订阅者位置</param>
        /// <returns></returns>
        public async Task<bool> RemoveSubsriberAt(int position)
        {
            if (position < 0 || position > instance.SubscriberList.Count)
            {
                return false;
            }
            instance.SubscriberList.RemoveAt(position);
            await db.Subscribers.DeleteSubscriber(instance.SubscriberList[position]);

            return true;
        }

        public async Task<List<Subscriber>> SearchSubscriber(string condition)
        {
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            return await subscriberController.SearchSubscriber(condition);
        }

        /// <summary>
        /// 在线搜索订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns></returns>
        public List<Subscriber> SearchSubscriberOnline(string condition)
        {
            client.FindSubscriber();
            return searchSubscriberResult.Where(s=>s.Username.Contains(condition)||s.UserId.Contains(condition)).ToList();
        }

        /// <summary>
        /// 搜索消息
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns></returns>
        public async Task<List<ChatMessage>> SearchMessage(string condition)
        {
            MessageController messageController = new MessageController(db.Messages);
            return await messageController.SearchMessage(condition);
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">登陆成功参数</param>
        private void LoginSuccess(object sender, LoginSuccessEventArgs args)
        {
            //用户状态改为在线
            instance.State = AccountState.Online;
        }

        /// <summary>
        /// 登陆失败
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">登陆失败参数</param>
        private void LoginFailed(object sender, LoginFailedEventArgs args)
        {
            //用户状态改为离线
            instance.State = AccountState.Offline;
        }

        /// <summary>
        /// 注册失败
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">注册失败参数</param>
        private void RegisterFailed(object sender, RegisterFailedEventArgs args)
        {
            //用户状态改为重复
            instance.State = AccountState.Duplicate;
        }

        /// <summary>
        /// 未连接服务器
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">未连接服务器参数</param>
        private void Disconnected(object sender, DisconnectedEventArgs args)
        {
            //用户状态改为离线
            instance.State = AccountState.Offline;
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">设置用户信息参数</param>
        private void SetAccountInformation(object sender, SetAccountEventArgs args)
        {
            if (args.UserId != null)
            {
                //设置用户信息
                instance.UserId = args.UserId;
            }
            else
            {
                //设置用户标签
                instance.Tags = args.Tags;
            }

        }

        /// <summary>
        /// 添加话题
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加话题参数</param>
        private async void AddTopic(object sender, AddTopicEventArgs args)
        {
            //调用添加话题方法
            await AddTopic(args.Topic);
        }

        /// <summary>
        /// 添加订阅者
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加订阅者参数</param>
        private async void AddSubscriber(object sender, AddSubscriberEventArgs args)
        {
             //调用添加订阅者方法
             await AddSubscriber(args.Subscriber,args.isTemporary);
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加信息参数</param>
        private async void AddMessage(object sender, AddMessageEventArgs args)
        {
            Topic currentTopic = GetTopicByName(args.TopicName);
            //移动话题位置
            MoveTopic(currentTopic);

            TopicController topicController = await GetTopicController(currentTopic);
            //调用对应话题添加消息方法
            await topicController.AddMessage(args.Message);
        }

        private void SubscriberStateChanged(object sender,SubscriberStateChangedEventArgs args)
        {
            SubscriberController subscriberController = GetSubscriberController(args.Subscriber);
            subscriberController.ChangeSubscriberState(args.IsOnline);
        }
    }
}
