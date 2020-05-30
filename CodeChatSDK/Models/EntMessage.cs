using Newtonsoft.Json;

namespace CodeChatSDK.Models
{
    /// <summary>
    /// 实体消息
    /// </summary>
    public class EntMessage : MessageBase
    {
        /// <summary>
        /// 类型
        /// </summary>
        [JsonProperty("tp")]
        public string Tp { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public EntData Data { get; set; }
    }
}
