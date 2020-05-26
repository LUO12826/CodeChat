using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public class MessageBase
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
