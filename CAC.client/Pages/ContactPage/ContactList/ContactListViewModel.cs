using CAC.client.Common;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeChatSDK.Models;
using Windows.Graphics.Printing.Workflow;

using System.Diagnostics;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Threading.Tasks;

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
            CommunicationCore.client.AddSubscriberEvent += Client_AddSubscriberEvent;
            CommunicationCore.client.RemoveSubscriberEvent += Client_RemoveSubscriberEvent;
        }

        private void Client_RemoveSubscriberEvent(object sender, CodeChatSDK.EventHandler.RemoveSubscriberEventArgs args)
        {
            var contact = AllContact.Where(x => (x as ContactItemViewModel).UserID == args.Subscriber.UserId)
                                    .FirstOrDefault();

            if(contact != null) {
                DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    AllContact.Remove(contact);
                });
            }
        }

        private async void Client_AddSubscriberEvent(object sender, CodeChatSDK.EventHandler.AddSubscriberEventArgs args)
        {
            if (args.isTemporary == true)
                return;
            var contact = ModelConverter.SubscriberToContact(args.Subscriber);
            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                AllContact.Add(contact);
            });

            await Task.Delay(2000);
            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                ReloadContact();
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

        public void DeleteContact(ContactItemViewModel contact)
        {
            AllContact.Remove(contact);
            Messenger.Default.Send(contact.UserID, "DeleteContactToken");
        }
    }
}
