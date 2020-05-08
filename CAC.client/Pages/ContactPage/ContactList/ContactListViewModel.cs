using CAC.client.Common;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CAC.client.ContactPage
{
    class ContactListViewModel : BaseViewModel
    {
        public ObservableCollection<ContactGroupViewModel> Contacts;

        public ContactListViewModel()
        {
            Contacts = new ObservableCollection<ContactGroupViewModel>();
            Contacts.Add(new ContactGroupViewModel() {
                GroupName = "测试分组",
                IsExpanded = false,
                Contacts = new ObservableCollection<ContactBaseViewModel>() {
                    new ContactItemViewModel() {
                        UserID = "11111",
                        UserName = "昵称1",
                        Base64Avatar = CAC.client.GlobalConfigs.testB64Avator
                    },
                    new ContactItemViewModel() {
                        UserID = "22222",
                        UserName = "昵称2",
                        Base64Avatar = CAC.client.GlobalConfigs.testB64Avator
                    }
                }
            });
        }

        public void DidSelectContact(ContactBaseViewModel contactItem)
        {
            Messenger.Default.Send(contactItem, "RequireOpenContactToken");
        }
    }
}
