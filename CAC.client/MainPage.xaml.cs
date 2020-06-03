using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using System;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CodeEditorPage;
using System.ComponentModel;

namespace CAC.client
{

    sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private CodeEditorPage.CodeEditorPage codeEditor => CodeEditorPage.CodeEditorPage.Default;

        private string avatar = "";
        public string Avatar {
            get => avatar;
            set {
                avatar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Avatar)));
            }
        }

        private bool CodeEditorPageExpanded = false;

        private string currentPage;

        public MainPage()
        {
            this.InitializeComponent();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Messenger.Default.Register<CodeEditSessionInfo>(this, "RequestEditCodeToken", RequestEditCode);
        }

        private void RequestEditCode(CodeEditSessionInfo session)
        {

            if(!CodeEditorPageExpanded) {
                int AddWidth = 300;
                if (GlobalFunctions.TryResizeWindow(AddWidth, 0)) {
                    Debug.WriteLine("resize ok");
                }
                MainGrid.Expan(AddWidth);
                codeEditorFrame.Content = codeEditor;
                CodeEditorPageExpanded = true;
            }
            
            codeEditor.RequestOpenSession(session);
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            codeEditor.AllSessionClosed += CodeEditor_AllSessionClosed;
            naviFrame.Navigate(typeof(MessagePage.MessagePage), 0);
            currentPage = "chat";
            Avatar = CommunicationCore.account.Avatar;
        }

        private void CodeEditor_AllSessionClosed()
        {
            if (CodeEditorPageExpanded) {

                MainGrid.Collapse();
                CodeEditorPageExpanded = false;
            }
        }

        private void Navigator_OnNavigationChanged(object sender, object e)
        {
            string destination = (string)e;
            if (destination == currentPage)
                return;

            currentPage = destination;
            Type type = typeof(MessagePage.MessagePage);
            switch (destination) {
                case "avatar":
                    type = typeof(AvatarPage.AvatarPage);
                    break;
                case "chat":
                    type = typeof(MessagePage.MessagePage);
                    break;
                case "contact":
                    type = typeof(ContactPage.ContactPage);
                    break;
                case "settings":
                    type = typeof(SettingsPage.SettingsPage);
                    break;
                default:
                    type = typeof(MessagePage.MessagePage);
                    break;
            }
            if(type != null) {
                naviFrame.Navigate((type), 0);
            }
        }
    }
}
