using System;
using CAC.client.Common;
using System.Drawing;

namespace CAC.client.MessagePage
{
    /// <summary>
    /// 各种类型消息的ViewModel的基类
    /// </summary>
    class MessageItemBaseVM : BaseViewModel
    {
        private string _UserID;
        private string _NickName;
        private bool _SendByMe;
        private DateTime _TimeStamp;
        private string _Base64Avatar;
        private Color _CellColor;

        public string UserID {
            get => _UserID;
            set {
                _UserID = value;
                RaisePropertyChanged(nameof(UserID));
            }
        }

        public string NickName {
            get => _NickName;
            set {
                _NickName = value;
                RaisePropertyChanged(nameof(UserID));
            }
        }

        public bool SendByMe {
            get => _SendByMe;
            set {
                _SendByMe = value;
                RaisePropertyChanged(nameof(SendByMe));
            }
        }

        public DateTime TimeStamp {
            get => _TimeStamp;
            set {
                _TimeStamp = value;
                RaisePropertyChanged(nameof(TimeStamp));
            }
        }

        public string Base64Avatar {
            get => _Base64Avatar;
            set {
                _Base64Avatar = value;
                RaisePropertyChanged(nameof(Base64Avatar));
            }
        }

        public Color CellColor {
            get => _CellColor;
            set {
                _CellColor = value;
                RaisePropertyChanged(nameof(CellColor));
            }
        }

        public void didChangeAvatar(string userID, string base64Avatar)
        {
            if (userID == this.UserID) {
                this.Base64Avatar = base64Avatar;
            }
        }
    }

    class NotificationMessageVM : MessageItemBaseVM
    {
        private string _Message;
        
        public string Message {
            get => _Message;
            set {
                _Message = value;
                RaisePropertyChanged("Message");
            }
        }
    }

    class TextMessageVM : MessageItemBaseVM
    {
        private string _Text;

        public string Text {
            get => _Text;
            set {
                _Text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }
    }

    class ImageMessageVM : MessageItemBaseVM
    {
        private string _ImageUri;
        private string _ImageBase64;

        public string ImageUri {
            get => _ImageUri;
            set {
                _ImageUri = value;
                RaisePropertyChanged(nameof(ImageUri));
            }
        }

        public string ImageBase64 {
            get => _ImageBase64;
            set {
                _ImageBase64 = value;
                RaisePropertyChanged(nameof(ImageBase64));
            }
        }
    }

    class FileMessageVM : MessageItemBaseVM
    {
        private Uri _FileUri;

        public Uri FileUri {
            get => _FileUri;
            set {
                _FileUri = value;
                RaisePropertyChanged(nameof(FileUri));
            }
        }
    }

    class CodeMessageVM : MessageItemBaseVM
    {
        private string _Language;
        private string _Code;
        private string _RunResult;

        public string Language {
            get => _Language;
            set {
                _Language = value;
                RaisePropertyChanged(nameof(Language));
            }
        }

        public string Code {
            get => _Code;
            set {
                _Code = value;
                RaisePropertyChanged(nameof(Code));
            }
        }

        public string RunResult {
            get => _RunResult;
            set {
                _RunResult = value;
                RaisePropertyChanged(nameof(RunResult));
            }
        }
    }
}
