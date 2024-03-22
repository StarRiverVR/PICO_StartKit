using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// MonoBehaviour 池子 只能在Unity主线程使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleMonoPool<T>:UnityEngine.Object where T : MonoBehaviour, ISimplePoolData
    {
        GameObject prefab;

        Transform poolRoot;

        /// <summary>
        /// 传入克隆预制体
        /// </summary>
        /// <param name="prefab"></param>
        public SimpleMonoPool(GameObject prefab)
        {
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    Init(prefab);
                },null);
            }
            else
            {
                Init(prefab);
            }
        }

        void Init(GameObject prefab)
        {
            this.prefab = prefab;
            if (this.prefab == null)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }
            T t = this.prefab.GetComponent<T>();
            if (t == null)
            {
                this.prefab = null;
                UnityEngine.Object.Destroy(this);
                return;
            }
            SimpleMonoPoolFlag simpleMonoPoolFlag = prefab.AddComponent<SimpleMonoPoolFlag>();
            simpleMonoPoolFlag.PoolClear = Clear;
            poolRoot = (new GameObject(typeof(T).Name)).transform;
            GameObject.DontDestroyOnLoad(poolRoot.gameObject);
            poolRoot.transform.position = new Vector3(100000,100000,100000);
#if !UNITY_EDITOR
            poolRoot.gameObject.hideFlags = HideFlags.HideInHierarchy;
#endif
        }

        private Stack<T> frees = new Stack<T>();

        public T Spawn()
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                VLog.Error($"SimpleMonoPool只能在Unity主线程使用:{prefab}");
                return null;
            }
            if (prefab == null) return null;
            T findData = null;
            while (frees.Count > 0 && (findData == null || findData.Disposed))
            {
                findData = frees.Pop();
            }
            if (findData != null)
            {
                if (!findData.Disposed)
                {
                    findData.PutOut();
                    return findData;
                }
            }
            GameObject newObj = Instantiate(prefab);
            T temp = newObj.GetComponent<T>();
            temp.PutOut();
            return temp;
        }

        public void Recycle(T one)
        {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
            {
                VLog.Error($"SimpleMonoPool只能在Unity主线程使用:{prefab}");
                return;
            }
            if (one != null)
            {
                if (one.IsUsed)
                {
                    one.PutIn();
                    frees.Push(one);
                    one.transform.SetParent(poolRoot,false);
                    one.transform.localPosition = Vector3.zero;
                }
            }
        }
        void Clear()
        {
            try{
                for (int i = 0; i < frees.Count; i++)
                {
                    T temp = frees.Pop();
                    lock (temp)
                    {
                        temp.Dispose();
                    }
                }
                if (poolRoot != null)
                {
                    GameObject.Destroy(poolRoot.gameObject);
                }
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }
            poolRoot = null;
            prefab = null;
        }
    }

}
