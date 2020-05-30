using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CAC.client.Common;
using RichTextControls;
using CAC.client;

namespace CAC.client.CustomControls
{
    sealed partial class SendCodePanel : UserControl, INotifyPropertyChanged
    {
        public event Action<string, string> DidSendCode;
        public event PropertyChangedEventHandler PropertyChanged;
        private string _Code;

        private IEnumerable<HighlightLanguage> LanguageOptions {
            get {
                foreach (var lang in Enum.GetValues(typeof(HighlightLanguage))) {
                    yield return (HighlightLanguage)lang;
                }
            }
        }

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
            editor.Options.Minimap = new Monaco.Editor.IEditorMinimapOptions() {
                Enabled = false,
            };
        }

        private void sendButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!Code.IsNullOrEmpty()) {
                DidSendCode?.Invoke(editor.CodeLanguage, Code);
            }
        }

        private void languageOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (int)(HighlightLanguage)languageOptionBox.SelectedItem;
            editor.CodeLanguage = GlobalConfigs.HighlightLanguageList[index].ToLower();
        }
    }
}
