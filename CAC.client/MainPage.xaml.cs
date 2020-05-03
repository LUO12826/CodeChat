using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml.Media.Animation;

namespace CAC.client
{

    sealed partial class MainPage : Page
    {

        private string avatar = "/Assets/640.jpeg";
        private string currentPage;

        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            naviFrame.ContentTransitions = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            naviFrame.Navigate(typeof(MessagePage.MessagePage), 0);
            currentPage = "chat";
        }

        private void Navigator_OnNavigationChanged(object sender, object e)
        {
            string destination = (string)e;
            if (destination == currentPage)
                return;

            currentPage = destination;
            Type type = typeof(MessagePage.MessagePage);
            switch (destination) {
                case "avatar":
                    break;
                case "chat":
                    type = typeof(MessagePage.MessagePage);
                    break;
                case "contact":
                    type = typeof(ContactPage.ContactPage);
                    break;
                case "settings":
                    type = typeof(SettingsPage.SettingsPage);
                    break;
                default:
                    type = typeof(MessagePage.MessagePage);
                    break;
            }
            if(type != null) {
                naviFrame.Navigate((type), 0);
            }
        }
    }
}
