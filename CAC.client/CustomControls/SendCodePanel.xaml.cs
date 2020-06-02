using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CAC.client.Common;
using RichTextControls;
using CAC.client;
using System.Diagnostics;

namespace CAC.client.CustomControls
{
    sealed partial class SendCodePanel : UserControl, INotifyPropertyChanged
    {
        public event Action<string, string> DidSendCode;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _Code;

        private IEnumerable<string> LanguageOptions => GlobalConfigs.HighlightLanguageList;

        public string Code {
            get => _Code;
            set {
                _Code = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Code)));
            }
        }

        public SendCodePanel()
        {
            this.InitializeComponent();
            languageOptionBox.SelectedIndex = 0;
            editor.Options.Minimap = new Monaco.Editor.IEditorMinimapOptions() {
                Enabled = false,
            };
        }

        private void sendButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            if (!Code.IsNullOrEmpty()) {
                DidSendCode?.Invoke(editor.CodeLanguage, Code);
                Debug.WriteLine(editor.CodeLanguage);
                Debug.WriteLine(Code);
            }
        }

        private void languageOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = languageOptionBox.SelectedIndex;
            editor.CodeLanguage = GlobalConfigs.HighlightLanguageListLower[index];
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }
    }
}
