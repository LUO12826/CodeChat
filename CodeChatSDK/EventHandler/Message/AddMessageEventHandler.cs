using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 添加消息
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">添加消息参数</param>
    public delegate void AddMessageEventHandler(Object sender, AddMessageEventArgs args);

    /// <summary>
    /// 添加消息参数
    /// </summary>
    public class AddMessageEventArgs
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
