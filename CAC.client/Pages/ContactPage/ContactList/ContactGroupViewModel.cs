using CAC.client.Common;
using System.Collections.ObjectModel;

namespace CAC.client.ContactPage
{
    /// <summary>
    /// 每个联系人分组的视图模型
    /// </summary>
    class ContactGroupViewModel : BaseViewModel
    {
        private string _GroupName;
        private bool _IsExpanded;
        private ObservableCollection<ContactBaseViewModel> _Contacts;

        public string GroupName {
            get => _GroupName;
            set {
                _GroupName = value;
                RaisePropertyChanged(nameof(GroupName));
            }
        }

        public bool IsExpanded {
            get => _IsExpanded;
            set {
                _IsExpanded = value;
                RaisePropertyChanged(nameof(IsExpanded));
            }
        }

        public ObservableCollection<ContactBaseViewModel> Contacts {
            get => _Contacts;
            set {
                _Contacts = value;
                RaisePropertyChanged(nameof(Contacts));
            }
        }
    }
}
