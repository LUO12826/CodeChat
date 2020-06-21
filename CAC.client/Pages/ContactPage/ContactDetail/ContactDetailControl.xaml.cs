using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;


namespace CAC.client.ContactPage
{
    sealed partial class ContactDetailControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ContactItemViewModel> DidDeleteContact;

        //是否是个人型联系人
        private bool _IsPerson = false;
        public bool IsPerson {
            get => _IsPerson;
            set {
                _IsPerson = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPerson)));
            }
        }

        private ContactItemViewModel _Contact;
        public ContactItemViewModel Contact {
            get => _Contact;
            set {
                _Contact = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Contact)));
            }
        }

        public static readonly DependencyProperty ContactItemProperty =
            DependencyProperty.Register("ContactItem", typeof(ContactBaseViewModel), 
                typeof(ContactDetailControl), new PropertyMetadata(null, ContactItemChanged));

        public ContactBaseViewModel ContactItem {
            get { return (ContactBaseViewModel)GetValue(ContactItemProperty); }
            set { SetValue(ContactItemProperty, value); }
        }

        private static void ContactItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactDetailControl cd) {
                var newContact = e.NewValue as ContactBaseViewModel;
                if(newContact is ContactItemViewModel contact) {
                    cd.IsPerson = true;
                    cd.Contact = contact;
                }
                else {
                    cd.IsPerson = false;
                    cd.Contact = null;
                }
            }
        }

        public ContactDetailControl()
        {
            this.InitializeComponent();
            Messenger.Default.Register<ContactBaseViewModel>(this, "RequestOpenContactToken", RequestOpenContact);
            
        }

        private void RequestOpenContact(ContactBaseViewModel obj)
        {
            this.ContactItem = obj;
        }

        //处理删除联系人事件
        private async void btnDeleteContact_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var msgDialog = new Windows.UI.Popups.MessageDialog("确定要删除联系人" + Contact.DisplayName + "吗?") 
            { Title = "删除联系人" };
            
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", (a) => {
                deleteContact();
            }));

            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消"));
            await msgDialog.ShowAsync();
        }

        private async void deleteContact()
        {
            var subscriber = CommunicationCore.accountController.GetSubscriberByUserId(Contact.UserID);
            bool result = await CommunicationCore.accountController.RemoveSubscriber(subscriber);
            if(result) {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    DidDeleteContact?.Invoke(this, Contact);
                    IsPerson = false;
                    Contact = null;
                });
                
            }
            else {
                GlobalRef.MainPageNotification.Show("删除联系人" + Contact.DisplayName + "失败，请稍后再尝试。", 2000);
            }
        }


        private void btnSendMessage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GlobalRef.Navigator.SelectItem(NaviItems.chat);
            Task.Run(async () => {
                await Task.Delay(500);
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    Messenger.Default.Send(Contact.UserID, "ProgrammlyOpenChatToken");
                });
            });
        }
    }
}
