using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CAC.client;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace CAC.client.CustomControls
{
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
