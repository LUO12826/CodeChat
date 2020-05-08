using CAC.client.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
            Messages.Add(b);
            MyUserName = "self";
            MyBase64Avatar = GlobalConfigs.testB64Avator;
        }


        private async Task<Tuple<List<MessageItemBaseVM>, bool>> loadMoreItems(uint itemsNum)
        {
            var a = await Task.Run(() => {
                Thread.Sleep(2000);
                var b = new List<MessageItemBaseVM>();
                for (int i = 0; i < itemsNum; i++) {

                    b.Add(new TextMessageVM() {
                        UserName = "bbb",
                        Text = "新加项目",
                        Base64Avatar = GlobalConfigs.testB64Avator
                    });
                }
                return b;
            });

            return new Tuple<List<MessageItemBaseVM>, bool>(a, false);
        }


    }
}
