
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 结束运行状态 销毁时会释放 CSingleton<T>
    /// </summary>
    public class CSingletonManager : MonoBehaviour
    {
        static object lockObj = new object();

        static CSingletonManager instance;

        public static CSingletonManager Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        if (System.Threading.SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((obj) => {
                                GameObject newObj = new GameObject("CSingletonManager");
                                instance = newObj.AddComponent<CSingletonManager>();
                                if (Application.isPlaying)
                                {
                                    GameObject.DontDestroyOnLoad(newObj);
                                }
#if !UNITY_EDITOR
                        newObj.hideFlags = HideFlags.HideInHierarchy;
#endif
                            }, null);
                        }
                        else
                        {
                            GameObject newObj = new GameObject("CSingletonManager");
                            instance = newObj.AddComponent<CSingletonManager>();
                            if (Application.isPlaying)
                            {
                                GameObject.DontDestroyOnLoad(newObj);
                            }
#if !UNITY_EDITOR
                        newObj.hideFlags = HideFlags.HideInHierarchy;
#endif
                        }
                    }
                    return instance;
                }

            }
        }

        System.Action releaseFun;

        public void AddReleaseFun(System.Action act)
        {
            lock (lockObj)
            {
                releaseFun -= act;
                releaseFun += act;
            }
        }

        void OnRelease()
        {
            try
            {
                lock (lockObj)
                {
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(string.Format("CSingletonManager.cs-->73-->OnRelease"));
                    }
                    releaseFun.Invoke();
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
            catch
            {

            }
        }

        private void OnDestroy()
        {
            instance = null;
            OnRelease();
        }

    }
}


