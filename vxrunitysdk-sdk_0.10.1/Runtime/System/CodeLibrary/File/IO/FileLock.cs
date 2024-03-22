using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 文件锁
    /// </summary>
    public class FileLock
    {
        static object stringNullLock = new object();

        static Dictionary<string, object> stringLocks = new Dictionary<string, object>();

        public static object GetStringLock(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return stringNullLock;
            }
            lock (stringLocks)
            {
                object obj = null;
                if (!stringLocks.TryGetValue(key,out obj))
                {
                    obj = new object();
                    stringLocks.Add(key, obj);
                }
                return obj;
            }
        }

        /// <summary>
        /// 文件路径列表上锁  使用完后需要手动释放 FileLockData.UnLock()
        /// </summary>
        /// <param name="filePaths">必须要由GetOneFileLockDataList()获得，不需要手动回收</param>
        /// <param name="callBack">传入参数，文件所列表 注意返回的文件所列表需要手动回收PutBackOneFileLockDataList(List<FileLockData> list)</param>
        /// <param name="parObj">传入参数</param>
        public static void GetFilePathsLock(List<string> filePaths, System.Action<object, List<FileLockData>> callBack, object parObj)
        {
            List<string> tempPaths = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string path = filePaths[i];
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                path = filePaths[i].Trim();
                if (!tempPaths.Contains(path))
                {
                    tempPaths.Add(path);
                }
            }
            ListPool.Instance.PutBackOneStringList(filePaths);
            if (tempPaths.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempPaths);
                callBack(parObj, null);
                return;
            }
            FileLockCountData fileLockCountData = fileLockCountDataPool.Spawn();
            fileLockCountData.fileLockDataList = GetOneFileLockDataList();
            fileLockCountData.Count = tempPaths.Count;
            fileLockCountData.callBack = callBack;
            fileLockCountData.parObj = parObj;
            for (int i = 0; i < tempPaths.Count; ++i)
            {
                FileLockData.GetFilePathLockData(tempPaths[i], (fileLock, obj) => {
                    FileLockCountData countData = (FileLockCountData)obj;
                    if (fileLock != null)
                    {
                        countData.AddData(fileLock);
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        countData.callBack(countData.parObj, countData.fileLockDataList);
                        fileLockCountDataPool.Recycle(countData);
                    }
                }, fileLockCountData);
            }
            ListPool.Instance.PutBackOneStringList(tempPaths);
        }

        /// <summary>
        /// 文件路径上锁  使用完后需要手动释放 FileLockData.UnLock()
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="callBack"></param>
        /// <param name="parObj"></param>
        public static void GetFilePathLock(string filePath, System.Action<FileLockData, object> callBack, object parObj)
        {
            FileLockData.GetFilePathLockData(filePath, callBack, parObj);
        }

        static SimpleListPool<List<FileLockData>, FileLockData> fileLockDataListPool = new SimpleListPool<List<FileLockData>, FileLockData>();

        public static List<FileLockData> GetOneFileLockDataList()
        {
            return fileLockDataListPool.Spawn();
        }

        public static void PutBackOneFileLockDataList(List<FileLockData> list)
        {
            fileLockDataListPool.Recycle(list);
        }

        static SimplePool<FileLockCountData> fileLockCountDataPool = new SimplePool<FileLockCountData>();

        class FileLockCountData : ISimplePoolData
        {
            public List<FileLockData> fileLockDataList;

            public System.Action<object, List<FileLockData>> callBack;

            public object parObj;

            int count = 0;

            object lockObj = new object();

            public int Count
            {
                get
                {
                    lock (lockObj)
                    {
                        return count;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        count = value;
                    }
                }
            }

            public void CountSub()
            {
                lock (lockObj)
                {
                    count = count - 1;
                }
            }

            public void AddData(FileLockData data)
            {
                lock (lockObj)
                {
                    fileLockDataList.Add(data);
                }
            }

            bool isUsed = false;

            public bool IsUsed
            {
                get
                {
                    return isUsed;
                }
            }

            void ISimplePoolData.PutIn()
            {
                parObj = null;
                fileLockDataList = null;
                isUsed = false;
            }

            void ISimplePoolData.PutOut()
            {
                isUsed = true;
            }

            bool disposed = false;

            public bool Disposed
            {
                get
                {
                    return disposed;
                }
            }

            void IDisposable.Dispose()
            {
                disposed = true;
            }
        }
    }

    /// <summary>
    /// 文件路径锁
    /// 1.GetFilePathLockData(string filePath, System.Action<FileLockData> callBack)
    /// 2.使用完成  UnLock()
    /// </summary>
    public class FileLockData : ISimplePoolData
    {
        static SimplePool<FileLockData> fileLockDataPool = new SimplePool<FileLockData>();

        public static void GetFilePathLockData(string filePath, System.Action<FileLockData, object> callBack, object parObj)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                callBack(null, parObj);
                return;
            }
            FileLockData data = fileLockDataPool.Spawn();
            data.FilePath = filePath;
            data.LockKey = filePath.PathToLower();
            data.parObj = parObj;
            data.Lock();

            ParData parData = parDataPool.Spawn();
            parData.FileLockData = data;
            parData.CallBack = callBack;
            ThreadHelper.StartTask<ParData>(FunC, parData, CallBack);
        }

        static object FunC(object obj)
        {
            ParData parData = (ParData)obj;
            FileLockData fileLockData = parData.FileLockData;
            lock (FileLock.GetStringLock(fileLockData.LockKey))
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileLockData--> 238-->{0}", fileLockData.LockKey));
                }
                try
                {
                    FileWriteHelper.Instance.Close(fileLockData.FilePath);
                    if (parData.Context == ThreadHelper.UnitySynchronizationContext)
                    {
                        parData.Context.Send((obj) => {
                            ParData d = (ParData)obj;
                            d.CallBack(d.FileLockData, d.FileLockData.parObj);
                        }, parData);
                    }
                    else
                    {
                        parData.Context.Post((obj) => {
                            ParData d = (ParData)obj;
                            d.CallBack(d.FileLockData, d.FileLockData.parObj);
                        }, parData);
                    }
                    while (true)
                    {
                        if (!fileLockData.IsLock)
                        {
                            break;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    fileLockData.UnLock();
                    if (parData.Context == ThreadHelper.UnitySynchronizationContext)
                    {
                        parData.Context.Send((obj) => {
                            ParData d = (ParData)obj;
                            d.CallBack(null, d.FileLockData.parObj);
                        }, parData);
                    }
                    else
                    {
                        parData.Context.Post((obj) => {
                            ParData d = (ParData)obj;
                            d.CallBack(null, d.FileLockData.parObj);
                        }, parData);
                    }
                    VLog.Exception(ex);
                }
                //
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                //
            }
            return parData;
        }

        static void CallBack(object obj, object parObj, System.Exception e)
        {
            ParData parData = (ParData)obj;
            parDataPool.Recycle(parData);
        }

        string FilePath;

        string LockKey;

        object parObj;

        bool isLock = false;

        bool IsLock
        {
            get
            {
                lock (lockObj)
                {
                    return isLock;
                }
            }
        }

        object lockObj = new object();

        void Lock()
        {
            lock (lockObj)
            {
                isLock = true;
            }
        }

        public void UnLock()
        {
            lock (lockObj)
            {
                if (isLock)
                {
                    isLock = false;
                    fileLockDataPool.Recycle(this);
                }
            }
        }

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        void ISimplePoolData.PutIn()
        {
            parObj = null;
            FilePath = null;
            LockKey = null;
            isUsed = false;
        }

        void ISimplePoolData.PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get
            {
                return disposed;
            }
        }

        void IDisposable.Dispose()
        {
            disposed = true;
        }

        static SimplePool<ParData> parDataPool = new SimplePool<ParData>();

        class ParData : IThreadHelperPar, ISimplePoolData
        {
            public FileLockData FileLockData;

            public System.Action<FileLockData, object> CallBack;

            public SynchronizationContext Context { get; set; }

            bool isUsed = false;

            public bool IsUsed
            {
                get
                {
                    return isUsed;
                }
            }

            void ISimplePoolData.PutIn()
            {
                FileLockData = null;
                CallBack = null;
                Context = null;
                isUsed = false;
            }

            void ISimplePoolData.PutOut()
            {
                isUsed = true;
            }

            bool disposed = false;

            public bool Disposed
            {
                get
                {
                    return disposed;
                }
            }

            void IDisposable.Dispose()
            {
                disposed = true;
            }
        }

    }
}


