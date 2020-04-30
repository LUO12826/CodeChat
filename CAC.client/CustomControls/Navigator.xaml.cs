using System;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;


namespace CAC.client.CustomControls
{
    public sealed partial class Navigator : UserControl
    {
        public event EventHandler<object> OnNavigationChanged;

        public Navigator()
        {
            this.InitializeComponent();
            this.DataContext = this;
            OnNavigationChanged += (a, b) => { };
        }

        private void naviItemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (sender == naviItemList) {
                additionItemList.SelectedItem = null;
            }
            else if (sender == additionItemList) {
                naviItemList.SelectedItem = null;
            }
            else {

            }
            OnNavigationChanged(this, (e.ClickedItem as FontIcon).Tag);
        }
    }

}
