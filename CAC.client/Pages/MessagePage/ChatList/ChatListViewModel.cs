using CAC.client.Common;
using CAC.client;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using CodeChatSDK.EventHandler;
using System.Linq;


namespace CAC.client.MessagePage
{
    class ChatListViewModel : BaseViewModel
    {
        public ObservableCollection<IChatListItem> Items = new ObservableCollection<IChatListItem>();

        private IChatListItem selectedChat;

        public ChatListViewModel()
        {
            CommunicationCore.client.AddMessageEvent += Client_AddMessageEvent;
            CommunicationCore.client.RemoveTopicEvent += Client_RemoveTopicEvent;
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
            var res = Items.Where(x => (x as ChatListBaseItemVM).RawTopic.Name == topic).FirstOrDefault();
            return res == null ? null : res as ChatListBaseItemVM;
        }

        public void ReloadChatList()
        {
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

            CommunicationCore.client.AddTopicEvent += Client_AddTopicEvent;
        }
    }
}
