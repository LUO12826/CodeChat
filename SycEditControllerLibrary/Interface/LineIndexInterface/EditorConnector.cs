using SynEditControllerLibrary.Core.Controllers;
using SynEditControllerLibrary.Core.EventHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Interface.LineIndexInterface
{
    /// <summary>
    /// 连接器类
    /// </summary>
    class EditorConnector : ILineIndexEditorConnector
    {
        /// <summary>
        /// 控制器
        /// </summary>
        SynchronousController SController = null;

        public bool AddNewChar(int lineIndex, int rowIndex, char character)
        {
            throw new NotImplementedException();
        }

        public bool AddNewLine(int lineIndex, string content)
        {
            throw new NotImplementedException();
        }

        public bool AskAddNewChar(int lineIndex, int rowIndex, char character)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 编辑器添加新行时调用
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AskAddNewLine(int lineIndex, string content)
        {
            return SController.AskAddNewLine(lineIndex, content);
        }

        public bool AskDeleteChar(int lineIndex, int rowIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 编辑器删除行时调用
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        public bool AskDeleteLine(int lineIndex)
        {
            return SController.AskDeleteLine(lineIndex);
        }

        /// <summary>
        /// 编辑器修改行时调用
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AskModifyLine(int lineIndex, string content)
        {
            return SController.AskModifyNewLine(lineIndex, content);
        }

        public bool DeleteChar(int lineIndex, int rowIndex)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLine(int lineIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public SynchronousController GetSynchronousController(string id, string url)
        {
            SController = new SynchronousController(id, url);
            return SController;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        public void Initialize(string id, string url)
        {
            GetSynchronousController(id, url); 
            SController.OnToAddNewLine += new ToAddNewLineEventHandler(ToAddNewLineEvent);
            SController.OnToDeleteLine += new ToDeleteLineEventHandler(ToDeleteLineEvent);
            SController.OnToModifyLine += new ToModifyLineEventHandler(ToModifyLineEvent);
            SController.OnGetConnected += new GetConnectedEventHandler(GetConnectedEvent);
            SController.OnConnected += new ConnectedEventHandler(ConnectedEvent);
            SController.OnEndConnection += new EndConnectionEventHandler(EndConnectionEvent);
        }

        /// <summary>
        /// 通知编辑器对方已经断开连接
        /// </summary>
        /// <param name="sender"></param>
        public void EndConnectionEvent(object sender)
        {
            //TODO:编辑器作出相应的响应
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通知编辑器已经成功连接到一个已存在的同步session
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private void ConnectedEvent(object sender)
        {
            //TODO:编辑器作出相应的响应
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通知编辑器收到连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private void GetConnectedEvent(object sender, GetConnectedEventArgs args)
        {
            //TODO:编辑器作出相应的响应
            throw new NotImplementedException();
        }

        /// <summary>
        /// 调用编辑器方法在指定位置添加新行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ToAddNewLineEvent(object sender, ToAddNewLineEventArgs args)
        {
            //TODO:调用编辑器方法在指定位置添加新行
            throw new NotImplementedException();
        }

        /// <summary>
        /// 调用编辑器方法删除指定位置的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ToDeleteLineEvent(object sender, ToDeleteLineEventArgs args)
        {
            //TODO:调用编辑器方法删除指定位置的行
            throw new NotImplementedException();
        }

        /// <summary>
        /// 调用编辑器方法修改指定位置的行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool ToModifyLineEvent(object sender, ToModifyLineEventArgs args)
        {
            //TODO:调用编辑器方法修改指定位置的行
            throw new NotImplementedException();
        }

        /// <summary>
        /// 开始同步
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="iniText"></param>
        public void Start(string callerID,string iniText = null)
        {
            if (callerID == SController.UserID)
            {
                SController.Start(true, rawText:iniText);
            }
            else
            {
                SController.Start(false, callerID: callerID);
            }
        }

        public bool ModifyLine(int lineIndex, string content)
        {
            throw new NotImplementedException();
        }
    }
}
