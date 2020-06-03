using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
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
            foreach(var sub in subList) {

                if (sub.UserId == topicName) {
                    return ModelConverter.SubscriberToContact(sub);
                }
            }
            return null;
        }

        public static async Task<string> GetResult(String code, String lang, String userID)
        {
            //StringBuilder builder = new StringBuilder();
            //builder.Append("?");
            //builder.Append("codeLanguage=" + lang);
            //builder.Append("&");
            //builder.Append("sourceCode=" + code);
            //builder.Append("&");
            //builder.Append("codesID=" + userID);
            //string url = applicationUrl + builder.ToString();
            var applicationUrl = "http://39.98.65.135:8080/Complier";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(applicationUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            #region 添加Post参数
            //StringBuilder builder = new StringBuilder();
            //builder.Append("&");
            //builder.Append("codeLanguage=" + lang);
            //builder.Append("sourceCode=" + code);
            //builder.Append("codesID=" + userID);
            String result;
            try {
                byte[] data = Encoding.UTF8.GetBytes(SerializeMessage(new Message { codeLanguage = lang, codesID = userID, sourceCode = code }));
                webRequest.ContentLength = data.Length;
                using (Stream reqStream = webRequest.GetRequestStream()) {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                #endregion
                var res = await webRequest.GetResponseAsync();
                HttpWebResponse webResponse = (HttpWebResponse)res;

                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream())) {
                    result = streamReader.ReadToEnd();
                }
            }
            catch(System.Net.WebException e) {
                Debug.WriteLine(e.Message);
                return null;
            }


            return result;
        }

        public static string SerializeMessage(Message message)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Message));
            using (MemoryStream stream = new MemoryStream()) {
                formatter.WriteObject(stream, message);
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                Debug.WriteLine(result);
                return result;
            }
        }

    }

    [Serializable]
    public class Message
    {
        public String codeLanguage;
        public String sourceCode;
        public String codesID;
    }
}
