﻿using System;
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
        /// 话题数据库
        /// </summary>
        ITopicRepository Topics { get; }

        /// <summary>
        /// 订阅者数据库
        /// </summary>
        ISubscriberRepository Subscribers { get; }

        /// <summary>
        /// 消息数据库
        /// </summary>
        IMessageRepository Messages { get; }

        IAccountRepository GetRepository();
    }
}
