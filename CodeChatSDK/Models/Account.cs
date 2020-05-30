using CodeChatSDK.EventHandler;
using CodeChatSDK.Repository;
using Google.Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChatSDK.Models
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
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string FormattedName { get; set; }

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
        public List<Subscriber> SubscriberList { get; set; }

        /// <summary>
        /// 话题列表
        /// </summary>
        public List<Topic> TopicList { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Account()
        {
            UserId = "";
            FormattedName = "";
            Email = "";
            Avatar = "";
            State = AccountState.Offline;
            SubscriberList = new List<Subscriber>();
            TopicList = new List<Topic>();
            Tags = new List<string>();
        }
    }
}
