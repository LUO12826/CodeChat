using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CustomControls;
using CAC.client.CodeEditorPage;

namespace CAC.client.MessagePage
{
    class ChatPanelViewModel : BaseViewModel
    {
        public event Action<string> SetInputBoxText;
        public event Func<string> GetInputBoxText;

        private ChatListChatItemVM _ChatListItem;
        private MessageViewer _CurrentViewer;

        //对messageViewer的缓存。我们希望切换回某个会话时保留上次的浏览位置，因此设立一个缓存。
        //其实直接缓存viewModel是更好的方案，但没有找到解决恢复浏览位置的好方法。
        private Dictionary<ChatListBaseItemVM, MessageViewer> messageViewerCache =
            new Dictionary<ChatListBaseItemVM, MessageViewer>();

        public ChatListChatItemVM ChatListItem {
            get => _ChatListItem;
            set {
                _ChatListItem = value;
                RaisePropertyChanged(nameof(ChatListItem));
            }
        }

        public MessageViewer CurrentViewer {
            get => _CurrentViewer;
            set {
                _CurrentViewer = value;
                RaisePropertyChanged(nameof(CurrentViewer));
            }
        }


        public ChatPanelViewModel()
        {
            Messenger.Default.Register<ChatListChatItemVM>(this, "RequestOpenChatToken", RequestOpenChat);
        }

        //当缓存中有时，直接从缓存中取，否则新建
        public void RequestOpenChat(ChatListChatItemVM chatListItem)
        {
            ChatListItem = chatListItem;
            if (messageViewerCache.Keys.Contains(chatListItem)) {
                CurrentViewer = messageViewerCache[chatListItem];
            }
            else {
                var viewerVM = new MessageViewer();
                CurrentViewer = viewerVM;
                messageViewerCache.Add(chatListItem, viewerVM);
            }

        }

        public void RequestCloseChat(ChatListChatItemVM chatListItem)
        {

        }

        public void DidSendContent(SentContentEventArgs e)
        {
            if(e.Type == MessageType.image) {
                CurrentViewer.VM.Messages.Add(new ImageMessageVM() {
                    SendByMe = true,
                    ImageUri = e.Content
                });
            }
        }
    }
}
