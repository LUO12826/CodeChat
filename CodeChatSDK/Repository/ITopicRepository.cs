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
        /// <returns>话题列表</returns>
        Task<IEnumerable<Topic>> GetAsync();

        /// <summary>
        /// 获取符合条件的话题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>话题列表</returns>
        Task<IEnumerable<Topic>> GetAsync(string condition);

        /// <summary>
        /// 获取符合条件的话题分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>话题列表</returns>
        IEnumerable<Topic> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 新增或更新话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>话题</returns>
        Task<Topic> UpsertTopic(Topic topic);

        /// <summary>
        /// 删除话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>话题</returns>
        Task DeleteTopic(Topic topic);

        /// <summary>
        /// 获取话题数据库
        /// </summary>
        /// <returns>话题数据库</returns>
        ITopicRepository GetRepository();
    }
}
