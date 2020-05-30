using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 移除消息
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">移除消息参数</param>
    public delegate void RemoveMessageEventHandler(Object sender, RemoveMessageEventArgs args);

    /// <summary>
    /// 移除消息参数
    /// </summary>
    public class RemoveMessageEventArgs
    {
        /// <summary>
        /// 所属话题名
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 聊天消息
        /// </summary>
        public ChatMessage Message { get; set; }
    }
}
