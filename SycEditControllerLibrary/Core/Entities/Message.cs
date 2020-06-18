using SynEditControllerLibrary.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Entities
{
    [DataContract]
    public class Message
    {
        /// <summary>
        /// CallerID用于标识具体的身份，用于在服务端的管理
        /// </summary>
        [DataMember]
        public string CallerID { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [DataMember]
        public MessageType Type { get; set; }

        /// <summary>
        /// 消息细节
        /// </summary>
        [DataMember]
        public string Detail { get; set; }

        /// <summary>
        /// 行hash值
        /// </summary>
        [DataMember]
        public int LineHash { get; set; }

        /// <summary>
        /// Id用于标识在一次同步session中的角色立场，用于区分是发起方还是加入方
        /// </summary>
        [DataMember]
        public Identity Identity { get; set; }
    }
}
