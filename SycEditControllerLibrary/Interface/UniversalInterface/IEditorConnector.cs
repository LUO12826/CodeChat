using SynEditControllerLibrary.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Interface.UniversalInterface
{
    /// <summary>
    /// 编辑器通用连接器接口
    /// </summary>
    interface IEditorConnector
    {
        /// <summary>
        /// 获取控制器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        SynchronousController GetSynchronousController(object obj);

        /// <summary>
        /// 调用编辑器在指定位置添加字符
        /// </summary>
        /// <param name="index"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        Boolean AddNewChar(int index,char character);

        /// <summary>
        /// 调用编辑器删除指定位置的字符
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Boolean DeleteChar(int index);

        /// <summary>
        /// 要求对方使用编辑器在指定位置添加字符
        /// </summary>
        /// <param name="index"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        Boolean AskAddNewChar(int index, char character);

        /// <summary>
        /// 要求对方使用编辑器删除指定位置的字符
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Boolean AskDeleteChar(int index);
    }
}
