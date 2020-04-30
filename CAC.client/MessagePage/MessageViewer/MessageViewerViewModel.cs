using CAC.client.Common;
using System;
using System.Collections.ObjectModel;

namespace CAC.client.MessagePage
{
    class MessageViewerViewModel : BaseViewModel
    {
        public ObservableCollection<TextMessageVM> Messages;
        public bool isGroupChat;
        public bool RequireCache;
        public DateTime LastOpenTime;

    }
}
