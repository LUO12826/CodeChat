using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.Repository.Sqlite
{
    /// <summary>
    /// SQLite用户数据库
    /// </summary>
    public class SqliteAccountRepository : IAccountRepository
    {
        /// <summary>
        /// 数据库路径
        /// </summary>
        private readonly string path;

        /// <summary>
        /// 数据库路径
        /// </summary>
        public string Path { get { return path; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="databasePath">数据库路径</param>
        public SqliteAccountRepository(string databasePath)
        {
            path = $@"Data Source={ databasePath }\account.db";
            var db = new AccountContext(path);
            db.Database.EnsureCreated();
            db.Dispose();
        }

        /// <summary>
        /// 话题数据表
        /// </summary>
        public ITopicRepository Topics => new SqliteTopicRepository(new AccountContext(path));

        /// <summary>
        /// 订阅者数据表
        /// </summary>
        public ISubscriberRepository Subscribers => new SqliteSubscriberRepository(new AccountContext(path));

        /// <summary>
        /// 消息数据表
        /// </summary>
        public IMessageRepository Messages => new SqliteMessageRepository(new AccountContext(path));

        /// <summary>
        /// 获取用户数据库
        /// </summary>
        /// <returns>用户数据库</returns>
        public IAccountRepository GetRepository()
        {
            return new SqliteAccountRepository(path);
        }
    }
}
