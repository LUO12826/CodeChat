using System.ComponentModel;

namespace CAC.client.Common
{
    /// <summary>
    /// ViewModel的基类
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A global lock for property checks so prevent locking on different instances of expressions.
        /// Considering how fast this check will always be it isn't an issue to globally lock all callers.
        /// </summary>
        protected object PropertyValueCheckLock = new object();

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// 当ViewModel中的属性变化时调用此方法。
        /// </summary>
        /// <param name="name"></param>
        public void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
}
