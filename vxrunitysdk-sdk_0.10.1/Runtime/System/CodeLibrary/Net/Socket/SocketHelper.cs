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
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class SocketHelper
    {
#if UNITY_EDITOR
        static SocketHelper()
        {
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
                InitEndian();
            }
        }
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            InitEndian();
        }

        /// <summary>
        /// 获取当前网络状态
        /// 返回值 -1 网络未连通
        /// 返回值 1 数据流量连通
        /// 返回值 2 Wifi网络连通
        /// </summary>
        public static int getGameNet()
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable://没有任何网络
                    return -1;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return 1;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return 2;
            }
            return -1;
        }

        public const int MAX_READ_BUFFER_SIZE = 4096;

        public const int MAX_SEND_BUFFER_SIZE = 1024;

        static Int16 sysEndian = -1;//1:小端模式 2:大端模式

        static Int16 endian = 1;//强制使用的端排列顺序 1:小端模式 2:大端模式

        static void InitEndian()
        {
            if (sysEndian == -1)
            {
                Int16 num = 1;
                byte[] b = BitConverter.GetBytes(num);
                if (b[0] == 1) { sysEndian = 1; } else { sysEndian = 2; }
            }
        }

        public static TaskEndData GetOneTaskEndData()
        {
            TaskEndData data = taskEndDataPool.Spawn();
            data.TaskStop = false;
            return data;
        }

        public static void PutBackOne(TaskEndData data)
        {
            if (data==null) return;
            taskEndDataPool.Recycle(data);
        }

        static SimplePool<TaskEndData> taskEndDataPool = new SimplePool<TaskEndData>();

        public class TaskEndData : SimpleData
        {
            bool taskStop = false;

            public bool TaskStop
            {
                get
                {
                    lock (lockObj)
                    {
                        return taskStop;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        taskStop = value;
                    }
                }
            }

            object lockObj = new object();
        }

    }

}


