using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.vivo.codelibrary
{
    public class SimplePool<T> where T : ISimplePoolData, new()
    {
        private Stack<T> frees = new Stack<T>();

        public int Count
        {
            get
            {
                lock (frees)
                {
                    return frees.Count;
                }
            }
        }

        public T Spawn()
        {
            T findData = default(T);
            lock (frees)
            {
                while (frees.Count > 0 && (findData == null || findData.Disposed))
                {
                    findData = frees.Pop();
                }
                if (findData != null)
                {
                    lock (findData)
                    {
                        if (!findData.Disposed)
                        {
                            findData.PutOut();
                            return findData;
                        }
                    }
                }
            }
            T temp = new T();
            temp.PutOut();
            return temp;
        }

        public void Recycle(T one)
        {
            if (one != null)
            {
                lock (frees)
                {
                    lock (one)
                    {
                        if (one.IsUsed)
                        {
                            one.PutIn();
                            frees.Push(one);
                        }
                    }
                    if (frees.Count>1024)
                    {
                        Clear();
                    }
                }
            }
        }
        public void Clear()
        {
            lock (frees)
            {
                try
                {
                    for (int i = 0; i < frees.Count; i++)
                    {
                        T temp = frees.Pop();
                        lock (temp)
                        {
                            temp.Dispose();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            }
        }
    }

}
