using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CAC.client.ContactPage;
using CodeChatSDK;
using CodeChatSDK.Controllers;
using CodeChatSDK.Models;

namespace CAC.client
{
    class CommunicationCore
    {
        public static Client client => Client.Instance;

        public static AccountController accountController = new AccountController();

        public static Account account = AccountController.Instance;


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

        public static ContactItemViewModel GetContactByTopicName(string topicName)
        {
            var subList = account.SubscriberList;
            foreach(var sub in subList) {

                if (sub.UserId == topicName) {
                    return ModelConverter.SubscriberToContact(sub);
                }
            }
            return null;
        }
    }
}
