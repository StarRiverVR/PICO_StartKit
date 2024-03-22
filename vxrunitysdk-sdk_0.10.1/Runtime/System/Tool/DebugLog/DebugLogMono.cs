using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
    public class DebugLogMono : MonoSingleton<DebugLogMono>
    {

        protected override void AwakeFun()
        {
            base.AwakeFun();
            if (Application.isPlaying)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif

        }

        private void Start()
        {

        }

        int frameCount = 0;

        int targetFrameCount = 3;

        private void Update()
        {
            frameCount = frameCount + 1;
            if (frameCount== targetFrameCount)
            {
#if !UNITY_EDITOR
            ips.Clear();
            if (DebugLogAsset.data != null && DebugLogAsset.data.ips.Count > 0)
            {
                for (int i = 0; i < DebugLogAsset.data.ips.Count; ++i)
                {
                    ips.Add(DebugLogAsset.data.ips[i]);
                }
                port=DebugLogAsset.data.Port;
            }
            StartConnect();
#endif
            }else if (frameCount==int.MaxValue-1)
            {
                frameCount = targetFrameCount+1;
            }
        }

        static List<string> ips = new List<string>();

        static int port;

        static void StartConnect()
        {
            if (ips.Count>0)
            {
                string ip = ips[0];
                ips.RemoveAt(0);

                //if (!DebugLogClient.Open(ip, ClientRestartConnect, ClientCloseCallBack))
                //{
                //    Invoke("StartConnect", 1f);
                //}

                DebugLogClient.OpenAsync((bl) =>
                {
                    if (!bl)
                    {
                        StartConnect();
                    }
                    else
                    {
                        VLog.Info("DebugLogMono Connect !");
                    }
                }, ip, port, ClientRestartConnect, ClientCloseCallBack);
            }

        }

        static void ClientRestartConnect(bool bl)
        {
            VLog.Info($"连接重连:{bl}");
        }

        static void ClientCloseCallBack()
        {
            VLog.Info("连接关闭");
        }
    }

}

