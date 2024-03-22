using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 被观察者
    /// </summary>
    public abstract class Subject : IDisposable
    {
        List<Observer> observers = new List<Observer>();

        List<Observer> tempobservers = new List<Observer>();

        /// <summary>
        /// 加入一个观察者
        /// </summary>
        /// <param name="observer"></param>
        public void AddObserver(Observer observer)
        {
            lock (observers)
            {
                if (observers == null) return;
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
            }
        }

        /// <summary>
        /// 移除一个观察者
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(Observer observer)
        {
            lock (observers)
            {
                if (observers==null) return;
                observers.Remove(observer);
            }
        }

        /// <summary>
        /// 数据变化时通知观察者
        /// </summary>
        /// <param name="obj">成员数据</param>
        public virtual void Notify(object obj)
        {
            lock (observers)
            {
                lock (tempobservers)
                {
                    if (observers==null || tempobservers==null) return;
                    for (int i = 0, listCount = observers.Count; i < listCount; ++i)
                    {
                        tempobservers.Add(observers[i]);
                    }
                }
            }
            lock (tempobservers)
            {
                if (tempobservers == null) return;
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("Subject.cs-->66-->Notify"));
                }
                Observer observer;
                for (int i = 0, listCount = tempobservers.Count; i < listCount; ++i)
                {
                    observer = tempobservers[i];
                    if (observer.Disposed)
                    {
                        tempobservers.RemoveAt(i);
                        i--;
                        listCount--;
                    }
                    else
                    {
                        tempobservers[i].OnNotify(obj);
                    }
                }
                tempobservers.Clear();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
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
            lock (observers)
            {
                lock (tempobservers)
                {
                    observers = null;
                    tempobservers = null;
                    disposed = true;
                }
            }
        }
    }

}

