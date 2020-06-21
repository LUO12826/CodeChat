using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CAC.client.MessagePage
{
    class ChatListDataTemplateSelector : DataTemplateSelector
    {
        private ResourceDictionary resourceDict;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(resourceDict == null) {
                resourceDict = (container as ListViewItem).FindAscendant<ListView>().Resources;
            }

            var itemVM = item as IChatListItem;

            if (itemVM is ChatListChatItemVM) {
                    return resourceDict["ChatListItemNormalTemplate"] as DataTemplate;
            }

            throw new NotImplementedException("ChatList模板选择器错误");
        }
    }
}
