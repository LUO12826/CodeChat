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
using CAC.client;


namespace CAC.client
{

    public sealed partial class testPage : Page
    {
        private DateTime date;
        private CodeEditorPage.CodeEditSessionInfo info;
        private CodeEditorPage.CodeEditorPage page = new CodeEditorPage.CodeEditorPage();

        public testPage()
        {
            this.InitializeComponent();
            date = DateTime.Now;
            info = new CodeEditorPage.CodeEditSessionInfo() {
                Contact = new ContactPage.ContactItemViewModel() {
                    UserID = "22234",

                },
                CreateTime = date,
                Language = "csharp",
                Code = "using System;"
            };
            testPre.Content = page;
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
        }

        private void Button_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }
}
