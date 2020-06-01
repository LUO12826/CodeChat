using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository
{
    /// <summary>
    /// 消息数据表接口
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// 获取所有消息
        /// </summary>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync();

        /// <summary>
        /// 获取符合条件的消息
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync(string condition);


        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic);

        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="since">起始SeqId</param>
        /// <param name="before">结束SeqId</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int since, int before);

        /// <summary>
        /// 获取限制条数的对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="limit">限制条数</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int limit);


        /// <summary>
        /// 获取对应话题的符合条件的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">条件</param>
        /// <returns>消息列表</returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition);

        /// <summary>
        /// 获取对应话题的符合条件的消息分页加载
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>消息列表</returns>
        IEnumerable<ChatMessage> GetSync(Topic topic, string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 获取符合条件的消息分页加载
        /// </summary>
        /// <param name="condition">搜索条件</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="pageCount">页面数目</param>
        /// <returns>消息列表</returns>
        IEnumerable<ChatMessage> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 新增或更新消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns>消息</returns>
        Task<ChatMessage> UpsertMessage(ChatMessage chatMessage);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns>消息</returns>
        Task DeleteMessage(ChatMessage chatMessage);

        /// <summary>
        /// 获取消息数据表
        /// </summary>
        /// <returns>消息数据表</returns>
        IMessageRepository GetRepository();
    }
}
