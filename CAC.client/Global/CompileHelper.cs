

using CAC.client.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CAC.client
{
    class CompileHelper
    {
        private static string complieUrl = "http://39.98.65.135:8080/Test";


        public static string SerializeMessage(CompileInfo info)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(CompileInfo));
            using (MemoryStream stream = new MemoryStream()) {
                formatter.WriteObject(stream, info);
                string result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                return result;
            }
        }

        public static string Post(string url, string content)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(content);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public static async Task<string> Compile(string lang, string code, string id)
        {
            if(lang.IsNullOrEmpty() || code.IsNullOrEmpty() || id.IsNullOrEmpty()) {  
                return null; 
            }

            var info = new CompileInfo() {
                codeLanguage = lang,
                sourceCode = code,
                codesID = id
            };

            string serialized = SerializeMessage(info);
            Debug.WriteLine(serialized);
            return await Task.Run(() => {
                return Post(complieUrl, serialized);
            });
        }
    }

    [Serializable]
    public class CompileInfo
    {
        public string codeLanguage;
        public string sourceCode;
        public string codesID;
    }
}
