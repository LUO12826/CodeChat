using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynEditControllerLibrary.Core.EventHandler
{
    /// <summary>
    /// 添加新行委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate bool AddNewLineEventHandler(Object sender, AddNewLineEventArgs args);

    /// <summary>
    /// 添加新行必要参数
    /// </summary>
    public class AddNewLineEventArgs
    {
        /// <summary>
        /// 目标行
        /// </summary>
        public int TargetLineIndex  { get; set; }

        /// <summary>
        /// 新增行
        /// </summary>
        public string NewLineContent { get; set; }
    }
}
