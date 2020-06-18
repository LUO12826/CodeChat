using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Mime;
using SynEditControllerLibrary.Core.Controllers.MessageManager;
using SynEditControllerLibrary.Core.Entities;

namespace SynEditControllerLibrary.Core.Controllers
{
    /// <summary>
    /// 与服务器交流
    /// </summary>
    public class HttpHelper
    {
        /// <summary>
        /// 异步向服务器发送信息,并将获得的消息加入到队列
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messages"></param>
        /// <param name="messageQueues"></param>
        public async static void RequestForSynMsg(String url, Queue<Message> messages, MessageHolder messageQueues)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            string msgJson = JsonHelper.SerializeMsgQueue(messages);

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(msgJson);
            }

            WebResponse webResponse =(HttpWebResponse)await webRequest.GetResponseAsync();
            String result;

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            messageQueues.GetNewMessagesToHandle(result);
        }

        /// <summary>
        /// 向服务器发送初始化消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messages"></param>
        /// <param name="messageQueues"></param>
        /// <returns>若成功发送则返回真</returns>
        public static Boolean RequestForIniMsg(String url, Queue<Message> messages, MessageHolder messageQueues)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json; charset=utf-8";

            string msgJson = JsonHelper.SerializeMsgQueue(messages);

            try
            {
                using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    streamWriter.Write(msgJson);
                }

                WebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                String result;

                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                messageQueues.GetNewMessagesToHandle(result);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
