using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 移除话题
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">移除话题参数</param>
    public delegate void RemoveTopicEventHandler(Object sender, RemoveTopicEventArgs args);

    /// <summary>
    /// 移除话题参数
    /// </summary>
    public class RemoveTopicEventArgs
    {
        /// <summary>
        /// 话题
        /// </summary>
        public Topic Topic { get; set; }
    }
}
