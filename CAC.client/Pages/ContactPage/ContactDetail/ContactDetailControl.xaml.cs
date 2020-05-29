using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CAC.client.ContactPage
{
    sealed partial class ContactDetailControl : UserControl
    {

        private bool isGroupContact;
        public static readonly DependencyProperty ContactItemProperty =
            DependencyProperty.Register("ContactItem", typeof(ContactBaseViewModel), typeof(ContactDetailControl), new PropertyMetadata(null, ContactItemChanged));
        public ContactBaseViewModel ContactItem {
            get { return (ContactBaseViewModel)GetValue(ContactItemProperty); }
            set { SetValue(ContactItemProperty, value); }
        }

        private static void ContactItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactDetailControl cd) {
                
            }
        }

        public ContactDetailControl()
        {
            this.InitializeComponent();
        }
    }
}
