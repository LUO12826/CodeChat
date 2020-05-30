using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 未连接服务器
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">未连接服务器参数</param>
    public delegate void DisconnectedEventHandler(Object sender, DisconnectedEventArgs args);

    /// <summary>
    /// 未连接服务器参数
    /// </summary>
    public class DisconnectedEventArgs
    {
        /// <summary>
        /// 未连接异常
        /// </summary>
        public Exception Exception { get; set; }
    }
}
