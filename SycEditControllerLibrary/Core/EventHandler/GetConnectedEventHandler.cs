using SynEditControllerLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SynEditControllerLibrary.Core.EventHandler
{
    /// <summary>
    /// 收到连接委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void GetConnectedEventHandler(Object sender, GetConnectedEventArgs args);

    /// <summary>
    /// 收到连接参数
    /// </summary>
    public class GetConnectedEventArgs
    {
        /// <summary>
        /// 对方的ID
        /// </summary>
        public string ResponderID{ get; set; }
    }
}
