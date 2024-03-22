using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 所有继续自MonoBehaviour类的单例的基类，以免每个单都写一次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T _instance = null;

        protected static object lockObj = new object();

        /// <summary>
        ///  获取单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance!=null)
                        {
                            return _instance;
                        }
                        if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((obj) => {

                                T[] objs = UnityEngine.Object.FindObjectsOfType<T>();
                                for (int i = 0; i < objs.Length; ++i)
                                {
                                    T t = objs[i];
                                    MonoBehaviour m = t;
                                    if (Application.isPlaying)
                                    {
                                        GameObject.Destroy(m.gameObject);
                                    }
                                    else
                                    {
                                        GameObject.DestroyImmediate(m.gameObject);
                                    }
                                }

                                Type type = typeof(T);
                                string instanceName = type.FullName;
                                GameObject go = new GameObject(instanceName);
                                _instance = go.GetComponent<T>();
                                if (_instance == null)
                                {
                                    _instance = go.AddComponent<T>();
                                }
                            }, null);
                        }
                        else
                        {
                            T[] objs = UnityEngine.Object.FindObjectsOfType<T>();
                            for (int i = 0; i < objs.Length; ++i)
                            {
                                T t = objs[i];
                                MonoBehaviour m = t;
                                if (Application.isPlaying)
                                {
                                    GameObject.Destroy(m.gameObject);
                                }
                                else
                                {
                                    GameObject.DestroyImmediate(m.gameObject);
                                }
                            }

                            Type type = typeof(T);
                            string instanceName = type.FullName;
                            GameObject go = new GameObject(instanceName);
                            _instance = go.GetComponent<T>();
                            if (_instance == null)
                            {
                                _instance = go.AddComponent<T>();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance.gameObject != gameObject)
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
                return;
            }
            AwakeFun();
            if (_instance == null)
                _instance = this as T;
        }

        protected virtual void AwakeFun()
        {

        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected virtual void OnApplicationQuit()
        {

        }
    }

}
