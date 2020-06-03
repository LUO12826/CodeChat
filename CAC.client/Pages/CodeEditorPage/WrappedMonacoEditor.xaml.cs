using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CAC.client.ContactPage;
using System.ComponentModel;
using RichTextControls;
using System.Diagnostics;
using Monaco;

namespace CAC.client.CodeEditorPage
{

    /// <summary>
    /// 将Monaco editor进行包装，增加语言选择、用户头像显示等功能。
    /// </summary>
    sealed partial class WrappedMonacoEditor : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool isEditorLoaded { get; private set; } = false;
        public event Action CodeEditorLoaded;
        public event Action<string> SendCodeBack;

        private CodeEditSessionInfo _currentSession;
        public CodeEditSessionInfo CurrentSession {
            get => _currentSession;
            set {
                _currentSession = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSession)));
                switchToCurrentSession();
            }
        }

        private IEnumerable<string> LanguageOptions => GlobalConfigs.HighlightLanguageList;


        public WrappedMonacoEditor()
        {
            this.InitializeComponent();
            
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            isEditorLoaded = true;
            CodeEditorLoaded?.Invoke();
        }

        public async void CloseSession(CodeEditSessionInfo session)
        {
            await editor.CloseSession(CurrentSession.GetHashCode().ToString());
        }

        private async void switchToCurrentSession()
        {
            if (!isEditorLoaded || CurrentSession == null)
                return;

            int langIndex = GlobalFunctions.FindPosInLangList(CurrentSession.Language);
            Debug.WriteLine("langIndex");
            Debug.WriteLine(langIndex);
            if(langIndex != -1) {
                languageOptionBox.SelectedIndex = langIndex;
            }
            await editor.SwitchToSession(CurrentSession.GetHashCode().ToString(), CurrentSession.Language, CurrentSession.Code);

        }

        private void languageOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSession.Language = GlobalConfigs.HighlightLanguageListLower[languageOptionBox.SelectedIndex];
            editor.CodeLanguage = CurrentSession.Language;
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }


        private void save_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
        }

        private void sendBack_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            
            SendCodeBack?.Invoke(editor.Text);
        }

        private void BtnRun_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {

        }
    }



}
