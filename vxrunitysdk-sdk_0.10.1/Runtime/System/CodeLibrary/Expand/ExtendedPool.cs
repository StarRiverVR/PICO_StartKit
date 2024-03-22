using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 数据池子
    /// </summary>
    public class ExtendedPool
    {
        static SimpleStringBuilderPool stringBuilderPool = new SimpleStringBuilderPool();

        public static StringBuilder GetOneStringBuilder()
        {
            return stringBuilderPool.Spawn();
        }

        public static void PutBackOneStringBuilder(StringBuilder sb)
        {
            stringBuilderPool.Recycle(sb);
        }
    }
}

