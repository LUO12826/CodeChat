using CodeChatSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeChatSDK.Repository
{
    /// <summary>
    /// 消息数据库接口
    /// </summary>
    public interface IMessageRepository
    {
        /// <summary>
        /// 获取所有消息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ChatMessage>> GetAsync();

        /// <summary>
        /// 获取符合条件的消息
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<IEnumerable<ChatMessage>> GetAsync(string condition);


        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic);

        /// <summary>
        /// 获取对应话题的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="since">起始SeqId</param>
        /// <param name="before">结束SeqId</param>
        /// <returns></returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int since, int before);

        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, int limit);


        /// <summary>
        /// 获取对应话题的符合条件的消息
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        Task<IEnumerable<ChatMessage>> GetAsync(Topic topic, string condition);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        IEnumerable<ChatMessage> GetSync(Topic topic, string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        IEnumerable<ChatMessage> GetSync(string condition, int pageIndex, int pageSize, ref int pageCount);

        /// <summary>
        /// 新增或更新消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns></returns>
        Task<ChatMessage> UpsertMessage(ChatMessage chatMessage);

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="chatMessage">消息</param>
        /// <returns></returns>
        Task DeleteMessage(ChatMessage chatMessage);

        IMessageRepository GetRepository();
    }
}
