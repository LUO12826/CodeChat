using CAC.client.Common;
using CAC.client;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using CodeChatSDK.EventHandler;
using System.Linq;
using CodeChatSDK.Models;
using System.Threading.Tasks;

namespace CAC.client.MessagePage
{
    class ChatListViewModel : BaseViewModel
    {
        public ObservableCollection<IChatListItem> Items = new ObservableCollection<IChatListItem>();

        private IChatListItem selectedChat = null;
        public IChatListItem SelectedChat {
            get => selectedChat;
            set {
                selectedChat = value;
                RaisePropertyChanged(nameof(SelectedChat));
            }
        }

        public ChatListViewModel()
        {
            CommunicationCore.client.AddMessageEvent += Client_AddMessageEvent;
            CommunicationCore.client.RemoveTopicEvent += Client_RemoveTopicEvent;
            Messenger.Default.Register<string>(this, "ProgrammlyOpenChatToken", ProgrammlyOpenChat);
            Messenger.Default.Register<string>(this, "DeleteContactToken", didDeleteContact);
        }

        private void didDeleteContact(string userID)
        {
            var chatItem = Items.Where(x => (x as ChatListChatItemVM).TopicName == userID).FirstOrDefault();
            if (chatItem == null) return;

            RequestRemoveChat(chatItem);

        }

        /// <summary>
        /// 程序内部调用的方式打开聊天会话
        /// </summary>
        private void ProgrammlyOpenChat(string topicName)
        {
            var chatItem = Items.Where(x => (x as ChatListChatItemVM).TopicName == topicName).FirstOrDefault();
            if (chatItem == null) {
                var topic = CommunicationCore.accountController.GetTopicByName(topicName);
                var newChatItem = ModelConverter.TopicToChatListItem(topic);
                Items.Insert(0, newChatItem);
                DidSelectChat(newChatItem);
                return;
            }

            DidSelectChat(chatItem);
        }

        private void Client_RemoveTopicEvent(object sender, RemoveTopicEventArgs args)
        {
            var item = findChatlistItemByTopicName(args.Topic.Name);
            if (item != null) {
                DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    Items.Remove(item);
                });
            }
        }

        private void Client_AddTopicEvent(object sender, AddTopicEventArgs args)
        {
            var chatlistItem = ModelConverter.TopicToChatListItem(args.Topic);
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                Items.Insert(0, chatlistItem);
            });
        }

        private void Client_AddMessageEvent(object sender, AddMessageEventArgs args)
        {
            var msg = ModelConverter.MessageToMessageVM(args.Message);
            var chat = findChatlistItemByTopicName(args.TopicName);

            DispatcherHelper.ExecuteOnUIThreadAsync(() => {

                if (chat != null && args.Message.SeqId > chat.MaxMsgSeq) {
                    
                    chat.LatestMessage = GlobalFunctions.MessageToLatestString(msg);
                    chat.MaxMsgSeq = args.Message.SeqId;

                    if (chat != selectedChat && !msg.SendByMe) {

                        if(Items.IndexOf(chat) != 0) {
                            Items.Remove(chat);
                            Items.Insert(0, chat);
                        }
                        chat.LastActiveTime = DateTime.Now;
                        chat.UnreadCount++;
                    }
                }

            });

        }

        /// <summary>
        /// 用户手动选择一个聊天会话时，将其打开。
        /// </summary>
        public void DidSelectChat(IChatListItem chatListItem)
        {
            if(chatListItem is ChatListChatItemVM chatItem) {
                chatItem.UnreadCount = 0;
                SelectedChat = chatItem;
                Task.Run(async () => {
                    await Task.Delay(20);
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                        Messenger.Default.Send(chatItem, "RequestOpenChatToken");
                    });
                });        
            }
        }


        public void RequestRemoveChat(IChatListItem chatItem)
        {
            //bool result = await CommunicationCore.accountController.RemoveTopic((chatItem as ChatListChatItemVM).RawTopic);
            if(true) {
                if (Items.Contains(chatItem)) {
                    SelectedChat = null;
                    Items.Remove(chatItem);
                    Messenger.Default.Send(chatItem as ChatListChatItemVM, "RequestCloseChatToken");
                }
            }
            else {

            }
        }

        public void RequestPinToTop(IChatListItem chatItem)
        {

        }

        private ChatListBaseItemVM findChatlistItemByTopicName(string topic)
        {
            var res = Items.Where(x => (x as ChatListBaseItemVM).RawTopic.Name == topic).FirstOrDefault();
            return res == null ? null : res as ChatListBaseItemVM;
        }

        public void ReloadChatList()
        {
            Items.Clear();
            var topics = CommunicationCore.account.TopicList;

            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                foreach (var topic in topics) {
                    if (topic.IsArchived || !topic.IsVisible)
                        continue;

                    var item = ModelConverter.TopicToChatListItem(topic);
                    Items.Add(item);
                }
            });

            CommunicationCore.client.AddTopicEvent += Client_AddTopicEvent;
        }
    }
}
