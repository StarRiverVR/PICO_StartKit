using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{

    /// <summary>
    /// 对于 Struct 需要重写 Equals() 和 GetHashCode()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataEquata<T> : IEquatable<T>
    {

    }

    //public struct KKKK : IDataEquata<KKKK>
    //{
    //    public int x;

    //    public bool Equals(KKKK other)
    //    {
    //        return x == other.x;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return x;
    //    }
    //}
}

