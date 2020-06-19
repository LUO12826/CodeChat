using CAC.client.Common;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.ObjectModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CAC.client;

namespace CAC.client.CustomControls
{


    /// <summary>
    /// 最右侧导航栏。没做成一个通用性很强的组件。
    /// </summary>
    sealed partial class Navigator : UserControl
    {
        public event EventHandler<object> OnNavigationChanged;

        private ObservableCollection<NavigatorItem> naviItem = new ObservableCollection<NavigatorItem>() {
            new NavigatorItem() {
                Symbol = Symbol.Message,
                FontSize = 20,
                Tag = "chat",
                Selected = true
            },
            new NavigatorItem() {
                Symbol = Symbol.ContactInfo,
                FontSize = 20,
                Tag = "contact",
                Selected = false
            }
        };

        private ObservableCollection<NavigatorItem> additionalItem = new ObservableCollection<NavigatorItem>() {
            new NavigatorItem() {
                Symbol = Symbol.Setting,
                FontSize = 20,
                Tag = "settings",
                Selected = false
            }
        };


        public static readonly DependencyProperty AvatarProperty =
            DependencyProperty.Register("Avatar", typeof(string), typeof(Navigator), new PropertyMetadata(""));
        public string Avatar {
            get { return (string)GetValue(AvatarProperty); }
            set { SetValue(AvatarProperty, value); }
        }

        public static readonly DependencyProperty UnreadCountProperty =
            DependencyProperty.Register("UnreadCount", typeof(int), typeof(Navigator), new PropertyMetadata(0));
        public int UnreadCount {
            get { return (int)GetValue(UnreadCountProperty); }
            set { SetValue(UnreadCountProperty, value); }
        }


        public Navigator()
        {
            this.InitializeComponent();
            this.DataContext = this;
            OnNavigationChanged += (a, b) => { };

        }

        public void SelectItem(NaviItems item)
        {
            additionItemList.SelectedItem = null;
            naviItemList.SelectedItem = null;

            switch (item) {
                case NaviItems.chat:
                    naviItemList.SelectedItem = naviItem[0];
                    OnNavigationChanged(this, naviItem[0].Tag);
                    break;
                case NaviItems.contact:
                    naviItemList.SelectedItem = naviItem[1];
                    OnNavigationChanged(this, naviItem[1].Tag);
                    break;
                case NaviItems.settings:
                    additionItemList.SelectedItem = additionalItem[0];
                    OnNavigationChanged(this, additionalItem[0].Tag);
                    break;
                default:
                    break;
            }
        }


        private void naviItemList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (sender == naviItemList) {
                additionItemList.SelectedItem = null;
            }
            else if (sender == additionItemList) {
                naviItemList.SelectedItem = null;
            }
            
            OnNavigationChanged(this, (e.ClickedItem as NavigatorItem).Tag);
        }

        private void avatar_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            naviItemList.SelectedItem = null;
            additionItemList.SelectedItem = null;
            OnNavigationChanged(this, (sender as ImageEx).Tag);
        }

        private void naviItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clearSelection();
            var sel = (sender as ListView).SelectedItem;
            if(sel != null) {
                (sel as NavigatorItem).Selected = true;
            }
        }

        private void additionItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            clearSelection();
            var sel = (sender as ListView).SelectedItem;
            if (sel != null) {
                (sel as NavigatorItem).Selected = true;
            }
        }

        //清除选中标记。
        private void clearSelection()
        {
            foreach (var item in naviItem) {
                item.Selected = false;
            }
            foreach (var item in additionalItem) {
                item.Selected = false;
            }
        }
    }

    class NavigatorItem: BaseViewModel
    {
        private Symbol _Symbol;
        private int _FontSize;
        private string _Tag;
        private bool _Selected;

        public Symbol Symbol {
            get => _Symbol;
            set {
                _Symbol = value;
                RaisePropertyChanged(nameof(Symbol));
            }
        }

        public int FontSize {
            get => _FontSize;
            set {
                _FontSize = value;
                RaisePropertyChanged(nameof(FontSize));
            }
        }

        public string Tag {
            get => _Tag;
            set {
                _Tag = value;
                RaisePropertyChanged(nameof(Tag));
            }
        }

        //界面根据此属性显示选中标记。
        public bool Selected {
            get => _Selected;
            set {
                _Selected = value;
                RaisePropertyChanged(nameof(Selected));
            }
        }
    }
}
