﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CodeChatSDK.MessageBuilder;

namespace CodeChatSDK
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public class Subscriber
    {
        /// <summary>
        /// 订阅者ID
        /// </summary>
        [Key]
        public string UserId { get; set; }

        /// <summary>
        /// 所属话题名
        /// </summary>
        [ForeignKey("TopicName")]
        public string TopicName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 头像数据
        /// </summary>
        public string PhotoData { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool Online { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 头像类型
        /// </summary>
        public string PhotoType { get; set; }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Subscriber subscriber = obj as Subscriber;
            return subscriber != null && this.UserId.Equals(subscriber.UserId);
        }
    }
}
