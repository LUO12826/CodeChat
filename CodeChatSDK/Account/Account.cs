using CodeChatSDK.EventHandler;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace CodeChatSDK
{
    /// <summary>
    /// 用户
    /// </summary>
    public class Account
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId{ get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string FormattedName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { private get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public AccountState State { get; set; }

        /// <summary>
        /// 订阅者列表
        /// </summary>
        public ObservableCollection<Subscriber> SubscriberList { get; private set; }

        /// <summary>
        /// 话题列表
        /// </summary>
        public ObservableCollection<Topic> TopicList { get; private set; }

        /// <summary>
        /// 用户数据库
        /// </summary>
        private AccountContext db;

        /// <summary>
        /// 客户端
        /// </summary>
        private Client client;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Account()
        {
            State = AccountState.Offline;

            //获取客户端单例
            client = Client.Instance;

            //客户端事件绑定
            client.LoginSuccessEvent += LoginSuccess;
            client.LoginFailedEvent += LoginFailed;
            client.RegisterFailedEvent += RegisterFailed;
            client.DisconnectedEvent += Disconnected;
            client.AddTopicEvent += AddTopic;
            client.AddSubscriberEvent += AddSubscriber;
            client.SetAccountEvent += SetAccountInformation;
            client.AddMessageEvent += AddMessage;
        }

        /// <summary>
        /// 初始化用户数据库
        /// </summary>
        public void initDatabase()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string path = $@"Data Source={localFolder.Path}\{Username}\account.db";
            AccountContext.Path = path;
            db = AccountContext.Instance;
            List<Subscriber> subscribers = db.Subscribers.ToList();
            List<Topic> topics = db.Topics.OrderByDescending(t => t.Weight).ThenByDescending(t => t.LastUsed).ToList();
            SubscriberList = new ObservableCollection<Subscriber>(subscribers);
            TopicList = new ObservableCollection<Topic>(topics);
            Tags = new List<string>();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public async Task Login()
        {
            client.Secret = GetSecret();
            client.LogIn();
            await client.Start();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public async Task Register()
        {
            client.Secret = GetSecret();
            client.Register(FormattedName,Email);
            await client.Start();
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        public async Task ForgetPassword()
        {
            client.ForgetPassword(Email);
            await client.Start();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public async Task ChangePassword(string newPassword)
        {
            Password = newPassword;
            client.Secret = GetSecret();
            await client.ChangePassword();
        }

        /// <summary>
        /// 设置显示名称
        /// </summary>
        /// <param name="newFormattedName">新显示名称</param>
        /// <returns></returns>
        public async Task SetFormattedName(string newFormattedName)
        {
            FormattedName = newFormattedName;
            await client.SetDescription(new Topic("me"), newFormattedName);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public async Task AddTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }
            Tags.Add(tag);
            await client.SetTags(new Topic("me"), Tags);
        }

        /// <summary>
        /// 移除标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public async Task RemoveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag) || !Tags.Contains(tag))
            {
                return;
            }
            Tags.Remove(tag);
            await client.SetTags(new Topic("me"), Tags);
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <returns>Secret</returns>
        public ByteString GetSecret()
        {
            return ByteString.CopyFromUtf8(Username+":"+Password);
        }

        /// <summary>
        /// 通过话题位置获取话题
        /// </summary>
        /// <param name="position">话题位置</param>
        /// <returns></returns>
        public Topic GetTopicAt(int position)
        {
            return TopicList[position];
        }

        /// <summary>
        /// 获取话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public Topic GetTopic(Topic topic)
        {
            int index = TopicList.IndexOf(topic);
            return TopicList[index];
        }

        /// <summary>
        /// 通过话题名获取话题
        /// </summary>
        /// <param name="name">话题名</param>
        /// <returns></returns>
        public Topic GetTopicByName(string name)
        {
            int index = TopicList.IndexOf(new Topic(name));
            return index==-1?null:TopicList[index];
        }

        /// <summary>
        /// 订阅话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool AddTopic(Topic topic)
        {
            if (TopicList.Contains(topic)||topic.Name.Equals("me"))
            {
                return false;
            }
            TopicList.Add(topic);
            db.Topics.Add(topic);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 通过话题位置移除话题
        /// </summary>
        /// <param name="position">话题位置</param>
        /// <returns></returns>
        public bool RemoveTopicAt(int position)
        {
            if (position < 0 || position > TopicList.Count)
            {
                return false;
            }
            TopicList.RemoveAt(position);
            db.Topics.Remove(TopicList[position]);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 移除话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public async Task<bool> RemoveTopic(Topic topic)
        {
            if (!TopicList.Contains(topic))
            {
                return false;
            }
            await client.RemoveTopic(topic);
            TopicList.Remove(topic);
            Topic deletedTopic = db.Topics.
                                    Include(t => t.MessageList).
                                    Include(t => t.SubsriberList).
                                    FirstOrDefault(t => t.Name == topic.Name);
            db.Topics.Remove(deletedTopic);
            db.SaveChanges();
            return true;
        }

        public bool MoveTopic(Topic topic)
        {
            if (!TopicList.Contains(topic))
            {
                return false;
            }
            int oldPosition = TopicList.IndexOf(topic);
            TopicList.Move(oldPosition, 0);
            db.Topics.Update(topic);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 置顶话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool PinTopic(Topic topic)
        {
            if (!TopicList.Contains(topic)||topic.Weight>0)
            {
                return false;
            }
            int oldPosition = TopicList.IndexOf(topic);
            topic.Weight = TopicList[0].Weight + 1;
            TopicList.Move(oldPosition, 0);
            db.Topics.Update(topic);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 取消置顶话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool UnpinTopic(Topic topic)
        {
            if (!TopicList.Contains(topic)||topic.Weight==0)
            {
                return false;
            }
            topic.Weight = 0;
            db.Topics.Update(topic);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 刷新话题列表
        /// </summary>
        /// <returns></returns>
        public async Task RefreshTopicList()
        {
            await client.RefreshTopicList();
        }

        /// <summary>
        /// 新增订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns></returns>
        public bool AddSubscriber(Subscriber subscriber)
        {
            if (SubscriberList.Contains(subscriber))
            {
                return false;
            }
            if (subscriber.TopicName == "me")
            {
                FormattedName = subscriber.Username;
                Avatar = subscriber.PhotoData;
                
            }
            SubscriberList.Add(subscriber);
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
            if (!SubscriberList.Contains(subscriber))
            {
                return false;
            }
            SubscriberList.Remove(subscriber);
            db.Subscribers.Remove(subscriber);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 通过订阅者位置移除订阅者
        /// </summary>
        /// <param name="position">订阅者位置</param>
        /// <returns></returns>
        public bool RemoveSubsriberAt(int position)
        {
            if (position < 0 || position > SubscriberList.Count)
            {
                return false;
            }
            SubscriberList.RemoveAt(position);
            db.Subscribers.Remove(SubscriberList[position]);
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">登陆成功参数</param>
        private void LoginSuccess(object sender, LoginSuccessEventArgs args)
        {
            //用户状态改为在线
            State = AccountState.Online;
        }

        /// <summary>
        /// 登陆失败
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">登陆失败参数</param>
        private void LoginFailed(object sender, LoginFailedEventArgs args)
        {
            //用户状态改为离线
            State = AccountState.Offline;
        }

        /// <summary>
        /// 注册失败
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">注册失败参数</param>
        private void RegisterFailed(object sender, RegisterFailedEventArgs args)
        {
            //用户状态改为重复
            State = AccountState.Duplicate;
        }

        /// <summary>
        /// 未连接服务器
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">未连接服务器参数</param>
        private void Disconnected(object sender, DisconnectedEventArgs args)
        {
            //用户状态改为离线
            State = AccountState.Offline;
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
                UserId = args.UserId;
            }
            else
            {
                //设置用户标签
                Tags = args.Tags;
            }
            
        }

        /// <summary>
        /// 添加话题
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加话题参数</param>
        private void AddTopic(object sender, AddTopicEventArgs args)
        {
            //调用添加话题方法
            AddTopic(args.Topic);
        }

        /// <summary>
        /// 添加订阅者
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加订阅者参数</param>
        private void AddSubscriber(object sender, AddSubscriberEventArgs args)
        {
            //调用添加订阅者方法
            AddSubscriber(args.Subscriber);
        }

        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">添加信息参数</param>
        private void AddMessage(object sender, AddMessageEventArgs args)
        {
            Topic currentTopic = GetTopicByName(args.TopicName);
            //移动话题位置
            MoveTopic(currentTopic);
            //调用对应话题添加消息方法
            currentTopic.AddMessage(args.Message);
        }
    }
}
