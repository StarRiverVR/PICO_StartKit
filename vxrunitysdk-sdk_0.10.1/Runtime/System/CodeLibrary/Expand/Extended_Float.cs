
using UnityEngine;

namespace com.vivo.codelibrary
{
    public static class Extended_Float
    {
        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="num">float本身</param>
        /// <param name="count">小数点后面几位</param>
        /// <param name="confine">边界 0-9的数,默认是5，逢5进1</param>
        /// <returns></returns>
        public static float Round(this float num, int count, int confine = 5)
        {
            if (num == 0) return 0;
            confine = (confine > 9) ? 9 : confine;
            confine = (confine < 0) ? 0 : confine;
            if (count < 0) { return num; }
            int a = (int)(Mathf.Abs(num) / num);
            num = Mathf.Abs(num);
            //整数部分
            float r = num * Mathf.Pow(10, count);
            int i = (int)r;
            int j = (int)((r - i) * 10);
            i = (j >= confine) ? i + 1 : i;
            return (i / Mathf.Pow(10, count)) * a;
        }

        /// <summary>
        /// Sin值 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float Sin(this float angle)
        {
            return Mathf.Sin(Mathf.Deg2Rad * angle);
        }

        /// <summary>
        /// 根据Sin值获得角度
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        public static float AngleBySin(this float sin)
        {
            return Mathf.Asin(sin) * 180 / Mathf.PI;
        }

        /// <summary>
        /// Cos值 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float Cos(this float angle)
        {
            return Mathf.Cos(Mathf.Deg2Rad * angle);
        }

        /// <summary>
        /// Tan值 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float Tan(this float angle)
        {
            return Mathf.Tan(Mathf.Deg2Rad * angle);
        }

        /// <summary>
        /// 根据Tan值获得角度
        /// </summary>
        /// <param name="tan"></param>
        /// <returns></returns>
        public static float AngleByTan(this float tan)
        {
            return Mathf.Atan(tan) * 180 / Mathf.PI;
        }
    }
}

