using Newtonsoft.Json;

namespace CodeChatSDK.Models
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
