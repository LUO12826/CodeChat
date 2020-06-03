using System;

using Windows.Storage;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System.Collections.Generic;

namespace CAC.client
{
    class AccountHelper
    {
        public static string AccountListFileName = "account.dat";

        public static async Task<List<AccountRecord>> GetAccountList()
        {
            
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = null;
            try {
                file = await folder.GetFileAsync(AccountListFileName);
            }
            catch {
                file = await folder.CreateFileAsync(AccountListFileName);
            }

            return BinRead(file.Path) as List<AccountRecord>;
        }

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

        public static void BinFormat(string fileName, object obj)
        {
            var fs = new FileStream(fileName, FileMode.Create);
            var formatter = new BinaryFormatter();
            formatter.Serialize(fs, obj);
            fs.Close();
        }

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
            foreach(var rec in records) {
                if(rec.UserName == userName) {
                    return rec;
                }
            }
            return null;
        }
    }

    [Serializable]
    class AccountRecord
    {
        public string UserName;
        public bool KeepLogin;
        public bool RememberPassword;
    }
}
