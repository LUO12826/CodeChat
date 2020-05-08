using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

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
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public Account(string username,string password)
        {
            Username = username;
            Password = password;
            State = AccountState.Offline;
            SubscriberList = new ObservableCollection<Subscriber>();
            TopicList = new ObservableCollection<Topic>();
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
            if (TopicList.Contains(topic))
            {
                return false;
            }
            TopicList.Add(topic);
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
            return true;
        }

        /// <summary>
        /// 移除话题
        /// </summary>
        /// <param name="topic">话题对象</param>
        /// <returns></returns>
        public bool RemoveTopic(Topic topic)
        {
            if (!TopicList.Contains(topic))
            {
                return false;
            }
            TopicList.Remove(topic);
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
            TopicList.Move(oldPosition, 0);
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
            return true;
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
            SubscriberList.Add(subscriber);
            return true;
        }

        /// <summary>
        /// 移除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns></returns>
        public bool RemoveSubsriber(Subscriber subscriber)
        {
            if (!SubscriberList.Contains(subscriber))
            {
                return false;
            }
            SubscriberList.Remove(subscriber);
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
            return true;
        }

        /// <summary>
        /// 屏蔽订阅者
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <returns></returns>
        public bool BlockSubsriber(Subscriber subscriber)
        {
            if (!SubscriberList.Contains(subscriber))
            {
                return false;
            }
            //TODO: block subsriber

            return true;
        }
    }
}
