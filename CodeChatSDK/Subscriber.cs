using System;
using System.Collections.Generic;
using System.Text;
using static CodeChatSDK.MessageBuilder;

namespace CodeChatSDK
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public class Subscriber
    {
        /// <summary>
        /// 订阅者ID
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// 所属话题
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// 头像数据
        /// </summary>
        public string PhotoData { get; private set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// 头像类型
        /// </summary>
        public string PhotoType { get; private set; }

        ///

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userId">订阅者ID</param>
        /// <param name="topic">所属话题</param>
        /// <param name="username">用户名</param>
        /// <param name="type">类型</param>
        /// <param name="photo">头像数据</param>
        /// <param name="photoType">头像类型</param>
        /// <param name="online">是否在线</param>
        public Subscriber(string userId, string topic, string username, string type, string photo, string photoType, bool online)
        {
            UserId = userId;
            Topic = topic;
            Username = username;
            Type = type;
            PhotoData = photo;
            PhotoType = photoType;
            Online = online;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Subscriber subscriber = obj as Subscriber;
            return subscriber != null && this.UserId.Equals(subscriber.UserId);
        }
    }
}
