using AngleSharp.Dom.Events;
using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Media;
using RichTextControls;


namespace CAC.client.MessagePage
{
    sealed partial class CodeMessageBubble : UserControl
    {
        public event EventHandler DidTapRunButton;
        public event EventHandler DidTapEditButton;


        private HighlightLanguage defaultLanguage { get; set; } = HighlightLanguage.PlainText;

        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CodeMessageBubble), new PropertyMetadata(""));
        /// <summary>
        /// 代码
        /// </summary>
        public string Code {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }


        public static readonly DependencyProperty CodeLanguageProperty =
            DependencyProperty.Register("CodeLanguage", typeof(string), typeof(CodeMessageBubble), new PropertyMetadata("csharp"));
        /// <summary>
        /// 代码语言
        /// </summary>
        public string CodeLanguage {
            get { return (string)GetValue(CodeLanguageProperty); }
            set { SetValue(CodeLanguageProperty, value); }
        }


        public static readonly DependencyProperty RunResultProperty =
            DependencyProperty.Register("RunResult", typeof(string), typeof(CodeMessageBubble), new PropertyMetadata(null));
        /// <summary>
        /// 运行结果
        /// </summary>
        public string RunResult {
            get { return (string)GetValue(RunResultProperty); }
            set { SetValue(RunResultProperty, value); }
        }

        public static readonly DependencyProperty SendByMeProperty =
            DependencyProperty.Register("SendByMe", typeof(bool), typeof(CodeMessageBubble), new PropertyMetadata(false, SendByMeChanged));

        private static void SendByMeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is CodeMessageBubble codeBubble) {
                codeBubble.ChangeBtnRow((bool)e.NewValue);
            }
        }

        public bool SendByMe {
            get { return (bool)GetValue(SendByMeProperty); }
            set { SetValue(SendByMeProperty, value); }
        }

        public static readonly DependencyProperty BgColorProperty =
            DependencyProperty.Register("BgColor", typeof(Brush), typeof(CodeMessageBubble),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))));
        public Brush BgColor {
            get { return (Brush)GetValue(BgColorProperty); }
            set { SetValue(BgColorProperty, value); }
        }

        public CodeMessageBubble()
        {
            this.InitializeComponent();
        }

        public void ChangeBtnRow(bool sendByMe)
        {
            if(sendByMe) {
                functionBtnRight.Visibility = Visibility.Collapsed;
                functionBtnsLeft.Visibility = Visibility.Visible;
            }
            else {
                functionBtnRight.Visibility = Visibility.Visible;
                functionBtnsLeft.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            DidTapRunButton?.Invoke(this, new EventArgs());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            DidTapEditButton?.Invoke(this, new EventArgs());
        }
    }
}
