using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.Repository.Sqlite
{
    public class SqliteAccountRepository : IAccountRepository
    {
        private readonly string path;

        public string Path { get { return path; } }

        public SqliteAccountRepository(string databasePath)
        {
            path = $@"Data Source={ databasePath }\account.db";
            var db = new AccountContext(path);
            db.Database.EnsureCreated();
            db.Dispose();
        }

        public ITopicRepository Topics => new SqliteTopicRepository(new AccountContext(path));

        public ISubscriberRepository Subscribers => new SqliteSubscriberRepository(new AccountContext(path));

        public IMessageRepository Messages => new SqliteMessageRepository(new AccountContext(path));

        public IAccountRepository GetRepository()
        {
            return new SqliteAccountRepository(path);
        }
    }
}
