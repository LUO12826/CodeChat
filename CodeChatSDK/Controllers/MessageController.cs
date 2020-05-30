using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using CodeChatSDK.Utils;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Controllers
{
    /// <summary>
    /// 消息控制器
    /// </summary>
    public class MessageController
    {
        /// <summary>
        /// 消息对象
        /// </summary>
        private ChatMessage instance;

        /// <summary>
        /// 消息数据库
        /// </summary>
        private IMessageRepository db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">消息数据库</param>
        public MessageController(IMessageRepository database)
        {
            db = database;
        }

        /// <summary>
        /// 设置消息对象
        /// </summary>
        /// <param name="message">消息对象</param>
        public void SetMessage(ChatMessage message)
        {
            instance = message;
        }

        /// <summary>
        /// 插入或更新消息
        /// </summary>
        public async void UpsertMessage()
        {
            await db.UpsertMessage(instance);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        public async void DeleteMessage()
        {
            await db.DeleteMessage(instance);
        }

        /// <summary>
        /// 标记消息为已读
        /// </summary>
        public async void NoteRead()
        {
            instance.IsRead = true;
            await db.UpsertMessage(instance);
        }

        /// <summary>
        /// 获取消息（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>消息列表</returns>
        public async Task<List<ChatMessage>> GetMessages(Topic topic)
        {
            List<ChatMessage> messages = await db.GetAsync(topic) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 获取消息（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="limit">消息条数限制</param>
        /// <returns>消息列表</returns>
        public async Task<List<ChatMessage>> GetMessages(Topic topic,int limit)
        {
            List<ChatMessage> messages = await db.GetAsync(topic,limit) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 获取消息（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="since">消息起始SeqId</param>
        /// <param name="before">消息结束SeqId</param>
        /// <returns>消息列表</returns>
        public async Task<List<ChatMessage>> GetMessages(Topic topic,int since,int before)
        {
            List<ChatMessage> messages = await db.GetAsync(topic,since,before) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 本地搜索消息（全部话题）
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns>搜索结果</returns>
        public async Task<List<ChatMessage>> SearchMessage(string condition)
        {
            List<ChatMessage> messages = await db.GetAsync(condition) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 本地搜索消息分页加载（全部话题）
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>搜索结果</returns>
        public List<ChatMessage> SearchMessage(string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            List<ChatMessage> messages = db.GetSync(condition,pageIndex,pageSize,ref pageCount) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 本地搜索消息（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">搜索条件</param>
        /// <returns>搜索结果</returns>
        public async Task<List<ChatMessage>> SearchMessage(Topic topic,string condition)
        {
            List<ChatMessage> messages = await db.GetAsync(topic,condition) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }

        /// <summary>
        /// 本地搜索消息分页加载（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>搜索结果</returns>
        public List<ChatMessage> SearchMessage(Topic topic, string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            List<ChatMessage> messages = db.GetSync(topic, condition, pageIndex, pageSize, ref pageCount) as List<ChatMessage>;
            messages.ForEach(m =>
            {
                ChatMessageParser.ParseContent(m);
                ChatMessageParser.ParseCode(m);
            });
            return messages;
        }
    }
}
