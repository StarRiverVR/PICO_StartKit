using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class MonoSingletonCreateInit
    {
#if UNITY_EDITOR
        static MonoSingletonCreateInit()
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

        [RuntimeInitializeOnLoadMethod]
        static void Install()
        {
            List<Type> list = new List<Type>();
            CLR.GetAllChildType(typeof(MonoSingleton<>), ref list,false);
            Type monoSingletonCreateType = typeof(MonoSingletonCreate);
            for (int i=0;i< list.Count;++i)
            {
                Type type = list[i];
                object[] objs = type.GetCustomAttributes(false);
                for (int j=0;j< objs.Length;++j)
                {
                    object obj = objs[j];
                    if (obj.GetType() == monoSingletonCreateType)
                    {
                        MonoSingletonCreate monoSingletonCreate = obj as MonoSingletonCreate;
                        if (!Application.isPlaying)
                        {
                            switch (monoSingletonCreate.MonoSingletonCreateType)
                            {
                                case MonoSingletonCreateType.OnGameStart:
                                    {

                                    }
                                    break;
                                case MonoSingletonCreateType.OnEditorStart:
                                case MonoSingletonCreateType.OnGameAndEditorStart:
                                    {
                                        try
                                        {
                                            System.Reflection.PropertyInfo propertyInfo = type.BaseType.GetProperty("Instance");
                                            object instance = propertyInfo.GetValue(System.Activator.CreateInstance(type));
                                            MonoBehaviour monoBehaviour = instance as MonoBehaviour;
                                            if (!string.IsNullOrEmpty(monoSingletonCreate.GameObjectName))
                                            {
                                                monoBehaviour.gameObject.name = monoSingletonCreate.GameObjectName;
                                            }
#if UNITY_EDITOR
                                            monoBehaviour.gameObject.hideFlags = monoSingletonCreate.EditorHideFlags;
#else
                                            monoBehaviour.gameObject.hideFlags = monoSingletonCreate.AppRuntimeHideFlags;
#endif
                                        }
                                        catch (System.Exception ex)
                                        {
                                            VLog.Exception(ex);
                                        }

                                    }
                                    break;
                            }
                        }
                        else
                        {
                            switch (monoSingletonCreate.MonoSingletonCreateType)
                            {

                                case MonoSingletonCreateType.OnGameStart:
                                case MonoSingletonCreateType.OnGameAndEditorStart:
                                    {
                                        try
                                        {
                                            System.Reflection.PropertyInfo propertyInfo = type.BaseType.GetProperty("Instance");
                                            object instance = propertyInfo.GetValue(System.Activator.CreateInstance(type));
                                            MonoBehaviour monoBehaviour = instance as MonoBehaviour;
                                            if (monoSingletonCreate.DontDestroyOnLoad)
                                            {
                                                GameObject.DontDestroyOnLoad(monoBehaviour.gameObject);
                                            }
                                            if (!string.IsNullOrEmpty(monoSingletonCreate.GameObjectName))
                                            {
                                                monoBehaviour.gameObject.name = monoSingletonCreate.GameObjectName;
                                            }
#if UNITY_EDITOR
                                            monoBehaviour.gameObject.hideFlags = monoSingletonCreate.EditorHideFlags;
#else
                                            monoBehaviour.gameObject.hideFlags = monoSingletonCreate.AppRuntimeHideFlags;
#endif
                                        }
                                        catch (System.Exception ex)
                                        {
                                            VLog.Exception(ex);
                                        }
                                    }
                                    break;
                                case MonoSingletonCreateType.OnEditorStart:
                                    {

                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class MonoSingletonCreate : Attribute
    {
        public MonoSingletonCreateType MonoSingletonCreateType;

        public bool DontDestroyOnLoad;

        public string GameObjectName;

        public HideFlags EditorHideFlags;

        public HideFlags AppRuntimeHideFlags;

        public MonoSingletonCreate(MonoSingletonCreateType createType,bool dontDestroyOnLoad,string gameObjectName=null, HideFlags editorHideFlags = HideFlags.None, HideFlags appRuntimeHideFlags= HideFlags.None)
        {
            MonoSingletonCreateType = createType;
            DontDestroyOnLoad = dontDestroyOnLoad;
            GameObjectName = gameObjectName;
            EditorHideFlags = editorHideFlags;
            AppRuntimeHideFlags = appRuntimeHideFlags;
        }
    }

    public enum MonoSingletonCreateType
    {
        OnGameStart,
        OnEditorStart,
        OnGameAndEditorStart,
    }

}


