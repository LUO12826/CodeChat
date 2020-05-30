using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.EventHandler
{
    /// <summary>
    /// 注册成功
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="args">注册成功参数</param>
    public delegate void RegisterSuceessEventHandler(Object sender, RegisterSuccessEventArgs args);

    /// <summary>
    /// 注册成功参数
    /// </summary>
    public class RegisterSuccessEventArgs
    {

    }
}
