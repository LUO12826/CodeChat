using CodeChatSDK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeChatSDK.Repository.Sqlite
{
    /// <summary>
    /// SQLite话题数据表
    /// </summary>
    public class SqliteTopicRepository : ITopicRepository
    {
        /// <summary>
        /// 用户数据库上下文
        /// </summary>
        private readonly AccountContext db;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">用户数据库上下文</param>
        public SqliteTopicRepository(AccountContext context)
        {
            db = context;
        }

        /// <summary>
        /// 删除话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>话题</returns>
        public async Task DeleteTopic(Topic topic)
        {
            var currentTopic = await db.Topics.FirstOrDefaultAsync(t => t.Name == topic.Name);

            if (currentTopic != null)
            {
                db.Topics.Remove(currentTopic);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取所有话题
        /// </summary>
        /// <returns>话题列表</returns>
        public async Task<IEnumerable<Topic>> GetAsync()
        {
            return await db.Topics.
                            Where(t => t.IsArchived == false).
                            OrderByDescending(t => t.Weight).
                            ThenByDescending(t => t.LastUsed).
                            ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的话题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>话题列表</returns>
        public async Task<IEnumerable<Topic>> GetAsync(string condition)
        {
            return await db.Topics.
                            Where(t => t.IsArchived == false && 
                            (t.Name.Contains(condition) ||
                            t.PrivateComment.Contains(condition))).
                            OrderByDescending(t => t.LastUsed).
                            ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的话题分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>话题列表</returns>
        public IEnumerable<Topic> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            var query = db.Topics.
                            Where(t => t.IsArchived == false &&
                            (t.Name.Contains(condition) ||
                            t.PrivateComment.Contains(condition))).
                            OrderByDescending(t => t.LastUsed);

            pageCount = query.Count() % pageSize == 0 ? (query.Count() / pageSize) : (query.Count() / pageSize) + 1;
            return query.Skip(pageIndex-1).Take(pageSize).ToList();

        }

        /// <summary>
        /// 新增或更新话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>话题</returns>
        public async Task<Topic> UpsertTopic(Topic topic)
        {
            var currentTopic = await db.Topics.FirstOrDefaultAsync(t => t.Name == topic.Name);

            if (currentTopic == null)
            {
                db.Topics.Add(topic);
            }
            else
            {
                db.Entry(currentTopic).CurrentValues.SetValues(topic);
            }

            await db.SaveChangesAsync();
            return topic;
        }

        /// <summary>
        /// 获取订阅者数据表
        /// </summary>
        /// <returns>订阅者数据表</returns>
        public ITopicRepository GetRepository()
        {
            return new SqliteTopicRepository(db.GetContext());
        }
    }
}
