using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 下载文件并保存
    /// 参数传入: 
    /// 1.GetOneList() 传入的列表必须要从池子里获取 <不需要手动回收></不需要手动回收>
    /// 2.FileHttpLoadAndSaveData GetOneData(string url, string savePath, long size) 列表中的参数必须要从池子里获取 <不需要手动回收></不需要手动回收>
    /// </summary>
    public class FileSaveLoader
    {

        /// <summary>
        /// Http下载失败列表 回调结束自动回收
        /// </summary>
        public List<FileSaveHttpLoadeData> LoadErrList;

        /// <summary>
        /// 回调错误列表 回调结束自动回收
        /// </summary>
        public List<FileSaveHttpLoadeData> CallBackErrList;

        /// <summary>
        /// 成功下载FileSave列表
        /// </summary>
        public List<FileSaveHttpLoadeData> SuccessList;

        /// <summary>
        /// 进度
        /// </summary>
        public float Progress
        {
            get
            {
                if (AllSize > 0)
                {
                    return SizeProgress;
                }
                else
                {
                    return CountProgress;
                }
            }
        }

        /// <summary>
        /// 进度 数量
        /// </summary>
        float CountProgress
        {
            get
            {
                return (float)(AllLoadCount - LastLoadCount) / (float)AllLoadCount;
            }
        }

        /// <summary>
        /// 下载文件总数量
        /// </summary>
        public int AllLoadCount
        {
            get
            {
                return loadListCount;
            }
        }

        /// <summary>
        /// 剩余文件下载数量
        /// </summary>
        public int LastLoadCount
        {
            get
            {
                return loadList.Count + LoadErrList.Count;
            }
        }

        /// <summary>
        /// 文件大小进度
        /// </summary>
        float SizeProgress
        {
            get
            {
                return (float)(AllSize - LastSize) / (float)AllSize;
            }
        }

        /// <summary>
        /// 下载文件总大小
        /// </summary>
        public long AllSize
        {
            get
            {
                return allSize;
            }
        }

        /// <summary>
        /// 剩余下载文件大小
        /// </summary>
        public long LastSize
        {
            get
            {
                return lastSize;
            }
        }

        /// <summary>
        /// 最大尝试重新下载次数
        /// </summary>
        int maxReDownloadCount = 50;

        /// <summary>
        /// 当前尝试重新下载次数
        /// </summary>
        int reDownloadCount = 0;

        /// <summary>
        /// 当前尝试重新下载次数
        /// </summary>
        public int CurReDownloadCount
        {
            get
            {
                return reDownloadCount;
            }
        }

        System.Action<bool, object, FileSaveLoader> finishCallBack;

        List<FileSaveHttpLoadeData> loadList;

        System.Action<bool, FileSaveHttpLoadeData> loadCallBack;

        /// <summary>
        /// 总进度+子进度
        /// </summary>
        System.Action<float, float> progressCall;

        int loadListCount = 0;

        long allSize = 0;

        long lastSize = 0;

        object paraData;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datas">需要由GetOneList() GetOneData()获取,不需要手动回收</param>
        /// <param name="loadCallBack"></param>
        /// <param name="finishCallBack">全部下载成功会返回true</param>
        /// <param name="progressCall">总进度,子进度</param>
        /// <param name="paraData"></param>
        /// <param name="_maxReDownloadCount"></param>
        public FileSaveLoader(List<FileSaveHttpLoadeData> datas, System.Action<bool, FileSaveHttpLoadeData> loadCallBack, System.Action<bool, object, FileSaveLoader> finishCallBack,
            System.Action<float, float> progressCall, object paraData, int _maxReDownloadCount)
        {
            if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    Run(datas,  loadCallBack,  finishCallBack,progressCall,  paraData,  _maxReDownloadCount);
                }, null);
            }
            else
            {
                Run(datas, loadCallBack, finishCallBack, progressCall, paraData, _maxReDownloadCount);
            }
        }

        void Run(List<FileSaveHttpLoadeData> datas, System.Action<bool, FileSaveHttpLoadeData> loadCallBack, System.Action<bool, object, FileSaveLoader> finishCallBack,
            System.Action<float, float> progressCall, object paraData, int _maxReDownloadCount)
        {
            if (datas == null || datas.Count == 0)
            {
                VLog.Error("数据为空!");
                try
                {
                    if (progressCall != null)
                    {
                        progressCall(1f, 0f);
                    }
                    finishCallBack(true, paraData, this);
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                }
                //资源回收
                if (datas != null)
                {
                    PutBackOneList(datas);
                }
                return;
            }
            this.progressCall = progressCall;
            this.paraData = paraData;
            httpLoaders = httpLoadersListPool.Spawn();
            SuccessList = GetOneList();
            LoadErrList = GetOneList();
            CallBackErrList = GetOneList();
            maxReDownloadCount = _maxReDownloadCount;
            loadList = datas;
            loadListCount = loadList.Count;
            this.loadCallBack = loadCallBack;
            this.finishCallBack = finishCallBack;
            for (int i = 0; i < loadList.Count; ++i)
            {
                allSize = allSize + loadList[i].Size;
            }
            lastSize = allSize;
            ProgressCall();
            Load();
            if (this.progressCall != null)
            {
                GameCoroutine.Instance.StartCoroutine(LoadProgress());
            }
        }

        void ProgressCall()
        {
            try
            {
                if (progressCall != null)
                {
                    if (httpLoaders.Count > 0)
                    {
                        HttpLoader httpLoader = httpLoaders[0];
                        progressCall(Progress, httpLoader.Progress);
                    }
                    else
                    {
                        progressCall(Progress, 0f);
                    }
                }
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }
        }

        IEnumerator LoadProgress()
        {
            while (curCount != 0)
            {
                ProgressCall();
                yield return null;
            }
            ProgressCall();
        }

        int curCount = 0;

        void FinishCallBack(bool bl)
        {
            VLog.Info($"下载完成: succ={bl} LoadErrList={LoadErrList.Count} CallBackErrList={CallBackErrList.Count} SuccessList={SuccessList.Count} 下载文件剩余数量={LastLoadCount + "/" + AllLoadCount} 剩余文件大小={LastSize + "/" + AllSize}");
            if (CallBackErrList.Count > 0)
            {
                VLog.Error("回调错误列表：" + CallBackErrList.Count);
                for (int i = 0; i < CallBackErrList.Count; ++i)
                {
                    VLog.Error($"--> url={CallBackErrList[i].Url} callBackErr={CallBackErrList[i].CallBackErr}");
                }
            }
            if (LoadErrList.Count > 0)
            {
                VLog.Error("下载错误列表：" + LoadErrList.Count);
                for (int i = 0; i < LoadErrList.Count; ++i)
                {
                    VLog.Error($"--> url={LoadErrList[i].Url} callBackErr={LoadErrList[i].LoadErr}");
                }
            }
            if (SuccessList.Count > 0)
            {
                VLog.Info("成功下载FileSave列表：" + SuccessList.Count + "/" + AllLoadCount);
                for (int i = 0; i < SuccessList.Count; ++i)
                {
                    VLog.Info($"--> url={SuccessList[i].Url}");
                }
            }

            try
            {
                if (this.finishCallBack != null)
                {
                    this.finishCallBack(bl, paraData, this);
                }
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }

            //资源回收
            for (int i = 0; i < SuccessList.Count; ++i)
            {
                PutBackOneData(SuccessList[i]);
                CallBackErrList.Remove(SuccessList[i]);
                LoadErrList.Remove(SuccessList[i]);
            }
            PutBackOneList(SuccessList);
            for (int i = 0; i < CallBackErrList.Count; ++i)
            {
                PutBackOneData(CallBackErrList[i]);
                LoadErrList.Remove(CallBackErrList[i]);
            }
            PutBackOneList(CallBackErrList);
            for (int i = 0; i < LoadErrList.Count; ++i)
            {
                PutBackOneData(LoadErrList[i]);
            }
            PutBackOneList(LoadErrList);
            PutBackOneList(loadList);
            httpLoadersListPool.Recycle(httpLoaders);
        }

        void LoadCallBack(bool bl, FileSaveHttpLoadeData data)
        {
            try
            {
                loadCallBack(bl, data);
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
                VLog.Error($"FileSave回调异常:url={data.Url} size={data.Size} Exception={ex.Message}");
                data.CallBackErr = ex.Message;
                CallBackErrList.Add(data);
            }
        }

        List<HttpLoader> httpLoaders;

        static SimpleListPool<List<HttpLoader>, HttpLoader> httpLoadersListPool = new SimpleListPool<List<HttpLoader>, HttpLoader>();

        void Load()
        {
            if (curCount == 0 && loadList.Count == 0)
            {
                if (LoadErrList.Count > 0)
                {
                    reDownloadCount++;
                    if (reDownloadCount >= maxReDownloadCount)
                    {
                        for (int i = 0; i < LoadErrList.Count; ++i)
                        {
                            VLog.Error($"(退出) 下载失败:url={LoadErrList[i].Url} size={LoadErrList[i].Size} loadErr={LoadErrList[i].LoadErr}");
                            LoadCallBack(false, LoadErrList[i]);
                        }
                        //尝试重新下载失败
                        FinishCallBack(false);
                        return;
                    }
                    for (int i = 0; i < LoadErrList.Count; ++i)
                    {
                        loadList.Add(LoadErrList[i]);
                    }
                    LoadErrList.Clear();
                    DelayFunHelper.DelayRun(Load, null, null, 1f);
                }
                else
                {
                    FinishCallBack(true);
                }
            }
            else
            {
                int count = loadList.Count;
                for (int i = 0; i < count; ++i)
                {
                    curCount++;
                    FileSaveHttpLoadeData data = loadList[0];
                    loadList.RemoveAt(0);
                    LoadErrList.Remove(data);

                    HttpLoader.LoadSaveFile(data.Url, (httpLoader) =>
                    {
                        LoadCompleted(httpLoader);
                    }, (httpLoader) => {
                        httpLoaders.Add(httpLoader);
                    }, data, data.SavePath);

                }
                loadList.Clear();
            }
        }

        void LoadCompleted(HttpLoader httpLoader)
        {
            httpLoaders.Remove(httpLoader);
            FileSaveHttpLoadeData data = (FileSaveHttpLoadeData)httpLoader.SetData;
            if (string.IsNullOrEmpty(httpLoader.WebRequest.error))
            {
                lastSize = lastSize - data.Size;
                SuccessList.Add(data);
                LoadCallBack(true, data);
            }
            else
            {
                data.LoadErr = httpLoader.WebRequest.error;
                if (!LoadErrList.Contains(data))
                {
                    VLog.Error($"尝试重新下载:重连次数={reDownloadCount + "/" + maxReDownloadCount} url={data.Url} size={data.Size} loadErr={data.LoadErr}");
                    LoadErrList.Add(data);
                }
            }
            curCount--;
            if (curCount == 0)
            {
                Load();
            }
        }

        static SimplePool<FileSaveHttpLoadeData> fileHttpLoadAndSaveDataPool = new SimplePool<FileSaveHttpLoadeData>();

        public static FileSaveHttpLoadeData GetOneData(string url, string savePath, long size)
        {
            FileSaveHttpLoadeData data = fileHttpLoadAndSaveDataPool.Spawn();
            data.Url = url;
            data.Size = size;
            data.SavePath = savePath;
            return data;
        }

        static void PutBackOneData(FileSaveHttpLoadeData data)
        {
            fileHttpLoadAndSaveDataPool.Recycle(data);
        }

        static SimpleListPool<List<FileSaveHttpLoadeData>, FileSaveHttpLoadeData> fileHttpLoadAndSaveDataListPool = new SimpleListPool<List<FileSaveHttpLoadeData>, FileSaveHttpLoadeData>();

        public static List<FileSaveHttpLoadeData> GetOneList()
        {
            return fileHttpLoadAndSaveDataListPool.Spawn();
        }

        static void PutBackOneList(List<FileSaveHttpLoadeData> list)
        {
            fileHttpLoadAndSaveDataListPool.Recycle(list);
        }

        ///// <summary>
        ///// 保存错误列表 回调结束自动回收
        ///// </summary>
        //public List<FileSaveHttpLoadeData> SaveErrList;

        ///// <summary>
        ///// 回调错误列表 回调结束自动回收
        ///// </summary>
        //public List<FileSaveHttpLoadeData> CallBackErrList;

        ///// <summary>
        ///// Http下载错误列表 回调结束自动回收
        ///// </summary>
        //public List<FileHttpLoadeData> LoadErrList
        //{
        //    get
        //    {
        //        if (fileHttpLoad==null)
        //        {
        //            return null;
        //        }
        //        return fileHttpLoad.LoadErrList;
        //    }
        //}

        ///// <summary>
        ///// 成功下载列表
        ///// </summary>
        //public List<FileHttpLoadeData> SuccessLoadList
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return null;
        //        }
        //        return fileHttpLoad.SuccessList;
        //    }
        //}

        ///// <summary>
        ///// 成功存储列表
        ///// </summary>
        //public List<FileSaveHttpLoadeData> SuccessList;

        ///// <summary>
        ///// 进度
        ///// </summary>
        //public float Progress
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.Progress;
        //    }
        //}

        ///// <summary>
        ///// 下载文件总数量
        ///// </summary>
        //public int AllLoadCount
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.AllLoadCount;
        //    }
        //}

        ///// <summary>
        ///// 剩余文件下载数量
        ///// </summary>
        //public int LastLoadCount
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.LastLoadCount;
        //    }
        //}

        ///// <summary>
        ///// 下载文件总大小
        ///// </summary>
        //public long AllSize
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.AllSize;
        //    }
        //}

        ///// <summary>
        ///// 剩余下载文件大小
        ///// </summary>
        //public long LastSize
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.LastSize;
        //    }
        //}

        ///// <summary>
        ///// 当前尝试重新下载次数
        ///// </summary>
        //public int CurReDownloadCount
        //{
        //    get
        //    {
        //        if (fileHttpLoad == null)
        //        {
        //            return 0;
        //        }
        //        return fileHttpLoad.CurReDownloadCount;
        //    }
        //}

        //List<FileSaveHttpLoadeData> list;

        //System.Action<bool, object, FileSaveLoader> finishCallBack;

        //System.Action<bool, FileSaveHttpLoadeData> elementCallBack;

        //FileHttpLoad fileHttpLoad;

        //Dictionary<FileHttpLoadeData, FileSaveHttpLoadeData> dataDic = new Dictionary<FileHttpLoadeData, FileSaveHttpLoadeData>();

        //bool succ = true;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="datas"></param>
        ///// <param name="elementCallBack"></param>
        ///// <param name="finishCallBack">全部下载并存储成功会返回true</param>
        ///// <param name="progressCall"></param>
        ///// <param name="paraData"></param>
        ///// <param name="_maxReDownloadCount"></param>
        //public FileSaveLoader(List<FileSaveHttpLoadeData> datas,System.Action<bool, FileSaveHttpLoadeData> elementCallBack,
        //    System.Action<bool, object, FileSaveLoader> finishCallBack, System.Action<float, float> progressCall, object paraData, int _maxReDownloadCount)
        //{
        //    if (datas==null || datas.Count==0)
        //    {
        //        DebugLog.LogError("数据为空!",true);
        //        try
        //        {
        //            finishCallBack(true, paraData,this);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            DebugLog.LogException(ex,true);
        //        }
        //        //资源回收
        //        if (datas!=null)
        //        {
        //            PutBackOneList(datas);
        //        }
        //        return;
        //    }
        //    this.elementCallBack = elementCallBack;
        //    this.list = datas;
        //    SuccessList = GetOneList();
        //    SaveErrList = GetOneList();
        //    CallBackErrList = GetOneList();
        //    this.finishCallBack = finishCallBack;

        //    List<FileHttpLoadeData> fileDataList = FileHttpLoad.GetOneList();
        //    for (int i=0;i< this.list.Count;++i)
        //    {
        //        FileSaveHttpLoadeData data = this.list[i];
        //        FileHttpLoadeData fileData = FileHttpLoad.GetOneData(data.Url, data.Size);
        //        fileDataList.Add(fileData);
        //        dataDic.Add(fileData, data);
        //    }
        //    fileHttpLoad = new FileHttpLoad(fileDataList, FileCallBack, FinishCallBack, progressCall, paraData, _maxReDownloadCount);
        //}

        //void FinishCallBack(bool bl, object paraData, FileHttpLoad fileHttpLoad)
        //{
        //    this.fileHttpLoad = fileHttpLoad;
        //    DebugLog.LogFormat("资源存储完成：success={0} CallBackErrList={1} SaveErrList={2} SuccessList={3} 剩余文件数量={4} 剩余文件={5} 重新下载次数={6}", true, bl,CallBackErrList.Count, SaveErrList.Count, SuccessList.Count,
        //        LastLoadCount+"/"+ AllLoadCount, LastSize+"/"+ AllSize, CurReDownloadCount);
        //    if (CallBackErrList.Count > 0)
        //    {
        //        DebugLog.LogError("回调错误列表：" + CallBackErrList.Count, true);
        //        for (int i = 0; i < CallBackErrList.Count; ++i)
        //        {
        //            DebugLog.LogErrorFormat("--> url={0} savePath={1} callBackErr={2}", true, CallBackErrList[i].Url, CallBackErrList[i].SavePath, CallBackErrList[i].CallBackErr);
        //        }
        //    }
        //    if (SaveErrList.Count > 0)
        //    {
        //        DebugLog.LogError("存储错误列表：" + SaveErrList.Count, true);
        //        for (int i = 0; i < SaveErrList.Count; ++i)
        //        {
        //            DebugLog.LogErrorFormat("--> url={0} savePath={1} callBackErr={2}", true, SaveErrList[i].Url, SaveErrList[i].SavePath, SaveErrList[i].SaveErr);
        //        }
        //    }
        //    if (SuccessList.Count > 0)
        //    {
        //        DebugLog.Log("成功存储列表：" + SuccessList.Count + "/" + AllLoadCount, true);
        //        for (int i = 0; i < SuccessList.Count; ++i)
        //        {
        //            DebugLog.LogFormat("--> url={0} savePath={1}", true, SuccessList[i].Url, SuccessList[i].SavePath);
        //        }
        //    }

        //    try
        //    {
        //        if (this.finishCallBack!=null)
        //        {
        //            if (succ && bl)
        //            {
        //                this.finishCallBack(true, paraData, this);
        //            }
        //            else{
        //                if (!succ)
        //                {
        //                    DebugLog.LogError("下载文件有错误！",true);
        //                }
        //                this.finishCallBack(false, paraData, this);
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        DebugLog.LogException(ex,true);
        //    }

        //    //资源回收
        //    for (int i = 0; i < SuccessList.Count; ++i)
        //    {
        //        PutBackOneData(SuccessList[i]);
        //    }
        //    PutBackOneList(SuccessList);
        //    //
        //    Dictionary<FileHttpLoadeData, FileSaveHttpLoadeData>.Enumerator en = dataDic.GetEnumerator();
        //    while (en.MoveNext())
        //    {
        //        FileSaveHttpLoadeData data = en.Current.Value;
        //        PutBackOneData(data);
        //        CallBackErrList.Remove(data);
        //    }
        //    dataDic = null;
        //    for (int i = 0; i < CallBackErrList.Count; ++i)
        //    {
        //        PutBackOneData(CallBackErrList[i]);
        //    }
        //    PutBackOneList(CallBackErrList);
        //    PutBackOneList(SaveErrList);
        //    PutBackOneList(list);
        //}

        //void ElementCallBack(bool bl, FileSaveHttpLoadeData data)
        //{
        //    try
        //    {
        //        if (elementCallBack!=null)
        //        {
        //            elementCallBack(bl, data);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        DebugLog.LogErrorFormat("下载保存回调异常:url={0} savePath={1} size={2} Exception={3}", true, data.Url, data.SavePath, data.Size, ex.Message);
        //        DebugLog.LogException(ex, true);
        //        data.CallBackErr = ex.Message;
        //        CallBackErrList.Add(data);
        //    }
        //}

        //void FileCallBack(string str, byte[] bytes, FileHttpLoadeData fileData)
        //{
        //    bool bl = false;
        //    FileSaveHttpLoadeData data = null;
        //    try
        //    {
        //        data = dataDic[fileData];
        //        if (bytes == null)
        //        {
        //            if (data != null)
        //            {
        //                data.SaveErr = "文件数据=null，下载不成功，存储失败！";
        //                if (!SaveErrList.Contains(data))
        //                {
        //                    DebugLog.LogErrorFormat("保存失败:url={0} size={1} save={2} saveErr={3}", true, data.Url, data.Size, data.SavePath, data.SaveErr);
        //                    SaveErrList.Add(data);
        //                }
        //            }
        //            else
        //            {
        //                DebugLog.LogErrorFormat("记录丢失，下载不成功，存储失败！ {0}", true, fileData.Url);
        //            }
        //            succ = false;
        //        }
        //        else
        //        {
        //            if (data!=null)
        //            {
        //                FileWriteHelper.Write(data.SavePath, bytes, false, false, true, true, true);
        //                bl = true;
        //                SuccessList.Add(data);
        //            }
        //            else
        //            {
        //                DebugLog.LogErrorFormat("记录丢失，存储失败！ {0}", true, fileData.Url);
        //                succ = false;
        //            }
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        succ = false;
        //        if (data!=null)
        //        {
        //            data.SaveErr = e.Message;
        //            if (!SaveErrList.Contains(data))
        //            {
        //                DebugLog.LogErrorFormat("保存失败:url={0} size={1} save={2} saveErr={3}", true, data.Url, data.Size, data.SavePath, data.SaveErr);
        //                SaveErrList.Add(data);
        //            }
        //        }
        //        else
        //        {
        //            DebugLog.LogErrorFormat("err={0} url={1}", true, e.Message, fileData.Url);
        //        }
        //    }
        //    ElementCallBack(bl, data);
        //}

        //static SimplePool<FileSaveHttpLoadeData> fileHttpLoadAndSaveDataPool = new SimplePool<FileSaveHttpLoadeData>();

        //public static FileSaveHttpLoadeData GetOneData(string url, string savePath, long size)
        //{
        //    FileSaveHttpLoadeData data = fileHttpLoadAndSaveDataPool.Spawn();
        //    data.Url = url;
        //    data.Size = size;
        //    data.SavePath = savePath;
        //    return data;
        //}

        //static void PutBackOneData(FileSaveHttpLoadeData data)
        //{
        //    fileHttpLoadAndSaveDataPool.Recycle(data);
        //}

        //static SimpleListPool<List<FileSaveHttpLoadeData>, FileSaveHttpLoadeData> fileHttpLoadAndSaveDataListPool = new SimpleListPool<List<FileSaveHttpLoadeData>, FileSaveHttpLoadeData>();

        //public static List<FileSaveHttpLoadeData> GetOneList()
        //{
        //    return fileHttpLoadAndSaveDataListPool.Spawn();
        //}

        //static void PutBackOneList(List<FileSaveHttpLoadeData> list)
        //{
        //    fileHttpLoadAndSaveDataListPool.Recycle(list);
        //}

    }

    public class FileSaveHttpLoadeData : ISimplePoolData
    {
        public FileSaveHttpLoadeData()
        {

        }

        public string Url;

        public string SavePath;

        public long Size;

        public string LoadErr;

        public string CallBackErr;

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        public void PutIn()
        {
            Url = null;
            SavePath = null;
            LoadErr = null;
            CallBackErr = null;
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


