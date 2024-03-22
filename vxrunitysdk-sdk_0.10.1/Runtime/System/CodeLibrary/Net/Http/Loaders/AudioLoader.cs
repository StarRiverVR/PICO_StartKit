using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 下载音频
    /// 参数传入: 
    /// 1.GetOneList() 传入的列表必须要从池子里获取 <不需要手动回收></不需要手动回收>
    /// 2.AudioHttpLoadeData GetOneData(string url, long size, UnityEngine.AudioType audioType) 列表中的参数必须要从池子里获取 <不需要手动回收></不需要手动回收>
    /// </summary>
    public class AudioLoader
    {
        /// <summary>
        /// Http下载失败列表 回调结束自动回收
        /// </summary>
        public List<AudioHttpLoadeData> LoadErrList;

        /// <summary>
        /// 回调错误列表 回调结束自动回收
        /// </summary>
        public List<AudioHttpLoadeData> CallBackErrList;

        /// <summary>
        /// Audio解析错误列表 回调结束自动回收
        /// </summary>
        public List<AudioHttpLoadeData> GetAudioErrList;

        /// <summary>
        /// 成功下载Audio列表
        /// </summary>
        public List<AudioHttpLoadeData> SuccessList;

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

        System.Action<bool, object, AudioLoader> finishCallBack;

        List<AudioHttpLoadeData> loadList;

        System.Action<AudioClip, AudioHttpLoadeData> loadCallBack;

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
        public AudioLoader(List<AudioHttpLoadeData> datas, System.Action<AudioClip, AudioHttpLoadeData> loadCallBack, System.Action<bool, object, AudioLoader> finishCallBack, 
            System.Action<float, float> progressCall, object paraData, int _maxReDownloadCount)
        {
            if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    Run(datas,  loadCallBack,  finishCallBack,progressCall, paraData, _maxReDownloadCount);
                }, null);
            }
            else
            {
                Run(datas, loadCallBack, finishCallBack, progressCall, paraData, _maxReDownloadCount);
            }
        }

        void Run(List<AudioHttpLoadeData> datas, System.Action<AudioClip, AudioHttpLoadeData> loadCallBack, System.Action<bool, object, AudioLoader> finishCallBack,
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
            GetAudioErrList = GetOneList();
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
                    if (httpLoaders.Count>0)
                    {
                        HttpLoader httpLoader = httpLoaders[0];
                        progressCall(Progress, httpLoader.Progress);
                    }
                    else
                    {
                        progressCall(Progress,0f);
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
            VLog.Info($"下载完成: succ={bl} LoadErrList={LoadErrList.Count} CallBackErrList={CallBackErrList.Count} GetAudioErrList={GetAudioErrList.Count} SuccessList={SuccessList.Count} 下载文件剩余数量={LastLoadCount + "/" + AllLoadCount} 剩余文件大小={LastSize + "/" + AllSize}");
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
            if (GetAudioErrList.Count > 0)
            {
                VLog.Error("Audio解析错误列表：" + GetAudioErrList.Count);
                for (int i = 0; i < GetAudioErrList.Count; ++i)
                {
                    VLog.Error($"--> url={GetAudioErrList[i].Url} callBackErr={GetAudioErrList[i].AudioErr}");
                }
            }
            if (SuccessList.Count > 0)
            {
                VLog.Info("成功下载Audio列表：" + SuccessList.Count + "/" + AllLoadCount);
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
                GetAudioErrList.Remove(SuccessList[i]);
                CallBackErrList.Remove(SuccessList[i]);
                LoadErrList.Remove(SuccessList[i]);
            }
            PutBackOneList(SuccessList);
            for (int i = 0; i < GetAudioErrList.Count; ++i)
            {
                PutBackOneData(GetAudioErrList[i]);
                CallBackErrList.Remove(GetAudioErrList[i]);
                LoadErrList.Remove(GetAudioErrList[i]);
            }
            PutBackOneList(GetAudioErrList);
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

        void LoadCallBack(AudioClip myClip, AudioHttpLoadeData data)
        {
            try
            {
                loadCallBack(myClip, data);
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
                VLog.Error($"AudioClip回调异常:url={data.Url} size={data.Size} Exception={ex.Message}");
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
                            LoadCallBack(null, LoadErrList[i]);
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
                    AudioHttpLoadeData data = loadList[0];
                    loadList.RemoveAt(0);
                    LoadErrList.Remove(data);

                    HttpLoader.LoadAudio(data.Url, data.AudioType, (httpLoader) =>
                    {
                        LoadCompleted(httpLoader);
                    },(httpLoader) => {
                        httpLoaders.Add(httpLoader);
                    }, data);

                }
                loadList.Clear();
            }
        }

        void LoadCompleted(HttpLoader httpLoader)
        {
            httpLoaders.Remove(httpLoader);
            AudioHttpLoadeData data = (AudioHttpLoadeData)httpLoader.SetData;
            if (string.IsNullOrEmpty(httpLoader.WebRequest.error))
            {
                AudioClip myClip = null;
                try
                {
                    lastSize = lastSize - data.Size;
                    string fileName = data.Url.GetNameDeleteSuffix();
                    myClip = DownloadHandlerAudioClip.GetContent(httpLoader.WebRequest);
                    myClip.name = fileName;
                    SuccessList.Add(data);
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"AudioClip获取异常:url={data.Url} size={data.Size} Exception={ex.Message}");
                    GetAudioErrList.Add(data);
                }
                LoadCallBack(myClip, data);
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

        static SimplePool<AudioHttpLoadeData> audioHttpLoadeDataPool = new SimplePool<AudioHttpLoadeData>();

        public static AudioHttpLoadeData GetOneData(string url, long size, UnityEngine.AudioType audioType)
        {
            AudioHttpLoadeData data = audioHttpLoadeDataPool.Spawn();
            data.Url = url;
            data.Size = size;
            data.AudioType = audioType;
            return data;
        }

        static void PutBackOneData(AudioHttpLoadeData data)
        {
            audioHttpLoadeDataPool.Recycle(data);
        }

        static SimpleListPool<List<AudioHttpLoadeData>, AudioHttpLoadeData> audioHttpLoadeDataListPool = new SimpleListPool<List<AudioHttpLoadeData>, AudioHttpLoadeData>();

        public static List<AudioHttpLoadeData> GetOneList()
        {
            return audioHttpLoadeDataListPool.Spawn();
        }

        static void PutBackOneList(List<AudioHttpLoadeData> list)
        {
            audioHttpLoadeDataListPool.Recycle(list);
        }
    }

    public class AudioHttpLoadeData : ISimplePoolData
    {
        public AudioHttpLoadeData()
        {

        }

        public string Url;

        public UnityEngine.AudioType AudioType;

        public long Size;

        public string LoadErr;

        public string AudioErr;

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
            LoadErr = null;
            AudioErr = null;
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


