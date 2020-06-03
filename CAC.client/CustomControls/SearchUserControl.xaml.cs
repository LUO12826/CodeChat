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

namespace CAC.client.CustomControls
{
    sealed partial class SearchUserControl : UserControl
    {
        public List<Subscriber> subscribers { get; set; } = new List<Subscriber>();

        public SearchUserControl()
        {
            this.InitializeComponent();
        }

        private async void BtnSearch_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string text = SearchTextBox.Text;
            if (text.IsNullOrEmpty())
                return;

            List<Subscriber> subscribers = await Task.Run(() => {
                return CommunicationCore.accountController.SearchSubscriberOnline(text);
            });

            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                this.subscribers.Clear();
                this.subscribers.AddRange(subscribers);
                Debug.WriteLine("获取到订阅者结果" + subscribers.Count);
            });
                
        }
    }
}
