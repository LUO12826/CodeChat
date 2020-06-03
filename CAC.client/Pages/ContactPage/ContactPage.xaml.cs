using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CAC.client.ContactPage
{

    sealed partial class ContactPage : Page
    {
        public ContactListViewModel ListVM => contactList.VM;

        public ContactPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ListVM.ReloadContact();
        }

    }
}
