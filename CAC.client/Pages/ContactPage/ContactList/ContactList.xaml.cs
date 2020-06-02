
using Windows.UI.Xaml.Controls;


namespace CAC.client.ContactPage
{
    sealed partial class ContactList : UserControl
    {
        public ContactListViewModel VM;

        public ContactList()
        {
            this.InitializeComponent();
            VM = new ContactListViewModel();

            contactListView.SelectedItem = null;
        }

        private void contactListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VM.DidSelectContact(e.ClickedItem as ContactBaseViewModel);
        }
    }
}
