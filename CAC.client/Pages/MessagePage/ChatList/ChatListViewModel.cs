using CAC.client.Common;
using CAC.client;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.VoiceCommands;
using Microsoft.Toolkit.Uwp.Helpers;
using CodeChatSDK.EventHandler;
using System.Linq;
using Windows.Devices.PointOfService;

namespace CAC.client.MessagePage
{
    class ChatListViewModel : BaseViewModel
    {
        public ObservableCollection<IChatListItem> Items = new ObservableCollection<IChatListItem>();

        private IChatListItem selectedChat;

        public ChatListViewModel()
        {
            CommunicationCore.client.AddMessageEvent += Client_AddMessageEvent;
            CommunicationCore.client.AddTopicEvent += Client_AddTopicEvent;
        }

        private void Client_AddTopicEvent(object sender, AddTopicEventArgs args)
        {
            //var chatlistItem = ModelConverter.TopicToChatListItem(args.Topic);
            //Items.Insert(0, chatlistItem);
        }

        private void Client_AddMessageEvent(object sender, AddMessageEventArgs args)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                var msg = ModelConverter.MessageToMessageVM(args.Message);
                var chat = findChatlistItemByTopicName(args.TopicName);

                if (chat != null && args.Message.SeqId > chat.MaxMsgSeq) {
                    string latestMsg = "";
                    if (msg is TextMessageVM t) {
                        latestMsg = t.Text;
                    }
                    else if (msg is CodeMessageVM c) {
                        latestMsg = "[代码]";
                    }
                    else if (msg is FileMessageVM f) {
                        latestMsg = f.FileName;
                    }
                    else if (msg is ImageMessageVM i) {
                        latestMsg = "[图片]";
                    }
                    chat.LatestMessage = latestMsg;
                    chat.MaxMsgSeq = args.Message.SeqId;
                }

                if (chat != null && chat != selectedChat && !msg.SendByMe) {

                    Items.Remove(chat);
                    Items.Insert(0, chat);
                    chat.LastActiveTime = DateTime.Now;
                    chat.UnreadCount++;
                }
            });

        }

        public void DidSelectChat(IChatListItem chatListItem)
        {
            if(chatListItem is ChatListChatItemVM chatItem) {
                selectedChat = chatItem;
                chatItem.UnreadCount = 0;
                Messenger.Default.Send(chatItem, "RequestOpenChatToken");
            }
            
        }

        public void RequestRemoveChat(IChatListItem chatItem)
        {

        }

        public void RequestPinToTop(IChatListItem chatItem)
        {

        }

        private ChatListBaseItemVM findChatlistItemByTopicName(string topic)
        {
            if(Items == null || Items.Count <= 0) {
                return null;
            }
            
            foreach(var item in Items) {
                if(item is ChatListBaseItemVM baseVM) {
                    if(baseVM.TopicName == topic) {
                        return baseVM;
                    }
                }
            }

            return null;
        }

        public void ReloadChatList()
        {
            Debug.WriteLine("--------ReloadChatList");
            Items.Clear();
            var topics = CommunicationCore.account.TopicList;

            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                foreach (var topic in topics) {
                    Debug.WriteLine(topic.Name);
                    if (topic.IsArchived || !topic.IsVisible)
                        continue;

                    var item = ModelConverter.TopicToChatListItem(topic);
                    Items.Add(item);
                }
            });
            
        }
    }
}
