
using UnityEngine;

namespace com.vivo.codelibrary
{
    public static class Extended_Double
    {
        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="num">double本身</param>
        /// <param name="count">小数点后面几位</param>
        /// <param name="confine">边界 0-9的数,默认是5，逢5进1</param>
        /// <returns></returns>
        public static double Round(this double num, int count, int confine = 5)
        {
            if (num == 0) return 0;
            confine = (confine > 9) ? 9 : confine;
            confine = (confine < 0) ? 0 : confine;
            if (count < 0) { return num; }
            int a = (num > 0) ? 1 : -1;
            num = (num > 0) ? num : -num;
            //整数部分
            double r = num * Mathf.Pow(10, count);
            int i = (int)r;
            int j = (int)((r - i) * 10);
            i = (j >= confine) ? i + 1 : i;
            return (i / Mathf.Pow(10, count)) * a;
        }
    }
}


