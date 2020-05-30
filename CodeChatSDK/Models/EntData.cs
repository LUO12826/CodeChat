using Newtonsoft.Json;

namespace CodeChatSDK.Models
{
    /// <summary>
    /// 实体数据
    /// </summary>
    public class EntData : MessageBase
    {
        /// <summary>
        /// MIME
        /// </summary>
        [JsonProperty("mime")]
        public string Mime { get; set; }

        /// <summary>
        /// Data值
        /// </summary>
        [JsonProperty("val")]
        public string Val { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// 相对URL
        /// </summary>
        [JsonProperty("ref")]
        public string Ref { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        [JsonProperty("width")]
        public int? Width { get; set; }

        /// <summary>
        /// 图片高度
        /// </summary>
        [JsonProperty("height")]
        public int? Height { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [JsonProperty("size")]
        public int? Size { get; set; }

        /// <summary>
        /// 按键动作
        /// </summary>
        [JsonProperty("act")]
        public string Act { get; set; }
    }
}
