using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynEditControllerLibrary.Core.EventHandler
{
    /// <summary>
    /// 修改行委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate bool ModifyLineEventHandler(Object sender, ModifyLineEventArgs args);

    /// <summary>
    /// 修改行行必要参数
    /// </summary>
    public class ModifyLineEventArgs
    {
        /// <summary>
        /// 目标行
        /// </summary>
        public int TargetLineIndex { get; set; }

        /// <summary>
        /// 修改后内容
        /// </summary>
        public string NewContent { get; set; }
    }
}