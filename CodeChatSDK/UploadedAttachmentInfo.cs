using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK
{
    public class UploadedAttachmentInfo
    {
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string Mime { get; set; }
        public long Size { get; set; }
        public string RelativeUrl { get; set; }
    }
}
