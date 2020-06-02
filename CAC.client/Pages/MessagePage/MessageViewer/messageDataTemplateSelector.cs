using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CAC.client.MessagePage
{
    //TODO:使用表驱动的方式改造选择器
    class messageDataTemplateSelector : DataTemplateSelector
    {
        private ResourceDictionary resourceDict;

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(resourceDict == null) {
                resourceDict = (container as ListViewItem).FindAscendant<ListView>().Resources;
            }

            var itemVM = item as MessageItemBaseVM;

            if (itemVM.SendByMe == false) {
                if(itemVM is TextMessageVM) {
                    return resourceDict["MessageViewerTextLeftCell"] as DataTemplate;
                }
                else if(itemVM is ImageMessageVM) {
                    return resourceDict["MessageViewerImageLeftCell"] as DataTemplate;
                }
                else if(itemVM is CodeMessageVM) {
                    return resourceDict["MessageViewerCodeLeftCell"] as DataTemplate;
                }
                else if(itemVM is FileMessageVM) {
                    return resourceDict["MessageViewerFileLeftCell"] as DataTemplate;
                }
            }
            else {
                if(itemVM is TextMessageVM) {
                    return resourceDict["MessageViewerTextRightCell"] as DataTemplate;
                }
                else if (itemVM is ImageMessageVM) {
                    return resourceDict["MessageViewerImageRightCell"] as DataTemplate;
                }
                else if (itemVM is CodeMessageVM) {
                    return resourceDict["MessageViewerCodeRightCell"] as DataTemplate;
                }
                else if (itemVM is FileMessageVM) {
                    return resourceDict["MessageViewerFileRightCell"] as DataTemplate;
                }
            }
            throw new NotImplementedException("messageViewer模板选择器错误");
        }
    }
}
