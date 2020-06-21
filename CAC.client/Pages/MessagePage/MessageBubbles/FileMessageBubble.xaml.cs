using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Microsoft.Toolkit.Uwp.Helpers;

namespace CAC.client.MessagePage
{
    sealed partial class FileMessageBubble : UserControl
    {
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileMessageBubble), new PropertyMetadata(""));

        public string FileName {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }


        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(FileMessageBubble), new PropertyMetadata(null));

        public string Url {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("Url", typeof(int), typeof(FileMessageBubble), new PropertyMetadata(-1, (d, arg) => { 
                if(d is FileMessageBubble fm) {
                    int state = (int)arg.NewValue;
                    if(state == -1) {
                        fm.downloadStateBlock.Visibility = Visibility.Collapsed;
                    }
                    else if(state == 0) {
                        fm.downloadStateBlock.Visibility = Visibility.Visible;
                        fm.downloadStateBlock.Text = "正在下载";
                    }
                    else if (state == 1) {
                        fm.downloadStateBlock.Visibility = Visibility.Visible;
                        fm.downloadStateBlock.Text = "下载完成";
                    }
                    else if (state == 2) {
                        fm.downloadStateBlock.Visibility = Visibility.Visible;
                        fm.downloadStateBlock.Text = "下载失败";
                    }
                }
            }));

        public int State {
            get { return (int)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty BgColorProperty =
            DependencyProperty.Register("BgColor", typeof(Brush), typeof(FileMessageBubble),
                new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255))));
        public Brush BgColor {
            get { return (Brush)GetValue(BgColorProperty); }
            set { SetValue(BgColorProperty, value); }
        }

        public FileMessageBubble()
        {
            this.InitializeComponent();
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.CommitButtonText = "确定";
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            var folder = await picker.PickSingleFolderAsync();

            if (folder != null) {
                dowloadFile(folder, FileName, Url);
            }

            worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
        }

 
        BackgroundWorker worker;
        bool isDownloading = false;

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }
 
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {    
        }
 
        //文件下载完成时会调用此方法
        public void Invoke(Action action, CoreDispatcherPriority Priority = CoreDispatcherPriority.Normal)
        {
            action();
        }

        /// <summary>
        /// 可报告进度的下载方法。
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        private async void dowloadFile(StorageFolder folder, string fileName, string url)
        {

            if (isDownloading)
                return;

            isDownloading = true;
            State = 0;

            try {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Headers.Add("X-Tinode-APIKey", CommunicationCore.client.ApiKey);
                request.Headers.Add("X-Tinode-Auth", "Token " + CommunicationCore.client.Token);
                System.Net.WebResponse response = await request.GetResponseAsync();

                System.IO.Stream ns = response.GetResponseStream();
                long totalSize = response.ContentLength;
                double hasDownSize = 0;
                byte[] nbytes = new byte[512];
                int nReadSize = 0;
                nReadSize = ns.Read(nbytes, 0, nbytes.Length);

                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync()) {

                    using (DataWriter dataWriter = new DataWriter(transaction.Stream)) {
                        while (nReadSize > 0) {
                            dataWriter.WriteBytes(nbytes);

                            nReadSize = ns.Read(nbytes, 0, 512);
                            hasDownSize += nReadSize;
                            
                        }

                        transaction.Stream.Size = await dataWriter.StoreAsync();
                        await dataWriter.FlushAsync();
                        await transaction.CommitAsync();

                        this.Invoke(() => {
                            DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                                State = 1;
                            });
                        });
                    }
                }
            }
            catch {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => {
                    State = 2;
                });
            }
            isDownloading = false;
        }

    }
    
}
