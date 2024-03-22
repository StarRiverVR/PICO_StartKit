using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace com.vivo.codelibrary
{
    public class SimpleStringBuilderPool
    {
        private Stack<StringBuilder> frees = new Stack<StringBuilder>();

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

        private int maxLimit = -1;

        public SimpleStringBuilderPool(int maxLimit = -1)
        {
            this.maxLimit = maxLimit;
        }

        public StringBuilder Spawn(int listCapacity = -1)
        {
            StringBuilder findData = null;
            lock (frees)
            {
                while (frees.Count > 0 && findData == null)
                {
                    findData = frees.Pop();
                }
                if (findData != null)
                {
                    return findData;
                }
            }
            if (listCapacity > 0)
            {
                findData = new StringBuilder(listCapacity);
                return findData;
            }
            else
            {
                return new StringBuilder();
            }
        }
        public void Recycle(StringBuilder one)
        {
            if (one != null)
            {
                lock (frees)
                {
                    lock (one)
                    {
                        if (one.Length > 0)
                        {
                            if (maxLimit > 0 && one.Length > maxLimit)
                            {
                                one.Capacity = maxLimit;
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
                    StringBuilder temp = frees.Pop();
                    if (temp!=null)
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


