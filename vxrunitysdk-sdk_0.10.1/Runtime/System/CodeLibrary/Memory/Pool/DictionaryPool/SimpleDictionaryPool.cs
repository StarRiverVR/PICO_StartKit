using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 用于字典内存池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SimpleDictionaryPool<T, TKey, TValue> where T : ICollection<KeyValuePair<TKey, TValue>>, new()
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

        private int maxLimit=-1;

        public SimpleDictionaryPool(int maxLimit=-1)
        {
            this.maxLimit = maxLimit;
        }

        public T Spawn(int listCapacity = -1)
        {
            T findData = default(T);
            lock (frees)
            {
                while (frees.Count > 0 && findData==null)
                {
                    findData = frees.Pop();
                }
                if (findData!=null)
                {
                    return findData;
                }
            }
            if (listCapacity > 0)
            {
                Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>(listCapacity);
                object obj = dic;
                return (T)obj;
            }
            else
            {
                T temp = new T();
                return temp;
            }
        }
        public void Recycle(T one)
        {
            if (one != null)
            {
                lock (frees)
                {
                    lock (one)
                    {
                        if (one.Count > 0)
                        {
                            if (maxLimit > 0 && one.Count > maxLimit)
                            {
                                Dictionary<TKey, TValue> dic = one as Dictionary<TKey, TValue>;
                                dic.EnsureCapacity(maxLimit);
                            }
                            one.Clear();
                        }
                    }
                    if (!frees.Contains(one))
                    {
                        frees.Push(one);
                    }
                    if (Count>1024)
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
                for (int i = 0; i < frees.Count; i++)
                {
                    T temp = frees.Pop();
                    if (temp != null)
                    {
                        lock (temp)
                        {
                            temp.Clear();
                        }
                    }
                }
            }
        }
    }

}


