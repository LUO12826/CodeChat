using CAC.client.Common;
using CAC.client;
using System.Collections.ObjectModel;
using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.VoiceCommands;
using Microsoft.Toolkit.Uwp.Helpers;

namespace CAC.client.MessagePage
{
    class ChatListViewModel : BaseViewModel
    {
        public ObservableCollection<IChatListItem> Items = new ObservableCollection<IChatListItem>();

        public ChatListViewModel()
        {
        }

        public void DidSelectChat(IChatListItem chatListItem)
        {
            if(chatListItem is ChatListChatItemVM chatItem) {
                Messenger.Default.Send(chatItem, "RequestOpenChatToken");
            }
            
        }

        public void RequestRemoveChat(IChatListItem chatItem)
        {

        }

        public void RequestPinToTop(IChatListItem chatItem)
        {

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
