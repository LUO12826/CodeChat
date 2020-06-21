using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using CAC.client.ContactPage;
using CodeChatSDK.SDKClient;
using CodeChatSDK.Controllers;
using CodeChatSDK.Models;

namespace CAC.client
{
    /// <summary>
    /// 存储使用SDK时必要的对象引用。实现与SDK的对接。
    /// </summary>
    class CommunicationCore
    {
        public static Client client => Client.Instance;

        public static AccountController accountController = new AccountController();

        public static Account account = AccountController.Instance;

        /// <summary>
        /// 配置客户端，一般在app启动时配置。
        /// </summary>
        public static void ConfigClient()
        {
            client.ServerHost = GlobalConfigs.ServerHost;
            client.SetHttpApi(GlobalConfigs.HttpApi, GlobalConfigs.ApiKey);
        }

        public static async Task Login(string userName, string password)
        {
            account.Username = userName;
            account.Password = password;
            var repo = await DatabaseHelper.GetAccountRepoForUser(userName);
            await accountController.SetDatabase(repo);
            accountController.Login();
        }

        public static async Task Register(string userName, string password, string formattedName, string email)
        {
            account.Username = userName;
            account.Password = password;
            account.FormattedName = formattedName;
            account.Email = email;

            var repo = await DatabaseHelper.GetAccountRepoForUser(userName);
            await accountController.SetDatabase(repo);
            accountController.Register();
        }

        public static ContactItemViewModel GetContactByTopicName(string topicName)
        {
            var subList = account.SubscriberList;

            var sub = subList.Where(x => x.UserId == topicName).FirstOrDefault();
            if(sub != null) {
                return ModelConverter.SubscriberToContact(sub);
            }
            return null;
        }

    }

}
