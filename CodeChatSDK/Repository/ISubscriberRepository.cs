using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository
{
    /// <summary>
    /// 订阅者数据库接口
    /// </summary>
    public interface ISubscriberRepository
    {
        /// <summary>
        /// 获取所有订阅者
        /// </summary>
        /// <returns>订阅者列表</returns>
        Task<IEnumerable<Subscriber>> GetAsync();

        /// <summary>
        /// 获取符合条件的订阅者
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>订阅者列表</returns>
        Task<IEnumerable<Subscriber>> GetAsync(string condition);

        /// <summary>
        /// 获取符合条件的订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>订阅者列表</returns>
        IEnumerable<Subscriber> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 获取对应话题的订阅者
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>订阅者列表</returns>
        Task<IEnumerable<Subscriber>> GetAsync(Topic topic);

        /// <summary>
        /// 新增或更新订阅者
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <returns>订阅者</returns>
        Task<Subscriber> UpsertSubscriber(Subscriber subscriber);

        /// <summary>
        /// 删除订阅者
        /// </summary>
        /// <param name="subscriber">订阅者</param>
        /// <returns>订阅者</returns>
        Task DeleteSubscriber(Subscriber subscriber);

        /// <summary>
        /// 获取订阅者数据库
        /// </summary>
        /// <returns>订阅者数据库</returns>
        ISubscriberRepository GetRepository();
    }
}
