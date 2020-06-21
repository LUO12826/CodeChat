using CAC.client.ContactPage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAC.client.CodeEditorPage
{
    /// <summary>
    /// 代码编辑会话信息。
    /// </summary>
    class CodeEditSessionInfo
    {
        public ContactItemViewModel Contact { get; set; }
        public long MessageID { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
        public DateTime CreateTime { get; set; }
        public override int GetHashCode()
        {
            return Contact.UserID.GetHashCode() + MessageID.GetHashCode();
        }
    }
}
