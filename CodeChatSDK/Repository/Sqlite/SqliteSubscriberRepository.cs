using CodeChatSDK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository.Sqlite
{
    /// <summary>
    /// SQLite订阅者数据表
    /// </summary>
    class SqliteSubscriberRepository:ISubscriberRepository
    {
        /// <summary>
        /// 用户数据库上下文
        /// </summary>
        private readonly AccountContext db;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">用户数据库上下文</param>
        public SqliteSubscriberRepository(AccountContext context)
        {
            db = context;
        }

        /// <summary>
        /// 删除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <returns>订阅者</returns>
        public async Task DeleteSubscriber(Subscriber Subscriber)
        {
            var currentSubscriber = await db.Subscribers.FirstOrDefaultAsync(s=>s.UserId==Subscriber.UserId);

            if (currentSubscriber != null)
            {
                var messages = await db.Messages.Where(m => m.TopicName == currentSubscriber.TopicName).ToListAsync();
                var topics = await db.Topics.Where(t => t.Name == Subscriber.TopicName).ToListAsync();
                db.Messages.RemoveRange(messages);
                db.Topics.RemoveRange(topics);
                db.Subscribers.Remove(currentSubscriber);
                await db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 获取所有订阅者
        /// </summary>
        /// <returns>订阅者列表</returns>
        public async Task<IEnumerable<Subscriber>> GetAsync()
        {
            return await db.Subscribers.ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的订阅者
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>订阅者列表</returns>
        public async Task<IEnumerable<Subscriber>> GetAsync(string condition)
        {
            return await db.Subscribers.
                            Where(s => s.UserId.Contains(condition) ||
                            s.Username.Contains(condition)).
                            ToListAsync();
        }

        /// <summary>
        /// 获取对应话题的订阅者
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>订阅者列表</returns>
        public async Task<IEnumerable<Subscriber>> GetAsync(Topic topic)
        {
            return await db.Subscribers.
                            Where(s => s.TopicName==topic.Name).
                            ToListAsync();
        }

        /// <summary>
        /// 获取符合条件的订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>订阅者列表</returns>
        public IEnumerable<Subscriber> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount)
        {
            var query = db.Subscribers.
                            Where(s => s.UserId.Contains(condition) ||
                            s.Username.Contains(condition));

            pageCount = query.Count() % pageSize == 0 ? (query.Count() / pageSize) : (query.Count() / pageSize) + 1;
            ;
            return query.Skip(pageIndex - 1).Take(pageSize).ToList();

        }

        /// <summary>
        /// 新增或更新订阅者
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <returns>订阅者</returns>
        public async Task<Subscriber> UpsertSubscriber(Subscriber subscriber)
        {
            var currentSubscriber = await db.Subscribers.FirstOrDefaultAsync(s => s.UserId == subscriber.UserId);

            if (currentSubscriber == null)
            {
                db.Subscribers.Add(subscriber);
            }
            else
            {
                db.Entry(currentSubscriber).CurrentValues.SetValues(subscriber);
            }

            await db.SaveChangesAsync();
            return subscriber;
        }

        /// <summary>
        /// 获取订阅者数据表
        /// </summary>
        /// <returns>订阅者数据表</returns>
        public ISubscriberRepository GetRepository()
        {
            return new SqliteSubscriberRepository(db.GetContext());
        }
    }
}
