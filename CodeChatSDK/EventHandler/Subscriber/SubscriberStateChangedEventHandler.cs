using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 订阅者状态改变
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">订阅者状态改变参数</param>
    public delegate void SubscriberStateChangedEventHandler(Object sender, SubscriberStateChangedEventArgs args);

    /// <summary>
    /// 订阅者状态改变参数
    /// </summary>
    public class SubscriberStateChangedEventArgs
    {
        /// <summary>
        /// 订阅者
        /// </summary>
        public Subscriber Subscriber { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { get; set; }
    }
}
