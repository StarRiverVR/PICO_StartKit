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
    /// Socket服务器
    /// 使用方法 继承后重写  public virtual bool OpenServer(int port) ， public virtual bool OpenServer(string serverIp, int port) ， public virtual void CloseServer() ， protected virtual void ProcessingData(Socket tcpSocket, ClientData clientData, Proto_Package_Data resData)
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SocketServer
    {
#if UNITY_EDITOR
        static SocketServer()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeStateChange;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeStateChange;
        }

        static void PlayModeStateChange(UnityEditor.PlayModeStateChange mod)
        {
            if (mod == UnityEditor.PlayModeStateChange.ExitingEditMode || mod == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                CloseAllServers();
            }
        }

        static void CloseAllServers()
        {
            lock (allServers)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketServer.cs-->45-->CloseAllServers"));
                }
                for (int i = 0; i < allServers.Count; ++i)
                {
                    if (allServers[i] != null)
                    {
                        allServers[i].CloseServer();
                    }
                }
                allServers.Clear();
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        static List<SocketServer> allServers = new List<SocketServer>();
#endif

        public SocketServer()
        {

#if UNITY_EDITOR
            lock (allServers)
            {
                allServers.Add(this);
            }
#endif
            processId = Process.GetCurrentProcess().Id;
        }

        #region//服务端

        int heartDeconnectTimeLenght = 25;

        /// <summary>
        /// 心跳断开连接时间长度
        /// </summary>
        public int HeartDeconnectTimeLenght
        {
            get
            {
                lock (serverLock)
                {
                    return heartDeconnectTimeLenght;
                }
            }
            set
            {
                lock (serverLock)
                {
                    heartDeconnectTimeLenght = value;
                    heartDeconnectTimeLenght = Mathf.Clamp(heartDeconnectTimeLenght, 5, int.MaxValue);
                }
            }
        }

        IPAddress serverIPAddress;

        public IPAddress ServerIPAddress
        {
            get
            {
                lock (serverLock)
                {
                    return serverIPAddress;
                }
            }
        }

        int serverPort;

        public int ServerPort
        {
            get
            {
                lock (serverLock)
                {
                    return serverPort;
                }
            }
        }

        Socket ServerSocket = null;//服务端  

        object serverLock = new object();

        public virtual bool OpenServer(int port)
        {
            lock (serverLock)
            {
                serverPort = port;
                serverIPAddress = IPAddress.Any;
                return OpenServer();
            }
        }

        public virtual bool OpenServer(string serverIp, int port)
        {
            lock (serverLock)
            {
                serverPort = port;
                serverIPAddress = IPAddress.Parse(serverIp);
                return OpenServer();
            }
        }

        int processId;

        public int ProcessId
        {
            get
            {
                lock (serverLock)
                {
                    return processId;
                }
            }
        }

        bool OpenServer()
        {
            try
            {
                CloseServer();
                IPEndPoint endPoint = new IPEndPoint(serverIPAddress, serverPort);
                //(IP4寻址协议,流式连接,TCP协议)
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    ServerSocket.Bind(endPoint);
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"服务器绑定端口失败，[ip]={serverIPAddress.ToString()} [port]={serverPort} [err]={ex.Message}");
                    return false;
                }
                //设置监听队列的长度；
                ServerSocket.Listen(100);
                StartListenConnectingTask();
                StartSendDataPollTask();
                StartProtoDataTask();
                VLog.Info($"服务器开启成功: [ip]={serverIPAddress.ToString()} [port]={serverPort}");
                return true;
            }
            catch (System.Exception ex)
            {
                if (ServerSocket != null)
                {
                    try
                    {
                        ServerSocket.Close();
                    }
                    catch (System.Exception e)
                    {
                        VLog.Error($"{ex.Message}");
                    }
                    ServerSocket = null;
                }
                VLog.Error($"服务器开启失败，[ip]={serverIPAddress.ToString()} [port]={serverPort} [err]={ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public virtual void CloseServer()
        {
            lock (serverLock)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketServer.cs-->213-->CloseServer"));
                }
                VLog.Warning(String.Format("服务器关闭: [ip]={0} [port]={1}", serverIPAddress?.ToString(), serverPort));
                StopStartListenConnectingTask();
                StopSendDataPollTask();
                StopProtoDataTask();
                lock (clients)
                {
                    ErrLockData errLockData2 = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData2 = ErrLock.LockStart(String.Format("SocketServer.cs-->220-->CloseServer"));
                    }
                    for (int i = 0; i < clients.Count; ++i)
                    {
                        clients[i].Close();
                    }
                    clients.Clear();
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData2);
                    }
                }
                lock (curClientSocketDatas)
                {
                    for (int i = 0; i < curClientSocketDatas.Count; ++i)
                    {
                        for (int j = 0; j < curClientSocketDatas[i].Datas.Count; ++j)
                        {
                            Proto.PutBakOneProtoData(curClientSocketDatas[i].Datas[j]);
                        }
                        Proto.PutBackOneProtoDataListList(curClientSocketDatas[i].Datas);
                        clientSocketDataPool.Recycle(curClientSocketDatas[i]);
                    }
                    curClientSocketDatas.Clear();
                }
                //服务端不能主动关闭连接,需要把监听到的连接逐个关闭
                if (ServerSocket != null)
                {
                    try
                    {
                        ServerSocket.Close();
                    }
                    catch (System.Exception e)
                    {
                        VLog.Error($"{e.Message}");
                    }
                    //ServerSocket.Shutdown(SocketShutdown.Both);
                    ServerSocket = null;
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        #region//Task 监听新的客户端连接

        SocketHelper.TaskEndData listenConnectingTaskFlag;

        object lockListenConnectingTaskFlag = new object();

        void StartListenConnectingTask()
        {
            lock (lockListenConnectingTaskFlag)
            {
                StopStartListenConnectingTask();
                listenConnectingTaskFlag = SocketHelper.GetOneTaskEndData();
                Task.Factory.StartNew((obj) => {
                    SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                    ListenConnecting(data);
                }, listenConnectingTaskFlag);
            }
        }

        void ListenConnecting(SocketHelper.TaskEndData data)
        {
            bool taskIsStop = false;
            lock (lockListenConnectingTaskFlag)
            {
                taskIsStop = data.TaskStop;
            }
            while (!taskIsStop)
            {
                try
                {
                    lock (lockListenConnectingTaskFlag)
                    {
                        taskIsStop = data.TaskStop;
                        if (taskIsStop)
                        {
                            break;
                        }
                    }
                    Socket sokConnection = null;
                    sokConnection = ServerSocket.Accept();
                    ClientData myTcpClient = clientDataPool.Spawn();
                    myTcpClient.SocketServer = this;
                    myTcpClient.TcpSocket = sokConnection;
                    //接受数据
                    myTcpClient.StartReceiveDataTask(ReceiveData);
                    //
                    AddOneClient(myTcpClient);
                    //
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"{ex.Message}");
                    CloseServer();
                }
            }
            SocketHelper.PutBackOne(data);
        }

        void StopStartListenConnectingTask()
        {
            lock (lockListenConnectingTaskFlag)
            {
                if (listenConnectingTaskFlag != null)
                {
                    listenConnectingTaskFlag.TaskStop = true;
                }
            }
        }

        public bool IsListening
        {
            get
            {
                lock (lockListenConnectingTaskFlag)
                {
                    if (listenConnectingTaskFlag == null || listenConnectingTaskFlag.TaskStop)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        #endregion

        #region//Task 发送数据

        List<SendDataThread> sendDataThreadList = new List<SendDataThread>();

        static SimpleListPool<List<SendDataThread>, SendDataThread> sendDataThreadListPool = new SimpleListPool<List<SendDataThread>, SendDataThread>();

        /// <summary>
        /// 发送数据 
        /// </summary>
        /// <param name="clientData">null：表示发送给全部客户端</param>
        /// <param name="buffer"></param>
        public void SendData(ClientData clientData, byte[] buffer)
        {
            int bufferL = 0;
            if (buffer == null || buffer.Length < 2)
            {
                bufferL = buffer.Length;
                VLog.Error($"服务器发送数据错误,[data]={buffer} [size]={bufferL}");
                return;
            }
            lock (lockSendDataPollFlag)
            {
                if (sendDataPollFlag==null) return;
            }
            SendDataThread data = sendDataThreadPool.Spawn();
            data.SetData(buffer);
            lock (sendDataThreadList)
            {
                sendDataThreadList.Add(data);
            }
        }

        public void SendData(ClientData clientData, Proto_Base data, int agreement)
        {
            try
            {
                Proto.ByteBufferData byteBufferData = Proto.Serialize(data, (int)agreement);
                SendData(clientData, byteBufferData.Buffer);
                Proto.PutBackByteBufferData(byteBufferData);
            }
            catch (System.Exception ex)
            {
                VLog.Error($"SocketNotSend:{ex.Message}");
            }
        }

        public void SendLog(ClientData clientData, string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            SendData(clientData,data, (int)Proto_Agreement_Enum.Debug_Log);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        public void SendLogWarning(ClientData clientData, string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            SendData(clientData, data, (int)Proto_Agreement_Enum.Debug_LogWarning);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        public void SendLogError(ClientData clientData, string logStr)
        {
            log_data data = proto.Proto.GetOneProtoData<log_data>();
            data.logStr = logStr;
            data.processId = ProcessId;
            SendData(clientData, data, (int)Proto_Agreement_Enum.Debug_LogError);
            proto.Proto.PutBackOneProtoData<log_data>(data);
        }

        SocketHelper.TaskEndData sendDataPollFlag;

        object lockSendDataPollFlag = new object();

        void StartSendDataPollTask()
        {
            lock (sendDataThreadList)
            {
                sendDataThreadList.Clear();
                lock (lockSendDataPollFlag)
                {
                    StopSendDataPollTask();
                    sendDataPollFlag = SocketHelper.GetOneTaskEndData();
                    Task.Factory.StartNew((obj) => {
                        SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                        SendDataPoll(data);
                    }, sendDataPollFlag);
                }
            }
        }

        void SendDataPoll(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (lockSendDataPollFlag)
            {
                isTaskStop = data.TaskStop;
            }
            int hertSendTime = 0;
            int whileTime = 1;
            List<SendDataThread> tempSendDataThreadList = sendDataThreadListPool.Spawn();
            while (!isTaskStop)
            {
                lock (lockSendDataPollFlag)
                {
                    isTaskStop = data.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }

                hertSendTime = whileTime + hertSendTime;
                if (hertSendTime >= 5000)
                {
                    int waitHeartCount = (HeartDeconnectTimeLenght * 1000) / 5000;
                    lock (curClientSocketDatas)
                    {
                        ErrLockData errLockData = null;
                        if (ErrLock.ErrLockOpen)
                        {
                            errLockData = ErrLock.LockStart(String.Format("SocketServer.cs-->466-->SendDataPoll"));
                        }
                        for (int i = 0; i < curClientSocketDatas.Count; ++i)
                        {
                            ClientData clientData = curClientSocketDatas[i].ClientData;
                            if (clientData != null)
                            {
                                clientData.HertCountAdd();
                                if (clientData.HertCount >= waitHeartCount)
                                {
                                    RemoveOneClient(clientData);
                                }
                            }
                        }
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                    }
                    hertSendTime = 0;
                    proto.heart_data heartData = Proto.GetOneProtoData<proto.heart_data>();
                    SendData(null, heartData, (int)Proto_Agreement_Enum.Heart_S);
                    proto.Proto.PutBackOneProtoData<heart_data>(heartData);
                }
                lock (sendDataThreadList)
                {
                    if (sendDataThreadList.Count > 0)
                    {
                        for (int i = 0; i < sendDataThreadList.Count; ++i)
                        {
                            tempSendDataThreadList.Add(sendDataThreadList[i]);
                        }
                        sendDataThreadList.Clear();
                    }
                }
                if (tempSendDataThreadList.Count > 0)
                {
                    for (int i = 0; i < tempSendDataThreadList.Count; ++i)
                    {
                        SendDataThread sendDataThread = tempSendDataThreadList[i];
                        if (sendDataThread.ClientData != null)
                        {
                            try
                            {
                                sendDataThread.ClientData.Send(sendDataThread.Buffer, sendDataThread.BufferSize);
                            }
                            catch (System.Exception ex)
                            {
                                VLog.Error($"SocketNotSend:发送数据异常,[server ip]={serverIPAddress.ToString()} [server port]={serverPort} [client]={sendDataThread.ClientData.GetIp()}");
                                RemoveOneClient(sendDataThread.ClientData);
                            }
                        }
                        else
                        {
                            lock (clients)
                            {
                                ErrLockData errLockData = null;
                                if (ErrLock.ErrLockOpen)
                                {
                                    errLockData = ErrLock.LockStart(String.Format("SocketServer.cs-->518-->SendDataPoll"));
                                }
                                for (int j = 0; j < clients.Count; ++j)
                                {
                                    try
                                    {
                                        clients[j].Send(sendDataThread.Buffer, sendDataThread.BufferSize);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        VLog.Error($"SocketNotSend:发送数据异常,[server ip]={serverIPAddress.ToString()} [server port]={serverPort} [client]={clients[j].GetIp()}");
                                        RemoveOneClient(clients[j]);
                                    }
                                }
                                if (ErrLock.ErrLockOpen)
                                {
                                    ErrLock.LockEnd(errLockData);
                                }
                            }
                        }
                        sendDataThreadPool.Recycle(sendDataThread);
                    }
                    tempSendDataThreadList.Clear();
                }
                Thread.Sleep(whileTime);
            }
            sendDataThreadListPool.Recycle(tempSendDataThreadList);
            SocketHelper.PutBackOne(data);
        }

        void StopSendDataPollTask()
        {
            lock (lockSendDataPollFlag)
            {
                if (sendDataPollFlag != null)
                {
                    sendDataPollFlag.TaskStop = true;
                    sendDataPollFlag = null;
                }
            }
        }

        static SimplePool<SendDataThread> sendDataThreadPool = new SimplePool<SendDataThread>();

        class SendDataThread : ISimplePoolData
        {

            public byte[] Buffer = new byte[SocketHelper.MAX_SEND_BUFFER_SIZE];

            public int BufferSize = 0;

            public ClientData ClientData;

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
                ClientData = null;
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

        #region//Task 处理数据

        static SimplePool<ClientSocketData> clientSocketDataPool = new SimplePool<ClientSocketData>();

        static SimpleListPool<List<Proto_Package_Data>, Proto_Package_Data> protoDatalsPool = new SimpleListPool<List<Proto_Package_Data>, Proto_Package_Data>();

        static SimpleListPool<List<ClientSocketData>, ClientSocketData> clientSocketDatasPool = new SimpleListPool<List<ClientSocketData>, ClientSocketData>();

        List<ClientSocketData> curClientSocketDatas = new List<ClientSocketData>();

        void AddClientSocketData(ClientData clientData, List<Proto_Package_Data> protoDatas)
        {
            if (protoDatas == null || !IsListening || clientData.TcpSocket == null) return;
            ClientSocketData clientSocketData = clientSocketDataPool.Spawn();
            clientSocketData.ClientData = clientData;
            clientSocketData.ClientSocket = clientData.TcpSocket;
            clientSocketData.Datas = protoDatas;
            lock (curClientSocketDatas)
            {
                curClientSocketDatas.Add(clientSocketData);
            }
        }

        List<SocketHelper.TaskEndData> protoEndDatas = new List<SocketHelper.TaskEndData>();

        void StartProtoDataTask()
        {
            lock (curClientSocketDatas)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->650-->StartProtoDataTask"));
                }
                curClientSocketDatas.Clear();
                lock (protoEndDatas)
                {
                    StopProtoDataTask();
                    for (int i = 0; i < 8; ++i)
                    {
                        SocketHelper.TaskEndData taskEndData = SocketHelper.GetOneTaskEndData();
                        protoEndDatas.Add(taskEndData);
                        Task.Factory.StartNew((obj) => {
                            SocketHelper.TaskEndData data = (SocketHelper.TaskEndData)obj;
                            ProtoDataTask(data);
                        }, taskEndData);
                    }
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        void StopProtoDataTask()
        {
            lock (protoEndDatas)
            {
                if (protoEndDatas.Count > 0)
                {
                    for (int i = 0; i < protoEndDatas.Count; ++i)
                    {
                        protoEndDatas[i].TaskStop = true;
                    }
                    protoEndDatas.Clear();
                }
            }
        }

        void ProtoDataTask(SocketHelper.TaskEndData data)
        {
            bool isTaskStop = false;
            lock (protoEndDatas)
            {
                isTaskStop = data.TaskStop;
            }
            List<ClientSocketData> tempCurClientSocketDatas = clientSocketDatasPool.Spawn();

            while (!data.TaskStop)
            {
                lock (protoEndDatas)
                {
                    isTaskStop = data.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }

                lock (curClientSocketDatas)
                {
                    if (curClientSocketDatas.Count > 0)
                    {
                        for (int i = 0; i < curClientSocketDatas.Count; ++i)
                        {
                            tempCurClientSocketDatas.Add(curClientSocketDatas[i]);
                        }
                        curClientSocketDatas.Clear();
                    }
                }
                if (tempCurClientSocketDatas.Count > 0)
                {
                    for (int i = 0; i < tempCurClientSocketDatas.Count; ++i)
                    {
                        ClientSocketData clientSocketData = tempCurClientSocketDatas[i];
                        List<Proto_Package_Data> protos = clientSocketData.Datas;
                        for (int j = 0; j < protos.Count; ++j)
                        {
                            ProcessingDataRun(clientSocketData.ClientSocket, clientSocketData.ClientData, protos[j]);
                        }
                        clientSocketDataPool.Recycle(clientSocketData);
                        Proto.PutBackOneProtoDataListList(protos);
                    }
                    tempCurClientSocketDatas.Clear();
                }
                Thread.Sleep(1);
            }
            clientSocketDatasPool.Recycle(tempCurClientSocketDatas);
        }

        void ProcessingDataRun(Socket tcpSocket, ClientData clientData, Proto_Package_Data resData)
        {
            try
            {
                if (tcpSocket != clientData.TcpSocket || !IsListening) return;
                switch (resData.agreement)
                {
                    case (int)Proto_Agreement_Enum.Heart_C://心跳包
                        {
                            heart_data heartData = proto.Proto.GetOneProtoData<heart_data>();
                            resData.GetData(heartData);
                            SendData(clientData, heartData, (int)Proto_Agreement_Enum.Heart_C);
                            proto.Proto.PutBackOneProtoData<heart_data>(heartData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Heart_S://心跳包
                        {
                            clientData.HertCount = 0;
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_Log:
                        {
                            log_data getData = proto.Proto.GetOneProtoData<log_data>();
                            resData.GetData(getData);
                            if (getData.processId != ProcessId)
                            {
                                VLog.Info(getData.logStr);
                            }
                            proto.Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_LogWarning:
                        {
                            log_data getData = proto.Proto.GetOneProtoData<log_data>();
                            resData.GetData(getData);
                            if (getData.processId != ProcessId)
                            {
                                VLog.Warning(getData.logStr);
                            }
                            proto.Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                    case (int)Proto_Agreement_Enum.Debug_LogError:
                        {
                            log_data getData = proto.Proto.GetOneProtoData<log_data>();
                            resData.GetData(getData);
                            if (getData.processId != ProcessId)
                            {
                                VLog.Error(getData.logStr);
                            }
                            proto.Proto.PutBackOneProtoData<log_data>(getData);
                        }
                        break;
                    default:
                        {
                           // DebugLog.LogErrorFormat("位置的数据协议:[agr]={0}", true, resData.agreement);
                        }
                        break;
                }
                ProcessingData(tcpSocket, clientData, resData);
            }
            catch (System.Exception ex)
            {
                VLog.Error($"服务端处理客户端数据异常:[serverIp]={serverIPAddress.ToString()} [serverPort]={serverPort} [clientIp]={clientData.GetIp()} [err]={ex.Message}");
            }
            Proto.PutBakOneProtoData(resData);
        }

        /// <summary>
        /// 根据协议号 处理接收到的数据
        /// </summary>
        /// <param name="tcpSocket"></param>
        /// <param name="clientData"></param>
        /// <param name="resData"></param>
        protected virtual void ProcessingData(Socket tcpSocket, ClientData clientData, Proto_Package_Data resData)
        {

        }

        #endregion

        #endregion

        #region//客户端

        List<ClientData> clients = new List<ClientData>();

        SimplePool<ClientData> clientDataPool = new SimplePool<ClientData>();

        void AddOneClient(ClientData data)
        {
            lock (serverLock)
            {
                if (ServerSocket == null) return;
                lock (clients)
                {
                    VLog.Info(String.Format("新的客户端进入:{0}", data.GetIp()));
                    clients.Add(data);
                }
            }
        }

        void RemoveOneClient(ClientData data)
        {
            if (data == null) return;
            lock (serverLock)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->841-->RemoveOneClient"));
                }
                lock (clients)
                {
                    ErrLockData errLockData2 = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData2 = ErrLock.LockStart(String.Format("SocketClient.cs-->844-->RemoveOneClient"));
                    }
                    if (clients.Contains(data))
                    {
                        VLog.Info(String.Format("客户端断开连接:{0}", data.GetIp()));
                        clients.Remove(data);
                        data.Close();
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData2);
                    }
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sokConnectionparn"></param>
        void ReceiveData(ClientData tcpClient)
        {
            Socket socketClient = tcpClient.TcpSocket;
            bool isTaskStop = false;
            lock (tcpClient.lockObj)
            {
                isTaskStop = tcpClient.ReceiveDataFlag.TaskStop;
            }
            while (!isTaskStop)
            {
                lock (tcpClient.lockObj)
                {
                    isTaskStop = tcpClient.ReceiveDataFlag.TaskStop;
                    if (isTaskStop)
                    {
                        break;
                    }
                }
                // 将接受到的数据存入到输入  arrMsgRec中；
                int length = -1;
                try
                {
                    length = socketClient.Receive(tcpClient.ReceiveBuffer); // 接收数据，并返回数据的长度；
                }
                catch (System.Exception ex)
                {
                    VLog.Error($"{ex.Message}");
                    RemoveOneClient(tcpClient);
                    tcpClient = null;
                    socketClient = null;
                    break;
                }
                if (length > 0)
                {
                    try
                    {
                        tcpClient.DataProcessingRun(length);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"客户端数据解析异常: [err]={ex.Message}");
                    }
                }
                else
                {
                    VLog.Error($"接受到的客户端数据小于0,[Ip]={tcpClient.GetIp()}");
                    RemoveOneClient(tcpClient);
                    tcpClient = null;
                    socketClient = null;
                    break;
                }
            }
            lock (tcpClient.lockObj)
            {
                SocketHelper.PutBackOne(tcpClient.ReceiveDataFlag);
            }
            clientDataPool.Recycle(tcpClient);
        }

        public class ClientData : ISimplePoolData
        {
            public SocketServer SocketServer;

            Socket tcpSocket;

            public Socket TcpSocket
            {
                get
                {
                    lock (lockObj)
                    {
                        return tcpSocket;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        tcpSocket = value;
                    }
                }
            }

            public byte[] ReceiveBuffer = new byte[SocketHelper.MAX_READ_BUFFER_SIZE];

            //处理粘包
            Proto.ByteBufferData lastBuffer = null;

            int hertCount = 0;

            public int HertCount
            {
                get
                {
                    lock (lockObj)
                    {
                        return hertCount;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        if (isClose) return;
                        hertCount = value;
                    }
                }
            }

            public void HertCountAdd()
            {
                lock (lockObj)
                {
                    hertCount = hertCount + 1;
                }
            }

            public object lockObj = new object();

            public ClientData()
            {

            }

            /// <summary>
            /// 发送数据
            /// </summary>
            /// <param name="buf"></param>
            public void Send(byte[] buf, int size)
            {
                lock (lockObj)
                {
                    if (buf != null && TcpSocket != null)
                    {
                        TcpSocket.Send(buf, size, SocketFlags.None);
                    }
                }
            }

            /// <summary>
            /// 数据处理
            /// </summary>
            /// <param name="lenght"></param>
            public void DataProcessingRun(int lenght)
            {
                lock (lockObj)
                {
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {

                        errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->1008-->DataProcessingRun"));
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
                            Array.Copy(ReceiveBuffer, 0, tempBuffer.Buffer, lastBuffer.Length, lenght);
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
                                if (SocketServer != null)
                                {
                                    SocketServer.AddClientSocketData(this, res);
                                }
                            }
                            else
                            {
                                VLog.Error("客户端解析数据错误");
                                if (SocketServer != null)
                                {
                                    SocketServer.AddClientSocketData(this, res);
                                }
                            }
                        }
                        else
                        {
                            res = Proto.Split_The_Parcel(ReceiveBuffer, lenght, ref err, ref lastLenght);
                            if (!err)
                            {
                                if (lastLenght > 0)
                                {
                                    lastBuffer = Proto.GetOneByteBufferData(lastLenght);
                                    Array.Copy(ReceiveBuffer, lenght - lastLenght, lastBuffer.Buffer, 0, lastLenght);
                                }
                                if (SocketServer != null)
                                {
                                    SocketServer.AddClientSocketData(this, res);
                                }
                            }
                            else
                            {
                                VLog.Error("客户端解析数据错误");
                                if (SocketServer != null)
                                {
                                    SocketServer.AddClientSocketData(this, res);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"收到的客户端数据解析错误,[ip]={GetIp()} [err]={ex.Message}");
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
            /// <summary>
            /// 获取连接的ip
            /// </summary>
            /// <returns></returns>
            public string GetIp()
            {
                lock (lockObj)
                {
                    if (TcpSocket != null)
                    {
                        IPEndPoint clientipe = (IPEndPoint)TcpSocket.RemoteEndPoint;
                        return clientipe.Address.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            #region//Task 数据接受

            public SocketHelper.TaskEndData ReceiveDataFlag;

            public void StartReceiveDataTask(System.Action<ClientData> taskFun)
            {
                lock (lockObj)
                {
                    ReceiveDataFlag = SocketHelper.GetOneTaskEndData();
                    ReceiveDataFlag.TaskStop = false;
                    Task.Factory.StartNew((obj) => {
                        ClientData clientData = (ClientData)obj;
                        taskFun(clientData);
                    }, this);
                }
            }

            public void StopReceiveDataTask()
            {
                lock (lockObj)
                {
                    if (ReceiveDataFlag != null)
                    {
                        ReceiveDataFlag.TaskStop = true;
                        ReceiveDataFlag = null;
                    }
                }
            }

            #endregion

            bool isClose = false;

            /// <summary>
            /// 关闭连接
            /// </summary>
            public void Close()
            {
                lock (lockObj)
                {
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("SocketClient.cs-->1137-->Close"));
                    }
                    if (isClose)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        return;
                    }
                    isClose = true;
                    if (lastBuffer != null)
                    {
                        Proto.PutBackByteBufferData(lastBuffer);
                        lastBuffer = null;
                    }
                    HertCount = 0;
                    StopReceiveDataTask();
                    if (TcpSocket != null)
                    {
                        try
                        {
                            TcpSocket.Shutdown(SocketShutdown.Both);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"{ex.Message}");
                        }
                        try
                        {
                            TcpSocket.Close();
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"{ex.Message}");
                        }
                        TcpSocket = null;
                    }
                    SocketServer = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
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

        class ClientSocketData : ISimplePoolData
        {
            public ClientData ClientData;

            public Socket ClientSocket;

            public List<Proto_Package_Data> Datas;

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
                Datas = null;
                ClientData = null;
                ClientSocket = null;
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

