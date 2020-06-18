using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynEditControllerLibrary.Core.Controllers.MessageManager;
using SynEditControllerLibrary.Core.Entities;
using SynEditControllerLibrary.Core.EventHandler;
using SynEditControllerLibrary.Core.Global;
using Windows.System;

namespace SynEditControllerLibrary.Core.Controllers
{
    /// <summary>
    /// 同步控制类
    /// </summary>
    public class SynchronousController
    {
        /// <summary>
        /// 增加新行事件
        /// </summary>
        public event ToAddNewLineEventHandler OnToAddNewLine;

        /// <summary>
        /// 删除行事件
        /// </summary>
        public event ToDeleteLineEventHandler OnToDeleteLine;

        /// <summary>
        /// 编辑行事件
        /// </summary>
        public event ToModifyLineEventHandler OnToModifyLine;

        /// <summary>
        /// 收到连接事件
        /// </summary>
        public event GetConnectedEventHandler OnGetConnected;

        /// <summary>
        /// 成功建立连接事件
        /// </summary>
        public event ConnectedEventHandler OnConnected;

        /// <summary>
        /// 收到结束会话请求事件
        /// </summary>
        public event EndConnectionEventHandler OnEndConnection;

        /// <summary>
        /// 服务器资源地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 维护的文本组织
        /// </summary>
        public TextDoc TextDoc { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 发起人ID
        /// </summary>
        string CallerID { get; set; }

        /// <summary>
        /// 一次连接中的身份
        /// </summary>
        public Identity Identity { get; set; }

        /// <summary>
        /// 消息队列
        /// </summary>
        public MessageHolder MessageQueues { get; }

        /// <summary>
        /// 监视器
        /// </summary>
        MessageWatcher Watcher { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="url"></param>
        public SynchronousController(string userID, string url)
        {
            TextDoc = new TextDoc();
            this.Url = url;
            MessageQueues = new MessageHolder();
            Watcher = new MessageWatcher(MessageQueues, url);
            UserID = userID;
        }

        /// <summary>
        /// 启动同步，如果isOrganiger为真
        /// </summary>
        /// <param name="isOrganiger">是否为发起方</param>
        /// <param name="callerID">发起方ID(非发起方必须指定)</param>
        /// <param name="rawText">初始文本(发起方可以选择是否指定)</param>
        /// <returns>启动成功则返回真</returns>
        public Boolean Start(Boolean isOrganiger, String callerID = null, String rawText = null)
        {
            bool flag;
            if (isOrganiger)
            {
                CallerID = UserID;
                Identity = Identity.Organiger;
                if (rawText != null)
                {
                    flag = ApplySession(rawText);
                }
                else
                {
                    flag = ApplySession();
                }
                if (flag)
                    Watcher.StartWatch(this);
                return flag;
            }
            else
            {
                CallerID = callerID;
                if (callerID == null)
                    return false;
                Identity = Identity.Responder;
                flag = JoinSession(UserID);
                if (flag)
                    Watcher.StartWatch(this);
                return flag;
            }
        }

        /// <summary>
        /// 向服务器申请一个同步session，初始文本为空
        /// </summary>
        /// <returns></returns>
        private Boolean ApplySession()
        {
            Queue<Message> applyMsgs = new Queue<Message>();

            Message applyMsg = MessageWrapper.ApplyMsg(CallerID);
            applyMsgs.Enqueue(applyMsg);

            Boolean isDone = HttpHelper.RequestForIniMsg(Url, applyMsgs, MessageQueues);
            return isDone;
        }

        /// <summary>
        /// 向服务器申请一个同步session，并上传初始文本
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns>如果成功则返回真</returns>
        private Boolean ApplySession(string rawText)
        {
            TextDoc rawDoc = new TextDoc(rawText);
            Queue<Message> applyMsgs = new Queue<Message>();

            foreach (TextLine textLine in rawDoc.TextLines)
            {
                if (textLine.Mark != LineMarkType.Head)
                {
                    Message msg = MessageManager.MessageWrapper.IniMsg(CallerID, textLine.ID, textLine.GetContent());
                    applyMsgs.Enqueue(msg);
                }
            }
            Message applyMsg = MessageManager.MessageWrapper.ApplyMsg(CallerID);
            applyMsgs.Enqueue(applyMsg);

            Boolean isDone = HttpHelper.RequestForIniMsg(Url, applyMsgs, MessageQueues);
            return isDone;
        }

        /// <summary>
        /// 加入服务器的一个同步session
        /// </summary>
        /// <param name="callerID"></param>
        /// <returns></returns>
        public Boolean JoinSession(string responderID)
        {
            Queue<Message> initMsgs = new Queue<Message>();

            Message msg = MessageManager.MessageWrapper.JoinMsg(CallerID, responderID);
            initMsgs.Enqueue(msg);

            Boolean isDone = HttpHelper.RequestForIniMsg(Url, initMsgs, MessageQueues);
            return isDone;
        }

        /// <summary>
        /// 告知控制器在编辑器有新增的行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AskAddNewLine(int lineIndex, String content)
        {
            AddNewLineEventArgs args = new AddNewLineEventArgs()
            {
                TargetLineIndex = lineIndex,
                NewLineContent = content
            };
            return AddNewLineEvent(this, args);
        }

        /// <summary>
        /// 告知控制器在编辑器有删除的行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public bool AskDeleteLine(int lineIndex)
        {
            DeleteLineEventArgs args = new DeleteLineEventArgs()
            {
                TargetLineIndex = lineIndex,
            };
            return DeleteLineEvent(this, args);
        }

        /// <summary>
        /// 告知控制器在编辑器有修改的行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AskModifyNewLine(int lineIndex, String content)
        {
            ModifyLineEventArgs args = new ModifyLineEventArgs()
            {
                TargetLineIndex = lineIndex,
                NewContent = content
            };
            return ModifyLineEvent(this, args);
        }

        /// <summary>
        /// 添加新行事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool AddNewLineEvent(object sender, AddNewLineEventArgs e)
        {
            if (this.TextDoc.InsertLine(TextDoc.GetTextLineByIndex(e.TargetLineIndex), out TextLine newLine))
            {
                newLine.EditContent(e.NewLineContent);
                MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(this.CallerID, this.Identity, MessageType.ADD, newLine.ID, e.NewLineContent));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 删除行事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool DeleteLineEvent(object sender, DeleteLineEventArgs e)
        {
            TextLine targetLine = TextDoc.GetTextLineByIndex(e.TargetLineIndex);
            if (targetLine == null)
                return false;
            if (this.TextDoc.MarkDeleteLine(targetLine))
            {
                MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(this.CallerID, this.Identity, MessageType.DEL, targetLine.ID, null));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 编辑行事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        bool ModifyLineEvent(object sender, ModifyLineEventArgs e)
        {
            if (TextDoc.GetLineCounts() > e.TargetLineIndex || e.TargetLineIndex <= 0)
                return false;
            else
            {
                TextLine targetLine = this.TextDoc.GetTextLineByIndex(e.TargetLineIndex);
                if (targetLine == null)
                    return false;
                targetLine.EditContent(e.NewContent);
                targetLine.Mark = LineMarkType.Changed;
                MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(this.CallerID, this.Identity, MessageType.UPD, targetLine.ID, e.NewContent));
                return true;
            }
        }

        /// <summary>
        /// 触发OnToAddNewLine
        /// </summary>
        /// <param name="index"></param>
        /// <param name="content"></param>
        public void ToAddNewLine(int index, string content)
        {
            ToAddNewLineEventArgs args = new ToAddNewLineEventArgs()
            {
                TargetLineIndex = index,
                NewLineContent = content
            };
            OnToAddNewLine(this, args);
        }

        /// <summary>
        /// 触发OnToDeleteLine
        /// </summary>
        /// <param name="index"></param>
        public void ToDeleteLine(int index)
        {
            ToDeleteLineEventArgs args = new ToDeleteLineEventArgs()
            {
                TargetLineIndex = index
            };
            OnToDeleteLine(this, args);
        }

        /// <summary>
        /// 触发OnToModifyLine
        /// </summary>
        /// <param name="index"></param>
        /// <param name="content"></param>
        public void ToModifyLine(int index, string content)
        {
            ToModifyLineEventArgs args = new ToModifyLineEventArgs()
            {
                TargetLineIndex = index,
                NewContent = content
            };
            OnToModifyLine(this, args);
        }

        /// <summary>
        /// 触发收到连接
        /// </summary>
        /// <param name="responderID"></param>
        public void GetConnected(string responderID)
        {
            GetConnectedEventArgs args = new GetConnectedEventArgs()
            {
                ResponderID = responderID,
            };
            OnGetConnected(this, args);
        }

        /// <summary>
        /// 触发已经建立连接
        /// </summary>
        public void Connected()
        {
            OnConnected(this);
        }

        /// <summary>
        /// 触发结束会话
        /// </summary>
        public void EndConnection()
        {
            OnEndConnection(this);
        }
    }
}