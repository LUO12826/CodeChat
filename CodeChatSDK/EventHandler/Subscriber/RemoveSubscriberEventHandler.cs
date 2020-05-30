using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 移除订阅者
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">移除订阅者参数</param>
    public delegate void RemoveSubscriberEventHandler(Object sender, RemoveSubscriberEventArgs args);

    /// <summary>
    /// 移除订阅者参数
    /// </summary>
    public class RemoveSubscriberEventArgs
    {
        /// <summary>
        /// 订阅者
        /// </summary>
        public Subscriber Subscriber { get; set; }
    }
}
