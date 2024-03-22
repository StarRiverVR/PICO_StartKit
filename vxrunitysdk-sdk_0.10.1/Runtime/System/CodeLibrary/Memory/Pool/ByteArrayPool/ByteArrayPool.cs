
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.vivo.codelibrary
{
    public class ByteArrayPool
    {
        private Dictionary<int, Stack<byte[]>> dic = new Dictionary<int, Stack<byte[]>>();

        public int Count
        {
            get
            {
                lock (dic)
                {
                    return dic.Count;
                }
            }
        }

        public byte[] Spawn(int size)
        {
            byte[] findData = null;
            lock (dic)
            {
                Stack<byte[]> frees = null;
                if (!dic.TryGetValue(size,out frees))
                {
                    frees = new Stack<byte[]>();
                    dic.Add(size, frees);
                }

                while (frees.Count > 0 && findData == null)
                {
                    findData = frees.Pop();
                }
                if (findData != null)
                {
                    return findData;
                }
            }
            return new byte[size];
        }

        public void Recycle(byte[] one)
        {
            if (one != null)
            {
                lock (dic)
                {
                    Stack<byte[]> frees = null;
                    if (!dic.TryGetValue(one.Length, out frees))
                    {
                        frees = new Stack<byte[]>();
                        dic.Add(one.Length, frees);
                    }
                    frees.Push(one);
                    if (frees.Count>128)
                    {
                        frees.Clear();
                    }
                    if (Count>512)
                    {
                        Clear();
                    }
                }
            }
        }
        public void Clear()
        {
            lock (dic)
            {
                dic.Clear();
            }
        }
    }
}


