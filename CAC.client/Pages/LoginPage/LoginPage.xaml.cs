﻿
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
using System.Linq;
using Windows.UI.Text;
using Windows.Security.Credentials;

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
            loadAccountInfo();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            recordAccountInfo();
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

        private void recordAccountInfo()
        {
            bool remember = false;
            if(rememberPwdCheckBox.IsChecked.HasValue) {
                remember = rememberPwdCheckBox.IsChecked.Value;
            }
            bool autoLogin = false;
            if (autoLoginCheckBox.IsChecked.HasValue) {
                autoLogin = autoLoginCheckBox.IsChecked.Value;
            }

            var newAccount = new AccountRecord() {
                UserName = userNameBox.Text,
                RememberPassword = remember,
                KeepLogin = autoLogin,
            };

            var account = accounts.Where(x => x.UserName == userNameBox.Text).FirstOrDefault();
            if(account == null) {
                accounts.Add(newAccount);
                if (newAccount.RememberPassword) {
                    var vault = new PasswordVault();
                    vault.Add(new PasswordCredential(Application.Current.ToString(), newAccount.UserName, passwordBox.Password));
                }
            }
            else {
                accounts.Remove(account);
                accounts.Insert(0, newAccount);
                if (!newAccount.RememberPassword) {
                    var vault = new PasswordVault();
                    var cred = vault.Retrieve(Application.Current.ToString(), newAccount.UserName);
                    if(cred != null) {
                        vault.Remove(cred);
                    }
                }
                else {
                    var vault = new PasswordVault();
                    vault.Add(new PasswordCredential(Application.Current.ToString(), newAccount.UserName, passwordBox.Password));
                }
            }


            AccountHelper.StorageAccountList(accounts);
        }

        private void loadAccountInfo()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(async () => {
                accounts = await AccountHelper.GetAccountList();
            });
            
        }

        private void userNameBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) {
                return;
            }
            userNameBox.ItemsSource = accounts.Where(x => x.UserName.Contains(userNameBox.Text)).ToList();
        }

        private void userNameBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var user = args.SelectedItem as AccountRecord;
            if (user == null)
                return;

            rememberPwdCheckBox.IsChecked = user.RememberPassword;
            
            if(user.RememberPassword) {
                var vault = new PasswordVault();
                var cred = vault.Retrieve(Application.Current.ToString(), user.UserName);
                if(cred != null) {
                    passwordBox.Password = cred.Password;
                }
                
            }
        }
    }
}
