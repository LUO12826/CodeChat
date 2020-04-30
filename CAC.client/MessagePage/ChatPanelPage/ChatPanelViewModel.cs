using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAC.client.MessagePage
{
    class ChatPanelViewModel : BaseViewModel
    {
        public event Action<string> SetInputBoxText;
        public event Func<string> GetInputBoxText;

        private ChatListBaseItemVM _ChatListItem;
        private MessageViewer _CurrentViewer;

        public MessageViewer CurrentViewer {
            get => _CurrentViewer;
            set {
                _CurrentViewer = value;
                RaisePropertyChanged(nameof(CurrentViewer));
            }
        }

        public ChatListBaseItemVM ChatListItem {
            get => _ChatListItem;
            set {
                _ChatListItem = value;
                RaisePropertyChanged(nameof(ChatListItem));
            }
        }

        private Dictionary<ChatListBaseItemVM, MessageViewer> messageViewerCache = new Dictionary<ChatListBaseItemVM, MessageViewer>();

        public ChatPanelViewModel()
        {

        }

        public void RequireOpenChat(ChatListBaseItemVM chatListItem)
        {
            ChatListItem = chatListItem;
            if (messageViewerCache.Keys.Contains(chatListItem)) {
                CurrentViewer = messageViewerCache[chatListItem];
            }
            else {
                var viewer = new MessageViewer();
                CurrentViewer = viewer;
                messageViewerCache.Add(chatListItem, viewer);
            }

            Debug.WriteLine("req");
        }

        public void RequireCloseChat(ChatListBaseItemVM chatListItem)
        {

        }
    }
}
