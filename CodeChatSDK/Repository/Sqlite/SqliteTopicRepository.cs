using CodeChatSDK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository.Sqlite
{
    public class SqliteTopicRepository : ITopicRepository
    {
        private readonly AccountContext db;

        public SqliteTopicRepository(AccountContext context)
        {
            db = context;
        }

        public async Task DeleteTopic(Topic topic)
        {
            var currentTopic = await db.Topics.FirstOrDefaultAsync(t => t.Name == topic.Name);

            if (currentTopic != null)
            {
                db.Topics.Remove(currentTopic);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Topic>> GetAsync()
        {
            return await db.Topics.
                            Where(t => t.IsArchived == false).
                            OrderByDescending(t => t.Weight).
                            ThenByDescending(t => t.LastUsed).
                            ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetAsync(string condition)
        {
            return await db.Topics.
                            Where(t => t.IsArchived == false && 
                            (t.Name.Contains(condition) ||
                            t.PrivateComment.Contains(condition))).
                            OrderByDescending(t => t.LastUsed).
                            ToListAsync();
        }

        public async Task<IEnumerable<Topic>> GetAsync(string condition, int skip, int take)
        {
            return await db.Topics.
                            Where(t => t.IsArchived == false &&
                            (t.Name.Contains(condition) ||
                            t.PrivateComment.Contains(condition))).
                            OrderByDescending(t => t.LastUsed).
                            Skip(skip).
                            Take(take).
                            ToListAsync();
        }

        public ITopicRepository GetRepository()
        {
            return new SqliteTopicRepository(db.GetContext());
        }

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
    }
}
