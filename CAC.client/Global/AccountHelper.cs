using System;
using Windows.Storage;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CAC.client
{
    /// <summary>
    /// 记录登录过的账号，以便在用户之后登录时给出提示。“自动登录”和“记住密码”功能也需要记录登录过的账号。
    /// </summary>
    class AccountHelper
    {
        public static string AccountListFileName = "account.dat";

        /// <summary>
        /// 读取账号列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<AccountRecord>> GetAccountList()
        {
            
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try {
                file = await folder.GetFileAsync(AccountListFileName);
            }
            catch {
                return new List<AccountRecord>();
            }

            return BinRead(file.Path) != null ? (BinRead(file.Path) as List<AccountRecord>) : new List<AccountRecord>();
        }

        /// <summary>
        /// 存储账号列表
        /// </summary>
        public static async void StorageAccountList(List<AccountRecord> account)
        {
            if (account == null)
                return;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try {
                file = await folder.GetFileAsync(AccountListFileName);
            }
            catch {
                file = await folder.CreateFileAsync(AccountListFileName);
            }

            BinFormat(file.Path, account);
        }

        //序列化对象
        public static void BinFormat(string fileName, object obj)
        {
            var fs = new FileStream(fileName, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, obj);
            fs.Close();
        }

        //反序列化对象
        public static object BinRead(string fileName)
        {
            var fs = new FileStream(fileName, FileMode.Open);
            
            var formatter = new BinaryFormatter();
            object obj;
            try {
                obj = formatter.Deserialize(fs);
            } 
            catch {
                obj = null;
            }
            finally {
                fs.Close();
            }
            return obj;
        }

        public static AccountRecord GetRecord(List<AccountRecord> records, string userName)
        {
            if (records == null || records.Count <= 0)
                return null;
            return records.Where(x => x.UserName == userName).FirstOrDefault();
        }
    }

    /// <summary>
    /// 一条账号记录
    /// </summary>
    [Serializable]
    class AccountRecord
    {
        public string UserName;
        public bool KeepLogin;
        public bool RememberPassword;

        public override string ToString()
        {
            return UserName;
        }
    }
}
