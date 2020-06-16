using System;
using System.Collections.Generic;
using System.Text;

namespace CodeChatSDK.SDKClient
{
    /// <summary>
    /// 回调类型
    /// </summary>
    public enum CallbackType
    {
        Unknown,
        Hi,
        Acc,
        Login,
        Sub,
        Get,
        Set,
        Pub,
        Note,
        Leave,
        DelTopic,
        DelMsg,
    }
}
