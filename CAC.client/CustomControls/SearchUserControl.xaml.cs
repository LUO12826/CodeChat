using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CodeChatSDK.Models;
using CAC.client.Common;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CAC.client.CustomControls
{
    sealed partial class SearchUserControl : UserControl, INotifyPropertyChanged
    {
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

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
