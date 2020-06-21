using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CAC.client.CodeEditorPage
{
    /// <summary>
    /// 将WrappedMonacoEditor再次进行包装，增加多tab切换的功能。
    /// </summary>
    sealed partial class CodeEditorPage : Page
    {
        public event Action AllSessionClosed;

        private static CodeEditorPage _instance = null;

        public static CodeEditorPage Default {
            get {
                if(_instance == null) {
                    _instance = new CodeEditorPage();
                }
                return _instance;
            }
        }

        private WrappedMonacoEditor codeEditor;

        private bool innerEditorLoaded = false;

        //由于MonacoEditor的加载需要一段时间，在此期间请求打开的session将被加入打开队列中。
        private Queue<CodeEditSessionInfo> waitingSessions = new Queue<CodeEditSessionInfo>();

        //用一个字典存放打开的session和tab。
        private Dictionary<CodeEditSessionInfo, muxc.TabViewItem> openedSessions 
            = new Dictionary<CodeEditSessionInfo, muxc.TabViewItem>();

        public CodeEditorPage()
        {
            this.InitializeComponent();
        }

        //如果内部的代码编辑器没有初始化，初始化代码编辑器
        //这里有个经验是，UWP中的元素如果没成为界面的可视元素，是不会进行加载的。
        //也就是“等编辑器加载完的事件发生后，再将其加入视图中”这样的做法结果是它永远加载不出来。
        private void initMonaco()
        {
            if (codeEditor != null)
                return;
            codeEditor = new WrappedMonacoEditor();
            editorPresenter.Content = codeEditor;
            codeEditor.CodeEditorLoaded += CodeEditor_CodeEditorLoaded;
        }


        private void CodeEditor_CodeEditorLoaded()
        {
            innerEditorLoaded = true;
            while (waitingSessions.TryDequeue(out var session)) {

                    handleAddSession(session);
            }
        }

        //请求开启编辑会话。
        //如果编辑会话已存在，则切换到它。
        //如果编辑会话还不存在，则加入会话集合并进行一些准备工作。
        //如果内部的编辑器还没加载好，加入等待队列。
        public void RequestOpenSession(CodeEditSessionInfo session)
        {
            if (!innerEditorLoaded) {
                initMonaco();
                waitingSessions.Enqueue(session);
            }
            else {
                handleAddSession(session);
            }
        }

        //当关闭一个会话时，需要：
        //将它的tab移除，将其从本类的字典openedSessions中移除，通知代码编辑器关闭这个会话。
        public void RequestCloseSession(CodeEditSessionInfo session)
        {
            editorTabView.TabItems.Remove(openedSessions[session]);
            openedSessions.Remove(session);
            codeEditor.CloseSession(session);
        }

        private async void editorTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //这里不延迟执行的话，在切换的一瞬间，SelectedItem会是null，导致不能正确切换。
            await Task.Delay(5);
            if (editorTabView.SelectedItem != null) {
                var session = (editorTabView.SelectedItem as muxc.TabViewItem).Tag as CodeEditSessionInfo;
                codeEditor.CurrentSession = session;
            }
        }

        //点击了关闭tab的按钮时
        private void editorTabView_TabCloseRequested(muxc.TabView sender, muxc.TabViewTabCloseRequestedEventArgs args)
        {
            RequestCloseSession(args.Tab.Tag as CodeEditSessionInfo);
            //如果最后一个也关掉了。
            if (editorTabView.TabItems.Count == 0) {
                AllSessionClosed?.Invoke();
                editorPresenter.Content = null;
            }
        }

        //判断当前编辑器中是否有某个session。哈希值是根据用户id和消息id计算的。
        private CodeEditSessionInfo hasSession(CodeEditSessionInfo info)
        {
            foreach(var session in openedSessions.Keys) {
                if(session.GetHashCode() == info.GetHashCode()) {
                    return session;
                }
            }
            return null;
        }

        private void handleAddSession(CodeEditSessionInfo session)
        {
            var temp = hasSession(session);
            if (temp != null) {
                editorTabView.SelectedItem = openedSessions[temp];
            }
            else {
                var newTab = createTabViewItem(session);
                editorTabView.TabItems.Add(newTab);
                openedSessions.Add(session, newTab);
                editorTabView.SelectedItem = newTab;
                if (editorPresenter.Content == null)
                    editorPresenter.Content = codeEditor;
            }
        }

        //测试添加session的方法。
        private int count = 0;
        private void editorTabView_AddTabButtonClick(muxc.TabView sender, object args)
        {
            var info = new CodeEditSessionInfo() {
                Contact = new ContactPage.ContactItemViewModel() {
                    UserID = count.ToString(),

                },
                CreateTime = DateTime.Now,
                Language = "csharp",
                Code = "using System;"
            };
            RequestOpenSession(info);
            count++;
        }

        //创建一个新tab。
        private muxc.TabViewItem createTabViewItem(CodeEditSessionInfo session)
        {
            var newTab = new muxc.TabViewItem() {
                Header = "与" + session.Contact.DisplayName + "聊天中的代码",
                IsClosable = true,
                IconSource = new muxc.SymbolIconSource() { Symbol = Symbol.Document },
                Content = "",
                Tag = session
            };
            return newTab;
        }
    }
}
