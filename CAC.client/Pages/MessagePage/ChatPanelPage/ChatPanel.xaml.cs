using CAC.client.CustomControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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


namespace CAC.client.MessagePage
{
    sealed partial class ChatPanel : Page
    {
        private ChatPanelViewModel VM;

        public ChatPanel()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            VM = MessagePageViewModel.ChatPanelViewModel;
        }

        private void inputBox_DidSendContent(object sender, SentContentEventArgs e)
        {
            VM.DidSendContent(e);
        }

        private void BtnLoad_Tapped(object sender, TappedRoutedEventArgs e)
        {
            VM.LoadMoreMessage();
        }
    }
}
