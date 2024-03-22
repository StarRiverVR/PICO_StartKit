using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 上传文件到HttpServer服务器
    /// </summary>
    public class HttpClient
    {

#if UNITY_EDITOR

        //[UnityEditor.MenuItem("Tools/Http/文件上传测试")]
        static void UploadTest()
        {
            string str = "abs发送测试";
            byte[] datas = Encoding.UTF8.GetBytes(str);
            SendAsyn("http://10.10.2.102:40133/?savePath=askd/ServerSaveDir/tyy.txt", datas, null, (str, err) =>
            {
                VLog.Info($"服务器返回:{str}");
            });
            List<string> filePaths = new List<string>();
            filePaths.Add("D:/Softs/web服务器.rar");
            filePaths.Add("D:/Softs/svn.rar");
            filePaths.Add("D:/Softs/xNormal.rar");
            filePaths.Add("Apifox-windows-latest.zip");
            filePaths.Add("D:/Softs/Clash_for_Windows_v0.19.11_x64_CN.rar");
            SendFileAsyn("http://10.10.2.102:40133/", filePaths, (p) => {
                Debug.Log(p);
            }, (bl) => {
                Debug.Log("下载完成:"+ bl);
            });

            SendFileAsyn("http://10.10.2.102:40133/?savePath=askd/ServerSaveDir/tyy.txt", "D:/Softs/web服务器.rar", (p) => {
                Debug.Log(p);
            }, (returnStr,err) => {
                Debug.Log("下载完成 returnStr:" + returnStr);
                Debug.Log("下载完成 err:" + err);
            });
        }

        //[UnityEditor.MenuItem("Tools/Http/服务器连通性测试")]
        static void SeverConnectTest()
        {
            SeverConnectAsyn("http://192.168.1.7:40133/", (p) =>
            {
                Debug.Log(p);
            }, (bl) => {
            });
        }

        //[UnityEditor.MenuItem("Tools/Http/文件下载测试")]
        static void FileLoadTest()
        {
            LoadFIleAsyn("http://192.168.1.7:40133/ABC/jjj.txt", (p) =>
            {
               // Debug.Log(p);
            }, (datas) => {
                if (datas==null)
                {
                    VLog.Error("下载数据为空");
                }
                else
                {
                    VLog.Info($"下载数据长度={datas.Length}");
                }
            });
        }

#endif

        static int CurCount = 0;

        /// <summary>
        /// 同时最大上传数量
        /// </summary>
        const int maxCount = 5;

        static List<WaitData> waitDataList = new List<WaitData>();

        static void AddWaitData(WaitData waitData)
        {
            waitDataList.Add(waitData);
        }

        /// <summary>
        /// 服务器连通性测试
        /// </summary>
        /// <param name="httpBaseUrl">根路径: http://10.10.2.102:40133/</param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack"></param>
        public static void SeverConnectAsyn(string httpBaseUrl, System.Action<float> uploadProgress, System.Action<bool> callBack)
        {
            string serverUrl = httpBaseUrl + "?type=connectTest";
            SendAsyn(serverUrl, Encoding.UTF8.GetBytes("SeverConnect"), uploadProgress, (returnStr, err) => {
                if (!string.IsNullOrEmpty(returnStr) && returnStr.CompareTo("IsConnect") ==0)
                {
                    VLog.Info("Http服务器已连通！");
                    callBack(true);
                }
                else
                {
                    VLog.Error($"服务器未连通！ returnStr={returnStr} err={err}");
                    callBack(false);
                }
            });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="loadProgress"></param>
        /// <param name="callBack"></param>
        public static void LoadFIleAsyn(string httpUrl, System.Action<float> loadProgress, System.Action<byte[]> callBack)
        {
            List<FileHttpLoadeData> datas = FileHttpLoad.GetOneList();
            FileHttpLoadeData data = FileHttpLoad.GetOneData(httpUrl,0);
            datas.Add(data);
            FileHttpLoad newLoad = new FileHttpLoad(datas, (path,datas,loadData) => {
                callBack(datas);
            }, (bl,o, fileHttpLoad) => { }, (p1,p2) => {
                loadProgress(p1);
            },null,25);
        }

        /// <summary>
        /// 异步上传文件 如果没有手动指定savePath=存储目录，则会使用文件名做为存储参数
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="filePath"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack">上传完成后服务器返回内容，错误信息</param>
        public static void SendFileAsyn(string httpUrl, string filePath, System.Action<float> uploadProgress, System.Action<string, string> callBack)
        {
            if (System.Threading.SynchronizationContext.Current!= ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    SendFileAsynRun(httpUrl, filePath, uploadProgress, callBack);
                }, null);
            }
            else
            {
                SendFileAsynRun(httpUrl, filePath, uploadProgress, callBack);
            }
        }

        static void SendFileAsynRun(string httpUrl, string filePath, System.Action<float> uploadProgress, System.Action<string, string> callBack)
        {
            if (filePath == null || filePath.Length == 0 || httpUrl == null || httpUrl.Length == 0)
            {
                if (uploadProgress != null)
                {
                    uploadProgress(100);
                }
                if (callBack != null)
                {
                    callBack(null, "上传数据为0");
                }
                return;
            }
            if (CurCount >= maxCount)
            {
                WaitData waitData = waitDataPool.Spawn();
                waitData.WaitType = WaitType.File;
                waitData.httpUrl = httpUrl;
                waitData.uploadProgress = uploadProgress;
                waitData.callBack = callBack;

                waitData.filePath = filePath;
                AddWaitData(waitData);
                return;
            }
            if (!httpUrl.Contains("savePath="))
            {
                string fileName = filePath.GetNameWithSuffix();
                if (httpUrl.Contains("?"))
                {
                    httpUrl = httpUrl + ",savePath=" + fileName;
                }
                else
                {
                    httpUrl = httpUrl + "?savePath=" + fileName;
                }
            }
            CurCount++;
            SendFileAsynData parData = sendFileAsynDataPool.Spawn();
            parData.httpUrl = httpUrl;
            parData.uploadProgress = uploadProgress;
            parData.callBack = callBack;
            FileReadHelper.Instance.ReadByteArrayAsyn(filePath, (path, datas, obj) =>
            {
                SendFileAsynData sendFileAsynData = (SendFileAsynData)obj;
                if (datas == null || datas.Length == 0 || sendFileAsynData.httpUrl == null || sendFileAsynData.httpUrl.Length == 0)
                {
                    CurCount--;
                    if (sendFileAsynData.uploadProgress != null)
                    {
                        sendFileAsynData.uploadProgress(100);
                    }
                    if (sendFileAsynData.callBack != null)
                    {
                        sendFileAsynData.callBack(null, "上传数据为0");
                    }
                    return;
                }
                else
                {
                    GameCoroutine.Instance.StartCoroutine(UploadBytes(sendFileAsynData.httpUrl, datas, sendFileAsynData.uploadProgress, sendFileAsynData.callBack));
                }
            }, (bl, path, obj) =>
            {

            }, parData, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 异步上传文件 如果没有手动指定savePath=存储目录，则会使用文件名做为存储参数
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="filePaths"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack"></param>
        public static void SendFileAsyn(string httpUrl, List<string> filePaths, System.Action<float> uploadProgress, System.Action<bool> callBack)
        {
            CountData countData = countDataPool.Spawn();
            countData.Count = filePaths.Count;
            int count = filePaths.Count;
            int index = 0;
            for (int i=0;i< filePaths.Count;++i)
            {
                SendFileAsyn(httpUrl, filePaths[i], null, (str,err) => {
                    if (!string.IsNullOrEmpty(err))
                    {
                        countData.AllIsTrue = false;
                    }
                    index++;
                    countData.Count--;
                    if (uploadProgress!=null)
                    {
                        uploadProgress((index / (float)count) * 100);
                    }
                    if (countData.Count==0)
                    {
                        callBack(countData.AllIsTrue);
                        countDataPool.Recycle(countData);
                    }
                });
            }
        }

        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="datas"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack">上传完成后服务器返回内容，错误信息</param>
        public static void SendAsyn(string httpUrl, byte[] datas,System.Action<float> uploadProgress, System.Action<string,string> callBack)
        {
            if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    SendAsynRun( httpUrl,  datas,  uploadProgress,callBack);
                }, null);
            }
            else
            {
                SendAsynRun(httpUrl, datas, uploadProgress, callBack);
            }
        }

        static void SendAsynRun(string httpUrl, byte[] datas, System.Action<float> uploadProgress, System.Action<string, string> callBack)
        {
            if (datas == null || datas.Length == 0 || httpUrl == null || httpUrl.Length == 0)
            {
                if (uploadProgress != null)
                {
                    uploadProgress(100);
                }
                if (callBack != null)
                {
                    callBack(null, "上传数据为0");
                }
                return;
            }
            if (CurCount >= maxCount)
            {
                WaitData waitData = waitDataPool.Spawn();
                waitData.WaitType = WaitType.Bytes;
                waitData.httpUrl = httpUrl;
                waitData.uploadProgress = uploadProgress;
                waitData.callBack = callBack;
                waitData.datas = datas;
                AddWaitData(waitData);
                return;
            }
            CurCount++;
            GameCoroutine.Instance.StartCoroutine(UploadBytes(httpUrl, datas, uploadProgress, callBack));
        }

        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="filePaths"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack"></param>
        public static void SendAsyn(string httpUrl, List<byte[]> datasList, System.Action<float> uploadProgress, System.Action<bool> callBack)
        {
            CountData countData = countDataPool.Spawn();
            countData.Count = datasList.Count;
            int count = datasList.Count;
            int index = 0;
            for (int i = 0; i < datasList.Count; ++i)
            {
                SendAsyn(httpUrl, datasList[i], null, (str, err) => {
                    if (!string.IsNullOrEmpty(err))
                    {
                        countData.AllIsTrue = false;
                    }
                    index++;
                    countData.Count--;
                    if (uploadProgress != null)
                    {
                        uploadProgress((index / (float)count) * 100);
                    }
                    if (countData.Count == 0)
                    {
                        callBack(countData.AllIsTrue);
                        countDataPool.Recycle(countData);
                    }
                });
            }
        }

        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="txt"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack">上传完成后服务器返回内容，错误信息</param>
        public static void SendAsyn(string httpUrl, string txt, System.Action<float> uploadProgress, System.Action<string, string> callBack)
        {
            if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                    SendAsynRun( httpUrl,  txt,  uploadProgress,  callBack);
                }, null);
            }
            else
            {
                SendAsynRun(httpUrl, txt, uploadProgress, callBack);
            }
        }

        static void SendAsynRun(string httpUrl, string txt, System.Action<float> uploadProgress, System.Action<string, string> callBack)
        {
            if (string.IsNullOrEmpty(txt) || httpUrl == null || httpUrl.Length == 0)
            {
                if (uploadProgress != null)
                {
                    uploadProgress(100);
                }
                if (callBack != null)
                {
                    callBack(null, "上传数据为0");
                }
                return;
            }
            if (CurCount >= maxCount)
            {
                WaitData waitData = waitDataPool.Spawn();
                waitData.WaitType = WaitType.Text;
                waitData.httpUrl = httpUrl;
                waitData.uploadProgress = uploadProgress;
                waitData.callBack = callBack;
                waitData.txt = txt;
                AddWaitData(waitData);
                return;
            }
            CurCount++;
            GameCoroutine.Instance.StartCoroutine(UploadBytes(httpUrl, Encoding.UTF8.GetBytes(txt), uploadProgress, callBack));
        }

        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="httpUrl"></param>
        /// <param name="filePaths"></param>
        /// <param name="uploadProgress"></param>
        /// <param name="callBack"></param>
        public static void SendAsyn(string httpUrl, List<string> txtList, System.Action<float> uploadProgress, System.Action<bool> callBack)
        {
            CountData countData = countDataPool.Spawn();
            countData.Count = txtList.Count;
            int count = txtList.Count;
            int index = 0;
            for (int i = 0; i < txtList.Count; ++i)
            {
                SendAsyn(httpUrl, txtList[i], null, (str, err) => {
                    if (!string.IsNullOrEmpty(err))
                    {
                        countData.AllIsTrue = false;
                    }
                    index++;
                    countData.Count--;
                    if (uploadProgress != null)
                    {
                        uploadProgress((index / (float)count) * 100);
                    }
                    if (countData.Count == 0)
                    {
                        callBack(countData.AllIsTrue);
                        countDataPool.Recycle(countData);
                    }
                });
            }
        }

        static IEnumerator UploadBytes(string httpUrl, byte[] datas, System.Action<float> uploadProgress, System.Action<string,string> callBack)
        {
            UnityWebRequest www=null;
            try
            {
                www = UnityWebRequest.Put(httpUrl, datas);
                www.SetRequestHeader("Content-Type", "application/octet-stream");
                www.timeout = 15;
                www.SendWebRequest();
            }
            catch (System.Exception ex)
            {
                if (www!=null)
                {
                    www.Dispose();
                }
                if (uploadProgress != null)
                {
                    uploadProgress(100);
                }
                if (callBack != null)
                {
                    callBack(null, ex.Message);
                }
                VLog.Exception(ex);
                yield break;
            }

            while (!www.isDone)
            {
                if (uploadProgress != null)
                {
                    float p = www.uploadProgress * 100;
                    try
                    {
                        uploadProgress(p);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Exception(ex);
                    }
                }
                yield return null;
            }

            try
            {
                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (callBack != null)
                    {
                        callBack(www.downloadHandler.text, www.error);
                    }
                }
                else
                {
                    if (callBack != null)
                    {
                        callBack(www.downloadHandler.text, null);
                    }
                }
                VLog.Info($"ServerReturn={www.downloadHandler.text} \n Err={www.error} \n HttpUrl={httpUrl}");
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }

            www.Dispose();

            CurCount--;
            if (waitDataList.Count>0)
            {
                WaitData waitData = waitDataList[0];
                waitDataList.RemoveAt(0);
                switch (waitData.WaitType)
                {
                    case WaitType.File:
                        {
                            SendFileAsyn(waitData.httpUrl, waitData.filePath, waitData.uploadProgress, waitData.callBack);
                        }
                        break;
                    case WaitType.Bytes:
                        {
                            SendAsyn(waitData.httpUrl, waitData.datas, waitData.uploadProgress, waitData.callBack);
                        }
                        break;
                    case WaitType.Text:
                        {
                            SendAsyn(waitData.httpUrl, waitData.txt, waitData.uploadProgress, waitData.callBack);
                        }
                        break;
                }
                waitDataPool.Recycle(waitData);
            }
        }

        static SimplePool<SendFileAsynData> sendFileAsynDataPool = new SimplePool<SendFileAsynData>();

        class SendFileAsynData : ISimplePoolData
        {
            public string httpUrl;

            public System.Action<float> uploadProgress;

            public System.Action<string, string> callBack;

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
                uploadProgress = null;
                callBack = null;
                httpUrl = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        enum WaitType
        {
            File,
            Text,
            Bytes,
        }

        static SimplePool<WaitData> waitDataPool = new SimplePool<WaitData>();

        class WaitData : ISimplePoolData
        {
            public string httpUrl;

            public System.Action<float> uploadProgress;

            public System.Action<string, string> callBack;

            public string filePath;

            public byte[] datas;

            public string txt;

            public WaitType WaitType;

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
                datas = null;
                uploadProgress = null;
                callBack = null;
                filePath = null;
                httpUrl = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        static SimplePool<CountData> countDataPool = new SimplePool<CountData>();

        class CountData : ISimplePoolData
        {
            public bool AllIsTrue = true;

            public int Count = 0;

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
                AllIsTrue = true;
                Count = 0;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

    }
}
