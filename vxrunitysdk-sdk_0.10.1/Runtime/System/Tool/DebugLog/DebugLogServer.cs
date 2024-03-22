using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using proto;

namespace com.vivo.codelibrary
{
    public class DebugLogServer : SocketServer
    {

        public static DebugLogServer CurLogServer;

        static object lockObj = new object();

        public static void Log(ClientData clientData, string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->27-->Log");
                }
                if (CurLogServer != null)
                {
                    CurLogServer.SendLog(clientData, str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void LogWarning(ClientData clientData, string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->39-->LogWarning");
                }
                if (CurLogServer != null)
                {
                    CurLogServer.SendLogWarning(clientData, str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void LogError(ClientData clientData, string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->53-->LogError");
                }
                if (CurLogServer != null)
                {
                    CurLogServer.SendLogError(clientData, str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        static void DebugIps()
        {
            //无路由
            List<string> ipv4_ips = IPUtility.GetLocalIpAddress("InterNetwork");
            for (int i=0;i< ipv4_ips.Count;++i)
            {
                VLog.Warning(ipv4_ips[i]);
            }
            ////外网
            //string t0_html = IPUtility.HttpGetPageHtml("http://www.net.cn/static/customercare/yourip.asp", "gbk");
            //Debug.LogWarning(string.Format("外网:{0}", IPUtility.GetIPFromHtml(t0_html)));
        }

        public static bool OpenServer()
        {
            return Open(DebugLogCoroutine.NetPort);
        }

        public static bool Open(int port)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->84-->Open");
                }
                if (CurLogServer == null)
                {
                    CurLogServer = new DebugLogServer();
                }
                bool bl = CurLogServer.OpenServer(port);
                if (bl)
                {
                    DebugIps();
                }
                else
                {
                    CurLogServer = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                return bl;
            }
        }

        public static bool OpenServer(string serverIp)
        {
           return Open( serverIp, DebugLogCoroutine.NetPort);
        }

        public static bool Open(string serverIp, int port)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->108-->Open");
                }
                if (CurLogServer == null)
                {
                    CurLogServer = new DebugLogServer();
                }
                bool bl = CurLogServer.OpenServer(serverIp, port);
                if (bl)
                {
                    DebugIps();
                }
                else
                {
                    CurLogServer = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                return bl;
            }
        }

        public static void Close()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogServer.cs-->127-->Close");
                }
                if (CurLogServer == null)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                }
                CurLogServer.CloseServer();
                CurLogServer = null;
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 根据协议号 处理接收到的数据
        /// </summary>
        /// <param name="tcpSocket"></param>
        /// <param name="clientData"></param>
        /// <param name="resData"></param>
        protected override void ProcessingData(Socket tcpSocket, ClientData clientData, Proto_Package_Data resData)
        {
            try
            {
                switch (resData.agreement)
                {
                    case (int)Proto_Agreement_Enum.Debug_JsonData:
                        {
                            //log_data getData = proto.Proto.GetOneProtoData<log_data>();
                            //resData.GetData(getData);
                            //if (getData.processId != ProcessId)
                            //{
                            //    JsonData jsonData = JsonMapper.ToObject(getData.logStr);
                            //    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.JsonData, true, jsonData);
                            //}
                            //proto.Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                VLog.Error($"服务端处理客户端数据异常:[err]={ex.Message}");
            }

        }
    }
}

