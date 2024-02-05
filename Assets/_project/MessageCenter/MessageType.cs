using System;
using System.Collections;
using System.Collections.Generic;

#region MessageType
namespace LightBand
{
    interface IMessageType { }
    public struct MessageType : IMessageType
    {
        public static readonly Type type = typeof(Action);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType));
        }
    }
    public struct MessageType<T1> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1>));
        }
    }
    public struct MessageType<T1, T2> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2>));
        }
    }
    public struct MessageType<T1, T2, T3> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3>));
        }
    }
    public struct MessageType<T1, T2, T3, T4> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5, T6> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5, T6>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5, T6>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5, T6, T7> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5, T6, T7>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5, T6, T7>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5, T6, T7, T8> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5, T6, T7, T8>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5, T6, T7, T8, T9>));
        }
    }
    public struct MessageType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IMessageType
    {
        public static readonly Type type = typeof(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>);
        public MessagegId msgId;

        public MessageType(MessagegId msgId)
        {
            this.msgId = msgId;
            MessageCenter.BindType(msgId, type, typeof(MessageType<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>));
        }
    }
    #endregion
}
