﻿using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using CAC.client.ContactPage;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CodeEditorPage;
using CodeChatSDK.Models;
using CodeChatSDK.Controllers;
using CodeChatSDK.EventHandler;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.WindowManagement;
using System.Linq;

namespace CAC.client.MessagePage
{
    class MessageViewerViewModel : BaseViewModel
    {
        private bool _IsGroupChat = false;
        private ContactItemViewModel _MyContactInfo;

        //指示此消息查看器查看的是哪个topic的消息。
        private string topicName;

        private ChatListChatItemVM chatItem;

        public Topic topic;

        public TopicController topicController;

        //测试
        private Brush _LeftBubbleBgColor = GlobalConfigs.LeftBubbleBgColor;
        private Brush _RightBubbleBgColor = GlobalConfigs.RightBubbleBgColor;

        public bool RequireCache { get; set; } = false;

        //记录scrollViewer的垂直滚动位置，现在用不到了。
        //public double VerticalScrollOffset { get; set; } = -1.0;

        public DateTime LastOpenTime { get; set; }

        public ObservableCollection<MessageItemBaseVM> Messages;

        public bool IsGroupChat {
            get => _IsGroupChat;
            set {
                _IsGroupChat = value;
                RaisePropertyChanged(nameof(IsGroupChat));
            }
        }
        public Brush RightBubbleBgColor {
            get => _RightBubbleBgColor;
            set {
                _RightBubbleBgColor = value;
                RaisePropertyChanged(nameof(RightBubbleBgColor));
            }
        }


        public Brush LeftBubbleBgColor {
            get => _LeftBubbleBgColor;
            set {
                _LeftBubbleBgColor = value;
                RaisePropertyChanged(nameof(LeftBubbleBgColor));
            }
        }

        public ContactItemViewModel MyContactInfo {
            get => _MyContactInfo;
            set {
                _MyContactInfo = value;
                RaisePropertyChanged(nameof(MyContactInfo));
            }
        }


        public MessageViewerViewModel()
        {

            Messages = new ObservableCollection<MessageItemBaseVM>();

            MyContactInfo = new ContactItemViewModel() {
                UserName = "self",
                Base64Avatar = GlobalConfigs.testB64Avator1
            };
            
        }

        public MessageViewerViewModel(ChatListChatItemVM chatItem)
        {
            Messages = new ObservableCollection<MessageItemBaseVM>();
            this.chatItem = chatItem;
            this.topicName = chatItem.TopicName;
            this.topic = chatItem.RawTopic;

            MyContactInfo = new ContactItemViewModel() {
                UserName = CommunicationCore.account.Username,
                Base64Avatar = CommunicationCore.account.Avatar.IsNullOrEmpty() ? GlobalConfigs.defaultAvatar : CommunicationCore.account.Avatar,
                UserID = CommunicationCore.account.UserId,
                IsOnline = true,
            };

            CommunicationCore.client.AddMessageEvent += Client_AddMessageEvent;
            initViewer(topicName);
        }

        private async void initViewer(string topicName)
        {
            await LoadHistoryMessages();
        }


        private void Client_AddMessageEvent(object sender, AddMessageEventArgs args)
        {
            Debug.WriteLine("Client_AddMessageEvent");
            Debug.WriteLine("消息的topic是" + args.TopicName);
            Debug.WriteLine("当前的topic是" + this.topicName);
            if (args.TopicName != this.topicName)
                return;

            var msg = ModelConverter.MessageToMessageVM(args.Message);
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {

                var oldestSeq = Messages.Count > 0 ? Messages[0].RawMessage.SeqId : -1;

                if (Messages.Count <= 0) {
                    Messages.Add(msg);
                    return;
                }
                if(msg.RawMessage.SeqId <= oldestSeq) {
                    Messages.Insert(0, msg);
                }
                else {
                    Messages.Add(msg);
                }
                Debug.WriteLine(Messages.Count + "========================");
            });
        }

        public async Task LoadHistoryMessages()
        {
            if (topic == null)
                return;
            Messages.Clear();

            if(topicController == null) {
                topicController = await CommunicationCore.accountController.GetTopicController(topic);
            }

            topicController.LoadMessage();
            List<ChatMessage> ChatMessageList = topic.MessageList;
            Debug.WriteLine("直接从topiccontroller加载了" + ChatMessageList.Count + "条消息");
            if (ChatMessageList.Count == 0)
                return;
            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                foreach (var message in ChatMessageList) {
                    var msg = ModelConverter.MessageToMessageVM(message);
                    Messages.Add(msg);

                    if(msg is TextMessageVM tm) {
                        Debug.WriteLine(tm.Text);
                    }
                }
                chatItem.LatestMessage = GlobalFunctions.MessageToLatestString(Messages.Last());
            });
            
        }

        public void RequestEditCode(CodeMessageVM codeMessage)
        {

            var newSession = new CodeEditSessionInfo() {
                Contact = codeMessage.Contact,
                CreateTime = DateTime.Now,
                Language = codeMessage.Language,
                MessageID = codeMessage.ID,
                Code = codeMessage.Code,
            };
            Messenger.Default.Send(newSession, "RequestEditCodeToken");
        }

        //不使用
        //private async Task<Tuple<List<MessageItemBaseVM>, bool>> loadMoreItems(uint itemsNum)
        //{
        //    var a = await Task.Run(async () => {
        //        Thread.Sleep(2000);
        //        var b = new List<MessageItemBaseVM>();
        //        var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
        //        var bb = await exampleFile.GetFolderAsync("Code");
        //        var cc = await bb.GetFileAsync("example.java");
        //        var text = await FileIO.ReadTextAsync(cc);
        //        return b;
        //    });

        //    return new Tuple<List<MessageItemBaseVM>, bool>(new List<MessageItemBaseVM>(), false);
        //}


        ~MessageViewerViewModel()
        {
            CommunicationCore.client.AddMessageEvent -= Client_AddMessageEvent;
        }
    }
}
