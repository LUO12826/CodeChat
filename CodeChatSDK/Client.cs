using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pbx;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Pbx.Node;

namespace CodeChatSDK
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        public string ClientName => "CodeChat";

        /// <summary>
        /// 类库版本
        /// </summary>
        public string LibaryVersion => "0.16.0";

        /// <summary>
        /// 平台信息
        /// </summary>
        public string Platform => $"({RuntimeInformation.OSDescription}{RuntimeInformation.OSArchitecture})";

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// 上传点
        /// </summary>
        public string UploadEndpoint { get; set; }

        /// <summary>
        /// 下载点
        /// </summary>
        public string DownloadEndpoint { get; set; }

        /// <summary>
        /// 监听端口
        /// </summary>
        public string Listen { get; set; }

        /// <summary>
        /// 验证提要
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public Account CurrentAccount { get; private set; }

        public event ReceiveMessageEventHandler ReceiveMessage;

        /// <summary>
        /// 消息Id
        /// </summary>
        private long nextId;

        /// <summary>
        /// 流
        /// </summary>
        private AsyncDuplexStreamingCall<ClientMsg, ServerMsg> stream;

        /// <summary>
        /// Key
        /// </summary>
        private string apiKey;

        /// <summary>
        /// BaseURL
        /// </summary>
        private string apiBaseUrl;

        /// <summary>
        /// Channel
        /// </summary>
        private Channel channel;

        /// <summary>
        /// TokenSource
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// 待发送消息列表
        /// </summary>
        private Queue<ClientMsg> sendMessageQueue;

        /// <summary>
        /// 已完成列表
        /// </summary>
        private Dictionary<string, Callback> onCompletion;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="account">当前用户</param>
        public Client(Account account)
        {
            nextId = new Random().Next(1, 1000);
            Schema = "basic";
            CurrentAccount = account;
            UploadEndpoint = "/v0/file/u";
            DownloadEndpoint = "/v0/file/s";
            apiKey = "http://127.0.0.1:6060";
            apiBaseUrl = "AQEAAAABAAD_rAp4DJh05a1HAwFT3A6K";
            cancellationTokenSource = new CancellationTokenSource();
            sendMessageQueue = new Queue<ClientMsg>();
            onCompletion = new Dictionary<string, Callback>();
        }

        /// <summary>
        /// 设置服务器API
        /// </summary>
        /// <param name="apiBaseUrl">BaseUrl</param>
        /// <param name="apikey">Key</param>
        public void SetHttpApi(string apiBaseUrl, string apikey)
        {
            this.apiBaseUrl = apiBaseUrl;
            this.apiKey = apikey;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            stream = InitStream();
            SendMessageLoop();
            try
            {
                await ClientMessageLoop();
            }
            catch (Exception e)
            {
                Thread.Sleep(2000);
                Reset();
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        public void LogIn()
        {
            ClientPost(Hello());
            ClientPost(Login(Schema, CurrentAccount.GetSecret()));
            ClientPost(Subscribe(new Topic("me")));
        }

        /// <summary>
        /// 注册
        /// </summary>
        public void Register()
        {
            ClientPost(Hello());
            ClientPost(Account(Schema, CurrentAccount.GetSecret(), "new"));
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        public void ForgetPassword()
        {
            Schema = "reset";
            string base64Secret = Convert.ToBase64String(Encoding.UTF8.GetBytes("basic:email:" + CurrentAccount.Email));
            ByteString secret = ByteString.CopyFromUtf8(base64Secret);
            ClientPost(Hello());
            ClientPost(Login(Schema, secret));
        }

        /// <summary>
        /// 重置连接
        /// </summary>
        public void Reset()
        {
            try
            {
                onCompletion.Clear();
                while (sendMessageQueue.Count > 0)
                {
                    sendMessageQueue.Dequeue();
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">当前Topic</param>
        /// <param name="chatMessage">消息内容</param>
        /// <param name="noEcho">是否获取当前信息</param>
        public async void Send(Topic topic,ChatMessage chatMessage,bool noEcho= false)
        {
            ClientPost(Subscribe(topic));
            ClientPost(Publish(topic, noEcho, chatMessage));
            try
            {
                await ClientMessageLoop();
            }catch(Exception e)
            {

            }
        }

        /// <summary>
        /// 刷新好友列表
        /// </summary>
        /// <returns></returns>
        public async void RefreshSubscriberList()
        {
            ClientPost(GetSubs(new Topic("me"), true));
            try
            {
                await ClientMessageLoop();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 获取历史信息
        /// </summary>
        /// <param name="topic">当前Topic</param>
        /// <param name="since">起始消息SeqID</param>
        /// <param name="before">结束消息SeqID</param>
        /// <returns></returns>
        public async Task Load(Topic topic,int since,int before)
        {
            ClientPost(GetData(topic,since,before));
            try
            {
                await ClientMessageLoop();
            }catch(Exception e)
            {

            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="redirectUrl">重定向URL</param>
        /// <returns></returns>
        public async Task<UploadedAttachmentInfo> Upload(string filePath, string redirectUrl = "")
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            try
            {
                var fullFileName = Path.GetFullPath(filePath);
                var fileInfo = new FileInfo(fullFileName);

                UploadedAttachmentInfo attachmentInfo = new UploadedAttachmentInfo();
                attachmentInfo.FullFileName = fullFileName;
                attachmentInfo.FileName = fileInfo.Name;
                attachmentInfo.Size = fileInfo.Length;
                attachmentInfo.Mime = $"file/{fileInfo.Extension}";

                var restClient = new RestClient(apiBaseUrl);
                RestRequest request;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    request = new RestRequest(UploadEndpoint, Method.PUT);
                }
                else
                {
                    request = new RestRequest(redirectUrl, Method.PUT);
                }

                request.AddHeader("X-Tinode-APIKey", apiKey);
                request.AddXmlBody("id", GetNextId());

                request.AddFile("file", fullFileName);
                var cancellationTokenSource = new CancellationTokenSource();
                var response = await restClient.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.BadRequest)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var obj = JsonConvert.DeserializeObject<JToken>(response.Content);
                        var code = obj["ctrl"]["code"].ToString();
                        var url = obj["ctrl"]["params"]["url"].ToString();
                        attachmentInfo.RelativeUrl = url;

                        return attachmentInfo;
                    }
                    else if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
                    {
                        var tmpRedirectUrl = string.Empty;
                        //307,should reupload to the redirect url
                        foreach (var h in response.Headers)
                        {
                            if (h.Name.ToLower() == "location")
                            {
                                tmpRedirectUrl = h.Value.ToString();
                                break;
                            }
                        }
                        return await Upload(filePath, redirectUrl);
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="filePath"></param>
        public void SetAvator(Topic topic,string filePath)
        {
            ClientPost(SetPhoto(topic, filePath));
        }

        /// <summary>
        /// 设置私密备注
        /// </summary>
        /// <returns></returns>
        public void SetPrivateComment(Topic topic,string comment)
        {
            ClientPost(SetPrivate(topic, comment));
        }

        private AsyncDuplexStreamingCall<ClientMsg,ServerMsg> InitStream()
        {
            var options = new List<ChannelOption>
            {
                new ChannelOption("grpc.keepalive_time_ms", 2000),
                new ChannelOption("grpc.keepalive_timeout_ms",2000)
            };

            channel = new Channel(ServerHost, ChannelCredentials.Insecure, options);
            var stub = new NodeClient(channel);
            var stream = stub.MessageLoop(cancellationToken: cancellationTokenSource.Token);

            return stream;
        }

        private void SendMessageLoop()
        {
            Task sendBackendTask = new Task(async () =>
             {
                 while (!cancellationTokenSource.IsCancellationRequested)
                 {
                     if (sendMessageQueue.Count > 0)
                     {
                         ClientMsg message = sendMessageQueue.Dequeue();
                         try
                         {
                             await stream.RequestStream.WriteAsync(message);
                         }
                         catch(Exception e)
                         {
                             sendMessageQueue.Enqueue(message);
                             Thread.Sleep(1000);
                         }
                     }
                     else
                     {
                         Thread.Sleep(10);
                     }
                 }
             },cancellationTokenSource.Token);
            sendBackendTask.Start();
        }

        private async Task ClientMessageLoop()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                if(!await stream.ResponseStream.MoveNext())
                {
                    break;
                }
                
                ServerMsg response = stream.ResponseStream.Current;
                if (response.Ctrl != null)
                {
                    ExecuteCallback(response.Ctrl.Id, response.Ctrl.Code, response.Ctrl.Text, response.Ctrl.Topic, response.Ctrl.Params);
                }
                else if (response.Data != null)
                {
                    Topic currentTopic = CurrentAccount.GetTopicByName(response.Data.Topic);
                    ClientPost(NoteRead(currentTopic.Name, response.Data.SeqId));
                    Thread.Sleep(50);
                    ChatMessage replyMessage = MessageBuilder.Parse(response.Data.Clone());

                    ReceiveMessage(this, new ReceiveMessageEventArgs() { Message = replyMessage });
                    int newSeqId = response.Data.SeqId;
                    int oldSeqId = currentTopic.MessageList.Count==0?0:currentTopic.MessageList[0].SeqId;
                    if (oldSeqId == 0 || oldSeqId >= newSeqId)
                    {
                        currentTopic.AddFirstMessage(replyMessage);
                    }
                    else
                    {
                        currentTopic.AddMessage(replyMessage);
                    }         
                }
                else if (response.Pres != null&& response.Pres.Topic == "me")
                {
                    if ((response.Pres.What == ServerPres.Types.What.On || response.Pres.What == ServerPres.Types.What.Msg))
                    {
                        ClientPost(Subscribe(new Topic(response.Pres.Topic)));
                    }
                    else if (response.Pres.What == ServerPres.Types.What.Off)
                    {
                        ClientPost(Leave(new Topic(response.Pres.Topic)));
                    }
                }
                else if (response.Meta != null)
                {
                    OnGetMeta(response.Meta);
                }
                else
                {

                }
            }
        }

        private ClientMsg NoteRead(string topic, int seqId)
        {
            return new ClientMsg() { Note = new ClientNote() { SeqId = seqId, Topic = topic, What = InfoNote.Read } };
        }

        private void ClientPost(ClientMsg message)
        {
            sendMessageQueue.Enqueue(message);
        }

        private ClientMsg Hello()
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Hi,null));

            return new ClientMsg() { Hi = new ClientHi() { Id = id, UserAgent = $"{ClientName}/{LibaryVersion} {Platform}; gRPC-csharp/{LibaryVersion}", Ver = LibaryVersion, Lang = "EN" } };
        }

        private ClientMsg Login(string scheme, ByteString secret)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Login,null));

            return new ClientMsg() { Login = new ClientLogin() { Id = id, Scheme = scheme, Secret = secret } };
        }

        private ClientMsg Account(string scheme, ByteString secret,string userId)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Acc, null));
            ByteString publicField = ByteString.CopyFromUtf8("fn:" + CurrentAccount.FormattedName);
            return new ClientMsg() { Acc = new ClientAcc() { Id = id, Scheme = scheme, Secret = secret, Login = true, UserId = userId, Desc = new SetDesc() { Public = publicField } } };
        }

        private ClientMsg Publish(Topic topic,bool noEcho,ChatMessage message)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Pub, null));

            ClientPub pub= new ClientPub() { Id = id, Topic = topic.Name, NoEcho = noEcho, Content = ByteString.CopyFromUtf8(message.ToString()) };
            pub.Head.Add("mime", ByteString.CopyFromUtf8("\"text/x-drafty\""));
            return new ClientMsg() { Pub = pub };
        }

        private ClientMsg Subscribe(Topic topic)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Sub, null,topic.Name));

            return new ClientMsg() { Sub = new ClientSub() { Id = id, Topic = topic.Name } };
        }

        private ClientMsg Leave(Topic topic)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Leave, null, topic.Name));

            return new ClientMsg() { Leave = new ClientLeave() { Id = id, Topic = topic.Name } };
        }

        private string GetNextId()
        {
            ++nextId;
            return nextId.ToString();
        }

        private ClientMsg GetSubs(Topic topic, bool getAll = false)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null, topic.Name));

            if (getAll)
            {
                return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "sub" } } };
            }
            else
            {
                return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name } };
            }
        }

        private ClientMsg GetData(Topic topic,int since,int before)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "data", Data = new GetOpts() { SinceId = since,BeforeId=before } } } };
        }


        private ClientMsg SetTags(Topic topic,RepeatedField<string> tags)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            return new ClientMsg() { Set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() {} } };
        }

        public ClientMsg SetPrivate(Topic topic,string comment)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));
            ByteString privateField = ByteString.CopyFromUtf8("{\"comment\":"+comment+"}");
            return new ClientMsg() { Set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() { Desc = new SetDesc() { Private = privateField } } } };
        }

        private ClientMsg SetPhoto(Topic topic,string filePath)
        {
            /*if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return null;
            }
            try
            {
                string fileExtension = Path.GetExtension(filePath).Trim('.');
                long fileSize = 0;
                string base64String = "";
                using (FileStream readStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    fileSize = readStream.Length;
                    byte[] array = new byte[fileSize];
                    readStream.Position = 0;
                    readStream.Read(array, 0, (int)fileSize);
                    base64String = Convert.ToBase64String(array);
                }*/
            long fileSize = 65535L;
            string fileExtension = "png";
            string base64String = "jajinekdinelainfdkgnsklgn";
                var id = GetNextId();
                AddCallback(id, new Callback(id, CallbackType.Set, null));
                //ByteString publicField = ByteString.CopyFromUtf8("{'photo':{'data':'< 65536,bytes:"+base64String+"','type':'"+fileExtension+"'} }");
            ByteString publicField = ByteString.CopyFromUtf8("'photo.data': < 65536,bytes:" + base64String);
            return new ClientMsg() { Set = new ClientSet() {Id = id, Topic = topic.Name, Query = new SetQuery() { Desc=new SetDesc() { Public=publicField}  } } };
            /*}
            catch(Exception e)
            {
                return null;
            }*/
        }

        private void AddCallback(string id, Callback bundle)
        {
            onCompletion.Add(id, bundle);
        }

        private void ExecuteCallback(string id,int code,string text,string topic,MapField<string,ByteString> parameters)
        {
            if (onCompletion.ContainsKey(id))
            {
                var bundle = onCompletion[id];
                var type = onCompletion[id].Type;
                onCompletion.Remove(id);

                if (code >= 200 && code <= 400)
                {
                    var arg = bundle.Arg;
                    switch (type)
                    {
                        case CallbackType.Login:
                            CurrentAccount.State = AccountState.Online;
                            break;
                        case CallbackType.Pub:
                            CurrentAccount.GetTopicByName(topic);
                            break;
                        case CallbackType.Sub:
                            CurrentAccount.AddTopic(new Topic(topic));
                            break;
                        case CallbackType.Leave:
                            CurrentAccount.RemoveTopic(new Topic(topic));
                            break;
                        default:
                            break;
                    }
                }else
                { 
                    switch (type)
                    {
                        case CallbackType.Login:
                            CurrentAccount.State = AccountState.Offline;
                            break;
                        case CallbackType.Acc:
                            CurrentAccount.State = AccountState.Duplicate;
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        private void OnGetMeta(ServerMeta meta)
        {
            if (meta.Sub != null)
            {
                foreach (var sub in meta.Sub)
                {
                    var userId = sub.UserId;
                    var online = sub.Online;
                    var topic = sub.Topic;
                    var publicInfo = sub.Public.ToStringUtf8();
                    var subObj = JsonConvert.DeserializeObject<JObject>(publicInfo);
                    string userName = topic;
                    string type = subObj == null ? "group" : "user";
                    string photoData = string.Empty;
                    string photoType = string.Empty;
                    if (subObj != null)
                    {
                        userName = subObj["fn"].ToString();
                        if (subObj.ContainsKey("photo"))
                        {
                            photoData = subObj["photo"]["data"].ToString();
                            photoType = subObj["photo"]["type"].ToString();
                        }
                    }
                    CurrentAccount.AddSubscriber(new Subscriber(userId, topic, userName, type, photoData, photoType, online));
                }
            }
        }

    }
}
