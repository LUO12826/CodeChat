using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using CAC.client.ContactPage;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using CAC.client.CodeEditorPage;

namespace CAC.client.MessagePage
{
    class MessageViewerViewModel : BaseViewModel
    {
        private bool _IsGroupChat = false;
        private ContactItemViewModel _MyContactInfo;
        //测试
        private Brush _LeftBubbleBgColor = GlobalConfigs.LeftBubbleBgColor;
        private Brush _RightBubbleBgColor = GlobalConfigs.RightBubbleBgColor;

        public bool RequireCache { get; set; } = false;

        //记录scrollViewer的垂直滚动位置，现在用不到了。
        //public double VerticalScrollOffset { get; set; } = -1.0;

        public DateTime LastOpenTime { get; set; }

        public IncrementalCollection<MessageItemBaseVM> Messages;

        public bool IsGroupChat {
            get => _IsGroupChat;
            set {
                _IsGroupChat = value;
                RaisePropertyChanged(nameof(IsGroupChat));
            }
        }
        public Brush RightBubbleBgColor {
            get => _RightBubbleBgColor;
            set {
                _RightBubbleBgColor = value;
                RaisePropertyChanged(nameof(RightBubbleBgColor));
            }
        }


        public Brush LeftBubbleBgColor {
            get => _LeftBubbleBgColor;
            set {
                _LeftBubbleBgColor = value;
                RaisePropertyChanged(nameof(LeftBubbleBgColor));
            }
        }

        public ContactItemViewModel MyContactInfo {
            get => _MyContactInfo;
            set {
                _MyContactInfo = value;
                RaisePropertyChanged(nameof(MyContactInfo));
            }
        }


        public MessageViewerViewModel()
        {

            Messages = new IncrementalCollection<MessageItemBaseVM>(loadMoreItems);
            for (int i = 0; i < 40; i++) {
                Messages.Add(new TextMessageVM() {
                    Contact = new ContactItemViewModel() {
                        Base64Avatar = GlobalConfigs.testB64Avator,
                        UserName = "aaa",
                    },
                    ID = i,
                    Text = "这是测试消息。",
                    SendByMe = i % 2 == 0 ? true : false
                });
            }

            MyContactInfo = new ContactItemViewModel() {
                UserName = "self",
                Base64Avatar = GlobalConfigs.testB64Avator1
            };
            
        }

        public void RequestEditCode(CodeMessageVM codeMessage)
        {

            var newSession = new CodeEditSessionInfo() {
                Contact = codeMessage.Contact,
                CreateTime = DateTime.Now,
                Language = codeMessage.Language,
                MessageID = codeMessage.ID,
                Code = codeMessage.Code,
            };
            Messenger.Default.Send(newSession, "RequestEditCodeToken");
        }

        private async Task<Tuple<List<MessageItemBaseVM>, bool>> loadMoreItems(uint itemsNum)
        {
            var a = await Task.Run(async () => {
                Thread.Sleep(2000);
                var b = new List<MessageItemBaseVM>();
                for (int i = 0; i < itemsNum; i++) {

                    b.Add(new TextMessageVM() {
                        Contact = new ContactItemViewModel() {
                            Base64Avatar = GlobalConfigs.testB64Avator,
                            UserName = "aaa",
                        },
                        Text = "新加项目",
                        ID = itemsNum + 300,
                        SendByMe = false,
                    });
                }


                var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var bb = await exampleFile.GetFolderAsync("Code");
                var cc = await bb.GetFileAsync("example.java");
                var text = await FileIO.ReadTextAsync(cc);

                var c = new CodeMessageVM() {
                    Code = text,
                    Language = "java",
                    Contact = new ContactItemViewModel() {
                        Base64Avatar = GlobalConfigs.testB64Avator,
                        UserName = "aaa",
                        UserID = "sdfewe",
                    },
                    ID = 1234567,
                    SendByMe = false,
                    
                };

                var d = new ImageMessageVM() {
                    Contact = new ContactItemViewModel() {
                        Base64Avatar = GlobalConfigs.testB64Avator,
                        UserName = "aaa",
                    },
                    SendByMe = false,
                    ImageBase64 = GlobalConfigs.testB64Avator,
                    ID = 1234667,
                };
                b.Add(d);
                b.Add(c);
                return b;
            });

            return new Tuple<List<MessageItemBaseVM>, bool>(a, false);
        }


    }
}
