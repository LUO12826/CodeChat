using System;
using CAC.client.Common;
using System.Drawing;
using CAC.client.ContactPage;
using CodeChatSDK.Models;

namespace CAC.client.MessagePage
{
    /// <summary>
    /// 各种类型消息的ViewModel的基类
    /// </summary>
    class MessageItemBaseVM : BaseViewModel
    {
        private long _ID;
        private bool _SendByMe;
        private bool _SendFailed;
        private DateTime _TimeStamp;

        private Color _CellColor;
        private ContactItemViewModel _Contact;

        public ChatMessage RawMessage { get; set; }

        public ContactItemViewModel Contact {
            get => _Contact;
            set {
                _Contact = value;
                RaisePropertyChanged(nameof(Contact));
            }
        }

        public long ID {
            get => _ID;
            set {
                _ID = value;
                RaisePropertyChanged(nameof(ID));
            }
        }

        public bool SendFailed {
            get => _SendFailed;
            set {
                _SendFailed = value;
                RaisePropertyChanged(nameof(SendFailed));
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


        public Color CellColor {
            get => _CellColor;
            set {
                _CellColor = value;
                RaisePropertyChanged(nameof(CellColor));
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
                RaisePropertyChanged(nameof(Message));
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
        private int _DownloadState;
        private string _FileName;
        private string _FileUri;

        public string FileName {
            get => _FileName;
            set {
                _FileName = value;
                RaisePropertyChanged(nameof(FileName));
            }
        }

        public int DownloadState {
            get => _DownloadState;
            set {
                _DownloadState = value;
                RaisePropertyChanged(nameof(DownloadState));
            }
        }

        public string FileUri {
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
