using CodeChatSDK.Models;
using CodeChatSDK.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository.Sqlite
{
    /// <summary>
    /// SQLite消息数据表
    /// </summary>
    public class SqliteMessageRepository:IMessageRepository
    {
        /// <summary>
        /// 用户数据库上下文
        /// </summary>
        private readonly AccountContext db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">用户数据库上下文</param>
        public SqliteMessageRepository(AccountContext context)
        {
            db = context;
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns>消息</returns>
        public async Task DeleteMessage(ChatMessage message)
        {
            var currentMessage = await db.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);

            if (currentMessage != null)
            {
                db.Messages.Remove(currentMessage);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取所有消息
        /// </summary>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync()
        {
            return await db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderBy(m=>m.SeqId).
                            ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的消息
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync(string condition)
        {
            return await db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderByDescending(m => m.SeqId).
                            Where(m => m.Content.Contains(condition)).
                            ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的消息分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>消息列表</returns>
        public IEnumerable<ChatMessage> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            var query = db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderBy(m => m.SeqId).
                            Where(m => m.Content.Contains(condition));

            pageCount = query.Count() % pageSize == 0 ? (query.Count() / pageSize) : (query.Count() / pageSize) + 1;

            return query.Skip(pageIndex - 1).Take(pageSize).ToList();

        }

        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name).
                            OrderBy(m => m.SeqId).
                            ToListAsync();
        }

        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="since">起始SeqId</param>
        /// <param name="before">结束SeqId</param>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int since, int before)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.SeqId >= since &&
                            m.SeqId < before).
                            OrderByDescending(m => m.SeqId).
                            ToListAsync();
        }

        /// <summary>
        /// 获取限制条数的对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="limit">限制条数</param>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int limit)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name).
                            OrderByDescending(m => m.SeqId).
                            Take(limit).
                            ToListAsync();
        }

        /// <summary>
        /// 获取对应话题的符合条件的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">条件</param>
        /// <returns>消息列表</returns>
        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.Content.Contains(condition)).
                            OrderBy(m => m.SeqId).
                            ToListAsync();
        }

        /// <summary>
        /// 获取对应话题的符合条件的消息分页加载
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>消息列表</returns>
        public IEnumerable<ChatMessage> GetSync(Topic topic, string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            var query = db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.Content.Contains(condition)).
                            OrderBy(m => m.SeqId);

            pageCount = query.Count() % pageSize == 0 ? (query.Count() / pageSize) : (query.Count() / pageSize) + 1;

            return query.Skip(pageIndex - 1).Take(pageSize).ToList();

        }

        /// <summary>
        /// 新增或更新消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns>消息</returns>
        public async Task<ChatMessage> UpsertMessage(ChatMessage message)
        {
            var currentMessage = await db.Messages.FirstOrDefaultAsync(s => s.TopicName== message.TopicName && s.SeqId == message.SeqId);

            if (currentMessage == null)
            {
                db.Messages.Add(message);
            }
            else
            {
                //db.Entry(currentMessage).CurrentValues.SetValues(message);
            }

            await db.SaveChangesAsync();
            return message;
        }


        /// <summary>
        /// 获取消息数据表
        /// </summary>
        /// <returns>消息数据表</returns>
        public IMessageRepository GetRepository()
        {
            return new SqliteMessageRepository(db.GetContext());
        }
    }
}
