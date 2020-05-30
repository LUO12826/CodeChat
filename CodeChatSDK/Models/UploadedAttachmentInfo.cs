namespace CodeChatSDK.Models
{
    /// <summary>
    /// 上传附件信息
    /// </summary>
    public class UploadedAttachmentInfo
    {
        /// <summary>
        /// 文件全名（带路径）
        /// </summary>
        public string FullFileName { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件MIME类型
        /// </summary>
        public string Mime { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 相对URL
        /// </summary>
        public string RelativeUrl { get; set; }
    }
}
