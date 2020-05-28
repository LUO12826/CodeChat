using CodeChatSDK.Models;
using CodeChatSDK.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Controllers
{
    public class SubscriberController
    {
        private Subscriber instance;

        private ISubscriberRepository db;

        public SubscriberController(ISubscriberRepository database)
        {
            db = database;
        }

        public void SetSubscriber(Subscriber subscriber)
        {
            instance = subscriber;
        }

        public async void UpsertSubscriber()
        {
            await db.UpsertSubscriber(instance);
        }

        public async void DeleteSubscriber()
        {
            await db.DeleteSubscriber(instance);
        }

        public async Task<List<Subscriber>> GetSubscribers()
        {
            return await db.GetAsync() as List<Subscriber>;
        }

        public async Task<List<Subscriber>> GetSubscribers(Topic topic)
        { 
            return await db.GetAsync(topic) as List<Subscriber>;
        }

        public async Task<List<Subscriber>> SearchSubscriber(string condition)
        {
            return await db.GetAsync(condition) as List<Subscriber>;
        }
    }
}
