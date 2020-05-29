﻿using CodeChatSDK.EventHandler;
using CodeChatSDK.Models;
using CodeChatSDK.Utils;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pbx;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using static Pbx.Node;

namespace CodeChatSDK
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class Client
    {

        /// <summary>
        /// 客户端单例
        /// </summary>
        static private Client instance;

        /// <summary>
        /// 客户端单例
        /// </summary>
        public static Client Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Client();
                }
                return instance;
            }
        }

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
        /// Key
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// BaseURL
        /// </summary>
        public string ApiBaseUrl { get; private set; }

        /// <summary>
        /// 登陆成功事件
        /// </summary>
        public event LoginSuccessEventHandler LoginSuccessEvent;

        /// <summary>
        /// 登陆失败事件
        /// </summary>
        public event LoginFailedEventHandler LoginFailedEvent;

        /// <summary>
        /// 未连接服务器事件
        /// </summary>
        public event DisconnectedEventHandler DisconnectedEvent;

        /// <summary>
        /// 注册成功事件
        /// </summary>
        public event RegisterSuceessEventHandler RegisterSuccessEvent;

        /// <summary>
        /// 注册失败事件
        /// </summary>
        public event RegisterFailedEventHandler RegisterFailedEvent;

        /// <summary>
        /// 设置用户信息事件
        /// </summary>
        public event SetAccountEventHandler SetAccountEvent;

        /// <summary>
        /// 接受消息事件
        /// </summary>
        public event AddMessageEventHandler AddMessageEvent;

        /// <summary>
        /// 移除消息事件
        /// </summary>
        public event RemoveMessageEventHandler RemoveMessageEvent;

        /// <summary>
        /// 添加话题事件
        /// </summary>
        public event AddTopicEventHandler AddTopicEvent;

        /// <summary>
        /// 移除话题事件
        /// </summary>
        public event RemoveTopicEventHandler RemoveTopicEvent;

        /// <summary>
        /// 添加订阅者事件
        /// </summary>
        public event AddSubscriberEventHandler AddSubscriberEvent;

        /// <summary>
        /// 移除订阅者事件
        /// </summary>
        public event RemoveSubscriberEventHandler RemoveSubscriberEvent;

        public event SubscriberStateChangedEventHandler SubscriberStateChangedEvent;

        /// <summary>
        /// 消息Id
        /// </summary>
        private long nextId;

        /// <summary>
        /// 流
        /// </summary>
        private AsyncDuplexStreamingCall<ClientMsg, ServerMsg> stream;
        
        /// <summary>
        /// 上传文件令牌
        /// </summary>
        private string token;

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
        public Client()
        {
            nextId = new Random().Next(1, 1000);
            Schema = "basic";
            ServerHost = "127.0.0.1:6061";
            UploadEndpoint = "/v0/file/u";
            DownloadEndpoint = "/v0/file/s";
            ApiBaseUrl = "http://127.0.0.1:6060";
            ApiKey = "AQEAAAABAAD_rAp4DJh05a1HAwFT3A6K";
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
            ApiBaseUrl = apiBaseUrl;
            ApiKey = apikey;
        }

        /// <summary>
        /// 开始连接
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            stream = InitStream();
            SendMessageLoop();
            try
            {
                ClientMessageLoop();
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
        public void LogIn(ByteString secret)
        {
            Topic me = new Topic("me");
            ClientPost(Hello());
            ClientPost(Login(Schema, secret));
            ClientPost(Subscribe(me));
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="formattedName">显示名称</param>
        public void Register(ByteString secret, string formattedName,string email)
        {
            ClientPost(Hello());
            ClientPost(Account(Schema, secret, "new", formattedName,email));
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="email">验证邮箱</param>
        public void ForgetPassword(string email)
        {
            Schema = "reset";
            string base64Secret = Convert.ToBase64String(Encoding.UTF8.GetBytes("basic:email:" + email));
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
        /// 发送验证码
        /// </summary>
        /// <param name="secret">密钥</param>
        /// <param name="response">验证码</param>
        public void SendVerificationCode(ByteString secret, string response)
        {
            ClientPost(Login(Schema, secret,response));
            ClientPost(Subscribe(new Topic("me")));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="topic">当前Topic</param>
        /// <param name="chatMessage">消息内容</param>
        /// <param name="noEcho">是否获取当前信息</param>
        public void Send(Topic topic, ChatMessage chatMessage, bool noEcho = false)
        {
            ClientPost(Publish(topic, noEcho, chatMessage));
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="topic">消息所属话题</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public void RemoveMessage(Topic topic, ChatMessage message)
        {
            ClientPost(DeleteMessage(topic, message.SeqId));
        }

        public void NoteMessage(Topic topic,ChatMessage message)
        {
            ClientPost(NoteRead(topic.Name, message.SeqId));
        }

        /// <summary>
        /// 删除话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        public void RemoveTopic(Topic topic)
        {
            ClientPost(SetArchived(topic,true));
            ClientPost(Leave(topic, true));
        }

        /// <summary>
        /// 订阅话题
        /// </summary>
        /// <param name="topic">话题</param>
        /// <returns></returns>
        public void SubscribeTopic(Topic topic)
        {
            ClientPost(Subscribe(topic));
        }

        /// <summary>
        /// 刷新话题列表
        /// </summary>
        /// <returns></returns>
        public void RefreshTopicList()
        {
            ClientPost(GetMeSubs());
        }

        /// <summary>
        /// 获取历史信息
        /// </summary>
        /// <param name="topic">当前Topic</param>
        /// <param name="since">起始消息SeqID</param>
        /// <param name="before">结束消息SeqID</param>
        /// <returns></returns>
        public void Load(Topic topic, int since, int before)
        {
            ClientPost(GetData(topic, since, before));
        }

        /// <summary>
        /// 获取历史消息
        /// </summary>
        /// <param name="topic">当前话题</param>
        /// <param name="limit">消息条数限制</param>
        public void Load(Topic topic, int limit)
        {
            ClientPost(GetData(topic,limit));
        }

        /// <summary>
        /// 搜索订阅者
        /// </summary>
        /// <param name="condition">搜索条件</param>
        public void FindSubscriber(string condition)
        {
            ClientPost(Subscribe(new Topic("fnd")));
            if (string.IsNullOrEmpty(condition) == false)
            {
                ClientPost(SetDesc(condition));
            }
        }

        /// <summary>
        /// 搜索订阅者
        /// </summary>
        public void FindSubscriber()
        {
            ClientPost(Subscribe(new Topic("fnd")));
            ClientPost(GetFindSubs());
        }

        public void RemoveSubscriber(Subscriber subscriber)
        {
            Topic removedTopic = new Topic(subscriber.TopicName);
            ClientPost(DeleteTopic(removedTopic, true));
            ClientPost(Leave(removedTopic, true));
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="redirectUrl">重定向URL</param>
        /// <returns>附件信息</returns>
        public async Task<UploadedAttachmentInfo> Upload(string filePath, string redirectUrl = "")
        {
            //判断文件是否存在
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                //不存在直接返回
                return null;
            }

            try
            {
                //获取文件信息
                var fullFileName = Path.GetFullPath(filePath);
                var fileInfo = new FileInfo(fullFileName);

                //填充文件信息
                UploadedAttachmentInfo attachmentInfo = new UploadedAttachmentInfo();
                attachmentInfo.FullFileName = fullFileName;
                attachmentInfo.FileName = fileInfo.Name;
                attachmentInfo.Size = fileInfo.Length;
                attachmentInfo.Mime = $"file/{fileInfo.Extension}";

                //新建PUT请求
                var restClient = new RestClient(ApiBaseUrl);
                RestRequest request;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    request = new RestRequest(UploadEndpoint, Method.PUT);
                }
                else
                {
                    request = new RestRequest(redirectUrl, Method.PUT);
                }

                //填充头部与主体
                request.AddHeader("X-Tinode-APIKey", ApiKey);
                request.AddHeader("X-Tinode-Auth", $"Token {token}");
                request.AddXmlBody("id", GetNextId());
                request.AddFile("file", fullFileName);

                //发送请求
                var cancellationTokenSource = new CancellationTokenSource();
                var response = await restClient.ExecuteAsync(request, cancellationTokenSource.Token);

                //判断应答结果
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

                        //应答码307则应重新上传到重定向地址
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
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="redirectUrl">重定向URL</param>
        /// <returns></returns>
        public async Task<UploadedAttachmentInfo> Upload(StorageFile file, ulong size, byte[] bytes, string redirectUrl = "")
        {
            //判断文件是否为空
            if (file == null)
            {
                //为空直接返回
                return null;
            }

            try
            {
                //获取文件信息
                var fullFileName = file.Path;

                //填充文件信息
                UploadedAttachmentInfo attachmentInfo = new UploadedAttachmentInfo();
                attachmentInfo.FullFileName = fullFileName;
                attachmentInfo.FileName = file.DisplayName+file.FileType;
                attachmentInfo.Size = (long)size;
                attachmentInfo.Mime = $"file/{file.FileType}";

                //新建PUT请求
                var restClient = new RestClient(ApiBaseUrl);
                RestRequest request;
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    request = new RestRequest(UploadEndpoint, Method.PUT);
                }
                else
                {
                    request = new RestRequest(redirectUrl, Method.PUT);
                }

                //填充头部与主体
                request.AddHeader("X-Tinode-APIKey", ApiKey);
                request.AddHeader("X-Tinode-Auth", $"Token {token}");
                request.AddXmlBody("id", GetNextId());
                request.AddFile("file", bytes, fullFileName);

                //发送请求
                var cancellationTokenSource = new CancellationTokenSource();
                var response = await restClient.ExecuteAsync(request, cancellationTokenSource.Token);

                //判断应答结果
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

                        //应答码307则应重新上传到重定向地址
                        foreach (var h in response.Headers)
                        {
                            if (h.Name.ToLower() == "location")
                            {
                                tmpRedirectUrl = h.Value.ToString();
                                break;
                            }
                        }
                        return await Upload(file, size, bytes, redirectUrl);
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
        public void SetAvator(Topic topic, StorageFile file, ulong size, byte[] bytes)
        {
            //判断文件是否为空
            if (file == null)
            {
                //为空直接返回
                return;
            }

            //填充文件信息
            string data = Convert.ToBase64String(bytes);
            ClientPost(Subscribe(topic));
            ClientPost(SetPhoto(topic, file.FileType, size, data));
        }

        /// <summary>
        /// 设置私密备注
        /// </summary>
        /// <returns></returns>
        public void SetPrivateComment(Topic topic, string comment)
        {
            ClientPost(Subscribe(topic));
            ClientPost(SetPrivate(topic, comment));
        }

        /// <summary>
        /// 设置标签
        /// </summary>
        /// <param name="topic">话题</param>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public void SetTags(Topic topic,List<string> tags)
        {
            ClientPost(Subscribe(topic));
            ClientPost(SetTag(topic, tags));
        }

        /// <summary>
        /// 设置显示名称
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="formattedName"></param>
        /// <returns></returns>
        public void SetDescription(Topic topic, string formattedName)
        {
            ClientPost(Subscribe(topic));
            ClientPost(SetDesc(topic, formattedName));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public void ChangePassword(ByteString secret)
        {
            ClientPost(Account(Schema, secret));
        }

        private AsyncDuplexStreamingCall<ClientMsg, ServerMsg> InitStream()
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
                         catch (Exception e)
                         {
                             sendMessageQueue.Enqueue(message);
                             Thread.Sleep(1000);
                         }
                     }
                     else
                     {
                         Thread.Sleep(100);
                     }
                 }
             }, cancellationTokenSource.Token);
            sendBackendTask.Start();
        }

        private void ClientMessageLoop()
        {
            Task receiveTask = new Task(async () =>
              {
                  while (!cancellationTokenSource.IsCancellationRequested)
                  {
                      try
                      {
                          if (!await stream.ResponseStream.MoveNext())
                          {
                              Thread.Sleep(100);
                          }
                          else
                          {
                              ServerMsg response = stream.ResponseStream.Current;
                              if (response.Ctrl != null)
                              {
                                  ExecuteCallback(response.Ctrl.Id, response.Ctrl.Code, response.Ctrl.Text, response.Ctrl.Topic, response.Ctrl.Params);
                              }
                              else if (response.Data != null)
                              {
                                  Thread.Sleep(100);
                                  ChatMessage replyMessage = ChatMessageParser.Parse(response.Data.Clone());
                                  AddMessageEvent(this, new AddMessageEventArgs() { TopicName = response.Data.Topic, Message = replyMessage });
                              }
                              else if (response.Pres != null)
                              {
                                  ClientPost(Subscribe(new Topic(response.Pres.Src)));

                                  if ((response.Pres.What == ServerPres.Types.What.On))
                                  {
                                      
                                      SubscriberStateChangedEvent?.Invoke(this, new SubscriberStateChangedEventArgs() { Subscriber = new Subscriber() { TopicName = response.Pres.Src, UserId = response.Pres.Src }, IsOnline = true });
                                      ClientPost(Subscribe(new Topic(response.Pres.Topic)));
                                  }
                                  else if (response.Pres.What == ServerPres.Types.What.Off)
                                  {

                                      SubscriberStateChangedEvent?.Invoke(this, new SubscriberStateChangedEventArgs() { Subscriber = new Subscriber() { TopicName = response.Pres.Src, UserId = response.Pres.Src }, IsOnline = false });
                                      ClientPost(Leave(new Topic(response.Pres.Topic)));
                                  }
                                  else
                                  {
                                      
                                  }
                              }
                              else if (response.Meta != null && response.Meta.Topic == "me")
                              {
                                  OnGetMeMeta(response.Meta);
                              }
                              else if (response.Meta != null && response.Meta.Topic == "fnd")
                              {
                                  OnGetFindMeta(response.Meta);
                              }
                          }
                      }
                      catch(Exception e)
                      {

                      }
                  }
              }, cancellationTokenSource.Token);
            receiveTask.Start();
            
        }

        private string GetNextId()
        {
            ++nextId;
            return nextId.ToString();
        }

        private void ClientPost(ClientMsg message)
        {
            sendMessageQueue.Enqueue(message);
        }

        private ClientMsg NoteRead(string topic, int seqId)
        {
            return new ClientMsg() { Note = new ClientNote() { SeqId = seqId, Topic = topic, What = InfoNote.Read } };
        }

        private ClientMsg Hello()
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Hi, null));

            return new ClientMsg() { Hi = new ClientHi() { Id = id, UserAgent = $"{ClientName}/{LibaryVersion} {Platform}; gRPC-csharp/{LibaryVersion}", Ver = LibaryVersion, Lang = "EN" } };
        }

        private ClientMsg Login(string scheme, ByteString secret)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Login, new Action<string, MapField<string, ByteString>>((none, paramaters) =>
            {
                OnLogin(paramaters);
            })));

            return new ClientMsg() { Login = new ClientLogin() { Id = id, Scheme = scheme, Secret = secret } };
        }

        private ClientMsg Login(string scheme, ByteString secret,string response)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Login, new Action<string, MapField<string, ByteString>>((none, paramaters) =>
            {
                OnLogin(paramaters);
            })));
            ClientLogin login = new ClientLogin() { Id = id, Scheme = scheme, Secret = secret };
            login.Cred.Add(new ClientCred() { Method = "email", Response = response });
            return new ClientMsg() { Login = login};
        }

        private ClientMsg Account(string scheme, ByteString secret, string userId, string formattedName,string email)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Acc, null));

            JObject publicObject = new JObject { { "fn", formattedName } };
            string publicInfo = publicObject.ToString();
            ByteString publicField = ByteString.CopyFromUtf8(publicInfo);
            ClientAcc acc = new ClientAcc() { Id = id, Scheme = scheme, Secret = secret, Login = true, UserId = userId, Desc = new SetDesc() { Public = publicField } };
            acc.Cred.Add(new ClientCred() { Method = "email", Value = email });
            return new ClientMsg() { Acc = acc  };
        }

        private ClientMsg Account(string scheme, ByteString secret)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Acc, null));
            return new ClientMsg() { Acc = new ClientAcc() { Id = id, Scheme = scheme, Secret = secret } };
        }

        private ClientMsg Publish(Topic topic, bool noEcho, ChatMessage message)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Pub, null));

            ClientPub pub = new ClientPub() { Id = id, Topic = topic.Name, NoEcho = noEcho, Content = ByteString.CopyFromUtf8(message.ToString()) };
            pub.Head.Add("mime", ByteString.CopyFromUtf8("\"text/x-drafty\""));
            return new ClientMsg() { Pub = pub };
        }

        private ClientMsg Subscribe(Topic topic,int limit=24)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Sub, null, topic.Name));

            ClientSub sub = new ClientSub() { Id = id, Topic = topic.Name };
            if (topic.Name != "me" && topic.Name != "fnd")
            {
                sub.GetQuery = new GetQuery() { Data = new GetOpts() { Limit = limit } };
            }
            return new ClientMsg() { Sub = sub  };
        }

        private ClientMsg Leave(Topic topic, bool unsubscribe = false)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Leave, null, topic.Name));

            return new ClientMsg() { Leave = new ClientLeave() { Id = id, Topic = topic.Name, Unsub = unsubscribe } };
        }

        private ClientMsg DeleteTopic(Topic topic, bool hard = false)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.DelTopic, null, topic.Name));

            return new ClientMsg() { Del = new ClientDel() { Id = id, Topic = topic.Name, What = ClientDel.Types.What.Topic, Hard = hard } };
        }

        private ClientMsg DeleteMessage(Topic topic, int deleteSeqId, bool hard = false)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.DelMsg, null, deleteSeqId.ToString()));

            ClientDel del = new ClientDel() { Id = id, Topic = topic.Name, What = ClientDel.Types.What.Msg, Hard = hard };
            del.DelSeq.Add(new SeqRange() { Low = deleteSeqId });
            return new ClientMsg() { Del = del };
        }

        private ClientMsg GetSubs(Topic topic)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null, topic.Name));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "sub" } } };
        }

        private ClientMsg GetMeSubs()
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null, "me"));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = "me", Query = new GetQuery() { What = "sub" } } };
        }

        private ClientMsg GetFindSubs()
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null, "fnd"));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = "fnd", Query = new GetQuery() { What = "sub" } } };
        }

        private ClientMsg GetData(Topic topic, int since, int before)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "data", Data = new GetOpts() { SinceId = since, BeforeId = before } } } };
        }

        private ClientMsg GetData(Topic topic, int limit)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "data", Data = new GetOpts() { Limit = limit } } } };
        }

        private ClientMsg GetTags(Topic topic)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "tags" } } };
        }

        private ClientMsg GetDesc(Topic topic)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Get, null));

            return new ClientMsg() { Get = new ClientGet() { Id = id, Topic = topic.Name, Query = new GetQuery() { What = "desc" } } };
        }

        private ClientMsg SetTag(Topic topic, List<string> tags)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            ClientSet set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() };
            tags.ForEach(t => set.Query.Tags.Add(t));
            return new ClientMsg() { Set =set  };
        }

        private ClientMsg SetArchived(Topic topic,bool isArchived)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            JObject privateObject = new JObject { { "arch", isArchived } };
            string privateInfo = privateObject.ToString();
            ByteString privateField = ByteString.CopyFromUtf8(privateInfo);
            return new ClientMsg() { Set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() { Desc = new SetDesc() { Private = privateField } } } };

        }

        private ClientMsg SetPrivate(Topic topic, string comment)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            JObject privateObject = new JObject { { "comment", comment } };
            string privateInfo = privateObject.ToString();
            ByteString privateField = ByteString.CopyFromUtf8(privateInfo);
            return new ClientMsg() { Set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() { Desc = new SetDesc() { Private = privateField } } } };
        }

        private ClientMsg SetPhoto(Topic topic, string fileType, ulong size, string data)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            JObject photoObject = new JObject { { "type", fileType }, { "data", data } };
            JObject publicObject = new JObject { };
            publicObject.Add("photo", photoObject);
            string publicInfo = publicObject.ToString();
            ByteString publicField = ByteString.CopyFromUtf8(publicInfo);
            ClientSet set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() { Desc = new SetDesc() { Public = publicField } } };
            return new ClientMsg() { Set = set };
        }

        private ClientMsg SetDesc(Topic topic,string formattedName)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            JObject publicObject = new JObject { { "fn", formattedName } };
            string publicInfo = publicObject.ToString();
            ByteString publicField = ByteString.CopyFromUtf8(publicInfo);
            ClientSet set = new ClientSet() { Id = id, Topic = topic.Name, Query = new SetQuery() { Desc = new Pbx.SetDesc() { Public = publicField } } };
            return new ClientMsg() { Set = set };
        }

        private ClientMsg SetDesc(string condition)
        {
            var id = GetNextId();
            AddCallback(id, new Callback(id, CallbackType.Set, null));

            ByteString publicField = ByteString.CopyFromUtf8(condition);
            ClientSet set = new ClientSet() { Id = id, Topic = "fnd", Query = new SetQuery() { Desc = new Pbx.SetDesc() { Public = publicField } } };
            return new ClientMsg() { Set = set };
        }

        private void AddCallback(string id, Callback bundle)
        {
            onCompletion.Add(id, bundle);
        }

        private void ExecuteCallback(string id, int code, string text, string topic, MapField<string, ByteString> parameters)
        {
            if (onCompletion.ContainsKey(id))
            {
                var bundle = onCompletion[id];
                var type = onCompletion[id].Type;
                onCompletion.Remove(id);

                if (code >= 200 && code <= 400)
                {
                    var arg = bundle.Arg;
                    bundle.Action?.Invoke(arg, parameters);
                    switch (type)
                    {
                        case CallbackType.Acc:
                            RegisterSuccessEvent(this, new RegisterSuccessEventArgs() { Exception = new Exception("Please enter the verification code.") });

                            break;
                        case CallbackType.Login:
                            if(code == 300)
                            {
                                RegisterFailedEvent(this, new RegisterFailedEventArgs() { Exception = new Exception(text) });
                            }
                            else
                            {
                                LoginSuccessEvent(this, new LoginSuccessEventArgs());
                            }

                            break;
                        case CallbackType.DelMsg:
                            ChatMessage removedMessage = new ChatMessage();
                            removedMessage.TopicName = topic;
                            removedMessage.SeqId = int.Parse(arg);

                            RemoveMessageEvent?.Invoke(this, new RemoveMessageEventArgs() { Message = removedMessage });
                            break;
                        case CallbackType.DelTopic:
                            Subscriber removedSubscriber = new Subscriber();
                            removedSubscriber.TopicName = topic;
                            removedSubscriber.UserId=topic;

                            RemoveSubscriberEvent?.Invoke(this, new RemoveSubscriberEventArgs() { Subscriber = removedSubscriber });
                            break;
                        case CallbackType.Sub:
                            Subscriber newSubscriber = new Subscriber();
                            newSubscriber.TopicName = topic;
                            newSubscriber.UserId = topic;
                            newSubscriber.Username = topic;

                            AddSubscriberEvent?.Invoke(this, new AddSubscriberEventArgs { Subscriber = newSubscriber });
                            break;
                        case CallbackType.Leave:
                            Topic removedTopic = new Topic(topic);

                            RemoveTopicEvent?.Invoke(this, new RemoveTopicEventArgs() { Topic = removedTopic });
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case CallbackType.Login:

                            LoginFailedEvent(this, new LoginFailedEventArgs() { Exception = new Exception(text) });
                            break;
                        case CallbackType.Acc:

                            RegisterFailedEvent(this, new RegisterFailedEventArgs() { Exception = new Exception(text) });
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        private void OnLogin(MapField<string, ByteString> paramaters)
        {
            if (paramaters == null)
            {
                return;
            }
            if (paramaters.ContainsKey("user"))
            {
                string userId = paramaters["user"].ToString(Encoding.ASCII);
                SetAccountEvent?.Invoke(this, new SetAccountEventArgs() { UserId = userId });
            }

            Topic me = new Topic("me");

            ClientPost(GetTags(me));
            ClientPost(GetMeSubs());

            //保存令牌供上传使用
            token = JsonConvert.DeserializeObject<string>(paramaters["token"].ToString(Encoding.UTF8));

        }

        private void OnGetMeMeta(ServerMeta meta)
        {
            if (meta.Sub != null && meta.Sub.Count != 0)
            {
                foreach (var sub in meta.Sub)
                {
                    Topic topic = new Topic(sub.Topic);
                    Subscriber subscriber = new Subscriber();

                    var publicInfo = sub.Public.ToStringUtf8();
                    var publicObject = JsonConvert.DeserializeObject<JObject>(publicInfo);
                    var privateInfo = sub.Private.ToStringUtf8();
                    var privateObject = JsonConvert.DeserializeObject<JObject>(privateInfo);

                    topic.Updated = sub.UpdatedAt;
                    topic.Read = sub.ReadId;
                    topic.Recieve = sub.RecvId;
                    topic.Clear = sub.DelId;
                    topic.LastUsed = sub.TouchedAt;
                    topic.MinLocalSeqId = sub.RecvId;
                    topic.MaxLocalSeqId = sub.RecvId;
                    topic.IsArchived = false;
                    topic.PrivateComment = sub.UserId;
                    topic.Weight = 0;
                    topic.Type = publicObject == null ? "group" : "user";

                    if (privateObject != null && privateObject.Count != 0 )
                    {
                        if (privateObject["arch"] != null)
                        {
                            topic.IsArchived = bool.Parse(privateObject["arch"].ToString());
                        }
                        if (privateObject["comment"] != null)
                        {
                            topic.PrivateComment = privateObject["comment"].ToString();
                        }
                        
                    }

                    subscriber.UserId = sub.Topic;
                    subscriber.Online = sub.Online;
                    subscriber.TopicName = sub.Topic;
                    subscriber.Username = sub.UserId;
                    subscriber.Type = publicObject == null ? "group" : "user";
                    subscriber.PhotoData = string.Empty;
                    subscriber.PhotoType = string.Empty;
                    subscriber.Status = 1;
                    if (publicObject != null)
                    {
                        subscriber.Username = publicObject["fn"].ToString();
                        if (publicObject.ContainsKey("photo"))
                        {
                            subscriber.PhotoData = publicObject["photo"]["data"].ToString();
                            subscriber.PhotoType = publicObject["photo"]["type"].ToString();
                        }
                    }

                    topic.SubsriberList.Add(subscriber);

                    AddTopicEvent?.Invoke(this, new AddTopicEventArgs() { Topic = topic });

                    AddSubscriberEvent?.Invoke(this, new AddSubscriberEventArgs() { Subscriber = subscriber,isTemporary=false });

                }
            }

            if (meta.Tags != null && meta.Tags.Count!=0)
            {
                List<string> tags = meta.Tags.ToList();
                SetAccountEvent(this, new SetAccountEventArgs() { Tags = tags });
            }
        }

        private void OnGetFindMeta(ServerMeta meta)
        {
            if (meta.Sub != null && meta.Sub.Count != 0)
            {
                foreach (var sub in meta.Sub)
                {
                    Subscriber subscriber = new Subscriber();

                    var publicInfo = sub.Public.ToStringUtf8();
                    var publicObject = JsonConvert.DeserializeObject<JObject>(publicInfo);

                    subscriber.UserId = sub.UserId;
                    subscriber.TopicName = sub.UserId;
                    subscriber.Username = sub.UserId;
                    subscriber.Type = publicObject == null ? "group" : "user";
                    subscriber.PhotoData = string.Empty;
                    subscriber.PhotoType = string.Empty;

                    if (publicObject != null)
                    {
                        subscriber.Username = publicObject["fn"].ToString();
                        if (publicObject.ContainsKey("photo"))
                        {
                            subscriber.PhotoData = publicObject["photo"]["data"].ToString();
                            subscriber.PhotoType = publicObject["photo"]["type"].ToString();
                        }
                    }

                    AddSubscriberEvent?.Invoke(this, new AddSubscriberEventArgs() { Subscriber = subscriber,isTemporary=true });
                }
            }
        }

    }
}
