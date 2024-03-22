
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    public static class Extended_Vector2
    {
        /// <summary>
        /// 获取点到直线的最小距离
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static float PointToLine(this Vector2 position, Vector2 point1, Vector2 point2)//point1和point2为线的两个端点
        {
            float space = 0;
            float a, b, c;
            a = Vector2.Distance(point1, point2);// 线段的长度      
            b = Vector2.Distance(point1, position);// position到点point1的距离      
            c = Vector2.Distance(point2, position);// position到point2点的距离 
            if (c <= 0.000001 || b <= 0.000001)
            {
                space = 0;
                return space;
            }
            if (a <= 0.000001)
            {
                space = b;
                return space;
            }
            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }
            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }
            float p = (a + b + c) / 2;// 半周长      
            float s = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));// 海伦公式求面积      
            space = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）      
            return space;
        }

    }
}


