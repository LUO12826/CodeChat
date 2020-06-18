using SynEditControllerLibrary.Core.Entities;
using SynEditControllerLibrary.Core.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Search;

namespace SynEditControllerLibrary.Core.Controllers.MessageManager
{
    /// <summary>
    /// 处理收到的Message
    /// </summary>
    public static class MessageAnalyst
    {
        /// <summary>
        /// 解析处理收到的消息
        /// </summary>
        /// <param name="msg"></param>
        public static void DoAnalyse(Message msg, SynchronousController sc)
        {
            switch (msg.Type)
            {
                case MessageType.ADD:
                    DoAdd(msg.CallerID, msg.LineHash,msg.Detail, sc);
                    break;
                case MessageType.DEL:
                    DoDel(msg.CallerID, msg.LineHash, sc);
                    break;
                case MessageType.UPD:
                    DoUpd(msg.CallerID, msg.LineHash, msg.Detail, sc);
                    break;
                case MessageType.APPLY:
                    DoApply(sc);
                    break;
                case MessageType.INI:
                    DoIni(msg.LineHash, msg.Detail, sc);
                    break;
                case MessageType.JOIN:
                    DoJoin(msg.Detail, sc);
                    break;
                case MessageType.VRF:
                    ;
                    break;
                case MessageType.END:
                    ;
                    break;
            }
        }

        /// <summary>
        /// 处理ADD消息
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="lineHash"></param>
        /// <param name="content"></param>
        /// <param name="sc"></param>
        public static void DoAdd(string callerID, int lineHash, string content, SynchronousController sc)
        {
            TextLine targetLine = sc.TextDoc.GetLineByHash(lineHash);
            TextLine newLine = new TextLine(content, lineHash);
            if (targetLine == null)
                throw new Exception("不存在的lineHash值");
            if (targetLine.NewLines.Count > 0)
            {
                TextLine oldLine = targetLine.NewLines.Dequeue();
                if (oldLine.ID < lineHash)
                {
                    TextLine positionLine = sc.TextDoc.ToAddNewLineAfterLine(newLine, oldLine);
                    int position = sc.TextDoc.GetIndexByHash(positionLine.ID);
                    sc.ToAddNewLine(position, content);
                }
                else if (oldLine.ID > lineHash)
                {
                    int positon = sc.TextDoc.GetIndexByHash(oldLine.ID) - 1;
                    sc.ToAddNewLine(positon, content);
                    sc.TextDoc.ToAddNewLineBeforeLine(newLine, oldLine);
                }
            }

            sc.MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(callerID, sc.Identity, MessageType.VRF, lineHash, "ADD"));
        }

        /// <summary>
        /// 处理DEL消息
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="lineHash"></param>
        /// <param name="sc"></param>
        public static void DoDel(string callerID, int lineHash, SynchronousController sc)
        {
            TextDoc textDoc = sc.TextDoc;
            int index = textDoc.GetIndexByHash(lineHash);
            textDoc.DeleteLine(textDoc.GetLineByHash(lineHash));
            sc.ToDeleteLine(index);
            sc.MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(callerID, sc.Identity, MessageType.VRF, lineHash, "DEL"));
        }

        /// <summary>
        /// 处理UPD消息
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="lineHash"></param>
        /// <param name="content"></param>
        /// <param name="sc"></param>
        public static void DoUpd(string callerID, int lineHash, string content, SynchronousController sc)
        {
            TextLine targetLine = sc.TextDoc.GetLineByHash(lineHash);
            if (targetLine.Mark == LineMarkType.Changed)
            {
                //自己的修改消息会先到达对面，无需发确认消息对面也会知道冲突存在
                //TODO:调用消息告知发生冲突的位置和对方文本，由编辑器显示(建议在对应行的下方以另一种颜色显示)
                return;
            }
            if (targetLine.Mark == LineMarkType.Deleted)
            {
                //由于删除优先级最高，故对方的该行也将被删除，不必回复确认消息
                return;
            }
            if (targetLine.Mark == LineMarkType.UnChanged)
            {
                targetLine.EditContent(content);
                int index = sc.TextDoc.GetIndexByHash(lineHash);
                sc.ToModifyLine(index, content);
                sc.MessageQueues.MessagesToSend.Enqueue(MessageWrapper.WriteMsg(callerID, sc.Identity, MessageType.VRF, lineHash, "UPD" + content));
                return;
            }
        }

        /// <summary>
        /// 处理APPLY消息
        /// </summary>
        /// <param name="sc"></param>
        public static void DoApply(SynchronousController sc)
        {
            sc.Connected();
        }

        /// <summary>
        /// 处理INI消息
        /// </summary>
        /// <param name="lineHash"></param>
        /// <param name="content"></param>
        /// <param name="sc"></param>
        public static void DoIni(int lineHash, string content, SynchronousController sc)
        {
            sc.ToAddNewLine(lineHash, content);
        }

        /// <summary>
        /// 处理Join消息
        /// </summary>
        /// <param name="responderID"></param>
        /// <param name="sc"></param>
        public static void DoJoin(string responderID, SynchronousController sc)
        {
            sc.GetConnected(responderID);
        }

        /// <summary>
        /// 处理VRF消息
        /// </summary>
        /// <param name="lineHash"></param>
        /// <param name="detail"></param>
        public static void DoVrf(int lineHash, string detail, SynchronousController sc)
        {
            if (detail.StartsWith("ADD"))
            {
                sc.TextDoc.GetLineByHash(lineHash).NewLines.Dequeue();
                return;
            }
            if (detail.StartsWith("DEL"))
            {
                TextLine targetLine = sc.TextDoc.GetLineByHash(lineHash);
                sc.TextDoc.ConfirmDeleteLine(targetLine);
                return;
            }
            if (detail.StartsWith("UPD"))
            {
                detail.Remove(0, 3);
                if (sc.TextDoc.GetLineByHash(lineHash).GetContent() == detail)
                {
                    sc.TextDoc.GetLineByHash(lineHash).Mark = LineMarkType.UnChanged;
                }
                return;
            }
        }

        /// <summary>
        /// 处理END消息
        /// </summary>
        /// <param name="sc"></param>
        public static void DoEnd(SynchronousController sc)
        {
            sc.EndConnection();
        }
    }
}
