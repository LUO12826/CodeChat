using CAC.client.Common;
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



namespace CAC.client.SignupPage
{

    sealed partial class SignupPage : Page
    {
        public SignupPage()
        {
            this.InitializeComponent();
        }

        private void registerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var info = getInfo();
            if(info ==null) {
                pwdHint.Text = "信息不完整，请检查";
                pwdHint.Visibility = Visibility.Visible;
            }

            if(passwordBox.Password != ConfirmPasswordBox.Password) {
                pwdHint.Text = "两次输入的密码不一致";
                pwdHint.Visibility = Visibility.Visible;
                return;
            }
            pwdHint.Visibility = Visibility.Collapsed;
            

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
    }
}
