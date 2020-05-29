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
            return await db.Messages.ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(string condition)
        {
            return await db.Messages.
                            Where(m => m.Content.Contains(condition)).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int since, int before)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.SeqId >= since &&
                            m.SeqId < before).
                            ToListAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition)
        {
            return await db.Messages.
                            Where(m => m.TopicName == topic.Name &&
                            m.Content.Contains(condition)).
                            ToListAsync();
        }

        public IMessageRepository GetRepository()
        {
            return new SqliteMessageRepository(db.GetContext());
        }

        public async Task<ChatMessage> UpsertMessage(ChatMessage message)
        {
            var currentMessage = await db.Messages.FirstOrDefaultAsync(s => s.Id == message.Id);

            if (currentMessage == null)
            {
                db.Messages.Add(message);
            }
            else
            {
                db.Entry(currentMessage).CurrentValues.SetValues(message);
            }

            await db.SaveChangesAsync();
            return message;
        }
    }
}
