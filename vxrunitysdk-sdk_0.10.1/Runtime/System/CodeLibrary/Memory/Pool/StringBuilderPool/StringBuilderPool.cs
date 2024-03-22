using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace com.vivo.codelibrary
{
    public class StringBuilderPool : CSingleton<StringBuilderPool>, IDisposable
    {
        SimpleStringBuilderPool simpleStringBuilderPool = new SimpleStringBuilderPool();

        public StringBuilder GetOneStringBuilder(int listCapacity = -1)
        {
            return simpleStringBuilderPool.Spawn(listCapacity);
        }

        public void PutBackOneStringBuilder(StringBuilder one)
        {
            simpleStringBuilderPool.Recycle(one);
        }

        public void Dispose()
        {
            simpleStringBuilderPool.Clear();
        }
    }
}


