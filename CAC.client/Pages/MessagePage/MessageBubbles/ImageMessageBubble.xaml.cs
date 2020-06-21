using CAC.client.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Net.Cache;
using Brush = Windows.UI.Xaml.Media.Brush;

namespace CAC.client.MessagePage
{
    sealed partial class ImageMessageBubble : UserControl, INotifyPropertyChanged
    {

        private bool _isLoading = false;
        private bool isDownloading = false;

        public bool isLoading {
            get => _isLoading;
            set {
                _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(isLoading)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region 依赖属性
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

        #endregion

        private static async void ImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ImageMessageBubble ib) {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    ib.isLoading = false;
                });

                if (!(e.NewValue as string).IsNullOrEmpty()) {

                    var bitmap = await ib.dowloadImage(e.NewValue as string);
                    if(bitmap != null) {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                            ib.image.Source = bitmap;  
                        });
                    }
                    //图片加载失败，显示加载失败图
                    else {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                            ib.loadFailedBorder.Visibility = Visibility.Visible;
                        });
                    }
                }
            }
        }

        private static void ImageBase64Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is ImageMessageBubble ib) {
                if (!(e.NewValue as string).IsNullOrEmpty() && ib.ImageUri.IsNullOrEmpty()) {
                    DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                        ib.image.Source = e.NewValue as string;
                    });
                }
            }
        }

        public ImageMessageBubble()
        {
            this.InitializeComponent();
            image.ImageExOpened += Image_ImageExOpened;
            image.ImageExFailed += Image_ImageExFailed;
        }

        private void Image_ImageExOpened(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExOpenedEventArgs e)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                isLoading = false;
            });
        }

        private void Image_ImageExFailed(object sender, Microsoft.Toolkit.Uwp.UI.Controls.ImageExFailedEventArgs e)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                loadFailedBorder.Visibility = Visibility.Visible;
            });
        }


        //下载图片。
        private async Task<BitmapImage> dowloadImage(string url)
        {

            if (isDownloading)
                return null;

            isDownloading = true;
            try {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                //系统缓存
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);

                request.Headers.Add("X-Tinode-APIKey", CommunicationCore.client.ApiKey);
                request.Headers.Add("X-Tinode-Auth", "Token " + CommunicationCore.client.Token);
                System.Net.WebResponse response = await request.GetResponseAsync();
                
                //这里存在一个将返回信息流转换为内存流的问题，参见msdn文档。
                Stream stream = response.GetResponseStream();
                // Create a .NET memory stream.
                var memStream = new MemoryStream();
                // Convert the stream to the memory stream, because a memory stream supports seeking.
                await stream.CopyToAsync(memStream);
                // Set the start position.
                memStream.Position = 0;
                var bitmap = new BitmapImage();
                bitmap.SetSource(memStream.AsRandomAccessStream());

                return bitmap;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return null;

            }
            finally {
                isDownloading = false;
            }
            
        }

        ~ImageMessageBubble()
        {
            image.ImageExOpened -= Image_ImageExOpened;
            image.ImageExFailed -= Image_ImageExFailed;
        }
    }
}
