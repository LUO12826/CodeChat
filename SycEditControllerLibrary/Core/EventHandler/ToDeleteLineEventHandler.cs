using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynEditControllerLibrary.Core.EventHandler
{
    /// <summary>
    /// 删除行委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate bool ToDeleteLineEventHandler(Object sender, ToDeleteLineEventArgs args);

    /// <summary>
    /// 删除行必要参数
    /// </summary>
    public class ToDeleteLineEventArgs
    {
        /// <summary>
        /// 目标行
        /// </summary>
        public int TargetLineIndex { get; set; }
    }
}
