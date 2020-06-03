using CAC.client.Common;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeChatSDK.Models;
using Windows.Graphics.Printing.Workflow;

using System.Diagnostics;

namespace CAC.client.ContactPage
{
    class ContactListViewModel : BaseViewModel
    {
        public ObservableCollection<ContactGroupViewModel> ContactGroup;

        public ObservableCollection<ContactBaseViewModel> AllContact = new ObservableCollection<ContactBaseViewModel>();

        public ContactListViewModel()
        {
            ContactGroup = new ObservableCollection<ContactGroupViewModel>();
            ContactGroup.Add(new ContactGroupViewModel() {
                GroupName = "所有联系人",
                IsExpanded = false,
                Contacts = AllContact
            });
        }

        public void DidSelectContact(ContactBaseViewModel contactItem)
        {
            Messenger.Default.Send(contactItem, "RequestOpenContactToken");
        }

        public void ReloadContact()
        {
            AllContact.Clear();
            var subscribers = CommunicationCore.account.SubscriberList;

            foreach(var sub in subscribers) {
                var contact = ModelConverter.SubscriberToContact(sub);
                AllContact.Add(contact);
            }
        }
    }
}
