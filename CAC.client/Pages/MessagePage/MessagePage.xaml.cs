
using CodeChatSDK.Controllers;
using CodeChatSDK.Models;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CAC.client.MessagePage
{

    sealed partial class MessagePage : Page
    {
        private bool isSearching = false;
        private List<string> searchResultUserIDList;

        public MessagePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            chatPanelFrame.Content = new ChatPanel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MessagePageViewModel.OnNavigateTo();
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

        private void searchUserBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var items = searchUserBox.ItemsSource as List<string>;
            if (items == null)
                return;

            var index = items.IndexOf(args.SelectedItem as string);
            if (index >= 0 && index < searchResultUserIDList.Count) {
                Messenger.Default.Send(searchResultUserIDList[index], "ProgrammlyOpenChatToken");
            }
        }
    }

}
