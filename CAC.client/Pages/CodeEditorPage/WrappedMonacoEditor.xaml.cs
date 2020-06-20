using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CAC.client.ContactPage;
using System.ComponentModel;
using RichTextControls;
using System.Diagnostics;
using Monaco;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.Common;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;

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
        public event Action<string, string> SendCodeBack;

        private bool isSwitching = false;
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
            isSwitching = true;
            if (!isEditorLoaded || CurrentSession == null)
                return;

            int langIndex = GlobalFunctions.FindPosInLangList(CurrentSession.Language);
            if(langIndex != -1) {
                languageOptionBox.SelectedIndex = langIndex;
            }
            else {
                languageOptionBox.SelectedIndex = 0;
            }
            await editor.SwitchToSession(CurrentSession.GetHashCode().ToString(),
                GlobalConfigs.HighlightLanguageListLower1[langIndex], 
                CurrentSession.Code);
            await Task.Delay(200);
            isSwitching = false;
        }

        private void languageOptionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(isSwitching) {
                return;
            }
            CurrentSession.Language = GlobalConfigs.HighlightLanguageListLower[languageOptionBox.SelectedIndex];
            editor.CodeLanguage = GlobalConfigs.HighlightLanguageListLower1[languageOptionBox.SelectedIndex];
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
        }


        private async void save_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            CurrentSession.Code = editor.Text;

            string extension;
            int langIndex = GlobalFunctions.FindPosInLangList(CurrentSession.Language);
            if (langIndex != -1) {
                extension = GlobalConfigs.HighlightLanguageExtension[langIndex];
            }
            else {
                extension = GlobalConfigs.HighlightLanguageExtension[0];
            }

            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add(CurrentSession.Language, new List<string>() { extension });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "code";

            StorageFile file = await savePicker.PickSaveFileAsync();
            writeCodeToFile(file);
        }

        private async void writeCodeToFile(StorageFile file)
        {

            if (file != null) {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, CurrentSession.Code);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete) {

                }
                else {

                }
            }
        }

        private void sendBack_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            editor.Focus(Windows.UI.Xaml.FocusState.Keyboard);
            CurrentSession.Code = editor.Text;
            Messenger.Default.Send(CurrentSession, "RequestSendCodeBackToken");
        }

        private async void BtnRun_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            string Code = editor.Text;
            string CodeLanguage = CurrentSession.Language;

            if (Code.IsNullOrEmpty()) {
                return;
            }

            string lang = "";
            switch (CodeLanguage) {
                case "python":
                    lang = "py";
                    break;
                case "cplusplus":
                    lang = "cpp";
                    break;
                case "c":
                    lang = "c";
                    break;
                case "java":
                    lang = "java";
                    break;
                default:
                    break;
            }

            if (lang == "") {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    resultTextBlock.Text = "暂不支持此语言";
                });
            }
            else {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    resultTextBlock.Text = "正在编译运行……";
                });
                string result = await CompileHelper.Compile(lang, Code, new Random().Next().ToString());

                Debug.WriteLine(lang);
                Debug.WriteLine(Code);
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    resultTextBlock.Text = result;
                });
            }
        }
    }



}
