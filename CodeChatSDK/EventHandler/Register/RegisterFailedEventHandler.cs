using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 注册失败
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">注册失败参数</param>
    public delegate void RegisterFailedEventHandler(Object sender, RegisterFailedEventArgs args);

    /// <summary>
    /// 注册失败参数
    /// </summary>
    public class RegisterFailedEventArgs
    {
        /// <summary>
        /// 注册失败异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
