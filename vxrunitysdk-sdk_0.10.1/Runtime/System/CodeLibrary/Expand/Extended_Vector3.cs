
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    public static class Extended_Vector3
    {
        /// <summary>
        /// 获取点到直线的最小距离
        /// </summary>
        /// <param name="position"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float PointToLine(this Vector3 position, Vector3 point1, Vector3 point2)//point1和point2为线的两个端点
        {
            Vector2 p = new Vector2(position.x, position.z);
            Vector2 p1 = new Vector2(point1.x, point1.z);
            Vector2 p2 = new Vector2(point2.x, point2.z);
            return p.PointToLine(p1, p2);
        }

        /// <summary>
        /// 去掉三维向量的Y轴，把向量投射到xz平面。
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector2 IgnoreYAxis(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        /// 判断目标点是否位于向量的左边
        /// </summary>
        /// <param name="startPoint">向量起点</param>
        /// <param name="endPoint">向量终点</param>
        /// <param name="point">目标点</param>
        /// <returns> >0 右边 ，==0 线上, <0左边</returns>
        public static float PointOnLeftSideOfVector(this Vector3 startPoint, Vector3 endPoint,
        Vector3 point)
        {
            return Vector3.Cross(endPoint - startPoint, point - startPoint).y;
        }

        /// <summary>
        /// 获得左右方向向量
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns>0:左向量 1:右向量</returns>
        public static Vector3[] GetLeftRightVector(this Vector3 startPoint, Vector3 endPoint)
        {
            Vector3[] res = new Vector3[2];
            Vector3 v = endPoint - startPoint;
            res[0] = Vector3.Cross(v, Vector3.up);
            res[1] = -res[0];
            return res;
        }
    }
}

