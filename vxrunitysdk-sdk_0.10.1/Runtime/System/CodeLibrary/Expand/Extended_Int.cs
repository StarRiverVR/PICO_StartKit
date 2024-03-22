using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public static class Extended_Int
    {
        /// <summary>
        /// 分解整数各个位
        /// SplitIntResoultData使用完之后需要进行数据回收 SplitIntResoultData.PutBackOne()
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static SplitIntResoultData SplitInt(this int number)
        {
            number = Mathf.Abs(number);
            int length = number.ToString().Length;
            SplitIntResoultData res = SplitIntResoultData.GetOne();
            int index = 0;
            while (number != 0)
            {
                res.Resoult.Add(number % 10);
                number /= 10;
                index++;
            }
            res.Resoult.Reverse();
            return res;
        }
    }

    /// <summary>
    /// 整数拆分结果  使用完之后需要进行数据回收 SplitIntResoultData.PutBackOne()
    /// </summary>
    public class SplitIntResoultData
    {

        public bool IsPutBack = false;

        /// <summary>
        /// 存放整数每个位的数
        /// </summary>
        public List<int> Resoult = new List<int>();

        #region//池子

        static Queue<SplitIntResoultData> pool = new Queue<SplitIntResoultData>();

        /// <summary>
        /// 取出数据
        /// </summary>
        /// <returns></returns>
        public static SplitIntResoultData GetOne()
        {
            if (pool.Count > 0)
            {
                SplitIntResoultData findData = pool.Dequeue();
                while (findData == null && pool.Count > 0)
                {
                    findData = pool.Dequeue();
                }
                if (findData != null)
                {
                    findData.IsPutBack = false;
                    return findData;
                }
            }
            SplitIntResoultData newData = new SplitIntResoultData();
            newData.IsPutBack = false;
            return newData;
        }

        /// <summary>
        /// 回收数据
        /// </summary>
        /// <param name="data"></param>
        public static void PutBackOne(SplitIntResoultData data)
        {
            if (data.IsPutBack) return;
            data.IsPutBack = true;
            data.Resoult.Clear();
            pool.Enqueue(data);
        }

        #endregion
    }
}

