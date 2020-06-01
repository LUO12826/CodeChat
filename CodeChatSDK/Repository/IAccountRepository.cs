using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.Repository
{
    /// <summary>
    /// 用户数据库接口
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// 话题数据表
        /// </summary>
        ITopicRepository Topics { get; }

        /// <summary>
        /// 订阅者数据表
        /// </summary>
        ISubscriberRepository Subscribers { get; }

        /// <summary>
        /// 消息数据表
        /// </summary>
        IMessageRepository Messages { get; }

        /// <summary>
        /// 获取用户数据库
        /// </summary>
        /// <returns>用户数据库</returns>
        IAccountRepository GetRepository();
    }
}
