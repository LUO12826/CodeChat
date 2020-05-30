using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 登陆失败
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">登陆失败参数</param>
    public delegate void LoginFailedEventHandler(Object sender, LoginFailedEventArgs args);

    /// <summary>
    /// 登陆失败参数
    /// </summary>
    public class LoginFailedEventArgs
    {
        /// <summary>
        /// 登陆失败异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
