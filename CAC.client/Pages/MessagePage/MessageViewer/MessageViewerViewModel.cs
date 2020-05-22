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

namespace CAC.client.MessagePage
{
    class MessageViewerViewModel : BaseViewModel
    {
        private bool _IsGroupChat = false;
        private string _MyUserName;
        private string _MyBase64Avatar;
        private Brush _LeftBubbleBgColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 192, 214, 191));
        private Brush _RightBubbleBgColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 244, 148, 118));

        public bool RequireCache { get; set; } = false;

        //记录scrollViewer的垂直滚动位置，现在用不到了。
        public double VerticalScrollOffset { get; set; } = -1.0;
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

        public string MyUserName {
            get => _MyUserName;
            set {
                _MyUserName = value;
                RaisePropertyChanged(nameof(MyUserName));
            }
        }

        public string MyBase64Avatar {
            get => _MyBase64Avatar;
            set {
                _MyBase64Avatar = value;
                RaisePropertyChanged(nameof(MyBase64Avatar));
            }
        }

        public MessageViewerViewModel()
        {

            Messages = new IncrementalCollection<MessageItemBaseVM>(loadMoreItems);
            for (int i = 0; i < 40; i++) {
                Messages.Add(new TextMessageVM() {
                    Base64Avatar = GlobalConfigs.testB64Avator,
                    Text = "aaaaaaaaaaaaaaaa",
                    UserName = "aaa",
                    SendByMe = i % 2 == 0 ? true : false
                });
            }
            var b = new ImageMessageVM() {
                Base64Avatar = GlobalConfigs.testB64Avator,
                ImageBase64 = GlobalConfigs.testB64Avator,
                UserName = "aaa",
                SendByMe = false
            };
            var c = new CodeMessageVM() {
                Code = "using System;",
                Language = "csharp",
                UserName = "aaa",
                SendByMe = false,
                Base64Avatar = GlobalConfigs.testB64Avator,
            };

            Messages.Add(b);
            Messages.Add(c);
            MyUserName = "self";
            MyBase64Avatar = GlobalConfigs.testB64Avator;
        }


        private async Task<Tuple<List<MessageItemBaseVM>, bool>> loadMoreItems(uint itemsNum)
        {
            var a = await Task.Run(async () => {
                Thread.Sleep(2000);
                var b = new List<MessageItemBaseVM>();
                for (int i = 0; i < itemsNum; i++) {

                    b.Add(new TextMessageVM() {
                        UserName = "bbb",
                        Text = "新加项目",
                        Base64Avatar = GlobalConfigs.testB64Avator
                    });
                }

                //var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\Code\example.js");
                var exampleFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
                var bb = await exampleFile.GetFolderAsync("Code");
                var cc = await bb.GetFileAsync("example.java");
                foreach(var file in await bb.GetFilesAsync()) {
                    Debug.WriteLine(file.Name);
                }
                var text = await FileIO.ReadTextAsync(cc);

                var c = new CodeMessageVM() {
                    Code = text,
                    Language = "php",
                    UserName = "aaa",
                    SendByMe = false,
                    Base64Avatar = GlobalConfigs.testB64Avator,
                };
                b.Add(c);
                return b;
            });

            return new Tuple<List<MessageItemBaseVM>, bool>(a, false);
        }


    }
}
