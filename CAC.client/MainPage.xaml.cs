using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CodeEditorPage;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using CAC.client.Common;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace CAC.client
{

    sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private CodeEditorPage.CodeEditorPage codeEditor => CodeEditorPage.CodeEditorPage.Default;

        private string avatar = GlobalConfigs.defaultAvatar;
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
            var tiWtleBar = ApplicationView.GetForCurrentView().TitleBar;
            tiWtleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);

            Messenger.Default.Register<CodeEditSessionInfo>(this, "RequestEditCodeToken", RequestEditCode);
            GlobalRef.Navigator = Navigator;
            GlobalRef.MainPageNotification = notify;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            codeEditor.AllSessionClosed += CodeEditor_AllSessionClosed;
            naviFrame.Navigate(typeof(MessagePage.MessagePage), 0);
            currentPage = "chat";

            Task.Run(async () => {
                await Task.Delay(1000);
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    Avatar = CommunicationCore.account.Avatar.IsNullOrEmpty() ? GlobalConfigs.defaultAvatar : CommunicationCore.account.Avatar;
                });
            });
            
        }

        private void RequestEditCode(CodeEditSessionInfo session)
        {

            if(!CodeEditorPageExpanded) {
                int AddWidth = 300;
                if (GlobalFunctions.TryResizeWindow(AddWidth, 0)) {

                }
                MainGrid.Expan(AddWidth);
                codeEditorFrame.Content = codeEditor;
                CodeEditorPageExpanded = true;
            }

            codeEditor.RequestOpenSession(session);
        }

        private async Task openEditorInNewWindow()
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                Frame frame = new Frame();
                frame.Navigate(typeof(CodeEditorPage.CodeEditorPage));
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
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
