using CAC.client.Common;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CAC.client.MessagePage
{
    sealed partial class ImageMessageBubble : UserControl, INotifyPropertyChanged
    {

        private bool _isLoading = false;
        public bool isLoading {
            get => _isLoading;
            set {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isLoading)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static readonly DependencyProperty BgColorProperty =
            DependencyProperty.Register("BgColor", typeof(Brush), typeof(ImageMessageBubble),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))));
        public Brush BgColor {
            get { return (Brush)GetValue(BgColorProperty); }
            set { SetValue(BgColorProperty, value); }
        }

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(string), typeof(ImageMessageBubble), new PropertyMetadata("", ImageUriChanged));
        public string ImageUri {
            get { return (string)GetValue(ImageUriProperty); }
            set { SetValue(ImageUriProperty, value); }
        }

        public static readonly DependencyProperty ImageBase64Property =
            DependencyProperty.Register("ImageBase64", typeof(string), typeof(ImageMessageBubble),
                new PropertyMetadata("", ImageBase64Changed));

        public string ImageBase64 {
            get { return (string)GetValue(ImageBase64Property); }
            set { SetValue(ImageBase64Property, value); }
        }

        private static void ImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ImageMessageBubble ib) {
                if(!(e.NewValue as string).IsNullOrEmpty()) {
                    ib.isLoading = true;
                    ib.image.Source = e.NewValue as string;
                }
            }
        }

        private static void ImageBase64Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is ImageMessageBubble ib) {
                if (!(e.NewValue as string).IsNullOrEmpty() && ib.ImageUri.IsNullOrEmpty()) {
                    ib.isLoading = true;
                    ib.image.Source = e.NewValue as string;
                }
            }
        }

        public ImageMessageBubble()
        {
            this.InitializeComponent();
            image.Loaded += Image_Loaded;
            image.ImageExFailed += Image_ImageExFailed;
        }

        private void Image_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
           
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.isLoading)
                return;
            this.isLoading = false;
            progressRing.IsActive = false;
            ProgressRingBorder.Visibility = Visibility.Collapsed;
            image.Visibility = Visibility.Visible;
        }
    }
}
