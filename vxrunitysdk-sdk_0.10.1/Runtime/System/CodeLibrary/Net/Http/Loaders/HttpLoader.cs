using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace com.vivo.codelibrary
{
    public class HttpLoader : SimpleData
    {

        UnityWebRequest webRequest;

        public UnityWebRequest WebRequest
        {
            get
            {
                return webRequest;
            }
        }

        List<FileLockData> fileLockDataList;

        UnityWebRequestAsyncOperation requestAsync;

        System.Action<HttpLoader> loadedAction;

        bool isFinish = false;

        public bool IsFinish
        {
            get
            {
                return isFinish;
            }
        }

        string url;

        public string URL
        {
            get
            {
                return url;
            }
        }

        string lockUrl;

        public object SetData;

        float progress = 0;

        /// <summary>
        /// 进度条
        /// </summary>
        public float Progress
        {
            get
            {
                return progress;
            }
        }

        static SimplePool<HttpLoader> pool = new SimplePool<HttpLoader>();

        static SimplePool<HttpLoader> Pool
        {
            get
            {
                if (pool == null)
                {
                    pool = new SimplePool<HttpLoader>();
                }
                return pool;
            }
        }

        static string FormUrl(string url,ref string lockUrl)
        {
            if (url.StartsWith(string.Intern("http:")) || url.StartsWith(string.Intern("https:")) || url.StartsWith(string.Intern("file://"))
                || url.StartsWith(string.Intern("jar:")) || url.StartsWith(string.Intern("ftp:")))
            {
                if (url.StartsWith(string.Intern("file://")))
                {
                    lockUrl = url.Replace("file://","");
                }
                return url;
            }
            else
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE
                lockUrl = url;
#else
                if (url.Contains(Application.persistentDataPath))
                {
                    lockUrl = url;
                }
#endif
                url = "file://" + url;
                return url;
            }
        }

        static HttpLoader GetData(string url,object setData = null)
        {
            HttpLoader data = Pool.Spawn();
            data.url = FormUrl(url,ref data.lockUrl);
            data.SetData = setData;
            data.progress = 0;
            data.isFinish = false;
            return data;
        }

        static HttpLoader Spawn(string url, object setData = null)
        {
            HttpLoader data = GetData(url, setData);
            try
            {
                data.webRequest = UnityWebRequest.Get(data.url + string.Intern("?") + DateTime.Now.ToString());
                data.webRequest.timeout = 15;
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                Recycle(data);
                return null;
            }
            data.PutOutFromPool();
            return data;
        }

        static HttpLoader Spawn(string url,  UnityEngine.AudioType audioType,object setData=null)
        {
            HttpLoader data = GetData(url, setData);
            try
            {
                data.webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
                data.webRequest.timeout = 15;
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                Recycle(data);
                return null;
            }
            data.PutOutFromPool();
            return data;
        }

        static void Recycle(HttpLoader data)
        {
            if (data.fileLockDataList!=null)
            {
                for (int i = 0; i < data.fileLockDataList.Count; ++i)
                {
                    data.fileLockDataList[i].UnLock();
                }
                data.fileLockDataList.Clear();
                FileLock.PutBackOneFileLockDataList(data.fileLockDataList);
            }
            ISimplePoolData iSimplePoolData = (ISimplePoolData)data;
            data.PutInPool();
            iSimplePoolData.Dispose();
            Pool.Recycle(data);
        }

        protected virtual void PutInPool()
        {
            if (requestAsync!=null)
            {
                requestAsync.completed -= Completed;
                requestAsync = null;
            }
            if (webRequest!=null)
            {
                webRequest.Abort();
                webRequest.Dispose();
                webRequest = null;
            }
            lockUrl = null;
            loadedAction = null;
            url = null;
            SetData = null;
        }

        protected virtual void PutOutFromPool()
        {

        }

        protected virtual void Completed(AsyncOperation asyncOperation)
        {
            isFinish = true;
            if (!string.IsNullOrEmpty(WebRequest.error))
            {
                VLog.Error(string.Format("UnityWebRequest Err :{0}", WebRequest.error));
                if (WebRequest.downloadHandler!=null)
                {
                    VLog.Error(string.Format("UnityWebRequest Err :{0}", WebRequest.downloadHandler.error));
                }
            }
            try
            {
                if (loadedAction != null)
                {
                    loadedAction.Invoke(this);
                }
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }
            if (webRequest != null && webRequest.downloadHandler != null)
            {
                webRequest.downloadHandler.Dispose();
            }
            curLoadingLoaders.Remove(URL);
            Recycle(this);
            curCount--;
            if (waitList.Count>0)
            {
                WaitData waitData = waitList[0];
                waitList.RemoveAt(0);
                waitData.HttpLoader.Load(waitData.LoadCallBack, waitData.LoaderType, waitData.FileSavePath, waitData.TextureReadable, waitData.AudioType);
                waitDataPool.Recycle(waitData);
            }
        }

        public static void LoadBuffer(string url,System.Action<HttpLoader> loadCallBack, System.Action<HttpLoader> getHttpLoader,object setData)
        {
            if (string.IsNullOrEmpty(url))
            {
                VLog.Error("url为Null !");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId!= ThreadHelper.UnityThreadId)
            {
                HttpLoaderThreadData threadData = httpLoaderThreadDataPool.Spawn();
                threadData.url = url;
                threadData.Data = setData;
                threadData.loadCallBack = loadCallBack;
                threadData.getHttpLoader = getHttpLoader;
                ThreadHelper.UnitySynchronizationContext.Post((obj)=> {
                    HttpLoaderThreadData httpLoaderThreadData = (HttpLoaderThreadData)obj;
                    HttpLoader data = HttpLoader.Spawn(httpLoaderThreadData.url, httpLoaderThreadData.Data);
                    httpLoaderThreadData.getHttpLoader(data);
                    data.Load(httpLoaderThreadData.loadCallBack, LoaderType.Buffer, null, false);
                    httpLoaderThreadDataPool.Recycle(httpLoaderThreadData);
                }, threadData);
            }
            else
            {
                HttpLoader data = HttpLoader.Spawn(url, setData);
                getHttpLoader(data);
                data.Load(loadCallBack, LoaderType.Buffer, null, false);
            }
        }

        public static void LoadAssetBundle(string url, System.Action<HttpLoader> loadCallBack, System.Action<HttpLoader> getHttpLoader, object setData)
        {
            if (string.IsNullOrEmpty(url))
            {
                VLog.Error("url为Null !");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                HttpLoaderThreadData threadData = httpLoaderThreadDataPool.Spawn();
                threadData.url = url;
                threadData.Data = setData;
                threadData.loadCallBack = loadCallBack;
                threadData.getHttpLoader = getHttpLoader;
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    HttpLoaderThreadData httpLoaderThreadData = (HttpLoaderThreadData)obj;
                    HttpLoader data = HttpLoader.Spawn(httpLoaderThreadData.url, httpLoaderThreadData.Data);
                    httpLoaderThreadData.getHttpLoader(data);
                    data.Load(httpLoaderThreadData.loadCallBack, LoaderType.AssetBundle, null, false);
                    httpLoaderThreadDataPool.Recycle(httpLoaderThreadData);
                }, threadData);
            }
            else
            {
                HttpLoader data = HttpLoader.Spawn(url, setData);
                getHttpLoader(data);
                data.Load(loadCallBack, LoaderType.AssetBundle, null, false);
            }
        }

        public static void LoadTexture(string url, System.Action<HttpLoader> loadCallBack, System.Action<HttpLoader> getHttpLoader, object setData, bool textureReadable)
        {
            if (string.IsNullOrEmpty(url))
            {
                VLog.Error("url为Null !");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                HttpLoaderThreadData threadData = httpLoaderThreadDataPool.Spawn();
                threadData.url = url;
                threadData.Data = setData;
                threadData.loadCallBack = loadCallBack;
                threadData.getHttpLoader = getHttpLoader;
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    HttpLoaderThreadData httpLoaderThreadData = (HttpLoaderThreadData)obj;
                    HttpLoader data = HttpLoader.Spawn(httpLoaderThreadData.url, httpLoaderThreadData.Data);
                    httpLoaderThreadData.getHttpLoader(data);
                    data.Load(httpLoaderThreadData.loadCallBack, LoaderType.Texture, null, textureReadable);
                    httpLoaderThreadDataPool.Recycle(httpLoaderThreadData);
                }, threadData);
            }
            else
            {
                HttpLoader data = HttpLoader.Spawn(url, setData);
                getHttpLoader(data);
                data.Load(loadCallBack, LoaderType.Texture, null, textureReadable);
            }
        }

        public static void LoadSaveFile(string url, System.Action<HttpLoader> loadCallBack, System.Action<HttpLoader> getHttpLoader, object setData, string fileSavePath)
        {
            if (string.IsNullOrEmpty(url))
            {
                VLog.Error("url为Null !");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (url.CompareTo(fileSavePath) ==0)
            {
                VLog.Error($"路径重复: url={url} fileSavePath={fileSavePath}");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                HttpLoaderThreadData threadData = httpLoaderThreadDataPool.Spawn();
                threadData.url = url;
                threadData.Data = setData;
                threadData.loadCallBack = loadCallBack;
                threadData.getHttpLoader = getHttpLoader;
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    HttpLoaderThreadData httpLoaderThreadData = (HttpLoaderThreadData)obj;
                    HttpLoader data = HttpLoader.Spawn(httpLoaderThreadData.url, httpLoaderThreadData.Data);
                    httpLoaderThreadData.getHttpLoader(data);
                    data.Load(httpLoaderThreadData.loadCallBack, LoaderType.SaveFile, fileSavePath, false);
                    httpLoaderThreadDataPool.Recycle(httpLoaderThreadData);
                }, threadData);
            }
            else
            {
                HttpLoader data = HttpLoader.Spawn(url, setData);
                getHttpLoader(data);
                data.Load(loadCallBack, LoaderType.SaveFile, fileSavePath, false);
            }
        }

        public static void LoadAudio(string url, UnityEngine.AudioType audioType,System.Action<HttpLoader> loadCallBack, System.Action<HttpLoader> getHttpLoader, object setData = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                VLog.Error("url为Null !");
                loadCallBack(null);
                getHttpLoader(null);
                return;
            }
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                HttpLoaderThreadData threadData = httpLoaderThreadDataPool.Spawn();
                threadData.url = url;
                threadData.Data = setData;
                threadData.loadCallBack = loadCallBack;
                threadData.getHttpLoader = getHttpLoader;
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    HttpLoaderThreadData httpLoaderThreadData = (HttpLoaderThreadData)obj;
                    HttpLoader data = HttpLoader.Spawn(httpLoaderThreadData.url, audioType, httpLoaderThreadData.Data);
                    httpLoaderThreadData.getHttpLoader(data);
                    data.Load(httpLoaderThreadData.loadCallBack, LoaderType.AudioClip, null, false, audioType);
                    httpLoaderThreadDataPool.Recycle(httpLoaderThreadData);
                }, threadData);
            }
            else
            {
                HttpLoader data = HttpLoader.Spawn(url, audioType, setData);
                getHttpLoader(data);
                data.Load(loadCallBack, LoaderType.AudioClip, null, false, audioType);
            }
        }

        static SimplePool<HttpLoaderThreadData> httpLoaderThreadDataPool = new SimplePool<HttpLoaderThreadData>();

        class HttpLoaderThreadData : ISimplePoolData
        {
            public string url;

            public object Data;

            public System.Action<HttpLoader> loadCallBack;

            public System.Action<HttpLoader> getHttpLoader;

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
                url = null;
                Data = null;
                loadCallBack = null;
                getHttpLoader = null;
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

        void Load(System.Action<HttpLoader> loadCallBack, LoaderType loaderType,string fileSavePath,bool textureReadable, UnityEngine.AudioType audioType= UnityEngine.AudioType.WAV)
        {
            if (curCount >= MaxCount || curLoadingLoaders.ContainsKey(URL))
            {
                WaitData waitData = waitDataPool.Spawn();
                waitData.HttpLoader = this;
                waitData.LoadCallBack = loadCallBack;
                waitData.LoaderType = loaderType;
                waitData.FileSavePath = fileSavePath;
                waitData.TextureReadable = textureReadable;
                waitData.AudioType = audioType;

                waitList.Add(waitData);
                return;
            }
            string lockPath = null;
            curLoadingLoaders.Add(URL,this);
            loadedAction = loadCallBack;
            switch (loaderType)
            {
                case LoaderType.Buffer:
                    {
                        if (webRequest.downloadHandler == null || webRequest.downloadHandler.GetType()!=typeof(DownloadHandlerBuffer))
                        {
                            webRequest.downloadHandler = new DownloadHandlerBuffer();
                        }
                    }
                    break;
                case LoaderType.SaveFile:
                    {
                        lockPath = fileSavePath;
                        if (webRequest.downloadHandler == null || webRequest.downloadHandler.GetType() != typeof(DownloadHandlerFile))
                        {
                            webRequest.downloadHandler = new DownloadHandlerFile(fileSavePath);
                        }
                    }
                    break;
                case LoaderType.Texture:
                    {
                        if (webRequest.downloadHandler == null || webRequest.downloadHandler.GetType() != typeof(DownloadHandlerTexture))
                        {
                            webRequest.downloadHandler = new DownloadHandlerTexture(textureReadable);
                            //Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height),Vector2.zero, 1f);
                        }
                    }
                    break;
                case LoaderType.AssetBundle:
                    {
                        if (webRequest.downloadHandler == null || webRequest.downloadHandler.GetType() != typeof(DownloadHandlerAssetBundle))
                        {
                            webRequest.downloadHandler = new DownloadHandlerAssetBundle(URL, uint.MaxValue);
                        }
                    }
                    break;
                case LoaderType.AudioClip:
                    {
                        if (webRequest.downloadHandler == null || webRequest.downloadHandler.GetType() != typeof(DownloadHandlerAudioClip))
                        {
                            webRequest.downloadHandler = new DownloadHandlerAudioClip(URL, audioType);
                        }
                    }
                    break;
            }

            //文件上锁
            if (!string.IsNullOrEmpty(lockPath) || !string.IsNullOrEmpty(lockUrl))
            {
                curCount++;
                List<string> list = ListPool.Instance.GetOneStringList();
                list.Add(lockPath);
                list.Add(lockUrl);
                FileLock.GetFilePathsLock(list, (obj,resList) => {
                    HttpLoader loader = (HttpLoader)obj;
                    loader.fileLockDataList = resList;
                    loader.requestAsync = loader.webRequest.SendWebRequest();
                    loader.requestAsync.completed -= loader.Completed;
                    loader.requestAsync.completed += loader.Completed;
                    GameCoroutine.Instance.StartCoroutine(loader.LoadProgress());
                },this);
            }
            else
            {
                requestAsync = webRequest.SendWebRequest();
                requestAsync.completed -= Completed;
                requestAsync.completed += Completed;
                curCount++;
                GameCoroutine.Instance.StartCoroutine(LoadProgress());
            }
        }

        IEnumerator LoadProgress()
        {
            while (!isFinish)
            {
                progress=requestAsync.progress;
                yield return null;
            }
        }

        static Dictionary<string, HttpLoader> curLoadingLoaders = new Dictionary<string, HttpLoader>();

        /// <summary>
        /// 最大同时下载数量
        /// </summary>
        public static int MaxCount = 6;

        /// <summary>
        /// 当前同时下载数量
        /// </summary>
        static int curCount;

        static List<WaitData> waitList = new List<WaitData>();

        static SimplePool<WaitData> waitDataPool = new SimplePool<WaitData>();

        class WaitData : ISimplePoolData
        {
            public HttpLoader HttpLoader;

            public System.Action<HttpLoader> LoadCallBack;

            public LoaderType LoaderType;

            public string FileSavePath;

            public bool TextureReadable;

            public UnityEngine.AudioType AudioType;

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
                HttpLoader = null;
                LoadCallBack = null;
                FileSavePath = null;
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

        public enum LoaderType
        {
            Buffer,
            SaveFile,
            Texture,
            AssetBundle,
            AudioClip,
        }

    }

}

