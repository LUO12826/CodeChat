using CodeChatSDK.EventHandler;
using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using CodeChatSDK.SDKClient;
using Google.Protobuf;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeChatSDK.Controllers
{
    /// <summary>
    /// 用户控制器
    /// </summary>
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

        /// <summary>
        /// 订阅者搜索结果列表
        /// </summary>
        private List<Subscriber> searchSubscriberResult;

        /// <summary>
        /// 话题控制器字典
        /// </summary>
        private ConcurrentDictionary<string, TopicController> topicControllerDictionary;

        /// <summary>
        /// 用户数据库
        /// </summary>
        private IAccountRepository db;

        /// <summary>
        /// 客户端
        /// </summary>
        private Client client;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AccountController()
        {
            client = Client.Instance;
            searchSubscriberResult = new List<Subscriber>();
            topicControllerDictionary = new ConcurrentDictionary<string, TopicController>();
            client.DisconnectedEvent += Disconnected;
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
        /// <returns>密钥</returns>
        public ByteString GetSecret()
        {
            return ByteString.CopyFromUtf8(instance.Username + ":" + instance.Password);
        }

        /// <summary>
        /// 设置用户数据库
        /// </summary>
        /// <param name="database">数据库</param>
        /// <returns></returns>
        public async Task SetDatabase(IAccountRepository database)
        {
            db = database;

            //获取控制器
            SubscriberController subscriberController = new SubscriberController(database.Subscribers);
            TopicController topicController = new TopicController(database);

            //填充数据
            instance.SubscriberList = await subscriberController.GetSubscribers();
            instance.TopicList = await topicController.GetTopics();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        public void Login()
        {
            ByteString secret = GetSecret();
            client.LogIn(secret);
            client.Start();
        }

        /// <summary>
        /// 注册
        /// </summary>
        public void Register()
        {
            ByteString secret = GetSecret();
            client.Register(secret, instance.FormattedName, instance.Email);
            client.Start();
        }

        /// <summary>
        /// 发送注册验证码
        /// </summary>
        /// <param name="code">验证码</param>
        public void SendVerificationCode(string code)
        {
            client.SendVerificationCode(GetSecret(), code);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        public void ForgetPassword()
        {
            client.ForgetPassword(instance.Email);
            client.Start();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPassword">新密码</param>
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
        public void SetFormattedName(string newFormattedName)
        {
            instance.FormattedName = newFormattedName;
            client.SetDescription(new Topic("me"), newFormattedName);
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="size">文件大小</param>
        /// <param name="bytes">字节数组</param>
        public void SetAvator(StorageFile file, ulong size, byte[] bytes)
        {
            client.SetAvator(new Topic("me"), file, size, bytes);
        }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="tag">标签</param>
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
        /// <returns>话题</returns>
        public Topic GetTopicAt(int position)
        {
            return position < 0 ? null : instance.TopicList[position];
        }

        /// <summary>
        /// 通过话题名获取话题
        /// </summary>
        /// <param name="name">话题名</param>
        /// <returns>话题</returns>
        public Topic GetTopicByName(string name)
        {
            int index = instance.TopicList.IndexOf(new Topic(name));
            if (index == -1)
            {
                return null;
            }
            return instance.TopicList[index];
        }

        /// <summary>
        /// 通过话题名获取话题控制器
        /// </summary>
        /// <param name="name">话题名</param>
        /// <returns>话题控制器</returns>
        public async Task<TopicController> GetTopicControllerByName(string name)
        {
            int index = instance.TopicList.IndexOf(new Topic(name));
            if (index == -1)
            {
                return null;
            }
            
            //获取或创建控制器
            TopicController topicController;
            if (topicControllerDictionary.TryGetValue(name, out topicController) == false)
            {
                client.SubscribeTopic(instance.TopicList[index]);
                topicController = new TopicController(db);
                await topicController.SetTopic(instance.TopicList[index]);
                topicControllerDictionary.TryAdd(name, topicController);
            }

            return topicController;
        }

        /// <summary>
        /// 通过话题获取话题控制器
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>话题控制器</returns>
        public async Task<TopicController> GetTopicController(Topic topic)
        {
            if (topic == null)
            {
                return null;
            }

            //获取或创建控制器
            TopicController topicController;
            if (topicControllerDictionary.TryGetValue(topic.Name, out topicController) == false)
            {
                client.SubscribeTopic(topic);
                topicController = new TopicController(db);
                await topicController.SetTopic(topic);
                topicControllerDictionary.TryAdd(topic.Name, topicController);
            }

            return topicController;
        }

        /// <summary>
        /// 订阅话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>结果</returns>
        public async Task<bool> AddTopic(Topic topic)
        {
            if (instance.TopicList.Contains(topic) || topic.Name.Equals("me") || topic.Name.Equals("fnd"))
            {
                return false;
            }

            //话题列表更新
            instance.TopicList.Add(topic);

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.UpsertTopic();

            return true;
        }

        /// <summary>
        /// 通过话题位置移除话题
        /// </summary>
        /// <param name="position">话题位置</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveTopicAt(int position)
        {
            if (position < 0 || position > instance.TopicList.Count)
            {
                return false;
            }

            Topic topic = instance.TopicList[position];
            topic.IsArchived = true;

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.DeleteTopic();

            //话题列表更新
            instance.TopicList.Remove(topic);


            //服务器更新
            client.RemoveTopic(topic);

            return true;
        }

        /// <summary>
        /// 移除话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic))
            {
                return false;
            }

            instance.TopicList.Remove(topic);
            topic.IsArchived = true;

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.UpsertTopic();

            //话题列表更新
            instance.TopicList.Remove(topic);

            //服务器更新
            client.RemoveTopic(topic);


            return true;
        }

        /// <summary>
        /// 移动话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        public async Task<bool> MoveTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic))
            {
                return false;
            }

            //话题列表更新
            int oldPosition = instance.TopicList.IndexOf(topic);
            instance.TopicList.RemoveAt(oldPosition);
            instance.TopicList.Insert(0, topic);

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.UpsertTopic();

            return true;
        }

        /// <summary>
        /// 置顶话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>结果</returns>
        public async Task<bool> PinTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic) || topic.Weight > 0)
            {
                return false;
            }

            //话题列表更新
            int oldPosition = instance.TopicList.IndexOf(topic);
            topic.Weight = instance.TopicList[0].Weight + 1;
            instance.TopicList.RemoveAt(oldPosition);
            instance.TopicList.Insert(0, topic);

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.UpsertTopic();

            return true;
        }

        /// <summary>
        /// 取消置顶话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>结果</returns>
        public async Task<bool> UnpinTopic(Topic topic)
        {
            if (!instance.TopicList.Contains(topic) || topic.Weight == 0)
            {
                return false;
            }

            //权重置为0
            topic.Weight = 0;

            //数据库更新
            TopicController topicController = await GetTopicController(topic);
            topicController.UpsertTopic();

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

        /// <summary>
        /// 通过订阅者ID获取订阅者
        /// </summary>
        /// <param name="userId">订阅者ID</param>
        /// <returns></returns>
        public Subscriber GetSubscriberByUserId(string userId)
        {
            int index = instance.SubscriberList.IndexOf(new Subscriber() { UserId = userId });
            if (index == -1)
            {
                return null;
            }

            return instance.SubscriberList[index];
        }

        /// <summary>
        /// 通过订阅者获取订阅者控制器
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <returns>订阅者控制器</returns>
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
        /// <returns>结果</returns>
        public async Task<bool> AddSubscriber(Subscriber subscriber, bool isTemporary = false)
        {
            //判断是否为有效订阅者
            if (instance.SubscriberList.Contains(subscriber) || subscriber.TopicName == "fnd" || subscriber.TopicName == "me")
            {
                //无效订阅者返回假
                return false;
            }

            //判断是否为临时查询
            if (isTemporary == true)
            {
                if (searchSubscriberResult.Contains(subscriber))
                {
                    return false;
                }
                searchSubscriberResult.Add(subscriber);
                return true;
            }

            //获取已存在话题或创建话题
            Topic newTopic = GetTopicByName(subscriber.TopicName);
            if (newTopic == null)
            {
                newTopic = new Topic(subscriber.TopicName);
            }

            if (newTopic == null)
            {
                newTopic = new Topic(subscriber.TopicName);
            }

            //话题列表更新
            await AddTopic(newTopic);

            //数据库更新
            TopicController topicController = await GetTopicController(newTopic);
            topicController.UpsertTopic();

            //订阅者列表更新
            instance.SubscriberList.Add(subscriber);

            //数据库更新
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.UpsertSubscriber();

            //服务器更新
            client.SubscribeTopic(newTopic);
            return true;
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveSubscriber(Subscriber subscriber)
        {
            //判断订阅者是否位于订阅者列表中
            if (!instance.SubscriberList.Contains(subscriber))
            {
                return false;
            }

            Topic removedTopic = GetTopicByName(subscriber.TopicName);

            //订阅者列表更新
            instance.SubscriberList.Remove(subscriber);

            //数据库更新
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.DeleteSubscriber();

            //话题列表更新
            instance.TopicList.Remove(removedTopic);

            //数据库更新
            TopicController topicController = await GetTopicController(removedTopic);
            topicController.DeleteTopic();

            //向服务器请求删除订阅者
            client.RemoveSubscriber(subscriber);

            return true;
        }

        /// <summary>
        /// 通过订阅者位置移除订阅者
        /// </summary>
        /// <param name="position">订阅者位置</param>
        /// <returns>结果</returns>
        public async Task<bool> RemoveSubsriberAt(int position)
        {
            if (position < 0 || position > instance.SubscriberList.Count)
            {
                return false;
            }

            Subscriber subscriber = instance.SubscriberList[position];
            Topic removedTopic = GetTopicByName(subscriber.TopicName);

            //订阅者列表更新
            instance.SubscriberList.Remove(subscriber);

            //数据库更新
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            subscriberController.SetSubscriber(subscriber);
            subscriberController.DeleteSubscriber();

            //话题列表更新
            instance.TopicList.Remove(removedTopic);

            //数据库更新
            TopicController topicController = await GetTopicController(removedTopic);
            topicController.DeleteTopic();

            //向服务器请求删除订阅者
            client.RemoveSubscriber(subscriber);

            //移除成功返回真
            return true;
        }

        /// <summary>
        /// 在线搜索订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns></returns>
        public List<Subscriber> SearchSubscriberOnline(string condition)
        {
            client.FindSubscriber();
            return searchSubscriberResult.Where(s => s.Username.Contains(condition) || s.UserId.Contains(condition)).ToList();
        }

        /// <summary>
        /// 在线搜索订阅者分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>搜索结果</returns>
        public List<Subscriber> SearchSubscriberOnline(string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            client.FindSubscriber();
            var query = searchSubscriberResult.
                                Where(s => s.Username.Contains(condition) ||
                                s.UserId.Contains(condition));

            pageCount = query.Count() % pageSize == 0 ? (query.Count() / pageSize) : (query.Count() / pageSize) + 1;

            return query.Skip(pageIndex - 1).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取消息控制器
        /// </summary>
        /// <returns>消息控制器</returns>
        public MessageController GetMessageController()
        {
            MessageController messageController = new MessageController(db.Messages);
            return messageController;
        }

        /// <summary>
        /// 获取话题控制器
        /// </summary>
        /// <returns>订阅者控制器</returns>
        public TopicController GetTopicController()
        {
            TopicController topicController = new TopicController(db);
            return topicController;
        }

        /// <summary>
        /// 获取订阅者控制器
        /// </summary>
        /// <returns>订阅者控制器</returns>
        public SubscriberController GetSubscriberController()
        {
            SubscriberController subscriberController = new SubscriberController(db.Subscribers);
            return subscriberController;
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
            if (string.IsNullOrEmpty(args.UserId) == false)
            {
                //设置用户信息
                instance.UserId = args.UserId;
            }
            if (args.Tags != null && args.Tags.Count != 0)
            {
                //设置用户标签
                instance.Tags = args.Tags;
            }
            if (string.IsNullOrEmpty(args.FormattedName) == false)
            {
                //设置用户显示名称
                instance.FormattedName = args.FormattedName;
            }
            if (string.IsNullOrEmpty(args.Avatar) == false)
            {
                //设置用户头像
                instance.Avatar = args.Avatar;
            }
            if (string.IsNullOrEmpty(args.Email) == false)
            {
                //设置邮箱地址
                instance.Email = args.Email;
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
            await AddSubscriber(args.Subscriber, args.isTemporary);
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
            await MoveTopic(currentTopic);

            TopicController topicController = await GetTopicController(currentTopic);
            //调用对应话题添加消息方法
            await topicController.AddMessage(args.Message);
        }

        /// <summary>
        /// 订阅者状态改变
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="args">订阅者状态改变参数</param>
        private void SubscriberStateChanged(object sender, SubscriberStateChangedEventArgs args)
        {
            //获取拥有完整信息的订阅者
            Subscriber subscriber = GetSubscriberByUserId(args.Subscriber.UserId);
            if (subscriber != null)
            {
                SubscriberController subscriberController = GetSubscriberController(subscriber);

                //调用更改订阅者状态方法
                subscriberController.ChangeSubscriberState(args.IsOnline);
            }
        }
    }
}
