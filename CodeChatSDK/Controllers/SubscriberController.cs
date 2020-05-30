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
        /// <summary>
        /// 订阅者对象
        /// </summary>
        private Subscriber instance;

        /// <summary>
        /// 订阅者数据库
        /// </summary>
        private ISubscriberRepository db;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="database">订阅者数据库</param>
        public SubscriberController(ISubscriberRepository database)
        {
            db = database;
        }

        /// <summary>
        /// 设置订阅者对象
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        public void SetSubscriber(Subscriber subscriber)
        {
            instance = subscriber;
        }

        /// <summary>
        /// 更改订阅者状态
        /// </summary>
        /// <param name="isOnline"></param>
        public async void ChangeSubscriberState(bool isOnline)
        {
            instance.Online = isOnline;
            await db.UpsertSubscriber(instance);
        }

        /// <summary>
        /// 插入或更新订阅者
        /// </summary>
        public async void UpsertSubscriber()
        {
            await db.UpsertSubscriber(instance);
        }

        /// <summary>
        /// 删除订阅者
        /// </summary>
        public async void DeleteSubscriber()
        {
            await db.DeleteSubscriber(instance);
        }

        /// <summary>
        /// 获取订阅者
        /// </summary>
        /// <returns>订阅者列表</returns>
        public async Task<List<Subscriber>> GetSubscribers()
        {
            return await db.GetAsync() as List<Subscriber>;
        }

        /// <summary>
        /// 获取订阅者（特定话题）
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>订阅者列表</returns>
        public async Task<List<Subscriber>> GetSubscribers(Topic topic)
        { 
            return await db.GetAsync(topic) as List<Subscriber>;
        }

        /// <summary>
        /// 本地搜索订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <returns>搜索结果</returns>
        public async Task<List<Subscriber>> SearchSubscriber(string condition)
        {
            return await db.GetAsync(condition) as List<Subscriber>;
        }

        /// <summary>
        /// 本地搜索订阅者分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>搜索结果</returns>
        public List<Subscriber> SearchSubscriber(string condition,int pageIndex,int pageSize,ref int pageCount)
        {
            return db.GetSync(condition,pageIndex,pageSize,ref pageCount) as List<Subscriber>;
        }
    }
}
