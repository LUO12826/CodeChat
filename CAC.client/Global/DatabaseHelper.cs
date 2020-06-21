using CodeChatSDK.Repository.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CAC.client
{
    /// <summary>
    /// 辅助SDK建立数据库。
    /// </summary>
    class DatabaseHelper
    {
        public static async Task<SqliteAccountRepository> GetAccountRepoForUser(string userName)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder databaseFolder = null;
            try {
                //存在则获取文件夹
                databaseFolder = await folder.GetFolderAsync(userName);
            }
            catch {
                //不存在则创建文件夹
                databaseFolder = await folder.CreateFolderAsync(userName);
            }

            //获取数据库存储路径
            string databasePath = databaseFolder.Path;

            //连接或创建数据库
            return new SqliteAccountRepository(databasePath);
        }
    }
}
