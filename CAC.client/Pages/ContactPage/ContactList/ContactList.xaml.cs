
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
        }

        private void contactListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
