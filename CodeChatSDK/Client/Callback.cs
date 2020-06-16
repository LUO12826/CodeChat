using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.SDKClient
{
    /// <summary>
    /// 回调
    /// </summary>
    public class Callback
    {
        /// <summary>
        /// 回调ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 回调参数
        /// </summary>
        public string Arg { get; private set; }

        /// <summary>
        /// 回调类型
        /// </summary>
        public CallbackType Type { get; private set; }

        /// <summary>
        /// 回调委托
        /// </summary>
        public Action<string,MapField<string,ByteString>> Action { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">回调ID</param>
        /// <param name="type">回调类型</param>
        /// <param name="action">回调委托</param>
        /// <param name="arg">回调参数</param>
        public Callback(string id,CallbackType type, Action<string, MapField<string, ByteString>> action,string arg="")
        {
            Id = id;
            Type = type;
            Action = action;
            Arg = arg;
        }
    }
}
