using Newtonsoft.Json;

namespace CodeChatSDK.Models
{
    /// <summary>
    /// 格式消息
    /// </summary>
    public class FmtMessage : MessageBase
    {
        /// <summary>
        /// 起始字符位置
        /// </summary>
        [JsonProperty("at")]
        public int? At { get; set; }

        /// <summary>
        /// 格式长度
        /// </summary>
        [JsonProperty("len")]
        public int? Len { get; set; }

        /// <summary>
        /// 格式类型
        /// </summary>
        [JsonProperty("tp")]
        public string Tp { get; set; }

        /// <summary>
        /// ENT索引
        /// </summary>
        [JsonProperty("key")]
        public int? Key { get; set; }
    }
}
