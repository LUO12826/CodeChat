using CAC.client.Common;

namespace CAC.client.ContactPage
{
    /// <summary>
    /// 联系人列表中每一项的基类
    /// </summary>
    class ContactBaseViewModel : BaseViewModel
    {

    }

    /// <summary>
    /// 联系人列表中“联系人”项。与之同等级的还有联系人列表中的“群组”项。
    /// </summary>
    class ContactItemViewModel : ContactBaseViewModel
    {
        private string _UserID;
        private string _UserName;
        private string _Note;
        private string _Base64Avatar;
        private string _Email;
        private string _Address;
        private bool _IsOnline;

        public string UserID {
            get => _UserID;
            set {
                _UserID = value;
                RaisePropertyChanged(nameof(UserID));
            }
        }

        public string UserName {
            get => _UserName;
            set {
                _UserName = value;
                RaisePropertyChanged(nameof(UserName));
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        public string Note {
            get => _Note;
            set {
                _Note = value;
                RaisePropertyChanged(nameof(Note));
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        //显示的优先级是：备注、用户名、用户ID。
        public string DisplayName {
            get {
                string user = UserName.IsNullOrEmpty() ? UserID : UserName;
                return Note.IsNullOrEmpty() ? user : Note;
            }
        }

        public string Base64Avatar {
            get => _Base64Avatar;
            set {
                _Base64Avatar = value;
                RaisePropertyChanged(nameof(Base64Avatar));
            }
        }
        public string Email {
            get => _Email;
            set {
                _Email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        public string Address {
            get => _Address;
            set {
                _Address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }

        public bool IsOnline {
            get => _IsOnline;
            set {
                _IsOnline = value;
                RaisePropertyChanged(nameof(IsOnline));
            }
        }
    }
}
