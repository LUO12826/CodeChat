using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SynEditControllerLibrary.Core.Global
{
    /// <summary>
    /// 一次同步中的身份立场
    /// </summary>
    [DataContract]
    public enum Identity 
    { 
        //未确定
        Unknown,
        //发起方
        Organiger,
        //相应方
        Responder 
    }

    /// <summary>
    /// 行脏位标记类型
    /// </summary>
    public enum LineMarkType
    {
        //未更改
        UnChanged = 0,
        //已更改
        Changed = 1,
        //已删除
        Deleted = 2,
        //不可更改
        Unchangeable = 3,
        //行链表头
        Head = 4
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    [DataContract]
    public enum MessageType
    {
        //申请
        APPLY,
        //加入
        JOIN, 
        //初始化
        INI,
        //添加
        ADD,
        //删除
        DEL,
        //更新
        UPD,
        //结束
        END,
        //确认
        VRF
    }
}
