using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
    public class DebugLogCoroutine
    {

        public static int NetPort
        {
            get
            {
                int min = 21001;
                int max = 31001;
                for (int i= min; i<= max;++i)
                {
                    if (!IPUtility.PortIsOccupy(i))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        static List<CoroutineData> coroutineDatas = new List<CoroutineData>();

        public static void StartCoroutine()
        {
            CoroutineData data = new CoroutineData();
            data.Open = true;
            coroutineDatas.Add(data);
            GameCoroutine.Instance.StartCoroutine(SeverStart(data));
        }

        public static void StopCoroutine()
        {
            for (int i = 0; i < coroutineDatas.Count; ++i)
            {
                if (coroutineDatas[i] != null)
                {
                    coroutineDatas[i].Open = false;
                }
            }
            coroutineDatas.Clear();
        }

        static IEnumerator SeverStart(CoroutineData data)
        {
            CoroutineData coroutineData = data;
            int editorCount = 0;
            while (coroutineData.Open)
            {
                if (Application.isPlaying)
                {
                    if (DebugLogServer.CurLogServer == null)
                    {
                        OpenServer();
                        if (DebugLogServer.CurLogServer!=null)
                        {
                            coroutineData.Open = false;
                            coroutineDatas.Remove(coroutineData);
                        }
                    }
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    if (editorCount>=15)
                    {
                        editorCount = 0;
                        if (DebugLogServer.CurLogServer == null)
                        {
                            OpenServer();
                            if (DebugLogServer.CurLogServer != null)
                            {
                                coroutineData.Open = false;
                                coroutineDatas.Remove(coroutineData);
                            }
                        }
                    }
                    else
                    {
                        editorCount++;
                    }
                    yield return null;
                }
            }
        }

        static int setPort = 0;

        static void OpenServer()
        {

#if UNITY_EDITOR

            int p = NetPort;
            bool bl = DebugLogServer.OpenServer();
            if (bl)
            {
                VLog.Warning("DebugLogServer Start !");
                DebugLogAsset.data.ips = new List<string>();
                DebugLogAsset.data.Port = p;
                List<string> ipv4s = IPUtility.GetLocalIpAddress("InterNetwork");
                if (ipv4s != null && ipv4s.Count > 0)
                {
                    for (int i = 0; i < ipv4s.Count; ++i)
                    {
                        DebugLogAsset.data.ips.Add(ipv4s[i]);
                    }
                }
                EditorUtility.SetDirty(DebugLogAsset.data);
            }
            if (Application.isPlaying)
            {
                DebugLogMono debugLogMono = DebugLogMono.Instance;
            }
#else
            DebugLogMono debugLogMono = DebugLogMono.Instance;
#endif
        }

        public class CoroutineData
        {
            public bool Open = false;
        }
    }
}


