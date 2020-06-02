using CodeChatSDK.Models;
using CAC.client.ContactPage;
using CAC.client.Common;
using System.Diagnostics;
using CAC.client.MessagePage;
using Windows.ApplicationModel.Chat;
using ChatMessage = CodeChatSDK.Models.ChatMessage;
using CodeChatSDK.Utils;
using System.Collections.Generic;


namespace CAC.client
{
    class ModelConverter
    {
        public static ContactItemViewModel SubscriberToContact(Subscriber subscriber)
        {
            if (subscriber == null)
                return null;

            string avatar = subscriber.PhotoData.IsNullOrEmpty() ? GlobalConfigs.defaultAvatar : subscriber.PhotoData;
            var contactVM = new ContactItemViewModel() {
                UserName = subscriber.Username,
                UserID = subscriber.UserId,
                Base64Avatar = avatar,
                Note = null,
                IsOnline = subscriber.Online
            };

            return contactVM;
        }


        public static ChatListChatItemVM TopicToChatListItem(Topic topic)
        {
            if (topic == null)
                return null;

            var contact = CommunicationCore.GetContactByTopicName(topic.Name);
       
            var chatListItem = new ChatListChatItemVM() {
                TopicName = topic.Name,
                Contact = contact,
                LastActiveTime = GlobalFunctions.TimestampToDateTime(topic.LastUsed),
            };

            return chatListItem;
        }

        public static MessageItemBaseVM MessageToMessageVM(ChatMessage msg)
        {
            if (msg == null) return null;
            Debug.WriteLine("进行消息转换");
            var contact = CommunicationCore.GetContactByTopicName(msg.TopicName);
            bool sendByMe = !(msg.From == msg.TopicName);
            Debug.WriteLine(msg.IsPlainText);

            //判断是否为代码消息
            if (msg.IsCode == true) {
                return new CodeMessageVM() {
                    Code = msg.Text,
                    Language = GlobalFunctions.SDKcodeTypeToString(msg.CodeType),
                    ID = msg.Id,
                    Contact = contact,
                    SendByMe = sendByMe,
                    RawMessage = msg
                };
            }

            //判断是否为附件消息
            if (msg.IsAttachment == true) {

                var attach = ChatMessageParser.ParseGenericAttachment(msg);
                //通过消息解析器获取URL列表
                List<string> urls = ChatMessageParser.ParseUrl(msg, CommunicationCore.client.ApiBaseUrl);

                string url = urls.Count > 0 ? urls[0] : "";
                string name = attach.Count > 0 ? attach[0].Name : "";

                return new FileMessageVM() {
                    Contact = contact,
                    ID = msg.Id,
                    SendByMe = sendByMe,
                    RawMessage = msg,
                    DownloadState = -1,
                    FileUri = url,
                    FileName = name,
                };
            }

            //判断是否为图片消息
            if (msg.IsPlainText == false) {
                List<string> base64s = ChatMessageParser.ParseImageBase64(msg);
                string base64 = base64s.Count > 0 ? base64s[0] : "";
                Debug.WriteLine("load message image");
                Debug.WriteLine(base64);
                return new ImageMessageVM() {
                    Contact = contact,
                    ID = msg.Id,
                    SendByMe = sendByMe,
                    RawMessage = msg,
                    ImageBase64 = base64,
                };
            }

            return new TextMessageVM() {
                Contact = contact,
                ID = msg.Id,
                SendByMe = sendByMe,
                RawMessage = msg,
                Text = msg.Text
            };
        }
    }
}
