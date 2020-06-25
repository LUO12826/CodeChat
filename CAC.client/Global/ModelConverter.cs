using CodeChatSDK.Models;
using CAC.client.ContactPage;
using CAC.client.Common;
using System.Diagnostics;
using CAC.client.MessagePage;
using ChatMessage = CodeChatSDK.Models.ChatMessage;
using CodeChatSDK.Utils;
using System.Collections.Generic;


namespace CAC.client
{
    /// <summary>
    /// 将数据转换为ViewModel。
    /// </summary>
    class ModelConverter
    {
        /// <summary>
        /// 将SDK的subscriber类型转换为界面联系人列表中的项类型
        /// </summary>
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
                IsOnline = subscriber.Online,
                
            };

            return contactVM;
        }

        /// <summary>
        /// 将SDK的topic类型转换为界面ChatList中的项类型
        /// </summary>
        public static ChatListChatItemVM TopicToChatListItem(Topic topic)
        {
            if (topic == null)
                return null;

            var contact = CommunicationCore.GetContactByTopicName(topic.Name);
       
            var chatListItem = new ChatListChatItemVM() {
                TopicName = topic.Name,
                Contact = contact,
                LastActiveTime = GlobalFunctions.TimestampToDateTime(topic.LastUsed),
                RawTopic = topic,
                MaxMsgSeq = topic.MaxLocalSeqId
            };

            return chatListItem;
        }

        /// <summary>
        /// 将SDK的消息类型转换为界面消息类型
        /// </summary>
        public static MessageItemBaseVM MessageToMessageVM(ChatMessage msg)
        {
            if (msg == null) return null;
            var contact = CommunicationCore.GetContactByTopicName(msg.TopicName);
            bool sendByMe = !(msg.From == msg.TopicName);

            //判断是否为代码消息
            if (msg.IsCode == true) {
                return new CodeMessageVM() {
                    Code = msg.Text,
                    Language = GlobalFunctions.SDKcodeTypeToString(msg.CodeType),
                    RunResult = null,
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
                string mime = attach.Count > 0 ? attach[0].Mime : "";
                
                //假如是以文件形式发送的图片
                if (GlobalFunctions.FindPosInImageMineList(GlobalConfigs.ImageMime, mime.ToLower()) != -1) {
                    return new ImageMessageVM() {
                        Contact = contact,
                        ID = msg.Id,
                        SendByMe = sendByMe,
                        RawMessage = msg,
                        ImageUri = url
                    };
                }

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
