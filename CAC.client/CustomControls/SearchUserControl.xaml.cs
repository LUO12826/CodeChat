using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CodeChatSDK.Models;
using CAC.client.Common;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CAC.client.CustomControls
{
    sealed partial class SearchUserControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<Subscriber> Subscribers { get; set; } = new ObservableCollection<Subscriber>();
        private bool isSearching { get; set; } = false;

        public bool IsSearching {
            get => isSearching;
            set {
                isSearching = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSearching)));
            }
        }

        public SearchUserControl()
        {
            this.InitializeComponent();
        }


        private async void BtnSearch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string text = SearchTextBox.Text;
            if (text.IsNullOrEmpty() || IsSearching)
                return;

            IsSearching = true;
            CommunicationCore.accountController.SearchSubscriberOnline(text);
            await Task.Delay(5000);

            List<Subscriber> subscribers = await Task.Run(() => {
                return CommunicationCore.accountController.SearchSubscriberOnline(text);
            });

            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                IsSearching = false;
                Subscribers.Clear();
                foreach(var res in subscribers) {
                    Subscribers.Add(res);
                }
            });
                
        }

        private async void searchResultList_ItemClick(object sender, ItemClickEventArgs e)
        {

            var sub = e.ClickedItem as Subscriber;
            string hint = "是否需要添加联系人" + sub.Username + "?";
            var msgDialog = new Windows.UI.Popups.MessageDialog(hint) { Title = "添加联系人" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("是", (a) => {
                addSubscriberOnline(sub);
            }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("否"));
            await msgDialog.ShowAsync();
        }

        private async void addSubscriberOnline(Subscriber sub)
        {
            bool result = await CommunicationCore.accountController.AddSubscriber(sub);
            if(result) {
                GlobalRef.MainPageNotification.Show("添加联系人成功", 2000);
            }
            else {
                GlobalRef.MainPageNotification.Show("添加联系人失败，请稍后重试", 2000);
            }
        }
    }
}
