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
    public class SqliteMessageRepository:IMessageRepository
    {
        private readonly AccountContext db;

        public SqliteMessageRepository(AccountContext context)
        {
            db = context;
        }

        public async Task DeleteMessage(ChatMessage message)
        {
            var currentMessage = await db.Messages.FirstOrDefaultAsync(m => m.Id == message.Id);

            if (currentMessage != null)
            {
                db.Messages.Remove(currentMessage);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync()
        {
            return await db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderByDescending(m=>m.SeqId).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(string condition)
        {
            return await db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderByDescending(m => m.SeqId).
                            Where(m => m.Content.Contains(condition)).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(string condition,int skip,int take)
        {
            return await db.Messages.
                            OrderBy(m => m.TopicName).
                            OrderByDescending(m => m.SeqId).
                            Where(m => m.Content.Contains(condition)).
                            Skip(skip).
                            Take(take).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name).
                            OrderBy(m => m.SeqId).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int since, int before)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.SeqId >= since &&
                            m.SeqId < before).
                            OrderBy(m => m.SeqId).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int limit)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name).
                            OrderBy(m => m.SeqId).
                            Take(limit).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.Content.Contains(condition)).
                            OrderBy(m => m.SeqId).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition, int skip, int take)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.Content.Contains(condition)).
                            OrderBy(m => m.SeqId).
                            Skip(skip).
                            Take(take).
                            ToListAsync();
        }

        public IMessageRepository GetRepository()
        {
            return new SqliteMessageRepository(db.GetContext());
        }

        public async Task<ChatMessage> UpsertMessage(ChatMessage message)
        {
            var currentMessage = await db.Messages.FirstOrDefaultAsync(s => s.TopicName== message.TopicName && s.SeqId == message.SeqId);

            if (currentMessage == null)
            {
                db.Messages.Add(message);
            }
            else
            {
                //long id = currentMessage.Id;
                //db.Entry(currentMessage).CurrentValues.SetValues(message);
                //currentMessage.Id = id;
                //message.Id = id;
            }

            await db.SaveChangesAsync();
            return message;
        }
    }
}
