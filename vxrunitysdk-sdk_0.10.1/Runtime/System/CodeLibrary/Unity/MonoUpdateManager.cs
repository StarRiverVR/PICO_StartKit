using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

namespace com.vivo.codelibrary
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class MonoUpdateManagerInit
    {
#if UNITY_EDITOR
        static MonoUpdateManagerInit()
        {
            UnityEditor.EditorApplication.delayCall -= DoSomethingPrepare;
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
                Install();
            }
            else
            {
                Install();
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            MonoUpdateManager monoUpdateManager = MonoUpdateManager.Instance;
        }
    }

    [ExecuteInEditMode]
    public class MonoUpdateManager : MonoSingleton<MonoUpdateManager>
    {
        object lockMonoUpdateObj = new object();

        float nowGameTime = 0;

        int frameCount = 0;

        public int FrameCount
        {
            get
            {
                return frameCount;
            }
        }

        /// <summary>
        /// 当前游戏时间
        /// </summary>
        public float NowGameTime
        {
            get
            {
                lock (lockMonoUpdateObj)
                {
                    return nowGameTime;
                }
            }
        }

        float nowRealtimeTime;

        /// <summary>
        /// 当前物理时间
        /// </summary>
        public float NowRealtimeTime
        {
            get
            {
                lock (lockMonoUpdateObj)
                {
                    return nowRealtimeTime;
                }
            }
        }

        protected override void AwakeFun()
        {
            base.AwakeFun();
            if (Application.isPlaying)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
//#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideInHierarchy;
//#endif
            lock (lockMonoUpdateObj)
            {
                nowGameTime = 0;
                nowRealtimeTime = 0;
            }
        }

        private void Update()
        {
            lock (lockMonoUpdateObj)
            {
                nowGameTime = Time.deltaTime + nowGameTime;
                nowRealtimeTime = Time.realtimeSinceStartup;
            }
            InformationManager.Instance.GameInformationCenter.Send<MonoUpdateMsg>((int)MonoUpdateMsg.Update,true);
            frameCount++;
            if (frameCount>=int.MaxValue-1)
            {
                frameCount = 0;
            }
        }

        private void LateUpdate()
        {
            InformationManager.Instance.GameInformationCenter.Send<MonoUpdateMsg>((int)MonoUpdateMsg.LateUpdate, true);
        }

        private void FixedUpdate()
        {
            InformationManager.Instance.GameInformationCenter.Send<MonoUpdateMsg>((int)MonoUpdateMsg.FixedUpdate, true);
        }
    }

    public enum MonoUpdateMsg
    {
        Update,
        FixedUpdate,
        LateUpdate,
    }
}


