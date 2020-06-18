using SynEditControllerLibrary.Core.Controllers;
using SynEditControllerLibrary.Core.Controllers.MessageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Interface.LineIndexInterface
{
    /// <summary>
    /// 提供行操作方法编辑器连接器接口
    /// </summary>
    interface ILineIndexEditorConnector
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize(string id, string url);

        /// <summary>
        /// 启动同步
        /// </summary>
        /// <param name="callerID"></param>
        /// <param name="rawText"></param>
        void Start(string callerID, string rawText);

        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        SynchronousController GetSynchronousController(string id,string url);

        /// <summary>
        /// 调用编辑器在指定行的指定位置添加字符
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        Boolean AddNewChar(int lineIndex, int rowIndex, char character);

        /// <summary>
        /// 调用编辑器删除指定位置的字符
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        Boolean DeleteChar(int lineIndex, int rowIndex);

        /// <summary>
        /// 要求对方使用编辑器在指定位置添加字符
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="character"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        Boolean AskAddNewChar(int lineIndex, int rowIndex, char character);

        /// <summary>
        /// 要求对方使用编辑器删除指定位置的字符
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        Boolean AskDeleteChar(int lineIndex, int rowIndex);

        /// <summary>
        /// 行后添加新行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Boolean AddNewLine(int lineIndex, string content);

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        Boolean DeleteLine(int lineIndex);

        /// <summary>
        /// 修改行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Boolean ModifyLine(int lineIndex, string content);

        /// <summary>
        /// 要求对方行后添加新行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Boolean AskAddNewLine(int lineIndex, string content);

        /// <summary>
        /// 要求对方删除行
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <returns></returns>
        Boolean AskDeleteLine(int lineIndex);

        /// <summary>
        /// 要求对方更改行内容
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Boolean AskModifyLine(int lineIndex, string content);
    }
}
