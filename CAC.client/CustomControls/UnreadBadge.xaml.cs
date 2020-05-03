using System;
using System.Collections.Generic;
using Windows.UI;
using CAC.client;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Diagnostics;

namespace CAC.client.CustomControls
{
    /// <summary>
    /// 未读消息角标
    /// </summary>
    sealed partial class UnreadBadge : UserControl
    {
        #region 依赖属性
        public static readonly DependencyProperty UnreadCountProperty =
            DependencyProperty.Register("UnreadCount", typeof(int), typeof(UnreadBadge), new PropertyMetadata(0));
        public int UnreadCount {
            get { return (int)GetValue(UnreadCountProperty); }
            set { SetValue(UnreadCountProperty, value); }
        }

        public static readonly DependencyProperty CommonBgColorProperty =
            DependencyProperty.Register("CommonBgColor", typeof(Color), typeof(UnreadBadge), new PropertyMetadata(Color.FromArgb(255, 238, 58, 48)));
        public Color CommonBgColor {
            get { return (Color)GetValue(CommonBgColorProperty); }
            set { SetValue(CommonBgColorProperty, value); }
        }

        public static readonly DependencyProperty MuteBgColorProperty =
            DependencyProperty.Register("MuteBgColor", typeof(Color), typeof(UnreadBadge), new PropertyMetadata(Color.FromArgb(255, 189, 189, 189)));
        public Color MuteBgColor {
            get { return (Color)GetValue(MuteBgColorProperty); }
            set { SetValue(MuteBgColorProperty, value); }
        }

        public static readonly DependencyProperty NotificationTypeProperty =
            DependencyProperty.Register("NotificationType", typeof(NotificationType), typeof(UnreadBadge), new PropertyMetadata(NotificationType.normal, NotificationTypeChanged));
        public NotificationType NotificationType {
            get { return (NotificationType)GetValue(NotificationTypeProperty); }
            set { SetValue(NotificationTypeProperty, value); }
        }

        private static void NotificationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is UnreadBadge ub) {
                if((NotificationType)e.NewValue == NotificationType.normal) {
                    ub.border.Background = new SolidColorBrush(ub.CommonBgColor);
                }
                else {
                    ub.border.Background = new SolidColorBrush(ub.MuteBgColor);
                }
            }
        }

        #endregion
        public UnreadBadge()
        {
            this.InitializeComponent();
        }
    }
}
