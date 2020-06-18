using SynEditControllerLibrary.Core.Entities;
using SynEditControllerLibrary.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Controllers.MessageManager
{
    public class MessageWrapper
    {
        /// <summary>
        /// 一般情况下的msg包装方法
        /// </summary>
        /// <param name="callerID">调用者ID</param>
        /// <param name="identity">通信身份立场</param>
        /// <param name="type">消息类型</param>
        /// <param name="hash">行hash</param>
        /// <param name="detail">内容细节</param>
        /// <returns></returns>
        public static Message WriteMsg(string callerID,Identity identity,MessageType type, int hash, string detail)
        {
            Message msg = new Message
            {
                CallerID = callerID,
                Identity = identity,
                Type = type,
                LineHash = hash,
                Detail = detail
            };
            return msg;
        }

        /// <summary>
        /// 申请新连接时保留的初始行信息
        /// 处理该消息时不用返回确认信息
        /// </summary>
        /// <param name="callerID"></param>
        /// <returns></returns>
        public static Message IniMsg(string callerID, int hash, string content)
        {
            Message msg = new Message
            {
                CallerID = callerID,
                Identity = Identity.Organiger,
                Type = MessageType.INI,
                LineHash = hash,
                Detail = content
            };
            return msg;
        }

        /// <summary>
        /// 加入一个连接时的申请消息，只用发送一次
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="responderID"></param>
        /// <returns></returns>
        public static Message JoinMsg(string callerID, string responderID)
        {
            Message msg = new Message
            {
                CallerID = callerID,
                Identity = Identity.Responder,
                Type = MessageType.JOIN,
                LineHash = -1,
                Detail = responderID
            };
            return msg;
        }

        /// <summary>
        /// 结束一次连接
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static Message EndMsg(string callerID, Identity identity)
        {
            Message msg = new Message
            {
                CallerID = callerID,
                Identity = identity,
                Type = MessageType.END,
                LineHash = -1,
                Detail = ""
            };
            return msg;
        }

        /// <summary>
        /// 申请连接
        /// </summary>
        /// <param name="callerID"></param>
        /// <returns></returns>
        public static Message ApplyMsg(string callerID)
        {
            Message msg = new Message
            {
                CallerID = callerID,
                Identity = Identity.Organiger,
                Type = MessageType.APPLY,
                LineHash = -1,
                Detail = ""
            };
            return msg;
        }
    }
}
