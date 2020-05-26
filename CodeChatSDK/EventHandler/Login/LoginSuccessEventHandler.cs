using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    /// <summary>
    /// 登陆成功
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">登陆成功参数</param>
    public delegate void LoginSuccessEventHandler(Object sender, LoginSuccessEventArgs args);

    /// <summary>
    /// 登陆成功参数
    /// </summary>
    public class LoginSuccessEventArgs
    {
        /// <summary>
        /// 聊天消息
        /// </summary>
        public ChatMessage Message { get; set; }
    }
}
