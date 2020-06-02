
using Windows.UI.Xaml.Controls;


namespace CAC.client.ContactPage
{
    sealed partial class ContactList : UserControl
    {
        public ContactListViewModel VM = new ContactListViewModel();

        public ContactList()
        {
            this.InitializeComponent();
        }

        private void contactListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.DidSelectContact(e.ClickedItem as ContactBaseViewModel);
        }
    }
}
