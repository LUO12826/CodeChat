
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CAC.client.MessagePage
{

    sealed partial class MessagePage : Page
    {
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

        private void BtnSearchUserOnline_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }

}
