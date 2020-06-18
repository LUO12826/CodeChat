using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Controllers.MessageManager
{
    public class MessageHolder
    {
        /// <summary>
        /// 待发送信息队列
        /// </summary>
        public Queue<Message> MessagesToSend
        {
            get;
        }

        /// <summary>
        /// 待处理信息队列
        /// </summary>
        public Queue<Message> MessagesToHandle
        {
            get;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public MessageHolder()
        {
            MessagesToSend = new Queue<Message>();
            MessagesToHandle = new Queue<Message>();
        }

        /// <summary>
        /// 由json字符串获取消息加入到待处理队列
        /// </summary>
        /// <param name="json"></param>
        public void GetNewMessagesToHandle(string json)
        {
            Queue<Message> newMessages = JsonHelper.DeserializeMsgQueue(json);
            foreach(Message msg in newMessages)
            {
                MessagesToHandle.Enqueue(msg);
            }
        }
    }
}
