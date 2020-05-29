using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository
{
    /// <summary>
    /// 话题数据库接口
    /// </summary>
    public interface ITopicRepository
    {
        /// <summary>
        /// 获取所有话题
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Topic>> GetAsync();

        /// <summary>
        /// 获取符合条件的话题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<IEnumerable<Topic>> GetAsync(string condition);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        IEnumerable<Topic> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 新增或更新话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        Task<Topic> UpsertTopic(Topic topic);

        /// <summary>
        /// 删除话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        Task DeleteTopic(Topic topic);

        ITopicRepository GetRepository();
    }
}
