using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 文件拷贝
    /// </summary>
    public class FileCopyHelper:CSingleton<FileCopyHelper>,IDisposable
    {
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="filePathA"></param>
        /// <param name="filePathB"></param>
        /// <returns></returns>
        public bool CopyFile(string filePathA, string filePathB)
        {
            string lockKeyA = filePathA.PathToLower();
            string lockKeyB = filePathB.PathToLower();
            return CopyFile(filePathA, lockKeyA, filePathB, lockKeyB);
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="filePathA"></param>
        /// <param name="lockKeyA"></param>
        /// <param name="filePathB"></param>
        /// <param name="lockKeyB"></param>
        /// <returns></returns>
        public bool CopyFile(string filePathA, string lockKeyA, string filePathB, string lockKeyB)
        {
            if (IsRelease || string.IsNullOrEmpty(filePathA) || string.IsNullOrEmpty(filePathB) || filePathA.CompareTo(filePathB) == 0)
            {
                return false;
            }
            lock (FileLock.GetStringLock(lockKeyA))
            {
                ErrLockData errLockDataA = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockDataA = ErrLock.LockStart(String.Format("FileCopyHelper.cs-->lockKeyA-->{0}", lockKeyA));
                }
                AddCurThreadCopyCount();
                FileWriteHelper.Instance.Close(filePathA, lockKeyA);
                lock (FileLock.GetStringLock(lockKeyB))
                {
                    ErrLockData errLockDataB = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockDataB = ErrLock.LockStart(String.Format("FileCopyHelper.cs-->lockKeyB-->{0}", lockKeyB));
                    }
                    FileWriteHelper.Instance.Close(filePathB, lockKeyB);
                    try
                    {
                        File.Copy(filePathA, filePathB);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"拷贝文件错误:filePathA={filePathA} filePathB={filePathB} ex={ex.Message}");
                        VLog.Exception(ex);
                        //
                        SubCurThreadCopyCount();
                        //
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockDataA);
                            ErrLock.LockEnd(errLockDataB);
                        }
                        return false;
                    }
                    //
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockDataB);
                    }
                    //
                }
                //
                SubCurThreadCopyCount();
                //
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockDataA);
                }
                return true;
            }
        }

        /// <summary>
        /// 异步拷贝文件
        /// </summary>
        /// <param name="filePathA"></param>
        /// <param name="filePathB"></param>
        /// <param name="callBack"></param>
        public void CopyFileAsynchronous(string filePathA, string filePathB, System.Action<bool, object> callBack, object parObj)
        {
            string lockKeyA = filePathA.PathToLower();
            string lockKeyB = filePathB.PathToLower();
            CopyFileAsynchronous(filePathA, lockKeyA, filePathB, lockKeyB, callBack, parObj);
        }

        /// <summary>
        /// 异步拷贝文件
        /// </summary>
        /// <param name="filePathA"></param>
        /// <param name="lockKeyA"></param>
        /// <param name="filePathB"></param>
        /// <param name="lockKeyB"></param>
        /// <param name="callBack">草被结果，传入参数parObj</param>
        /// <param name="parObj"></param>
        public void CopyFileAsynchronous(string filePathA, string lockKeyA, string filePathB, string lockKeyB, System.Action<bool, object> callBack, object parObj)
        {
            if (callBack == null)
            {
                return;
            }
            CopyFileAsynchronousData data = copyFileAsynchronousDataPool.Spawn();
            data.parObj = parObj;
            data.filePathA = filePathA;
            data.lockKeyA = lockKeyA;
            data.filePathB = filePathB;
            data.lockKeyB = lockKeyB;
            data.callBack = callBack;
            if (SynchronizationContext.Current == ThreadHelper.UnitySynchronizationContext)
            {
                data.Context = ThreadHelper.UnitySynchronizationContext;
                AddCopyThread(data);
            }
            else
            {
                ThreadHelper.StartTask<CopyFileAsynchronousData>(CopyFileThread, data, (obj, copyFileAsynchronousData, ex) => {
                    CopyFileAsynchronousData resData = (CopyFileAsynchronousData)copyFileAsynchronousData;
                    System.Action<bool, object> finishCallBack = resData.callBack;
                    object callobj = resData.parObj;
                    try
                    {
                        if (ex != null)
                        {
                            VLog.Error($"异步拷贝文件错误:filePathA={resData.filePathA} filePathB={resData.filePathB} ex={ex.Message}");
                            VLog.Exception(ex);
                            finishCallBack(false, callobj);
                        }
                        else
                        {
                            bool bl = (bool)obj;
                            finishCallBack(bl, callobj);
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Error($"异步拷贝文件回调错误:filePathA={resData.filePathA} filePathB={resData.filePathB} ex={e.Message}");
                        VLog.Exception(e);
                    }
                    copyFileAsynchronousDataPool.Recycle(resData);
                });
            }
        }

        /// <summary>
        /// 同时写入数量限制
        /// </summary>
        int maxThreadCopyCount = 64;

        int curThreadCopyCount = 0;

        object curThreadCopyCountLock = new object();

        void AddCurThreadCopyCount()
        {
            lock (curThreadCopyCountLock)
            {
                curThreadCopyCount = curThreadCopyCount + 1;
            }
        }

        void SubCurThreadCopyCount()
        {
            lock (curThreadCopyCountLock)
            {
                curThreadCopyCount = curThreadCopyCount - 1;
                RunCopyThreadList();
            }
        }

        int GetCurThreadCopyCount()
        {
            lock (curThreadCopyCountLock)
            {
                return curThreadCopyCount;
            }
        }

        Queue<CopyFileAsynchronousData> copyAddThreads = new Queue<CopyFileAsynchronousData>();

        SimpleListPool<List<CopyFileAsynchronousData>, CopyFileAsynchronousData> copyOutThreadPool = new SimpleListPool<List<CopyFileAsynchronousData>, CopyFileAsynchronousData>();

        void AddCopyThread(CopyFileAsynchronousData copyThreadData)
        {

            lock (copyAddThreads)
            {
                if (IsRelease)
                {
                    return;
                }
                copyAddThreads.Enqueue(copyThreadData);
            }

            RunCopyThreadList();
        }

        void RunCopyThreadList()
        {
            int count = maxThreadCopyCount - GetCurThreadCopyCount();
            if (count > 0)
            {
                Task task = Task.Factory.StartNew((obj) => {
                    int addCount = (int)obj;
                    List<CopyFileAsynchronousData> copyOutThreads = copyOutThreadPool.Spawn();
                    lock (copyAddThreads)
                    {
                        int num = 0;
                        while (copyAddThreads.Count > 0)
                        {
                            CopyFileAsynchronousData data = copyAddThreads.Dequeue();
                            copyOutThreads.Add(data);
                            num++;
                            if (num >= addCount)
                            {
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < copyOutThreads.Count; ++i)
                    {
                        CopyFileAsynchronousData resData = copyOutThreads[i];
                        System.Action<bool, object> finishCallBack = resData.callBack;
                        object callobj = resData.parObj;
                        bool bl = (bool)CopyFileThread(resData);
                        if (SynchronizationContext.Current!= ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                ErrLockData errLockDataA = null;
                                if (ErrLock.ErrLockOpen)
                                {
                                    errLockDataA = ErrLock.LockStart("FileCopyHelper.cs-->RunCopyThreadList-->250");
                                }
                                try
                                {
                                    finishCallBack(bl, callobj);
                                }
                                catch (System.Exception ex)
                                {
                                    VLog.Error($"异步拷贝文件回调错误:filePathA={resData.filePathA} filePathB={resData.filePathB} ex={ex.Message}");
                                    VLog.Exception(ex);
                                }
                                if (ErrLock.ErrLockOpen)
                                {
                                    ErrLock.LockEnd(errLockDataA);
                                }
                            }, null);
                        }
                        else
                        {
                            try
                            {
                                finishCallBack(bl, callobj);
                            }
                            catch (System.Exception ex)
                            {
                                VLog.Error($"异步拷贝文件回调错误:filePathA={resData.filePathA} filePathB={resData.filePathB} ex={ex.Message}");
                                VLog.Exception(ex);
                            }
                        }
                        copyFileAsynchronousDataPool.Recycle(resData);
                    }
                    copyOutThreadPool.Recycle(copyOutThreads);
                }, count);
            }
        }

        /// <summary>
        /// 异步拷贝文件
        /// </summary>
        /// <param name="filePathsA">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="filePathsB">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="finishReadCallBack"></param>
        public void CopyFileAsynchronous(List<string> filePathsA, List<string> filePathsB, System.Action<bool> finishReadCallBack)
        {
            if (filePathsA == null || filePathsA.Count == 0 || filePathsB == null || filePathsB.Count == 0 || filePathsA.Count!= filePathsB.Count)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempListA = ListPool.Instance.GetOneStringList();
            List<string> tempListB = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePathsA.Count; ++i)
            {
                string filePathA = filePathsA[i];
                string filePathB = filePathsB[i];
                if (!string.IsNullOrEmpty(filePathA) && !tempListA.Contains(filePathA) 
                    && !string.IsNullOrEmpty(filePathB) && !tempListB.Contains(filePathB)
                    && filePathA.CompareTo(filePathB) !=0)
                {
                    tempListA.Add(filePathA);
                    tempListB.Add(filePathB);
                }
            }
            if (tempListA.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempListA);
                ListPool.Instance.PutBackOneStringList(tempListB);
                finishReadCallBack(false);
                return;
            }
            CopyThreadCountData copyThreadCountData = copyThreadCountDataPool.Spawn();
            copyThreadCountData.Count = tempListA.Count;
            copyThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempListA.Count; ++i)
            {
                string filePathA = tempListA[i];
                string filePathB = tempListB[i];
                CopyFileAsynchronous(filePathA, filePathB, (bl, obj) =>
                {
                    CopyThreadCountData countData = (CopyThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        copyThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件拷贝错误!");
                            }
                            else
                            {
                                VLog.Info("多文件拷贝完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件拷贝完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, copyThreadCountData);
            }
            ListPool.Instance.PutBackOneStringList(tempListA);
            ListPool.Instance.PutBackOneStringList(tempListB);
        }

        bool CopyFile(CopyFileAsynchronousData data)
        {
            return CopyFile(data.filePathA, data.lockKeyA, data.filePathB, data.lockKeyB);
        }

        object CopyFileThread(object obj)
        {
            CopyFileAsynchronousData data = (CopyFileAsynchronousData)obj;
            bool bl = CopyFile(data);
            return bl;
        }

        static SimplePool<CopyFileAsynchronousData> copyFileAsynchronousDataPool = new SimplePool<CopyFileAsynchronousData>();

        class CopyFileAsynchronousData : ISimplePoolData, IThreadHelperPar
        {
            public string filePathA;
            public string lockKeyA;
            public string filePathB;
            public string lockKeyB;
            public object parObj;
            public System.Action<bool, object> callBack;
            public SynchronizationContext Context { get; set; }

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
                parObj = null;
                Context = null;
                filePathA = null;
                lockKeyA = null;
                filePathB = null;
                lockKeyB = null;
                callBack = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        SimplePool<CopyThreadCountData> copyThreadCountDataPool = new SimplePool<CopyThreadCountData>();

        class CopyThreadCountData : ISimplePoolData
        {

            public System.Action<bool> finishReadCallBack;

            bool allIsFinish = true;

            public bool AllIsFinish
            {
                get
                {
                    return allIsFinish;
                }
                set
                {
                    lock (lockObj)
                    {
                        allIsFinish = value;
                    }
                }
            }

            int count = 0;

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

            object lockObj = new object();

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
                finishReadCallBack = null;
                AllIsFinish = true;
                Count = 0;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        public void Dispose()
        {
            copyThreadCountDataPool.Clear();
            copyFileAsynchronousDataPool.Clear();
        }
    }
}

