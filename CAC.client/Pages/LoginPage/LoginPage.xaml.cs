
using CodeChatSDK.EventHandler;
using CodeChatSDK.Models;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CAC.client;


namespace CAC.client.LoginPage
{

    sealed partial class LoginPage : Page, INotifyPropertyChanged
    {
        private List<AccountRecord> accounts;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool _LoginRunning = false;
        public bool LoginRunning {
            get => _LoginRunning;
            set {
                _LoginRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoginRunning)));
            }
        }



        public LoginPage()
        {
            this.InitializeComponent();

            CommunicationCore.client.LoginSuccessEvent += LoginSucceed;
            CommunicationCore.client.LoginFailedEvent += LoginFailed;
        }


        private void LoginFailed(object sender, LoginFailedEventArgs args)
        {
            LoginRunning = false;
            Debug.WriteLine(args.Exception.Message);
        }

        private void LoginSucceed(object sender, LoginSuccessEventArgs args)
        {
            Debug.WriteLine("login succeed");
            
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                LoginRunning = false;
                GlobalRef.RootFrame.Navigate(typeof(MainPage), 0);
            });
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            accounts = await AccountHelper.GetAccountList();
            
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

        }


        private void loginButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (LoginRunning)
                return;
            LoginRunning = true;

            CommunicationCore.Login(userNameBox.Text, passwordBox.Password);

        }

    }
}
