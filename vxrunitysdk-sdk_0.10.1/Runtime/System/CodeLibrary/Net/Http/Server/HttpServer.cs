using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// Put/Post 用于接收存储文件，文件由HttpClient发送  Url="http://10.10.2.102:40133/?savePath=askd/kdfsf/tyy.txt" savePath=存储目录
    /// Get 用于客户端下载文件  "http://192.168.0.104:40133/ABC/jjj.txt"
    /// </summary>
    public class HttpServer
    {
#if UNITY_EDITOR

        //[UnityEditor.MenuItem("Tools/Http/开启Http服务")]
        static void OpenHttpServer()
        {
            StartHttpServer();
        }

#endif

        static string httpSaveDirectory;

        public static string HttpSaveDirectory
        {
            get
            {
                if (httpSaveDirectory == null)
                {
                    httpSaveDirectory = FilePath.LocalStorageDir + "Http/";
                }
                return httpSaveDirectory;
            }
        }

        static string defaultHttpSavePath;

        public static string DefaultHttpSavePath
        {
            get
            {
                if (defaultHttpSavePath == null)
                {
                    defaultHttpSavePath = HttpSaveDirectory + "DefaultHttpData.txt";
                }
                return defaultHttpSavePath;
            }
        }

        static HttpServer curHttpServer;

        static object lockObj = new object();

        static List<string> ipList = null;

        static int ipIndex = 0;

        static int port = 40133;

        /// <summary>
        /// 开启一个Http服务器 ，每次开启会轮询本设备的所有IP开启
        /// </summary>
        /// <returns></returns>
        public static string StartHttpServer()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("HttpServer.cs-->71-->StartHttpServer"));
                }
                #region //IP 获取，每次重新开启会去轮询本地的IP地址

                if (ipList == null)
                {
                    ipIndex = 0;
                    //本地IP列表
                    ipList = IPUtility.GetLocalIpAddress("InterNetwork");
                    //外网IP
                    string outerNetIp = IPUtility.GetOuterNet();
                    if (!string.IsNullOrEmpty(outerNetIp))
                    {
                        VLog.Info($"外网Ip={outerNetIp}");
                        ipList.Add(outerNetIp);
                    }
                    for (int i = 0; i < ipList.Count; ++i)
                    {
                        VLog.Info($"Http Ip[{i}]={ipList[i]}");
                    }
                }
                if (ipIndex < 0 || ipIndex >= ipList.Count)
                {
                    ipIndex = 0;
                }
                string ip = ipList[ipIndex];
                VLog.Info($"Http Ip={ip} Index={ipIndex} Port={port}");
                string url = string.Format("http://{0}:{1}/", ip, port);
                VLog.Info($"HttpUrl={url}");
                ipIndex++;
                if (ipIndex >= ipList.Count)
                {
                    ipIndex = 0;
                }

                #endregion

                if (curHttpServer != null)
                {
                    curHttpServer.Close();
                    curHttpServer = null;
                }
                curHttpServer = new HttpServer();
                string httpUrl = curHttpServer.Start(url);
                if (string.IsNullOrEmpty(httpUrl))
                {
                    curHttpServer.Close();
                    curHttpServer = null;
                    VLog.Error("HttpServer开启失败");
                }
                else
                {
                    VLog.Info($"HttpServer开启成功: httpUrl={httpUrl} HttpSaveDirectory={HttpSaveDirectory}");
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                return httpUrl;
            }
        }

        public static void CloseHttpServer()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("HttpServer.cs-->135-->CloseHttpServer"));
                }
                if (curHttpServer != null)
                {
                    curHttpServer.Close();
                    curHttpServer = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static bool HttpServerIsStart()
        {
            lock (lockObj)
            {
                if (curHttpServer != null)
                {
                    return true;
                }
                return false;
            }
        }

        public static string GetHttpUrl()
        {
            lock (lockObj)
            {
                if (curHttpServer != null)
                {
                    return curHttpServer.HttpUrl;
                }
                return null;
            }
        }

        ~HttpServer()
        {
            Close();
        }

        HttpListener httpobj;

        string httpUrl;

        public string HttpUrl
        {
            get
            {
                return httpUrl;
            }
        }

        /// <summary>
        /// 开启Http服务器，返回HttpUrl
        /// </summary>
        /// <returns>返回HttpUrl</returns>
        string Start(string url)
        {
            try
            {
                httpUrl = url;
                httpobj = new HttpListener();
                httpobj.Prefixes.Add(httpUrl);
                httpobj.Start();
                httpobj.BeginGetContext(Result, null);
                return httpUrl;
            }
            catch (System.Exception ex)
            {
                VLog.Error($"Http服务器开启异常: httpUrl={httpUrl} ex={ex.Message}");
                VLog.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// URL参数解析
        /// 参数 savePath:文件保存路径  如: savePath=ABC/Jk.txt
        ///      type:功能类型 如:type=connectTest //测试Http连通性,连通成功返回 IsConnect
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        Dictionary<string,string> AnalyticParameter(string rawUrl)
        {
            Dictionary<string, string> paramDic = DictionaryPool.Instance.GetOneStringStringDic();
            int index = rawUrl.IndexOf("?");
            if (index>=0)
            {
                if (index>1)
                {
                    //URL中的文件路径 如："http://192.168.0.104:40133/ABC/jjj.txt"  urlFilePath="ABC/jjj.txt"
                    paramDic.Add("urlFilePath", rawUrl.Substring(1, index-1));
                }
                rawUrl = rawUrl.Substring(index+1, rawUrl.Length- index- 1);
                string[] strs = rawUrl.Split(',');
                for (int i=0;i< strs.Length;++i)
                {
                    string str = strs[i];
                    int indexX = str.IndexOf('=');
                    if (indexX>0)
                    {
                        string key = str.Substring(0, indexX).Trim();
                        string value= str.Substring(indexX+1, str.Length- indexX-1).Trim();
                        paramDic.Add(key, value);
                    }
                }
            }
            return paramDic;
        }

        Dictionary<string, byte[]> utf8StrBytes = new Dictionary<string, byte[]>();

        byte[] FindUtf8StrBytes(string str)
        {
            if (str==null)
            {
                str = "";
            }
            if (str.Length>256 || utf8StrBytes.Count>1024)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            byte[] res = null;
            if (!utf8StrBytes.TryGetValue(str,out res))
            {
                res=Encoding.UTF8.GetBytes(str);
                utf8StrBytes.Add(str, res);
            }
            return res;
        }

        /// <summary>
        /// 接受客户端请求
        /// </summary>
        /// <param name="ar"></param>
        void Result(IAsyncResult ar)
        {
            try
            {
                //继续异步监听
                httpobj.BeginGetContext(Result, null);
                HttpListenerContext context = httpobj.EndGetContext(ar);
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Dictionary<string, string> paramDic = AnalyticParameter(request.RawUrl);
                //
                //DebugLog.LogFormat("{0} {1} HTTP/1.1", true, request.HttpMethod, request.RawUrl);
                //DebugLog.LogFormat("Accept: {0}", true, string.Join(",", request.AcceptTypes));
                //DebugLog.LogFormat("User-Agent: {0}", true, request.UserAgent);
                //DebugLog.LogFormat("Accept-Encoding: {0}", true, request.Headers["Accept-Encoding"]);
                //DebugLog.LogFormat("Connection: {0}", true, request.KeepAlive ? "Keep-Alive" : "close");
                //DebugLog.LogFormat("Host: {0}", true, request.Url);
                //DebugLog.LogFormat("RemoteEndPoint: {0}", true, request.RemoteEndPoint);
                //如果是js的ajax请求，还可以设置跨域的ip地址与参数
                //context.Response.AppendHeader("Access-Control-Allow-Origin", "*");//后台跨域请求，通常设置为配置文件
                //context.Response.AppendHeader("Access-Control-Allow-Headers", "ID,PW");//后台跨域参数设置，通常设置为配置文件
                //context.Response.AppendHeader("Access-Control-Allow-Method", "post");//后台跨域请求设置，通常设置为配置文件
                //
                context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
                                                                          //context.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
                context.Response.AddHeader("Content-type", "application/octet-stream");//添加响应头信息
                context.Response.ContentEncoding = Encoding.UTF8;
                string returnStr = null;//定义返回客户端的信息
                string connectTest = null;
                paramDic.TryGetValue("type", out connectTest);
                if (!string.IsNullOrEmpty(connectTest) && connectTest.CompareTo("connectTest") ==0)
                {
                    DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
                    //连通性测试
                    returnStr = "IsConnect";
                    VLog.Info($"连通性测试: serverHttpUrl={httpUrl} \n httpMethod={request.HttpMethod} \n clientHost={request.Url} \n msg={returnStr}");
                }
                else
                {
                    if ((request.HttpMethod.CompareTo("PUT") == 0 || request.HttpMethod.CompareTo("POST") == 0) && request.InputStream != null)
                    {
                        //客户端上传文件到服务器
                        //处理客户端发送的请求并返回处理信息
                        if (request.InputStream != null)
                        {
                            returnStr = HandleRequest(request, response, paramDic);
                            VLog.Info($"服务器反馈消息： serverHttpUrl={httpUrl} \n httpMethod={request.HttpMethod} \n clientHost={request.Url} \n msg={returnStr}");
                        }
                        else
                        {
                            DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
                            returnStr = $"InputStream为空";
                            VLog.Error($"InputStream为空: serverHttpUrl={httpUrl} \n httpMethod={request.HttpMethod} \n clientHost={request.Url}");
                        }
                    }
                    else
                    {
                        //客户端从服务器下载文件
                        string urlFilePath = null;
                        paramDic.TryGetValue("urlFilePath", out urlFilePath);
                        if (!string.IsNullOrEmpty(urlFilePath))
                        {
                            returnStr = null;
                            FileReadHelper.Instance.ReadByteArrayAsyn(HttpSaveDirectory+ urlFilePath, (p, datas, o) => {
                                if (datas!=null)
                                {
                                    try
                                    {
                                        using (System.IO.Stream stream = response.OutputStream)
                                        {
                                            //把处理信息返回到客户端
                                            stream.Write(datas, 0, datas.Length);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        VLog.Error($"Http服务器挂掉了: serverHttpUrl={httpUrl} ex={ex.ToString()}");
                                    }
                                }
                                else
                                {
                                    VLog.Error($"下载文件读取失败: serverHttpUrl={httpUrl} \n httpMethod={request.HttpMethod} \n clientHost={request.Url} \n urlFilePath={urlFilePath}");
                                    try
                                    {
                                        //返回客户端
                                        byte[] readFileDatas = FindUtf8StrBytes("");//设置客户端返回信息的编码
                                        using (System.IO.Stream stream = response.OutputStream)
                                        {
                                            //把处理信息返回到客户端
                                            stream.Write(readFileDatas, 0, readFileDatas.Length);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        VLog.Error($"Http服务器挂掉了: serverHttpUrl={httpUrl} ex={ex.ToString()}");
                                    }
                                }
                            },(bl,str,o)=> { },null,System.Text.Encoding.UTF8);
                            DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
                        }
                        else
                        {
                            DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
                            returnStr = null;
                            VLog.Error($"不是post/put请求或者传过来的数据为空: serverHttpUrl={httpUrl} \n httpMethod={request.HttpMethod} \n clientHost={request.Url}");
                            try
                            {
                                //返回客户端
                                byte[] readFileDatas = FindUtf8StrBytes("");//设置客户端返回信息的编码
                                using (System.IO.Stream stream = response.OutputStream)
                                {
                                    //把处理信息返回到客户端
                                    stream.Write(readFileDatas, 0, readFileDatas.Length);
                                }
                            }
                            catch (Exception ex)
                            {
                                VLog.Error($"Http服务器挂掉了: serverHttpUrl={httpUrl} ex={ex.ToString()}");
                            }
                            DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
                        }
                    }
                }
                
                try
                {
                    if (returnStr!=null)
                    {
                        //返回客户端
                        byte[] returnByteArr = FindUtf8StrBytes(returnStr);//设置客户端返回信息的编码
                        using (System.IO.Stream stream = response.OutputStream)
                        {
                            //把处理信息返回到客户端
                            stream.Write(returnByteArr, 0, returnByteArr.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    VLog.Error($"Http服务器挂掉了: serverHttpUrl={httpUrl} ex={ex.ToString()}");
                }
            }
            catch (System.Exception ex)
            {
                VLog.Error($"Http服务器异常: serverHttpUrl={httpUrl} ex={ex.ToString()}");
            }
        }

        SimpleListPool<List<byte>, byte> byteListPool = new SimpleListPool<List<byte>, byte>();

        ByteArrayPool byteArrayPool = new ByteArrayPool();

        /// <summary>
        /// 处理客户端数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        string HandleRequest(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> paramDic)
        {
            try
            {
                byte[] byteArr = byteArrayPool.Spawn(2048);
                List<byte> byteList = byteListPool.Spawn();
                byteList.Clear();
                int readLen = 0;
                int len = 0;
                do
                {
                    readLen = request.InputStream.Read(byteArr, 0, byteArr.Length);
                    len += readLen;
                    for (int i=0;i< readLen;++i)
                    {
                        byteList.Add(byteArr[i]);
                    }
                } while (readLen != 0);
                byteArrayPool.Recycle(byteArr);

                VLog.Info($"Http文件接收长度: {byteList.Count}");

                //参数 savePath=ABC/Jk.txt
                string savePath = null;
                paramDic.TryGetValue("savePath", out savePath);
                if (!string.IsNullOrEmpty(savePath))
                {
                    savePath= HttpSaveDirectory + savePath;
                    FileWriteHelper.Instance.WriteAsyn(savePath, byteList.ToArray(), false, (path, parObj) =>
                    {
                        VLog.Info($"Http文件保存完成: path={path}");
                    }, null, true, true, true);
                }
                //else
                //{
                //    savePath = DefaultHttpSavePath;
                //}
                byteListPool.Recycle(byteList);
            }
            catch (Exception ex)
            {
                response.StatusDescription = "404";
                response.StatusCode = 404;
                VLog.Error($"在接收数据时发生错误: httpUrl={httpUrl} ex={ex.ToString()}");
                return $"在接收数据时发生错误:{ex.ToString()}";
            }
            DictionaryPool.Instance.PutBackOneStringStringDic(paramDic);
            response.StatusDescription = "200";
            response.StatusCode = 200;

            VLog.Info("HttpSever接收数据完成");
            return $"数据已接收";
        }


        void Close()
        {
            if (httpobj!=null)
            {
                try
                {
                    VLog.Info($"HttpServer关闭:{httpUrl}");
                    httpobj.Stop();
                    httpobj.Close();
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                }
                httpobj = null;
            }
        }

    }
}

