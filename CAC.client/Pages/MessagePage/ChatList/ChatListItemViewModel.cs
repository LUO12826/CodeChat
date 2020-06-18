using System;
using Windows.UI.Xaml.Controls;
using CAC.client.Common;
using CAC.client.ContactPage;
using Windows.Storage.AccessCache;
using CodeChatSDK.Models;

namespace CAC.client.MessagePage
{
    /// <summary>
    /// 考虑到聊天列表中还可能有其它类型的消息（好友通知消息等，虽然不一定做），这里做一个空接口统一起来。
    /// </summary>
    interface IChatListItem
    {

    }

    /// <summary>
    /// 聊天列表中，每一个消息项的基础模型。
    /// </summary>
    class ChatListBaseItemVM : BaseViewModel, IChatListItem
    {
        private string _TopicName;
        private int _UnreadCount;
        private bool _CanPinToTop;
        private string _LatestMessage;
        private DateTime _LastActiveTime;

        public long MaxMsgSeq { get; set; } = 0;
        public Topic RawTopic { get; set; }

        public string TopicName {
            get => _TopicName;
            set {
                _TopicName = value;
                RaisePropertyChanged(nameof(TopicName));
            }
        }


        public string LatestMessage {
            get => _LatestMessage;
            set {
                _LatestMessage = value;
                RaisePropertyChanged(nameof(LatestMessage));
            }
        }

        public bool CanPinToTop {
            get => _CanPinToTop;
            set {
                _CanPinToTop = value;
                RaisePropertyChanged(nameof(CanPinToTop));
            }
        }

        //此聊天上次活跃的时间。活跃指的是，
        //对方发来消息或己方发出消息。
        public DateTime LastActiveTime {
            get => _LastActiveTime;
            set {
                _LastActiveTime = value;
                RaisePropertyChanged(nameof(LastActiveTime));
            }
        }

        //未读消息计数
        public int UnreadCount {
            get => _UnreadCount;
            set {
                _UnreadCount = value;
                RaisePropertyChanged(nameof(UnreadCount));
            }
        }

    }

    /// <summary>
    /// 聊天列表中，每一个聊天项的模型。
    /// </summary>
    class ChatListChatItemVM : ChatListBaseItemVM
    {
        #region private members

        private string _Draft;
        private bool _PinToTop;
        private bool _Sendfailed;
        private ChatType _ChatType;
        private NotificationType _NotificationType;
        private ContactItemViewModel _Contact;

        #endregion

        #region properties

        public ContactItemViewModel Contact {
            get => _Contact;
            set {
                _Contact = value;
                RaisePropertyChanged(nameof(Contact));
            }
        }


        //草稿（未发送）的消息
        public string Draft {
            get => _Draft;
            set {
                _Draft = value;
                RaisePropertyChanged(nameof(Draft));
            }
        }

        //该聊天是否置顶
        public bool PinToTop {
            get => _PinToTop;
            set {
                _PinToTop = value;
                RaisePropertyChanged(nameof(PinToTop));
            }
        }

        //是否有发送失败的消息
        public bool Sendfailed {
            get => _Sendfailed;
            set {
                _Sendfailed = value;
                RaisePropertyChanged(nameof(Sendfailed));
            }
        }


        public ChatType ChatType {
            get => _ChatType;
            set {
                _ChatType = value;
                RaisePropertyChanged(nameof(ChatType));
            }
        }

        public NotificationType NotificationType {
            get => _NotificationType;
            set {
                _NotificationType = value;
                RaisePropertyChanged(nameof(NotificationType));
            }
        }
        #endregion
    }
}
