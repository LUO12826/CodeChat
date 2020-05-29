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
    public class MessageController
    {

        private ChatMessage instance;

        private IMessageRepository db;

        public MessageController(IMessageRepository database)
        {
            db = database;
        }

        public void SetMessage(ChatMessage message)
        {
            instance = message;
        }

        public async void UpsertMessage()
        {
            await db.UpsertMessage(instance);
        }

        public async void DeleteMessage()
        {
            await db.DeleteMessage(instance);
        }

        public async void NoteRead()
        {
            instance.IsRead = true;
            await db.UpsertMessage(instance);
        }

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
