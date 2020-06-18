using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Controllers.MessageManager
{
    /// <summary>
    /// 队列监视器
    /// </summary>
    class MessageWatcher
    {
        /// <summary>
        /// 拥有该监视器的控制器
        /// </summary>
        private SynchronousController holder;

        /// <summary>
        /// 队列
        /// </summary>
        private MessageHolder messageQueues;

        /// <summary>
        /// 服务器位置
        /// </summary>
        private string Url { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="messageHolder"></param>
        /// <param name="url"></param>
        public MessageWatcher(MessageHolder messageHolder,string url)
        {
            messageQueues = messageHolder;
            Url = url;
        }

        /// <summary>
        /// 开始监视
        /// </summary>
        public void StartWatch(SynchronousController caller)
        {
            holder = caller;
            Thread WatchThread = new Thread(WatchMessageQueue);
            WatchThread.Start();
        }

        /// <summary>
        /// 监视队列并转交MessageAnalyst处理
        /// </summary>
        private void WatchMessageQueue()
        {
            while (true)
            {
                Debug.WriteLine("Watcher is running. ");
                Thread.Sleep(1000);

                if (messageQueues.MessagesToHandle.Count > 0)
                {
                    CallMessageAnalyst();
                }

                if (messageQueues.MessagesToSend.Count > 0)
                {
                    HttpHelper.RequestForSynMsg(Url, messageQueues.MessagesToSend, messageQueues);
                    messageQueues.MessagesToSend.Clear();
                }
            }
        }

        /// <summary>
        /// 调用消息分析器
        /// </summary>
        private void CallMessageAnalyst()
        {
            do
            {
                Message msg = messageQueues.MessagesToHandle.Dequeue();
                MessageAnalyst.DoAnalyse(msg, holder);
            }
            while (messageQueues.MessagesToHandle.Count > 0);
        }
    }
}
