using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CAC.client.ContactPage
{
    class ContactListItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ContactListContactTemplate { get; set; }
        public DataTemplate ContactListGroupChatTemplate { get; set; }
        public DataTemplate ContactListGroupHeaderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ContactGroupViewModel) {
                return ContactListGroupHeaderTemplate;
            }
            else {
                if(item is ContactItemViewModel) {
                    return ContactListContactTemplate;
                }
                else {
                    throw new NotImplementedException();
                }
            }

            throw new NotImplementedException("ChatList模板选择器错误");
        }
    }
}
