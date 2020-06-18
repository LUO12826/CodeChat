using CAC.client.Common;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Xaml.Input;
using Windows.Storage.Pickers;
using System.IO;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
using Windows.Storage;

namespace CAC.client.CustomControls
{
    sealed partial class InputBox : UserControl
    {
        public event EventHandler<SentContentEventArgs> DidSentContent;


        public InputBox()
        {
            this.InitializeComponent();
            TextInputBox.AddHandler(RichEditBox.KeyDownEvent, new KeyEventHandler(TextInputBox_KeyDown), true);
            var a = new RichEditBox();
            var b = new Paragraph();
            var c = new RichTextBlock();

        }

        //将表情插入字串中。注意，一个表情占字符串中的两个字符。
        private void EmojiPicker_DidSelectAnEmoji(object sender, string e)
        {
            //获取选中文本的起始位置
            int textSelectStartPosition = TextInputBox.TextDocument.Selection.StartPosition;
            int textSelectEndPosition = TextInputBox.TextDocument.Selection.EndPosition;

            //获取完整文本
            TextInputBox.TextDocument.GetText(Windows.UI.Text.TextGetOptions.None, out string text);

            int afterLength = text.Length - 1 - textSelectStartPosition;
            //拆分成前后两段
            string before = text.Substring(0, textSelectStartPosition);
            string after = text.Substring(textSelectStartPosition, afterLength);
            text = before + e + after;
            //再将文本设置回去
            TextInputBox.TextDocument.SetText(Windows.UI.Text.TextSetOptions.None, text);
            //隐藏表情选择面板
            emojiPickerButton.Flyout.Hide();

            //恢复光标位置
            TextInputBox.TextDocument.Selection.StartPosition = textSelectStartPosition + 2;
            TextInputBox.TextDocument.Selection.EndPosition = textSelectEndPosition + 2;
        }

        private void TextInputBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            int textSelectStartPosition = TextInputBox.TextDocument.Selection.StartPosition;
            int textSelectEndPosition = TextInputBox.TextDocument.Selection.EndPosition;
        }

        private void sendButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            sendText();
        }

        private void TextInputBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Enter) {
                sendText();
            }
        }

        private void sendText()
        {
            TextInputBox.TextDocument.GetText(Windows.UI.Text.TextGetOptions.UseLf, out string text);
            if (text.IsNullOrEmpty()) {
                return;
            }
            var arg = new SentContentEventArgs() {
                Type = MessageType.text,
                Language = null,
                Content = text.Trim()
            };
            DidSentContent?.Invoke(this, arg);
            TextInputBox.TextDocument.SetText(Windows.UI.Text.TextSetOptions.None, "");
        }

        private async void selectImageButton_Tapped(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.CommitButtonText = "发送";
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();

            if (file != null) {

                var prop = await file.GetBasicPropertiesAsync();
                if (prop.Size > GlobalConfigs.MaxUploadFileSize) {
                    showFileTooLarge();
                    return;
                }

                var arg = new SentContentEventArgs() {
                    Type = MessageType.image,
                    Language = null,
                    File = file
                };
                DidSentContent?.Invoke(this, arg);
            }
            
        }

        private async void selectFileButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.CommitButtonText = "发送";
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();

            if (file != null) {

                var prop = await file.GetBasicPropertiesAsync();
                if(prop.Size > GlobalConfigs.MaxUploadFileSize) {
                    showFileTooLarge();
                    return;
                }

                var arg = new SentContentEventArgs() {
                    Type = MessageType.file,
                    Language = null,
                    File = file
                };
                DidSentContent?.Invoke(this, arg);
            }
        }

        private void SendCodePanel_DidSendCode(string lang, string code)
        {
            if (code.IsNullOrEmpty()) {
                return;
            }
            var arg = new SentContentEventArgs() {
                Type = MessageType.code,
                Language = lang,
                Content = code
            };
            DidSentContent?.Invoke(this, arg);

            sendCodeButton.Flyout.Hide();

        }

        private async void showFileTooLarge()
        {
            var msgDialog = new Windows.UI.Popups.MessageDialog("当前仅支持上传小于8MB的文件。") { Title = "文件过大" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定"));
            await msgDialog.ShowAsync();
        }
    }

    class SentContentEventArgs : EventArgs
    {
        public MessageType Type { get; set; }
        public string Language { get; set; }
        public string Content { get; set; }
        public StorageFile File { get; set; }
    }
}
