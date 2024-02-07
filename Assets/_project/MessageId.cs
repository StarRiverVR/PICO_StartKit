/// <summary>
/// 消息类型变量类, 所有定义的消息枚举都需要在此定义一个指定类型的变量
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Collections.ObjectModel;

using LightBand;

public enum MessagegId
{
    ShowMessage,
    ControlModeUpdate
}


public static class MessageTypes
{
    public static readonly MessageType<string> ShowMessage = new MessageType<string>(MessagegId.ShowMessage);
    public static readonly MessageType<IControlMode> ControlModeUpdate= new MessageType<IControlMode>(MessagegId.ControlModeUpdate);
}
