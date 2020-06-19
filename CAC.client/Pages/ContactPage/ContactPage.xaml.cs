using CodeChatSDK.Models;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CAC.client.ContactPage
{

    sealed partial class ContactPage : Page
    {
        public ContactListViewModel ListVM => contactList.VM;
        private bool isSearching = false;
        private List<string> searchResultUserIDList;

        public ContactPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ListVM.ReloadContact();
        }

        private void BtnSearchUserOnline_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }

        private async void searchUserBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (isSearching && !(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput))
                return;
            isSearching = true;
            var subscriberController = CommunicationCore.accountController.GetSubscriberController();
            List<Subscriber> subscribers = await subscriberController.SearchSubscriber(sender.Text);
            searchUserBox.ItemsSource = subscribers.Select(x => x.Username).ToList();
            searchResultUserIDList = subscribers.Select(x => x.UserId).ToList();
            isSearching = false;
        }

        //点击搜索框的建议结果后，跳转至聊天页面并打开对应topic
        private void searchUserBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var items = searchUserBox.ItemsSource as List<string>;
            if (items == null)
                return;

            var index = items.IndexOf(args.SelectedItem as string);
            if(index >= 0 && index < searchResultUserIDList.Count) {
                GlobalRef.Navigator.SelectItem(NaviItems.chat);
                Task.Run(async () => {
                    await Task.Delay(500);
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                        Messenger.Default.Send(searchResultUserIDList[index], "ProgrammlyOpenChatToken");
                    });
                });
                
            }
            
        }

        private void contactDetail_DidDeleteContact(object sender, ContactItemViewModel e)
        {
            ListVM.DeleteContact(e);
        }
    }
}
