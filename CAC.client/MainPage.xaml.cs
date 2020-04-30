
using Windows.ApplicationModel.Core;

using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;


namespace CAC.client
{

    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            naviFrame.Navigate(typeof(MessagePage.MessagePage), 0);
        }

    }
}
