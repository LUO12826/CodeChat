using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace CAC.client.Common
{

    /// <summary>
    /// 支持增量加载的可通知变更的集合。
    /// 需要更多项目时，会通知它的代理。初始化时必须为其设置代理。
    /// </summary>
    class IncrementalCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public event Action<int> OnLoadMoreStarted;
        public event Action<int> OnLoadMoreCompleted;

        protected bool _busy = false;
        public bool HasMoreItems { get; private set; }

        //用于加载更多项的代理。传出需要加载的项目数，传回加载的项目和指示是否还有更多项目的布尔值。
        Func<uint, Task<Tuple<List<T>, bool>>> _dataFetchDelegate = null;

        public IncrementalCollection(Func<uint, Task<Tuple<List<T>, bool>>> dataFetchDelegate)
        {
            this._dataFetchDelegate = dataFetchDelegate;
            HasMoreItems = true;
        }

        
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy) {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }
            _busy = true;

            //将常用的task型异步转换成此方法的特殊型异步
            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        protected async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            if (!HasMoreItems)
                return new LoadMoreItemsResult { Count = 0 };

            if (_dataFetchDelegate == null)
                throw new NotImplementedException("IncrementalCollection's delegate is null");

            try {
                OnLoadMoreStarted?.Invoke((int)count);

                //忽略了CancellationToken
                var result = await this._dataFetchDelegate(count);

                var items = result.Item1;
                if (items != null) {
                    foreach (var item in items) {
                        this.Add(item);
                    }
                }

                // 是否还有更多
                this.HasMoreItems = result.Item2;

                // 加载完成事件
                OnLoadMoreCompleted?.Invoke(items == null ? 0 : items.Count);

                return new LoadMoreItemsResult { Count = items == null ? 0 : (uint)items.Count };
            }
            finally {
                _busy = false;
            }
        }

    }

}
