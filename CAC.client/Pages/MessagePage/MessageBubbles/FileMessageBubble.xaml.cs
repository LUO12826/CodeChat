﻿
using System;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.Storage.Pickers;

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
            DependencyProperty.Register("Url", typeof(int), typeof(FileMessageBubble), new PropertyMetadata(-1));

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
 
        public async void Invoke(Action action, CoreDispatcherPriority Priority = CoreDispatcherPriority.Normal)
        {
            await Windows.ApplicationModel.Core.CoreApplication
                .MainView.CoreWindow.Dispatcher.RunAsync(Priority, () => { action(); });
        }

        private async void dowloadFile(StorageFolder folder, string fileName, string url)
        {

            if (isDownloading)
                return;

            isDownloading = true;

            try {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                System.Net.WebResponse response = await request.GetResponseAsync();

                System.IO.Stream ns = response.GetResponseStream();
                long totalSize = response.ContentLength;
                double hasDownSize = 0;
                byte[] nbytes = new byte[512];//521,2048 etc
                int nReadSize = 0;
                nReadSize = ns.Read(nbytes, 0, nbytes.Length);

                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync()) {

                    using (DataWriter dataWriter = new DataWriter(transaction.Stream)) {
                        while (nReadSize > 0) {
                            dataWriter.WriteBytes(nbytes);

                            nReadSize = ns.Read(nbytes, 0, 512);
                            hasDownSize += nReadSize;
                            this.Invoke(new Action(() => {
                            }));

                        }

                        transaction.Stream.Size = await dataWriter.StoreAsync();
                        await dataWriter.FlushAsync();
                        await transaction.CommitAsync();

                        this.Invoke(new Action(() => {
                            

                        }));
                        // MessageBox.Show("下载完成");

                    }
                }

            }
            catch (Exception ex) {
                // fs.Close();

            }
            isDownloading = false;
        }

    }
    
}
