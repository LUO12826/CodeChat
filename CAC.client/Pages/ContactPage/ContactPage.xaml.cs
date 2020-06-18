using CodeChatSDK.Models;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CAC.client.ContactPage
{

    sealed partial class ContactPage : Page
    {
        public ContactListViewModel ListVM => contactList.VM;
        private bool isSearching = false;

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
            isSearching = false;
        }

        private void searchUserBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

    }
}
