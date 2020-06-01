using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    sealed partial class ContactDetailControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _IsPerson = false;
        public bool IsPerson {
            get => _IsPerson;
            set {
                _IsPerson = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsPerson)));
            }
        }

        private ContactItemViewModel _Contact;
        public ContactItemViewModel Contact {
            get => _Contact;
            set {
                _Contact = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Contact)));
            }
        }

        public static readonly DependencyProperty ContactItemProperty =
            DependencyProperty.Register("ContactItem", typeof(ContactBaseViewModel), 
                typeof(ContactDetailControl), new PropertyMetadata(null, ContactItemChanged));

        public ContactBaseViewModel ContactItem {
            get { return (ContactBaseViewModel)GetValue(ContactItemProperty); }
            set { SetValue(ContactItemProperty, value); }
        }

        private static void ContactItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContactDetailControl cd) {
                var newContact = e.NewValue as ContactBaseViewModel;
                if(newContact is ContactItemViewModel contact) {
                    cd.IsPerson = true;
                    cd.Contact = contact;
                }
                else {
                    cd.IsPerson = false;
                    cd.Contact = null;
                }
            }
        }

        public ContactDetailControl()
        {
            this.InitializeComponent();
            Messenger.Default.Register<ContactBaseViewModel>(this, "RequestOpenContactToken", RequestOpenContact);
        }

        private void RequestOpenContact(ContactBaseViewModel obj)
        {
            this.ContactItem = obj;
        }
    }
}
