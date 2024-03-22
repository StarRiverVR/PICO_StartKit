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
using System.Diagnostics;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// Socket客户端
    /// //使用方法 继承后重写 protected virtual void ProcessingData(Proto_Package_Data package_Data)
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SocketClient
    {

#if UNITY_EDITOR
        static SocketClient()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChange;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChange;
        }

        static void PlayModeStateChange(UnityEditor.PlayModeStateChange mod)
        {
            if (mod == UnityEditor.PlayModeStateChange.ExitingEditMode || mod == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                CloseAllClients();
            }
        }
        static void CloseAllClients()
        {
            lock (allAsyncTcpClients)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->45-->CloseAllClients"));
                }
                for (int i = 0; i < allAsyncTcpClients.Count; ++i)
                {
                    if (allAsyncTcpClients[i] != null)
                    {
                        allAsyncTcpClients[i].CloseClient();
                    }
                }
                allAsyncTcpClients.Clear();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        static List<SocketClient> allAsyncTcpClients = new List<SocketClient>();
#endif

        public SocketClient()
        {
#if UNITY_EDITOR
            lock (allAsyncTcpClients)
            {
                allAsyncTcpClients.Add(this);
            }
#endif
            processId = Process.GetCurrentProcess().Id;
        }

        int heartDeconnectTimeLenght = 25;

        /// <summary>
        /// 心跳断开连接时间长度
        /// </summary>
        public int HeartDeconnectTimeLenght
        {
            get
            {
                lock (lockObj)
                {
                    return heartDeconnectTimeLenght;
                }
            }
            set
            {
                lock (lockObj)
                {
                    heartDeconnectTimeLenght = value;
                    heartDeconnectTimeLenght = Mathf.Clamp(heartDeconnectTimeLenght, 5, int.MaxValue);
                }
            }
        }

        object lockObj = new object();

        int processId;

        public int ProcessId
        {
            get
            {
                lock (lockObj)
                {
                    return processId;
                }
            }
        }

        Socket client;

        IPAddress serverIp;

        public IPAddress ServerIp
        {
            get
            {
                lock (lockObj)
                {
                    return serverIp;
                }
            }
        }

        int clientPort = 12000;

        public int ClientPort
        {
            get
            {
                lock (lockObj)
                {
                    return clientPort;
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                lock (lockObj)
                {
                    if (client != null)
                    {
                        return client.Connected;
                    }
                    return false;
                }
            }
        }

        public virtual bool OpenClient(string ipAdr, int port, System.Action<bool> restartConnect, System.Action closeCallBack)
        {
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->158-->OpenClient"));
                }
                clientPort = port;
                try
                {
                    CloseClient();
                    isClose = false;
                    lock (curSendList)
                    {
                        curSendList.Clear();
                    }
                    clientCloseCallBack = closeCallBack;
                    reConnectLoopCount = 0;
                    hearSendCount = 0;
                    reConnectCallBack = restartConnect;
                    serverIp = IPAddress.Parse(ipAdr);
                    IPEndPoint endPoint = new IPEndPoint(ServerIp, ClientPort);
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult result = client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), client);
                    bool success = result.AsyncWaitHandle.WaitOne(1000, true);
                    if (!success)
                    {
                        //超时     
                        VLog.Error($"客户端创建失败：[ip]={ipAdr} [port]={ClientPort} [result]={result.ToString()}");
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        return false;
                    }
                    else
                    {
                        //数据接收
                        StartAcceptMsgsTask();
                        //状态判断
                        StartReConnectLoopTask();
                        //数据发送
                        StartSendPollTask();
                        //数据处理
                        StartProtoSplitPollTask();

                        VLog.Info($"客户端创建成功：[ip]={ipAdr} [port]={port}");
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        return true;
                    }
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"客户端创建异常：[ip]={ipAdr} [port]={port} [err]={ex.Message}");
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return false;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        void socketEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //数据接收
                StartAcceptMsgsTask();
                //状态判断
                StartReConnectLoopTask();
                //数据发送
                StartSendPollTask();
                //数据处理
                StartProtoSplitPollTask();

                VLog.Info($"客户端创建成功：[ip]={ipAdrAsync} [port]={portAsync}");
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errAsyncLockData);
                    errAsyncLockData = null;
                }
                asyncCallBack(true);
            }
            else
            {
                //超时     
                VLog.Error($"客户端创建失败：[ip]={ipAdrAsync} [port]={ClientPort} [result]={e.SocketError.ToString()}");
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errAsyncLockData);
                    errAsyncLockData = null;
                }

                asyncCallBack(false);
            }


        }

        System.Action<bool> asyncCallBack;
        ErrLockData errAsyncLockData = null;
        string ipAdrAsync;
        int portAsync;

        public virtual void OpenClientAsync(System.Action<bool> callBack, string ipAdr, int port, System.Action<bool> restartConnect, System.Action closeCallBack)
        {
            asyncCallBack = callBack;
            ipAdrAsync = ipAdr;
            portAsync = port;
            lock (lockObj)
            {
                ErrLockData errAsyncLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errAsyncLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->158-->OpenClient"));
                }
                clientPort = port;
                try
                {
                    CloseClient();
                    isClose = false;
                    lock (curSendList)
                    {
                        curSendList.Clear();
                    }
                    clientCloseCallBack = closeCallBack;
                    reConnectLoopCount = 0;
                    hearSendCount = 0;
                    reConnectCallBack = restartConnect;
                    serverIp = IPAddress.Parse(ipAdr);
                    IPEndPoint endPoint = new IPEndPoint(ServerIp, ClientPort);

                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs()
                    {
                        RemoteEndPoint = new IPEndPoint(serverIp, ClientPort)
                    };
                    socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(socketEventArg_Completed);
                    client.ConnectAsync(socketEventArg);
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"客户端创建异常：[ip]={ipAdr} [port]={port} [err]={ex.Message}");
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errAsyncLockData);
                    }
                    asyncCallBack(false);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errAsyncLockData);
                }
            }
        }

        void ConnectCallback(IAsyncResult asyncConnect)
        {

        }

        bool isClose = false;

        System.Action clientCloseCallBack;

        public virtual void CloseClient()
        {
            lock (lockObj)
            {
                if (isClose) return;
                isClose = true;
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->225-->CloseClient"));
                }
                StopReConnectLoopTask();
                StopAcceptMsgsTask();
                StopProtoSplitPollTask();
                StopSendPollTask();
                if (clientCloseCallBack != null)
                {
                    //Unity主线程执行重连回调
                    ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                        clientCloseCallBack();
                    }, null);
                }
                lock (curProtoSplitDatas)
                {
                    for (int i = 0; i < curProtoSplitDatas.Count; ++i)
                    {
                        for (int j = 0; j < curProtoSplitDatas[i].PackageDatas.Count; ++j)
                        {
                            Proto.PutBakOneProtoData(curProtoSplitDatas[i].PackageDatas[j]);
                        }
                        Proto.PutBackOneProtoDataListList(curProtoSplitDatas[i].PackageDatas);
                        protoSplitDataPool.Recycle(curProtoSplitDatas[i]);
                    }
                    curProtoSplitDatas.Clear();
                }
                if (client != null)
                {
                    try
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    try
                    {
                        client.Close();
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    client = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        #region//重连检测

        SocketHelper.TaskEndData reConnectLoopFlag;

        object lockReConnectLoop = new object();

        /// <summary>
        /// 断线重连回调
        /// </summary>
        System.Action<bool> reConnectCallBack;

        int reConnectLoopCount = 0;

        int hearSendCount = 0;

        void StartReConnectLoopTask()
        {
            lock (lockReConnectLoop)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->293-->StartReConnectLoopTask"));
                }
                StopReConnectLoopTask();
                reConnectLoopFlag = SocketHelper.GetOneTaskEndData();
                reConnectLoopFlag.TaskStop = false;
                Task.Factory.StartNew((obj) => {
                    SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                    ReConnectLoop(data);
                }, reConnectLoopFlag);
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        void StopReConnectLoopTask()
        {
            lock (lockReConnectLoop)
            {
                if (reConnectLoopFlag != null)
                {
                    reConnectLoopFlag.TaskStop = true;
                    reConnectLoopFlag = null;
                }
            }
        }

        void ReConnectLoop(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (lockReConnectLoop)
            {
                isTaskStop = data.TaskStop;
            }
            while (!isTaskStop)
            {
                lock (lockObj)
                {
                    lock (lockReConnectLoop)
                    {
                        isTaskStop = data.TaskStop;
                        if (isTaskStop)
                        {
                            break;
                        }
                    }
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->336-->ReConnectLoop"));
                    }
                    //发送心跳包
                    heart_data heartData = proto.Proto.GetOneProtoData<heart_data>();
                    heartData.res = 5;
                    Send(heartData, (int)Proto_Agreement_Enum.Heart_C);
                    proto.Proto.PutBackOneProtoData<heart_data>(heartData);
                    hearSendCount++;
                    int waitHeartCount = (HeartDeconnectTimeLenght * 1000) / 3000;
                    //
                    if (!IsConnected || hearSendCount >= waitHeartCount)
                    {
                        StopAcceptMsgsTask();
                        StopSendPollTask();
                        StopProtoSplitPollTask();
                        IPEndPoint endPoint = new IPEndPoint(ServerIp, ClientPort);
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IAsyncResult result = client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), client);
                        bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                        if (success)
                        {
                            Thread.Sleep(3000);
                            if (hearSendCount == 0)
                            {
                                VLog.Info($"客户端重连成功：[ip]={ServerIp.ToString()} [port]={ClientPort}");
                                //数据接收
                                StartAcceptMsgsTask();
                                //数据发送
                                StartSendPollTask();
                                //数据处理
                                StartProtoSplitPollTask();
                                reConnectLoopCount = 0;
                                if (reConnectCallBack != null)
                                {
                                    //Unity主线程执行重连回调
                                    ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                                        reConnectCallBack(true);
                                    }, null);
                                }
                            }
                            else
                            {
                                reConnectLoopCount++;
                                VLog.Info($"客户端重连失败{reConnectLoopCount}：[ip]={ServerIp.ToString()} [port]={ClientPort}");
                                if (reConnectCallBack != null)
                                {
                                    //Unity主线程执行重连回调
                                    ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                                        reConnectCallBack(false);
                                    }, null);
                                }
                                if (reConnectLoopCount >= 5)
                                {
                                    VLog.Info($"客户端无法重新连接服务器：[ip]={ServerIp.ToString()} [port]={ClientPort}");
                                    CloseClient();
                                    if (ErrLock.ErrLockOpen)
                                    {
                                        ErrLock.LockEnd(errLockData);
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            reConnectLoopCount++;
                            VLog.Info($"客户端重连失败{reConnectLoopCount}：[ip]={ServerIp.ToString()} [port]={ClientPort}");
                            if (reConnectCallBack != null)
                            {
                                //Unity主线程执行重连回调
                                ThreadHelper.UnitySynchronizationContext.Post((obj) => {
                                    reConnectCallBack(false);
                                }, null);
                            }
                            if (reConnectLoopCount >= 5)
                            {
                                VLog.Info($"客户端无法重新连接服务器：[ip]={ServerIp.ToString()} [port]={ClientPort}");
                                CloseClient();
                                if (ErrLock.ErrLockOpen)
                                {
                                    ErrLock.LockEnd(errLockData);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        reConnectLoopCount = 0;
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
                Thread.Sleep(3000);
            }
            SocketHelper.PutBackOne(data);
        }

        #endregion

        #region//Task 数据接受

        SocketHelper.TaskEndData acceptMsgsFlag;

        object lockAcceptMsgsFlag = new object();

        void StartAcceptMsgsTask()
        {
            lock (lockAcceptMsgsFlag)
            {
                acceptMsgsFlag = SocketHelper.GetOneTaskEndData();
                Task.Factory.StartNew((obj) => {
                    SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                    AcceptMsgs(data);
                }, acceptMsgsFlag);
            }
        }

        void StopAcceptMsgsTask()
        {
            lock (lockAcceptMsgsFlag)
            {
                if (acceptMsgsFlag != null)
                {
                    acceptMsgsFlag.TaskStop = true;
                    acceptMsgsFlag = null;
                }
            }
            lock (lockObj)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->458-->StopAcceptMsgsTask"));
                }
                if (client != null)
                {
                    try
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    try
                    {
                        client.Close();
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    client = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 收数据。
        /// </summary>
        void AcceptMsgs(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (lockAcceptMsgsFlag)
            {
                isTaskStop = data.TaskStop;
            }
            byte[] buffer = new byte[SocketHelper.MAX_READ_BUFFER_SIZE];
            //处理粘包
            Proto.ByteBufferData lastBuffer = null;
            while (!isTaskStop)
            {
                lock (lockAcceptMsgsFlag)
                {
                    isTaskStop = data.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }
                if (client.Connected)
                {
                    int lenght = 0;
                    try
                    {
                        lenght = client.Receive(buffer);
                    }
                    catch (Exception ex)
                    {
                        VLog.Error($"客户端异常：[ip]={ServerIp.ToString()} [port]={ClientPort} [err]={ex.Message}");
                        //长度为0表示没接受到内容。断开。
                        StopAcceptMsgsTask();
                        StopProtoSplitPollTask();
                        break;
                    }
                    if (lenght <= 0)
                    {
                        VLog.Error($"客户端异常,服务器断开：[ip]={ServerIp.ToString()} [port]={ClientPort} [dataLength]={lenght}");
                        //长度为0表示没接受到内容。断开。
                        StopAcceptMsgsTask();
                        StopProtoSplitPollTask();
                        break;
                    }
                    try
                    {
                        bool err = false;
                        Int32 lastLenght = 0;
                        List<Proto_Package_Data> res = null;
                        if (lastBuffer != null)
                        {
                            //处理粘包
                            Proto.ByteBufferData tempBuffer = Proto.GetOneByteBufferData(lastBuffer.Length + lenght);
                            Array.Copy(lastBuffer.Buffer, 0, tempBuffer.Buffer, 0, lastBuffer.Length);
                            Array.Copy(buffer, 0, tempBuffer.Buffer, lastBuffer.Length, lenght);
                            Proto.PutBackByteBufferData(lastBuffer);
                            lenght = lastBuffer.Length + lenght;
                            lastBuffer = null;
                            res = Proto.Split_The_Parcel(tempBuffer.Buffer, lenght, ref err, ref lastLenght);
                            if (!err)
                            {
                                if (lastLenght > 0)
                                {
                                    lastBuffer = Proto.GetOneByteBufferData(lastLenght);
                                    Array.Copy(tempBuffer.Buffer, lenght - lastLenght, lastBuffer.Buffer, 0, lastLenght);
                                }
                                AddProtoSplitData(res);
                            }
                            else
                            {
                                VLog.Error("客户端解析数据错误");
                                AddProtoSplitData(res);
                            }
                        }
                        else
                        {
                            res = Proto.Split_The_Parcel(buffer, lenght, ref err, ref lastLenght);
                            if (!err)
                            {
                                if (lastLenght > 0)
                                {
                                    lastBuffer = Proto.GetOneByteBufferData(lastLenght);
                                    Array.Copy(buffer, lenght - lastLenght, lastBuffer.Buffer, 0, lastLenght);
                                }
                                AddProtoSplitData(res);
                            }
                            else
                            {
                                VLog.Error("客户端解析数据错误");
                                AddProtoSplitData(res);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        VLog.Error($"客户端解析数据异常,[err]={ex.Message}");
                    }
                }
                else
                {
                    VLog.Error("客户端连接断开");
                    StopAcceptMsgsTask();
                    StopProtoSplitPollTask();
                    break;
                }
            }
            if (lastBuffer != null)
            {
                Proto.PutBackByteBufferData(lastBuffer);
                lastBuffer = null;
            }
            SocketHelper.PutBackOne(data);
        }

        #endregion

        #region//Task 数据处理

        List<ProtoSplitData> curProtoSplitDatas = new List<ProtoSplitData>();

        void AddProtoSplitData(List<Proto_Package_Data> packageDatas)
        {
            ProtoSplitData protoSplitData = protoSplitDataPool.Spawn();
            protoSplitData.PackageDatas = packageDatas;
            lock (curProtoSplitDatas)
            {
                curProtoSplitDatas.Add(protoSplitData);
            }
        }

        SocketHelper.TaskEndData protoSplitPollFlag;

        object lockProtoSplitPollFlag = new object();

        void StartProtoSplitPollTask()
        {
            lock (lockProtoSplitPollFlag)
            {
                protoSplitPollFlag = SocketHelper.GetOneTaskEndData();
                Task.Factory.StartNew((obj) => {
                    SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                    ProtoSplitPoll(data);
                }, protoSplitPollFlag);
            }
        }

        void StopProtoSplitPollTask()
        {
            lock (lockProtoSplitPollFlag)
            {
                if (protoSplitPollFlag != null)
                {
                    protoSplitPollFlag.TaskStop = true;
                }
            }
        }

        void ProtoSplitPoll(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (lockProtoSplitPollFlag)
            {
                isTaskStop = data.TaskStop;
            }
            List<ProtoSplitData> tempProtoSplitDatas = protoSplitDataListPool.Spawn();
            while (!isTaskStop)
            {
                lock (lockProtoSplitPollFlag)
                {
                    isTaskStop = data.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }
                lock (curProtoSplitDatas)
                {
                    if (curProtoSplitDatas.Count > 0)
                    {
                        for (int i = 0; i < curProtoSplitDatas.Count; ++i)
                        {
                            tempProtoSplitDatas.Add(curProtoSplitDatas[i]);
                        }
                        curProtoSplitDatas.Clear();
                    }
                }
                if (tempProtoSplitDatas.Count > 0)
                {
                    for (int i = 0; i < tempProtoSplitDatas.Count; ++i)
                    {
                        ProtoSplitData protoSplitData = tempProtoSplitDatas[i];
                        for (int j = 0; j < protoSplitData.PackageDatas.Count; ++j)
                        {
                            ProcessingDataRun(protoSplitData.PackageDatas[j]);
                        }
                        Proto.PutBackOneProtoDataListList(protoSplitData.PackageDatas);
                        protoSplitDataPool.Recycle(protoSplitData);
                    }
                    tempProtoSplitDatas.Clear();
                }
            }
            protoSplitDataListPool.Recycle(tempProtoSplitDatas);
            SocketHelper.PutBackOne(data);
        }

        void ProcessingDataRun(Proto_Package_Data package_Data)
        {
            try
            {
                switch (package_Data.agreement)
                {
                    case (int)Proto_Agreement_Enum.Heart_C://心跳包
                        {
                            lock (lockObj)
                            {
                                hearSendCount = 0;
                            }
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Heart_S://心跳包
                        {
                            heart_data heartData = proto.Proto.GetOneProtoData<heart_data>();
                            package_Data.GetData(heartData);
                            Send(heartData, (int)Proto_Agreement_Enum.Heart_S);
                            Proto.PutBackOneProtoData<heart_data>(heartData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_Log:
                        {
                            log_data getData = Proto.GetOneProtoData<log_data>();
                            package_Data.GetData(getData);
                            if (getData.processId!= ProcessId)
                            {
                                VLog.Info(getData.logStr);
                            }
                            Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_LogWarning:
                        {
                            log_data getData = Proto.GetOneProtoData<log_data>();
                            package_Data.GetData(getData);
                            if (getData.processId != ProcessId)
                            {
                                VLog.Warning(getData.logStr);
                            }
                            Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_LogError:
                        {
                            log_data getData = Proto.GetOneProtoData<log_data>();
                            package_Data.GetData(getData);
                            if (getData.processId != ProcessId)
                            {
                                VLog.Error(getData.logStr);
                            }
                            Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                }
                ProcessingData(package_Data);
            }
            catch (System.Exception ex)
            {
                VLog.Error($"客户端数据处理异常,[err]={ex.Message}");
            }
            Proto.PutBakOneProtoData(package_Data);
        }

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="package_Data"></param>
        protected virtual void ProcessingData(Proto_Package_Data package_Data)
        {

        }

        static SimpleListPool<List<ProtoSplitData>, ProtoSplitData> protoSplitDataListPool = new SimpleListPool<List<ProtoSplitData>, ProtoSplitData>();

        static SimplePool<ProtoSplitData> protoSplitDataPool = new SimplePool<ProtoSplitData>();

        class ProtoSplitData : ISimplePoolData
        {
            public List<Proto_Package_Data> PackageDatas;

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
                PackageDatas = null;
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

        #endregion

        #region//Task 数据发送

        SocketHelper.TaskEndData sendPollFlag;

        object lockSendPollFlag = new object();

        void StartSendPollTask()
        {
            lock (lockSendPollFlag)
            {
                sendPollFlag = SocketHelper.GetOneTaskEndData();
                sendPollFlag.TaskStop = false;
                Task.Factory.StartNew((obj) => {
                    SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                    SendPoll(data);
                }, sendPollFlag);
            }
        }

        void StopSendPollTask()
        {
            lock (lockSendPollFlag)
            {
                if (sendPollFlag != null)
                {
                    sendPollFlag.TaskStop = true;
                    sendPollFlag = null;
                }
            }
            lock (lockObj)
            {
                if (client != null)
                {
                    try
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    try
                    {
                        client.Close();
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    client = null;
                }
            }
        }

        List<SendData> curSendList = new List<SendData>();

        SimpleListPool<List<SendData>, SendData> sendDataListPool = new SimpleListPool<List<SendData>, SendData>();

        public void Send(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                VLog.Error($"SocketNotSend:客户端数据发送错误,数据长度=0：[ip]={ServerIp.ToString()} [port]={ClientPort} [size]={data.Length}");
                return;
            }
            lock (lockSendPollFlag)
            {
                if (sendPollFlag==null) return;
            }
            SendData sendData = sendDataPool.Spawn();
            sendData.SetData(data);
            lock (curSendList)
            {
                curSendList.Add(sendData);
            }
        }

        public void Send(Proto_Base data, int agreement)
        {
            try
            {
                Proto.ByteBufferData byteBufferData = Proto.Serialize(data, agreement);
                Send(byteBufferData.Buffer);
                Proto.PutBackByteBufferData(byteBufferData);
            }
            catch (System.Exception ex)
            {
                VLog.Error($"SocketNotSend:{ex.Message}");
            }
        }

        public void SendLog(string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            Send(data, (int)Proto_Agreement_Enum.Debug_Log);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        public void SendLogWarning(string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            Send(data, (int)Proto_Agreement_Enum.Debug_LogWarning);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        public void SendLogError(string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            Send(data, (int)Proto_Agreement_Enum.Debug_LogError);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        void SendPoll(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (lockSendPollFlag)
            {
                isTaskStop = data.TaskStop;
            }
            while (!isTaskStop)
            {
                List<SendData> temCurSendList = sendDataListPool.Spawn();
                lock (lockSendPollFlag)
                {
                    isTaskStop = data.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }
                lock (curSendList)
                {
                    if (curSendList.Count > 0)
                    {
                        for (int i = 0; i < curSendList.Count; ++i)
                        {
                            temCurSendList.Add(curSendList[i]);
                        }
                        curSendList.Clear();
                    }
                }
                if (temCurSendList.Count > 0)
                {
                    bool err = false;
                    for (int i = 0; i < temCurSendList.Count; ++i)
                    {
                        SendData sendData = temCurSendList[i];
                        lock (lockObj)
                        {
                            ErrLockData errLockData = null;
                            if (ErrLock.ErrLockOpen)
                            {
                                errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->966-->SendPoll"));
                            }
                            if (client != null && client.Connected && !err)
                            {
                                try
                                {
                                    client.Send(sendData.Buffer, sendData.BufferSize, SocketFlags.None);
                                }
                                catch (System.Exception ex)
                                {
                                    VLog.Error($"SocketNotSend:客户端数据发送异常,客户端关闭：[ip]={ServerIp.ToString()} [port]={ClientPort} [size]={sendData.BufferSize} [err]={ex.Message}");
                                    err = true;
                                }
                            }
                            if (ErrLock.ErrLockOpen)
                            {
                                ErrLock.LockEnd(errLockData);
                            }
                        }
                        if (err)
                        {
                            break;
                        }
                        sendDataPool.Recycle(sendData);
                    }
                    temCurSendList.Clear();
                    if (err)
                    {
                        StopSendPollTask();
                        break;
                    }
                }
                sendDataListPool.Recycle(temCurSendList);
                Thread.Sleep(1);
            }
            SocketHelper.PutBackOne(data);
        }

        static SimplePool<SendData> sendDataPool = new SimplePool<SendData>();

        class SendData : ISimplePoolData
        {
            public byte[] Buffer = new byte[SocketHelper.MAX_SEND_BUFFER_SIZE];

            public int BufferSize = 0;

            public void SetData(byte[] data)
            {
                BufferSize = data.Length;
                if (data.Length > SocketHelper.MAX_SEND_BUFFER_SIZE)
                {
                    Array.Resize<byte>(ref Buffer, data.Length);
                }
                Array.Copy(data, Buffer, BufferSize);
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
                if (Buffer.Length > SocketHelper.MAX_SEND_BUFFER_SIZE)
                {
                    Array.Resize<byte>(ref Buffer, SocketHelper.MAX_SEND_BUFFER_SIZE);
                }
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

        #endregion

    }

}

