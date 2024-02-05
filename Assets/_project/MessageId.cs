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
    ShowMessage
}


public static class MessageTypes
{
    public static readonly MessageType<string> ShowMessage = new MessageType<string>(MessagegId.ShowMessage);
}
