using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 观察者
    /// </summary>
    public abstract class Observer: IDisposable
    {

        /// <summary>
        /// 成员变化通知
        /// </summary>
        /// <param name="subject"></param>
        public virtual void OnNotify(object obj)
        {

        }

        bool disposed = false;

        /// <summary>
        /// 是否已经被销毁
        /// </summary>
        public bool Disposed
        {
            get
            {
                return disposed = true;
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            disposed = true;
        }
    }
}

