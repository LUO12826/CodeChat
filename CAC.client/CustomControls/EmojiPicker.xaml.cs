using System;
using Windows.UI.Xaml.Controls;

/*
 *将表情正确地分组显示后，滚动流畅度已经没问题，但打开速度仍然很慢。
 * 使用增量加载的方式可能可以解决问题。
 *
 */
namespace CAC.client.CustomControls
{
    /// <summary>
    /// 表情选择器。
    /// </summary>
    sealed partial class EmojiPicker : UserControl
    {
        private EmojisCollection collection = Emojis.InternalEmojis;
        public event EventHandler<string> DidSelectAnEmoji;

        public EmojiPicker()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            DidSelectAnEmoji?.Invoke(this, (e.ClickedItem as Emoji).EmojiString);
        }

    }

}
