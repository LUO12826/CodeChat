using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    /// <summary>
    /// 接受消息
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">接受消息参数</param>
    public delegate void ReceiveMessageEventHandler(Object sender, ReceiveMessageEventArgs args);

    /// <summary>
    /// 接受消息参数
    /// </summary>
    public class ReceiveMessageEventArgs
    {
        /// <summary>
        /// 聊天消息
        /// </summary>
        public ChatMessage Message { get; set; }
    }
}
