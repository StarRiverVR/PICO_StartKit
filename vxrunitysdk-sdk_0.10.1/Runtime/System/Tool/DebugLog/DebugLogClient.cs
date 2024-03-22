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
    public class DebugLogClient : SocketClient
    {
        public static DebugLogClient CurClient;

        static object lockObj = new object();

        public static void JsonData(int index,string logStr)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->26-->Log");
                }
                if (CurClient != null)
                {
                    log_data data = proto.Proto.GetOneProtoData<log_data>();
                    data.logStr = logStr;
                    data.processId = CurClient.ProcessId;
                    data.index = index;
                    CurClient.Send(data, (int)Proto_Agreement_Enum.Debug_JsonData);
                    proto.Proto.PutBackOneProtoData<log_data>(data);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void Log(string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->26-->Log");
                }
                if (CurClient != null)
                {
                    CurClient.SendLog(str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void LogWarning(string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->39-->LogWarning");
                }
                if (CurClient != null)
                {
                    CurClient.SendLogWarning(str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void LogError(string str)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->52-->LogError");
                }
                if (CurClient != null)
                {
                    CurClient.SendLogError(str);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        public static void OpenAsync(System.Action<bool> callBack,string ipAdr, int port, System.Action<bool> restartConnect, System.Action closeCallBack)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->71-->Open");
                }
                if (CurClient == null)
                {
                    CurClient = new DebugLogClient();
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
            CurClient.OpenClientAsync(callBack,ipAdr, port, restartConnect, closeCallBack);
        }

        //public static bool Open(string ipAdr, System.Action<bool> restartConnect, System.Action closeCallBack)
        //{
        //    return Open(ipAdr, 21001, restartConnect, closeCallBack);
        //    //return CurClient.OpenClient(ipAdr, 21001, restartConnect, closeCallBack);
        //}

        public static bool Open(string ipAdr,int port, System.Action<bool> restartConnect, System.Action closeCallBack)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->71-->Open");
                }
                if (CurClient == null)
                {
                    CurClient = new DebugLogClient();
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
            return CurClient.OpenClient(ipAdr, port, restartConnect, closeCallBack);
        }

        public static void Close()
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogClient.cs-->85-->Close");
                }
                if (CurClient == null)
                {
                    return;
                }
                CurClient.CloseClient();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="package_Data"></param>
        protected override void ProcessingData(Proto_Package_Data package_Data)
        {

        }
    }
}

