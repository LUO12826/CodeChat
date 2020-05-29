using CodeChatSDK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository.Sqlite
{
    class SqliteSubscriberRepository:ISubscriberRepository
    {
        private readonly AccountContext db;

        public SqliteSubscriberRepository(AccountContext context)
        {
            db = context;
        }

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

        public async Task<IEnumerable<Subscriber>> GetAsync()
        {
            return await db.Subscribers.ToListAsync();
        }

        public async Task<IEnumerable<Subscriber>> GetAsync(string condition)
        {
            return await db.Subscribers.
                            Where(s => s.UserId.Contains(condition) ||
                            s.Username.Contains(condition)).
                            ToListAsync();
        }

        public async Task<IEnumerable<Subscriber>> GetAsync(Topic topic)
        {
            return await db.Subscribers.
                            Where(s => s.TopicName==topic.Name).
                            ToListAsync();
        }

        public ISubscriberRepository GetRepository()
        {
            return new SqliteSubscriberRepository(db.GetContext());
        }

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
    }
}
