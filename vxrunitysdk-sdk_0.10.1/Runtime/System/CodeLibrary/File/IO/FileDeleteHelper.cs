using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 文件删除
    /// </summary>
    public class FileDeleteHelper : CSingleton<FileDeleteHelper>, IDisposable
    {
#if UNITY_EDITOR

        //[MenuItem("Tools/File/删除重复文件")]
        static void DeleteSameFiles()
        {
            WindowsHelper.SelectFolder((logDir) => {
                if (!string.IsNullOrEmpty(logDir))
                {
                    DeleteDirSameFiles(logDir);
                }
            }, "请选择Log记录文件夹");
        }

#endif

        /// <summary>
        /// 删除重复文件
        /// </summary>
        /// <param name="logDir"></param>
        public static void DeleteDirSameFiles(string logDir)
        {
            string dirLockKey = logDir.PathToLower();
            string[] files = null;
            lock (FileLock.GetStringLock(dirLockKey))
            {
                FileWriteHelper.Instance.Close(logDir, dirLockKey);
                files = Directory.GetFiles(logDir, "*.*", SearchOption.AllDirectories);
            }
            if (files == null || files.Length == 0)
            {
                VLog.Info($"Log记录排序，目录为空:{logDir}");
                return;
            }
            VLog.Info("FileCount=" + files.Length);
            Dictionary<long, List<FileInfo>> allFileInfos = new Dictionary<long, List<FileInfo>>();
            for (int i = 0; i < files.Length; ++i)
            {
                string filePath = files[i];
                string lockKey = filePath.PathToLower();
                lock (FileLock.GetStringLock(lockKey))
                {
                    FileWriteHelper.Instance.Close(filePath, lockKey);
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo!=null)
                    {
                        List<FileInfo> list;
                        if (!allFileInfos.TryGetValue(fileInfo.Length,out list))
                        {
                            list = new List<FileInfo>();
                            allFileInfos.Add(fileInfo.Length, list);
                        }
                        list.Add(fileInfo);
                    }
                }
            }
            int count = 0;
            Dictionary<long, List<FileInfo>>.Enumerator enumerator = allFileInfos.GetEnumerator();
            int allCount = 0;
            while (enumerator.MoveNext())
            {
                List<FileInfo> list = enumerator.Current.Value;
                if (list.Count > 0)
                {
                    allCount = allCount + list.Count;
                }
            }
            int curCount = 0;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            enumerator = allFileInfos.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<FileInfo> list = enumerator.Current.Value;
                if (list.Count>0)
                {
                    dic.Clear();
                    for (int i=0;i< list.Count;++i)
                    {
                        FileInfo fileInfo = list[i];
                        string filePath = fileInfo.FullName;
                        curCount++;
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayCancelableProgressBar("pro", filePath, (float)curCount / allCount);
#endif
                        string md5 = filePath.CreateFileMD5();
                        if (dic.ContainsKey(md5))
                        {
                            count++;
                            FileDeleteHelper.Instance.DeleteFileAsynchronous(filePath, (bl, obj) => { }, null);
                        }
                        else
                        {
                            dic.Add(md5, filePath);
                        }
                    }
                }
            }
            VLog.Info(count + "");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool DeleteFile(string filePath)
        {
            string lockKey = filePath.PathToLower();
            return DeleteFile(filePath, lockKey);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey"></param>
        /// <param name="filePathB"></param>
        /// <param name="lockKeyB"></param>
        /// <returns></returns>
        public bool DeleteFile(string filePath, string lockKey)
        {
            if (string.IsNullOrEmpty(filePath) || IsRelease)
            {
                return false;
            }
            lock (FileLock.GetStringLock(lockKey))
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileDeleteHelper.cs-->lockKeyA-->{0}", lockKey));
                }
                AddCurThreadDeleteCount();
                FileWriteHelper.Instance.Close(filePath, lockKey);
                try
                {
                    File.Delete(filePath);
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"删除文件错误:filePathA={filePath} ex={ex.Message}");
                    VLog.Exception(ex);
                    //
                    SubCurThreadDeleteCount();
                    //
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return false;
                }
                //
                SubCurThreadDeleteCount();
                //
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                return true;
            }
        }

        /// <summary>
        /// 异步删除文件
        /// </summary>
        /// <param name="filePathA"></param>
        /// <param name="filePathB"></param>
        /// <param name="callBack"></param>
        public void DeleteFileAsynchronous(string filePathA, System.Action<bool, object> callBack, object parObj)
        {
            string lockKeyA = filePathA.PathToLower();
            DeleteFileAsynchronous(filePathA, lockKeyA,callBack, parObj);
        }

        /// <summary>
        /// 异步删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey"></param>
        /// <param name="callBack">草被结果，传入参数parObj</param>
        /// <param name="parObj"></param>
        public void DeleteFileAsynchronous(string filePath, string lockKey, System.Action<bool, object> callBack, object parObj)
        {
            if (callBack == null)
            {
                return;
            }
            DeleteFileAsynchronousData data = deleteFileAsynchronousDataPool.Spawn();
            data.parObj = parObj;
            data.filePath = filePath;
            data.lockKey = lockKey;
            data.callBack = callBack;
            if (SynchronizationContext.Current == ThreadHelper.UnitySynchronizationContext)
            {
                data.Context = ThreadHelper.UnitySynchronizationContext;
                AddDeleteThread(data);
            }
            else
            {
                ThreadHelper.StartTask<DeleteFileAsynchronousData>(DeleteFileThread, data, (obj, deleteFileAsynchronousData, ex) => {
                    DeleteFileAsynchronousData resData = (DeleteFileAsynchronousData)deleteFileAsynchronousData;
                    System.Action<bool, object> finishCallBack = resData.callBack;
                    object callobj = resData.parObj;
                    try
                    {
                        if (ex != null)
                        {
                            VLog.Error($"异步删除文件错误:filePathA={resData.filePath} ex={ex.Message}");
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
                        VLog.Error($"异步删除文件回调错误:filePathA={resData.filePath} ex={e.Message}");
                        VLog.Exception(e);
                    }
                    deleteFileAsynchronousDataPool.Recycle(resData);
                });
            }
        }

        /// <summary>
        /// 同时写入数量限制
        /// </summary>
        int maxThreadDeleteCount = 64;

        int curThreadDeleteCount = 0;

        object curThreadDeleteCountLock = new object();

        void AddCurThreadDeleteCount()
        {
            lock (curThreadDeleteCountLock)
            {
                curThreadDeleteCount = curThreadDeleteCount + 1;
            }
        }

        void SubCurThreadDeleteCount()
        {
            lock (curThreadDeleteCountLock)
            {
                curThreadDeleteCount = curThreadDeleteCount - 1;
                RunDeleteThreadList();
            }
        }

        int GetCurThreadDeleteCount()
        {
            lock (curThreadDeleteCountLock)
            {
                return curThreadDeleteCount;
            }
        }

        Queue<DeleteFileAsynchronousData> deleteAddThreads = new Queue<DeleteFileAsynchronousData>();

        SimpleListPool<List<DeleteFileAsynchronousData>, DeleteFileAsynchronousData> deleteOutThreadPool = new SimpleListPool<List<DeleteFileAsynchronousData>, DeleteFileAsynchronousData>();

        void AddDeleteThread(DeleteFileAsynchronousData deleteThreadData)
        {

            lock (deleteAddThreads)
            {
                if (IsRelease)
                {
                    return;
                }
                deleteAddThreads.Enqueue(deleteThreadData);
            }

            RunDeleteThreadList();
        }

        void RunDeleteThreadList()
        {
            int count = maxThreadDeleteCount - GetCurThreadDeleteCount();
            if (count > 0)
            {
                Task task = Task.Factory.StartNew((obj) => {
                    int addCount = (int)obj;
                    List<DeleteFileAsynchronousData> deleteOutThreads = deleteOutThreadPool.Spawn();
                    lock (deleteAddThreads)
                    {
                        int num = 0;
                        while (deleteAddThreads.Count > 0)
                        {
                            DeleteFileAsynchronousData data = deleteAddThreads.Dequeue();
                            deleteOutThreads.Add(data);
                            num++;
                            if (num >= addCount)
                            {
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < deleteOutThreads.Count; ++i)
                    {
                        DeleteFileAsynchronousData resData = deleteOutThreads[i];
                        System.Action<bool, object> finishCallBack = resData.callBack;
                        object callobj = resData.parObj;
                        bool bl = (bool)DeleteFileThread(resData);
                        if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                ErrLockData errLockDataA = null;
                                if (ErrLock.ErrLockOpen)
                                {
                                    errLockDataA = ErrLock.LockStart("FileDeleteHelper.cs-->RunDeleteThreadList-->227");
                                }
                                try
                                {
                                    finishCallBack(bl, callobj);
                                }
                                catch (System.Exception ex)
                                {
                                    VLog.Error($"异步删除文件回调错误:filePathA={resData.filePath} ex={ex.Message}");
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
                                VLog.Error($"异步删除文件回调错误:filePathA={resData.filePath} ex={ex.Message}");
                                VLog.Exception(ex);
                            }
                        }
                        deleteFileAsynchronousDataPool.Recycle(resData);
                    }
                    deleteOutThreadPool.Recycle(deleteOutThreads);
                }, count);
            }
        }

        /// <summary>
        /// 异步删除文件
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="finishReadCallBack"></param>
        public void DeleteFileAsynchronous(List<string> filePaths , System.Action<bool> finishReadCallBack)
        {
            if (filePaths == null || filePaths.Count == 0 )
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePathA = filePaths[i];
                if (!string.IsNullOrEmpty(filePathA) && !tempList.Contains(filePathA))
                {
                    tempList.Add(filePathA);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            DeleteThreadCountData deleteThreadCountData = deleteThreadCountDataPool.Spawn();
            deleteThreadCountData.Count = tempList.Count;
            deleteThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePathA = tempList[i];
                DeleteFileAsynchronous(filePathA, (bl, obj) =>
                {
                    DeleteThreadCountData countData = (DeleteThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        deleteThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件删除错误!");
                            }
                            else
                            {
                                VLog.Info("多文件删除完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件删除完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, deleteThreadCountData);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        bool DeleteFile(DeleteFileAsynchronousData data)
        {
            return DeleteFile(data.filePath, data.lockKey);
        }

        object DeleteFileThread(object obj)
        {
            DeleteFileAsynchronousData data = (DeleteFileAsynchronousData)obj;
            bool bl = DeleteFile(data);
            return bl;
        }

        static SimplePool<DeleteFileAsynchronousData> deleteFileAsynchronousDataPool = new SimplePool<DeleteFileAsynchronousData>();

        class DeleteFileAsynchronousData : ISimplePoolData, IThreadHelperPar
        {
            public string filePath;
            public string lockKey;
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
                filePath = null;
                lockKey = null;
                callBack = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        SimplePool<DeleteThreadCountData> deleteThreadCountDataPool = new SimplePool<DeleteThreadCountData>();

        class DeleteThreadCountData : ISimplePoolData
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
            deleteFileAsynchronousDataPool.Clear();
            deleteThreadCountDataPool.Clear();
        }
    }
}


