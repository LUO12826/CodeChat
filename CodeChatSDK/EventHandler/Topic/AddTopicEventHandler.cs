using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 添加话题
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">添加话题参数</param>
    public delegate void AddTopicEventHandler(Object sender, AddTopicEventArgs args);

    /// <summary>
    /// 添加话题参数
    /// </summary>
    public class AddTopicEventArgs
    {
        /// <summary>
        /// 话题
        /// </summary>
        public Topic Topic { get; set; }
    }
}
