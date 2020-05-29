using CAC.client.Common;
using CAC.client;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.VoiceCommands;

namespace CAC.client.MessagePage
{
    class ChatListViewModel : BaseViewModel
    {
        public ObservableCollection<IChatListItem> Items = new ObservableCollection<IChatListItem>();

        public ChatListViewModel()
        {

            var temp = new ChatListChatItemVM() {
                ChatName = "静音聊天",
                LastActiveTime = DateTime.Now,
                LatestMessage = "最近活跃的消息aaaAAA",
                UnreadCount = 5,
                Base64Avatar = GlobalConfigs.testB64Avator,
                NotificationType = NotificationType.mute
            };

            Items.Add(temp);

            var temp2 = new ChatListChatItemVM() {
                ChatName = "没有未读",
                LastActiveTime = DateTime.Now,
                LatestMessage = "最近活跃的消息aaaAAA最近活跃的消息最近活跃的消息",
                UnreadCount = 0,
                Base64Avatar = GlobalConfigs.testB64Avator,
                NotificationType = NotificationType.normal
            };

            Items.Add(temp2);

            for (int i = 0; i < 20; i++) {
                var a = new ChatListChatItemVM() {
                    ChatName = "一个聊天",
                    LastActiveTime = DateTime.Now,
                    LatestMessage = "最近活跃的消息",
                    UnreadCount = 4,
                    Base64Avatar = GlobalConfigs.testB64Avator,
                    NotificationType = NotificationType.normal
                };
                Items.Add(a);
            }
        }

        public void DidSelectChat(IChatListItem chatListItem)
        {
            if(chatListItem is ChatListChatItemVM chatItem) {
                Messenger.Default.Send(chatItem, "RequireOpenChatToken");
            }
            
        }

        public void RequestRemoveChat(IChatListItem chatItem)
        {

        }
    }
}
