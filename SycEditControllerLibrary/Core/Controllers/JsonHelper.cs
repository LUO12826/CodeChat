using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Controllers
{
    /// <summary>
    /// 序列化与反序列化消息
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 序列化消息
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string SerializeMessage(Message target)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Message));
            using(MemoryStream stream = new MemoryStream())
            {
                formatter.WriteObject(stream, target);
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                return result;
            }
        }

        /// <summary>
        /// 序列化消息队列
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static string SerializeMsgQueue(Queue<Message> targets)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Queue<Message>));
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.WriteObject(stream, targets);
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                return result;
            }
        }

        /// <summary>
        /// 反序列化消息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Message DeserializeMessage(string json)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Message));
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                Message result = formatter.ReadObject(stream) as Message;
                return result;
            }
        }

        /// <summary>
        /// 反序列化消息队列
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Queue<Message> DeserializeMsgQueue(string json)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Queue<Message>));
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
            {
                Queue<Message> result = formatter.ReadObject(stream) as Queue<Message>;
                return result;
            }
        }
    }
}
