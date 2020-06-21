using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CustomControls;
using CAC.client.CodeEditorPage;
using CodeChatSDK.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using CodeChatSDK;
using CodeChatSDK.Utils;

namespace CAC.client.MessagePage
{
    class ChatPanelViewModel : BaseViewModel
    {
        public event Action<string> SetInputBoxText;
        public event Func<string> GetInputBoxText;

        //当前正在显示的聊天对应的聊天列表项
        private ChatListChatItemVM _ChatListItem;

        //当前正在显示的聊天对应的MessageViewer
        private MessageViewer _CurrentViewer;

        //对messageViewer的缓存。我们希望切换回某个会话时保留上次的浏览位置，因此设立一个缓存。
        //其实直接缓存viewModel是更好的方案，但没有找到解决恢复浏览位置的好方法。
        private Dictionary<ChatListBaseItemVM, MessageViewer> messageViewerCache =
            new Dictionary<ChatListBaseItemVM, MessageViewer>();

        public ChatListChatItemVM ChatListItem {
            get => _ChatListItem;
            set {
                _ChatListItem = value;
                RaisePropertyChanged(nameof(ChatListItem));
            }
        }

        public MessageViewer CurrentViewer {
            get => _CurrentViewer;
            set {
                _CurrentViewer = value;
                RaisePropertyChanged(nameof(CurrentViewer));
            }
        }


        public ChatPanelViewModel()
        {
            Messenger.Default.Register<ChatListChatItemVM>(this, "RequestCloseChatToken", RequestCloseChat);
            Messenger.Default.Register<ChatListChatItemVM>(this, "RequestOpenChatToken", RequestOpenChat);
            Messenger.Default.Register<CodeEditSessionInfo>(this, "RequestSendCodeBackToken", RequestSendCodeBack);
        }

        //当缓存中有时，直接从缓存中取，否则新建
        private void RequestOpenChat(ChatListChatItemVM chatListItem)
        {
            ChatListItem = chatListItem;
            if (messageViewerCache.Keys.Contains(chatListItem)) {
                CurrentViewer = messageViewerCache[chatListItem];
            }
            else {
                var viewerVM = new MessageViewer(chatListItem);
                CurrentViewer = viewerVM;
                messageViewerCache.Add(chatListItem, viewerVM);
            }

        }

        private void RequestSendCodeBack(CodeEditSessionInfo obj)
        {
            var type = GlobalFunctions.StringToSDKcodeType(obj.Language);
            ChatMessage message = ChatMessageBuilder.BuildCodeMessage(type, obj.Code);
            CurrentViewer.VM.topicController.SendMessage(message);
        }

        private void RequestCloseChat(ChatListChatItemVM chatListItem)
        {
            if (ChatListItem == chatListItem) {
                CurrentViewer = null;
                ChatListItem = null;
            }
            if (messageViewerCache.ContainsKey(chatListItem)) {
                messageViewerCache.Remove(chatListItem);
            }
        }

        private async void sendFileHelper(StorageFile file)
        {
            if (file == null)
                return;

            BasicProperties property = await file.GetBasicPropertiesAsync();
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            byte[] bytes = buffer.ToArray();

            //试上传
            UploadedAttachmentInfo uploadedAttachmentInfo = await CommunicationCore.client.Upload(file, property.Size, bytes);

            //判断上传是否成功
            if (uploadedAttachmentInfo != null) {
                //附件消息说明（可为空）
                string optionalMessage = "This is an attachment.";

                //运用消息构造器构造消息
                ChatMessage chatMessage = ChatMessageBuilder.BuildAttachmentMessage(uploadedAttachmentInfo, optionalMessage);
                CurrentViewer.VM.topicController.SendMessage(chatMessage);
            }
            else {
                //创建发送消息对象
                ChatMessage chatMessage = new ChatMessage() { Text = "Fail to send.", IsPlainText = true };
                CurrentViewer.VM.topicController.SendMessage(chatMessage);
            }

        }

        public void DidSendContent(SentContentEventArgs e)
        {

            switch (e.Type) {
                case MessageType.text:
                    ChatMessage chatMessage = new ChatMessage() { Text = e.Content, IsPlainText = true };
                    //发送消息
                    CurrentViewer.VM.topicController.SendMessage(chatMessage);
                    break;

                case MessageType.code:

                    var type = GlobalFunctions.StringToSDKcodeType(e.Language);
                    ChatMessage message = ChatMessageBuilder.BuildCodeMessage(type, e.Content);
                    CurrentViewer.VM.topicController.SendMessage(message);
                    break;
                case MessageType.image:
                    sendFileHelper(e.File);
                    break;
                case MessageType.file:
                    sendFileHelper(e.File);
                    break;
                default:
                    break;
            }

        }

        public void LoadMoreMessage()
        {
            CurrentViewer.VM.topicController.LoadMessage();
        }
    }
}
