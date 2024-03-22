using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 需要在游戏启动的时候执行
    /// 用于对 Model 进行初始化
    /// </summary>
    public class MVCInitialization : MonoSingleton<MVCInitialization>
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            MVCInitialization sc = MVCInitialization.Instance;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void AwakeFun()
        {
            gameObject.name = "MVCInitialization";
            gameObject.isStatic = true;
            GameObject.DontDestroyOnLoad(gameObject);
#if !UNITY_EDITOR
                gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
            lock (lockObj)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
            }
            Init();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        bool isInit = false;

        public void Init()
        {
            if (isInit) return;
            isInit = true;
            List<Type> list = new List<Type>();
            CLR.GetTypes(GetType(), null, typeof(IModel), ref list);
            for (int i = 0, listCount = list.Count; i < listCount; ++i)
            {
                IModel data = CLR.CreateInstance<IModel>(list[i]);
                if (data != null)
                {
                    System.Reflection.MethodInfo methodInfo = CLR.GetMethod(data.GetType(), null, "Initialization");
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(data, null);
                    }
                }
            }
        }
    }
}



