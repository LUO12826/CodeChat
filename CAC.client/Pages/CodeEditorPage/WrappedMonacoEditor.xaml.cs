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

        private CodeEditSessionInfo _currentSession;
        public CodeEditSessionInfo CurrentSession {
            get => _currentSession;
            set {
                _currentSession = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSession)));
                switchToCurrentSession();
            }
        }

        private IEnumerable<HighlightLanguage> LanguageOptions {
            get {
                foreach (var lang in Enum.GetValues(typeof(HighlightLanguage))) {
                    yield return (HighlightLanguage)lang;
                }
            }
        }



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
            if (!isEditorLoaded)
                return;
            await editor.SwitchToSession(CurrentSession.GetHashCode().ToString(), CurrentSession.Language, CurrentSession.Code);
        }

        private void languageOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }



}
