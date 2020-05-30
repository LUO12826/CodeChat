using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 添加订阅者
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">添加订阅者参数</param>
    public delegate void AddSubscriberEventHandler(Object sender, AddSubscriberEventArgs args);

    /// <summary>
    /// 添加订阅者参数
    /// </summary>
    public class AddSubscriberEventArgs
    {
        /// <summary>
        /// 订阅者
        /// </summary>
        public Subscriber Subscriber { get; set; }

        /// <summary>
        /// 是否临时添加
        /// </summary>
        public bool isTemporary { get; set; }
    }
}
