using CAC.client.Common;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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



namespace CAC.client.SignupPage
{

    sealed partial class SignupPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool _SignupRunning = false;
        public bool SignupRunning {
            get => _SignupRunning;
            set {
                _SignupRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignupRunning)));
            }
        }

        public bool _SignupFailed = false;
        public bool SignupFailed {
            get => _SignupFailed;
            set {
                _SignupFailed = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SignupFailed)));
            }
        }

        public SignupPage()
        {
            this.InitializeComponent();
            CommunicationCore.client.RegisterSuccessEvent += Client_RegisterSuccessEvent;
            CommunicationCore.client.RegisterFailedEvent += Client_RegisterFailedEvent;
        }

        

        private void Client_RegisterFailedEvent(object sender, CodeChatSDK.EventHandler.RegisterFailedEventArgs args)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                pwdHint.Text = "注册失败，信息为：" + args.Exception.Message;
                SignupFailed = true;
                SignupRunning = false;
            });
        }

        private void Client_RegisterSuccessEvent(object sender, CodeChatSDK.EventHandler.RegisterSuccessEventArgs args)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                SignupRunning = false;
                
                notify.Show("注册成功", 1500);
                GlobalRef.RootFrame.Navigate(typeof(LoginPage.LoginPage), 
                    new Tuple<string, string>(userNameBox.Text, passwordBox.Password));

            });

            CommunicationCore.accountController.SendVerificationCode("123456");
        }

        private async void registerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            var info = getInfo();
            if(info == null) {
                pwdHint.Text = "信息不完整，请检查";
                SignupFailed = true;
                return;
            }

            if(passwordBox.Password != ConfirmPasswordBox.Password) {
                pwdHint.Text = "两次输入的密码不一致";
                SignupFailed = true;
                return;
            }

            SignupRunning = true;
            SignupFailed = false;
            await CommunicationCore.Register(info[0], info[1], info[2], info[3]);
        }

        private string[] getInfo()
        {
            string userName = userNameBox.Text;
            string pwd = passwordBox.Password;
            string nickName = nickNameBox.Text;
            string email = emailBox.Text;
            var info = new string[] { userName, pwd, nickName, email };
            foreach(var i in info) {
                if(i.IsNullOrEmpty()) {
                    return null;
                }
            }
            return info;
        }

        private void btnReturn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            GlobalRef.RootFrame.Navigate(typeof(LoginPage.LoginPage));
        }
    }
}
