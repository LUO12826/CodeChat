
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
using Windows.UI.Xaml.Input;

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

        public bool _IsLoginFailed = false;
        public bool IsLoginFailed {
            get => _IsLoginFailed;
            set {
                _IsLoginFailed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoginFailed)));
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
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                LoginRunning = false;
                IsLoginFailed = true;
                failedHintBlock.Text = "登录失败，原因为：" + args.Exception.Message;
            });
        }

        private void LoginSucceed(object sender, LoginSuccessEventArgs args)
        {
            
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                LoginRunning = false;
                GlobalRef.RootFrame.Navigate(typeof(MainPage), 0);
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter != null && e.Parameter is Tuple<string, string> info) {
                userNameBox.Text = info.Item1;
                passwordBox.Password = info.Item2;
            }

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

        }


        private async void loginButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (LoginRunning)
                return;

            await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                LoginRunning = true;
                IsLoginFailed = false;
            });    
            await CommunicationCore.Login(userNameBox.Text, passwordBox.Password);
        }

        private void BtnSignup_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GlobalRef.RootFrame.Navigate(typeof(SignupPage.SignupPage), 0);
        }
    }
}
