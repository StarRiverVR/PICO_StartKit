using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.vivo.codelibrary
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class InformationManagerInit
    {
#if UNITY_EDITOR
        static InformationManagerInit()
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
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
            SceneManager.activeSceneChanged += ActiveSceneChanged;
        }

        static void ActiveSceneChanged(Scene s1, Scene s2)
        {
            InformationManager.Instance.SceneInformationCenterClear();
        }
    }

    public class InformationManager : CSingleton<InformationManager>, IDisposable
    {
        /// <summary>
        /// 不会清理消息池
        /// </summary>
        public InformationCenter GameInformationCenter = new InformationCenter();

        /// <summary>
        /// 场景跳转需要清理消息池
        /// </summary>
        public InformationCenter SceneInformationCenter = new InformationCenter();

        public void SceneInformationCenterClear()
        {
            SceneInformationCenter.Clear();
        }

        void IDisposable.Dispose()
        {
            GameInformationCenter.Dispose();
            SceneInformationCenter.Dispose();
        }
    }
}


