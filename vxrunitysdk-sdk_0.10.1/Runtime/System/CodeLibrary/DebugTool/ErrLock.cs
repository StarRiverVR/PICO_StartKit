using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 死锁判断
    /// 遇到程序死锁卡死的情况，开启ErrLockOpen，卡死后等待 WaitTime 豪秒左右 Application.dataPath + "/../死锁.txt";
    /// lock(obj){
    ///      ErrLockData data = LockStart(string info);
    ///      .........
    ///      LockEnd(data)
    /// }
    /// </summary>
    public class ErrLock
    {
        /// <summary>
        /// 是否开启死锁检测
        /// </summary>
        public static bool ErrLockOpen = false;

        /// <summary>
        /// 毫秒
        /// </summary>
        public const int WaitTime = 10000;

        public static ErrLockData LockStart(string info)
        {
            if (!ErrLockOpen) return null;
            ErrLockData errLockData = ErrLockData.errLockDataPool.Spawn();
            errLockData.Info = info;
            return errLockData;
        }

        public static void LockEnd(ErrLockData data)
        {
            if (data==null) return;
            data.Close = true;
        }
    }

    public class ErrLockData : ISimplePoolData
    {
        public static SimplePool<ErrLockData> errLockDataPool = new SimplePool<ErrLockData>();

        object lockObj = new object();

        static string savePath;

        static object pathLock = new object();

        static string SavePath
        {
            get
            {
                if (savePath==null)
                {
                    lock (pathLock)
                    {
                        if (savePath!=null)
                        {
                            return savePath;
                        }
                        if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((obj) => {
                                savePath = Application.dataPath + "/../死锁.txt";
                            }, null);
                        }
                        else
                        {
                            savePath = Application.dataPath + "/../死锁.txt";
                        }
                    }
                }
                return savePath;
            }
        }

        public string Info;

        bool close = false;

        public bool Close
        {
            get
            {
                lock (lockObj)
                {
                    return close;
                }
            }
            set
            {
                lock (lockObj)
                {
                    close = value;
                }
            }
        }

        public void Start()
        {
            Task.Factory.StartNew((obj) => {
                ErrLockData data = (ErrLockData)obj;
                int time = 0;
                while (!data.Close)
                {
                    Thread.Sleep(1000);
                    time = time + 1000;
                    //超出10秒钟判断为死锁
                    if (time>= ErrLock.WaitTime)
                    {
                        data.Write();
                        break;
                    }
                }
                errLockDataPool.Recycle(data);
            },this);
        }

        void Write()
        {
            string filePath = SavePath;
            lock (lockObj)
            {
                if (Info!=null)
                {
                    lock (FileLock.GetStringLock(filePath.PathToLower()))
                    {
                        try
                        {
                            string dir = Path.GetDirectoryName(filePath);
                            lock (FileLock.GetStringLock(dir.PathToLower()))
                            {
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                            }
                            File.AppendAllText(filePath, "\n");
                            File.AppendAllText(filePath, Info);
                        }
                        catch (System.Exception ex)
                        {
                            UnityEngine.Debug.LogException(ex);
                            VLog.Exception(ex);
                        }
                    }
                }
            }
        }

        bool isDispose = false;

        public void Dispose()
        {
            isDispose = true;
        }

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        public bool Disposed
        {
            get
            {
                return isDispose;
            }
        }

        public void PutIn()
        {
            lock (lockObj)
            {
                Info = null;
                close = false;
            }
            isUsed = false;
        }

        public void PutOut()
        {
            isUsed = true;
        }
    }
}

