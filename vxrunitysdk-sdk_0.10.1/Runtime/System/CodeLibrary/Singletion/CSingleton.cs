using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 所有单例的基类，免得每个单例都实现一次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSingleton<T> where T : CSingleton<T>, System.IDisposable, new()
    {
        protected static T _instance = null;

        protected static object lockObj = new object();

        static int count;

        /// <summary>
        /// 统计有多少单例
        /// </summary>
        public static int Count
        {
            get
            {
                lock (lockObj)
                {
                    return count;
                }
            }
        }

        /// <summary>
        ///  获取单例
        /// </summary>
        /// 
        public static T Instance
        {
            get
            {

                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance!=null)
                        {
                            return _instance;
                        }
                        count++;
                        _instance = new T();
                        _instance.Init();
                        if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((obj) =>
                            {
                                if (Application.isPlaying)
                                {
                                    CSingletonManager.Instance.AddReleaseFun(_instance.Release);
                                }
                            }, null);
                        }
                        else
                        {
                            if (Application.isPlaying)
                            {
                                CSingletonManager.Instance.AddReleaseFun(_instance.Release);
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        bool isInit = false;

        public bool IsInit
        {
            get
            {
                lock (lockObj)
                {
                    return isInit;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isInit = value;
                }
            }
        }

        private void Init()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("CSingleton.cs-->103-->Init-->{0}", typeof(T)));
                }
                if (IsInit)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                } 
                IsInit = true;
                InitFun();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        protected virtual void InitFun()
        {

        }

        bool isRelease = false;

        public bool IsRelease
        {
            get
            {
                lock (lockObj)
                {
                    return isRelease;
                }
            }
            set
            {
                lock (lockObj)
                {
                    isRelease = value;
                }
            }
        }

        /// <summary>
        /// 会在Unity销毁时强制调用
        /// </summary>
        public void Release()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("CSingleton.cs-->143-->Release-->{0}", typeof(T)));
                }
                if (IsRelease)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                }
                IsRelease = true;
                count--;
                _instance.Dispose();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                _instance = null;
            }
        }
    }

}


