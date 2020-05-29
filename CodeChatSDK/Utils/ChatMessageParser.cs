﻿using CodeChatSDK.Models;
using Newtonsoft.Json;
using Pbx;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.Utils
{
    /// <summary>
    /// 消息解析器
    /// </summary>
    public class ChatMessageParser
    {
        /// <summary>
        /// 解析服务器信息
        /// </summary>
        /// <param name="message">服务器信息</param>
        /// <returns>聊天消息</returns>
        public static ChatMessage Parse(ServerData message)
        {
            ChatMessage chatMsg;
            if (message.Head.ContainsKey("mime"))
            {
                chatMsg = JsonConvert.DeserializeObject<ChatMessage>(message.Content.ToStringUtf8()
                        , new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                chatMsg.Content = message.Content.ToStringUtf8();
                chatMsg.SeqId = message.SeqId;
                chatMsg.TopicName = message.Topic;
                if(ParseGenericAttachment(chatMsg).Count != 0)
                {
                    chatMsg.IsPlainText = false;
                }
                else
                {
                    chatMsg.IsPlainText = true;
                }
                ParseCode(chatMsg);
            }
            else
            {
                chatMsg = new ChatMessage() { Text = JsonConvert.DeserializeObject<string>(message.Content.ToStringUtf8()) };
                chatMsg.Content = message.Content.ToStringUtf8();
                chatMsg.SeqId = message.SeqId;
                chatMsg.TopicName = message.Topic;
                chatMsg.IsPlainText = true;
            }

            if (message.Topic.StartsWith("usr"))
            {
                chatMsg.MessageType = "user";
            }
            else if (message.Topic.StartsWith("grp"))
            {
                chatMsg.MessageType = "group";
            }
            return chatMsg;
        }

        /// <summary>
        /// 解析消息Content
        /// </summary>
        /// <param name="message"></param>
        public static void ParseContent(ChatMessage message)
        {
            if (message.Content == null)
            {
                return;
            }

            if (message.IsPlainText == true)
            {
                message.Text = JsonConvert.DeserializeObject<string>(message.Content);
            }
            else
            {
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message.Content
                        , new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });
                message.Ent = chatMessage.Ent;
                message.Fmt = chatMessage.Fmt;
                message.Text = chatMessage.Text;
            }
        }

        /// <summary>
        /// 解析代码
        /// </summary>
        /// <param name="message"></param>
        public static void ParseCode(ChatMessage message)
        {
            if (message.Text == null || message.Fmt == null)
            {
                return;
            }
            foreach (var fmt in message.Fmt)
            {
                if (!string.IsNullOrEmpty(fmt.Tp))
                {
                    if (fmt.Tp == "CO")
                    {
                        string code = message.Text.Substring(fmt.At.Value, fmt.Len.Value);
                        string type = message.Text.Substring(fmt.At.Value + fmt.Len.Value);
                        CodeType codeType = (CodeType)Enum.Parse(typeof(CodeType), type);
                        message.Text = code;
                        message.IsCode = true;
                        message.CodeType = codeType;
                    }
                }
            }
        }

        public static List<string> ParseUrl(ChatMessage message, string baseurl)
        {
            List<string> urls = new List<string>();
            List<EntData> attachments = ParseGenericAttachment(message);
            foreach (EntData attachment in attachments)
            {
                string url = baseurl + attachment.Ref;
                urls.Add(url);
            }
            return urls;
        }

        public static List<string> ParseImageBase64(ChatMessage message)
        {
            List<string> base64s = new List<string>();
            List<EntData> images = ParseImages(message);
            foreach (EntData image in images)
            {
                string base64 = image.Val;
                base64s.Add(base64);
            }
            return base64s;
        }

        /// <summary>
        /// 获取富文本内容
        /// </summary>
        /// <returns>富文本内容</returns>
        public static string ParseFormattedText(ChatMessage message)
        {
            if (message.Text == null)
            {
                return null;
            }
            var textArray = message.Text.ToCharArray();
            if (message.Fmt != null)
            {
                foreach (var fmt in message.Fmt)
                {
                    if (!string.IsNullOrEmpty(fmt.Tp))
                    {
                        if (fmt.Tp == "BR")
                        {
                            textArray[fmt.At.Value] = '\n';
                        }
                    }
                }
            }
            return new String(textArray);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="tp">类型</param>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseEntDatas(ChatMessage message, string tp)
        {
            var ret = new List<EntData>();
            if (message.Ent != null)
            {
                foreach (var ent in message.Ent)
                {
                    if (ent.Tp == tp)
                    {
                        ret.Add(ent.Data);
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取实体数据列表
        /// </summary>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseMentions(ChatMessage message)
        {
            return ParseEntDatas(message, "MN");
        }

        /// <summary>
        /// 获取图像
        /// </summary>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseImages(ChatMessage message)
        {
            return ParseEntDatas(message, "IM");
        }

        /// <summary>
        /// 获取哈希标签
        /// </summary>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseHashTags(ChatMessage message)
        {
            return ParseEntDatas(message, "HT");
        }

        /// <summary>
        /// 获取超链接
        /// </summary>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseLinks(ChatMessage message)
        {
            return ParseEntDatas(message, "LN");
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <returns>实体数据列表</returns>
        public static List<EntData> ParseGenericAttachment(ChatMessage message)
        {
            return ParseEntDatas(message, "EX");
        }
    }
}
