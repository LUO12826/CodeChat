﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 设置用户信息
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">设置用户信息参数</param>
    public delegate void SetAccountEventHandler(Object sender, SetAccountEventArgs args);

    /// <summary>
    /// 设置用户信息参数
    /// </summary>
    public class SetAccountEventArgs
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
