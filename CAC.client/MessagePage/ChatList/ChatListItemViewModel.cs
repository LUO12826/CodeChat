using System;
using Windows.UI.Xaml.Controls;
using CAC.client.Common;

namespace CAC.client.MessagePage
{
    interface IChatListItem
    {

    }
    /// <summary>
    /// 聊天列表中，每一个消息项的基础模型。
    /// </summary>
    class ChatListBaseItemVM : BaseViewModel, IChatListItem
    {
        private string _ChatName;
        private Uri _AvatarPath;
        private string _Base64Avatar;
        private int _UnreadCount;
        private bool _CanPinToTop;
        private string _LatestMessage;
        private DateTime _LastActiveTime;

        public string ChatName {
            get => _ChatName;
            set {
                _ChatName = value;
                RaisePropertyChanged(nameof(ChatName));
            }
        }

        public Uri AvatarPath {
            get => _AvatarPath;
            set {
                _AvatarPath = value;
                RaisePropertyChanged(nameof(AvatarPath));
            }
        }

        public string Base64Avatar {
            get => _Base64Avatar;
            set {
                _Base64Avatar = value;
                RaisePropertyChanged(nameof(Base64Avatar));
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
        private string _UserID;

        private string _Draft;
        private bool _PinToTop;
        private bool _Sendfailed;
        private ChatType _ChatType;
        private NotificationType _NotificationType;

        #endregion

        #region properties

        public string UserID {
            get => _UserID;
            set {
                _UserID = value;
                RaisePropertyChanged(nameof(UserID));
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
