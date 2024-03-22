using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class Geometry
    {

        /// <summary>
        /// 获得圆圈的点集合
        /// </summary>
        /// <param name="centerPoint">圆中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="count">点数</param>
        /// <returns></returns>
        public static List<Vector2> GetCirclePoint(Vector2 centerPoint, float radius, int count)
        {
            List<Vector2> list = new List<Vector2>();
            //模
            float rad = (360f / count) * UnityEngine.Mathf.Deg2Rad;
            float angle = 0;
            for (int i = 0; i < count; ++i)
            {
                float x = radius * UnityEngine.Mathf.Cos(angle);
                float y = radius * UnityEngine.Mathf.Sin(angle);
                angle = angle + rad;
                list.Add(new Vector2(x + centerPoint.x, y + centerPoint.y));
            }
            return list;
        }

        /// <summary>
        /// 圆形区间
        /// </summary>
        public struct CircleArea
        {
            /// <summary>
            /// 圆点
            /// </summary>
            public Vector2 o;
            /// <summary>
            /// 半径
            /// </summary>
            public float r;
        }

        /// <summary>
        /// 判断圆形与圆形相交
        /// </summary>
        /// <param name="circleArea"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool Circle(CircleArea circleArea, CircleArea target)
        {
            return (circleArea.o - target.o).sqrMagnitude < (circleArea.r + target.r) * (circleArea.r + target.r);
        }

        /// 判断目标点是否位于向量的左边
        /// </summary>
        /// <param name="startPoint">向量起点</param>
        /// <param name="endPoint">向量终点</param>
        /// <param name="point">目标点</param>
        /// <returns> <0 右边 ，==0 线上, >0左边</returns>
        public static float PointOnLeftSideOfVector(Vector3 startPoint, Vector3 endPoint,
        Vector3 point)
        {
            return Vector3.Cross(endPoint - startPoint, point - startPoint).z;
        }

        /// <summary>
        /// 获得一个向量的角度 (与Y轴的旋转角度)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetVectorAngle(Vector2 target)
        {
            float res = PointOnLeftSideOfVector(Vector3.zero, new Vector3(0, 1, 0), target);
            if (res == 0)
            {
                if (target.y < 0)
                {
                    return 180;
                }
                return 0;
            }
            float ang = Vector2.Angle(target, new Vector2(0, 1));
            if (res < 0)
            {
                return ang;
            }
            ang = 360 - ang;
            return ang;
        }

        /// <summary>
        /// Sin值 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float Sin(float angle)
        {
            return Mathf.Sin(Mathf.Deg2Rad * angle);
        }

        /// <summary>
        /// 根据Sin值获得角度
        /// </summary>
        /// <param name="sin"></param>
        /// <returns></returns>
        public static float AngleBySin(float sin)
        {
            return Mathf.Asin(sin) * 180f / Mathf.PI;
        }

        /// <summary>
        /// Cos值 
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float Cos(float angle)
        {
            return Mathf.Cos(Mathf.Deg2Rad * angle);
        }

        static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="P"></param>
        /// <param name="rectCorners"></param>
        /// <returns></returns>
        public static bool IsPointInRectangle(Vector2 P, Vector2[] rectCorners)
        {
            return IsPointInRectangle(P, rectCorners[0], rectCorners[1], rectCorners[2], rectCorners[3]);
        }

        /// <summary>
        /// 判断点是否在矩形内
        /// </summary>
        /// <param name="P"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <returns></returns>
        public static bool IsPointInRectangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C, Vector2 D)
        {
            Vector2 AB = A - B;
            Vector2 AP = A - P;
            Vector2 CD = C - D;
            Vector2 CP = C - P;

            Vector2 DA = D - A;
            Vector2 DP = D - P;
            Vector2 BC = B - C;
            Vector2 BP = B - P;

            bool isBetweenAB_CD = Cross(AB, AP) * Cross(CD, CP) > 0;
            bool isBetweenDA_BC = Cross(DA, DP) * Cross(BC, BP) > 0;
            return isBetweenAB_CD && isBetweenDA_BC;
        }

        #region//点 线

        /// <summary>
        ///  0：在线内 1：在线内端点外 靠近端点1 2：在线内端点外 靠近端点2 3：在端点1重合 4：在端点2重合 5:不在线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns></returns>
        public static byte PointInLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            if (point == linePoint1) { return 3; }
            if (point == linePoint2) { return 4; }
            if (linePoint1 == linePoint2) return 5;
            Vector3 n1 = (point - linePoint1).normalized;
            Vector3 n2 = (point - linePoint2).normalized;
            if (n1 == -n2) { return 0; }
            if (n1 == n2)
            {
                n1 = (linePoint2 - linePoint1).normalized;
                if (n1 == n2) { return 2; }
                else { return 1; }
            }
            return 5;
        }

        /// <summary>
        /// 在ZX投影上 点point在线linePoint1，linePoint2的左边还是右边
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns>0:参数错误，线的点重合 1：点的投影在线的投影上 2：点的投影在线的投影的左边 3：点的投影在线的投影的左边</returns>
        public static byte PointInLineSideZX(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            return PointInLineSideXZ(point, linePoint1, linePoint2, 1);
        }

        /// <summary>
        /// 在XY投影上 点point在线linePoint1，linePoint2的左边还是右边
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns>0:参数错误，线的点重合 1：点的投影在线的投影上 2：点的投影在线的投影的左边 3：点的投影在线的投影的左边</returns>
        public static byte PointInLineSideXY(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            return PointInLineSideXZ(point, linePoint1, linePoint2, 2);
        }

        /// <summary>
        /// 在YZ投影上 点point在线linePoint1，linePoint2的左边还是右边
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns>0:参数错误，线的点重合 1：点的投影在线的投影上 2：点的投影在线的投影的左边 3：点的投影在线的投影的左边</returns>
        public static byte PointInLineSideYZ(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            return PointInLineSideXZ(point, linePoint1, linePoint2, 3);
        }

        /// <summary>
        /// 点point在线linePoint1，linePoint2的左边还是右边
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="type">1:ZX 2:XY 3:YZ</param>
        /// <returns>0:参数错误，线的点重合 1：点的投影在线的投影上 2：点的投影在线的投影的左边 3：点的投影在线的投影的左边</returns>
        static byte PointInLineSideXZ(Vector3 point, Vector3 linePoint1, Vector3 linePoint2, int type)
        {
            if (type == 1) { point.y = 0; linePoint1.y = 0; linePoint2.y = 0; }
            else if (type == 2) { point.z = 0; linePoint1.z = 0; linePoint2.z = 0; }
            else { point.x = 0; linePoint1.x = 0; linePoint2.x = 0; }
            if (linePoint1 == linePoint2) return 0;
            if (point == linePoint1 || point == linePoint2) return 1;
            Vector3 n2 = Vector3.Cross((point - linePoint1), (linePoint2 - linePoint1));
            float res = 0;
            if (type == 1) { res = point.y; }
            else if (type == 2) { res = point.z; }
            else { res = point.x; }
            if (res >= 0) { return 3; }
            else { return 4; }
        }

        /// <summary>
        /// 点到线的最短距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns></returns>
        public static float DisPointToLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            byte res = PointInLine(point, linePoint1, linePoint2);
            if (res != 5) { return 0; }
            if (linePoint1 == linePoint2) { return Vector3.Distance(point, linePoint1); }
            float len = Vector3.Dot((point - linePoint1), (linePoint2 - linePoint1).normalized);
            float len2 = Vector3.Distance(point, linePoint1);
            if (len == 0) { return len2; }
            return UnityEngine.Mathf.Sqrt(len2 * len2 - len * len);
        }

        /// <summary>
        /// 点到直线的最短距离的直线上的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <returns></returns>
        public static Vector3 PointToLineTarget(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            byte res = PointInLine(point, linePoint1, linePoint2);
            if (res != 5) { return point; }
            if (linePoint1 == linePoint2) { return linePoint1; }
            Vector3 n = (linePoint2 - linePoint1).normalized;
            float len = Vector3.Dot((point - linePoint1), n);
            if (len == 0) { return linePoint1; }
            return linePoint1 + n * len;
        }

        #endregion

        #region//点 面

        /// <summary>
        /// 点是否在面上
        /// 三个面点不可重合
        /// </summary>
        /// <param name="point"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns>0:重合 1：在面的正面 2：在面的反面</returns>
        public static byte PointInPanel(Vector3 point, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            Vector3 n1 = Vector3.Cross(panel2 - panel1, panel3 - panel1);
            Vector3 v3;
            if (point == panel1) { v3 = point - panel2; }
            else { v3 = point - panel1; }
            float dot = Vector3.Dot(v3, n1);
            if (dot == 0) { return 0; }
            else if (dot > 0) { return 1; }
            return 2;
        }

        /// <summary>
        /// 点到面的最短距离
        /// 三个面点不可重合
        /// </summary>
        /// <param name="point"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns></returns>
        public static float DisPointToPanel(Vector3 point, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            Vector3 n1 = Vector3.Cross(panel2 - panel1, panel3 - panel1).normalized;
            Vector3 v3;
            if (point == panel1) { v3 = point - panel2; }
            else { v3 = point - panel3; }
            float dot = Vector3.Dot(v3, n1);
            if (dot < 0) { dot = -dot; }
            return dot;
        }

        /// <summary>
        /// 点到面的最短距离的面上的点
        /// 三个面点不可重合
        /// </summary>
        /// <param name="point"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns></returns>
        public static Vector3 PointToPanelTarget(Vector3 point, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            Vector3 n1 = Vector3.Cross(panel2 - panel1, panel3 - panel1).normalized;
            Vector3 v3;
            if (point == panel1) { v3 = point - panel2; }
            else { v3 = point - panel3; }
            float dot = Vector3.Dot(v3, n1);
            if (dot == 0) { return point; }
            return point - dot * n1;
        }

        /// <summary>
        /// 点与面的关系
        /// </summary>
        /// <param name="point"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns>0:参数重复错误 1:在面的正面 2:在面的背面 3:在面上</returns>
        public static byte PointPanelRelation(Vector3 point, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            Vector3 v1 = panel2 - panel1;
            if (v1 == Vector3.zero) return 0;
            Vector3 v2 = panel3 - panel1;
            if (v2 == Vector3.zero || v1 == v2) return 0;
            Vector3 v3 = point - panel1;
            if (v3 == Vector3.zero) return 3;
            Vector3 panelNor = Vector3.Cross(v1, v2);
            float dot = Vector3.Dot(v3, panelNor);
            if (dot >= 0.00001f)
            {
                return 1;
            }
            else if (dot <= -0.00001f)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        #endregion

        #region//线 面

        /// <summary>
        /// 线与面的关系 可获取线与面的交点
        /// 线的两个点不可重合， 三个面点不可重合
        /// 线与面相交,垂直 可获得交点
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <param name="interPoint">如果相交 则是交点坐标</param>
        /// <returns>0：线在面上 1：线与面平行但不重合 2：线与面相交 3:垂直 4:参数错误 传入的点重合 </returns>
        public static byte LinePanelRelation(Vector3 line1, Vector3 line2, Vector3 panel1, Vector3 panel2, Vector3 panel3, ref Vector3 interPoint)
        {
            if (line1 == line2 || panel1 == panel2 || panel1 == panel3 || panel2 == panel3) return 4;
            //先判断平行
            Vector3 lineVect = (line2 - line1).normalized;
            Vector3 n1 = Vector3.Cross(lineVect, (panel2 - panel1)).normalized;
            Vector3 n2 = Vector3.Cross(lineVect, (panel3 - panel1)).normalized;
            Vector3 n3 = Vector3.Cross(lineVect, (panel3 - panel2)).normalized;
            if ((n1 == n2 || n1 == -n2) || (n1 == n3 || n1 == -n3) | (n2 == n3 || n2 == -n3))
            {
                //平行 或者 重合
                float dis = DisPointToPanel(line1, panel1, panel2, panel3);
                if (dis <= 0.00001f) { return 0; }
                else { return 1; }
            }
            else
            {
                //相交
                Vector3 start1 = line1;
                Vector3 p1 = PointToPanelTarget(start1, panel1, panel2, panel3);
                if (p1 == start1)
                {
                    start1 = line2;
                    lineVect = -lineVect;
                }
                float dis = Vector3.Distance(p1, start1);
                float ang = Vector3.Angle((p1 - start1), lineVect);
                if (ang == 180)
                {
                    //垂直
                    interPoint = p1;
                    return 3;
                }
                else if (ang > 90)
                {
                    lineVect = -lineVect;
                    ang = 180 - ang;
                }
                float len = dis / UnityEngine.Mathf.Cos(ang * UnityEngine.Mathf.Deg2Rad);
                interPoint = start1 + lineVect * len;
                return 2;
            }
        }

        /// <summary>
        /// 获得线与面的交点
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <param name="interPoint"></param>
        /// <returns></returns>
        public static bool LinePanelIntersection(Vector3 line1, Vector3 line2, Vector3 panel1, Vector3 panel2, Vector3 panel3, ref Vector3 interPoint)
        {
            interPoint = Vector3.zero;
            if (line1 == line2 || panel1 == panel2 || panel1 == panel3 || panel2 == panel3) return false;
            //先判断平行
            Vector3 lineVect = (line2 - line1).normalized;
            Vector3 n1 = Vector3.Cross(lineVect, (panel2 - panel1)).normalized;
            Vector3 n2 = Vector3.Cross(lineVect, (panel3 - panel1)).normalized;
            Vector3 n3 = Vector3.Cross(lineVect, (panel3 - panel2)).normalized;
            if ((n1 == n2 || n1 == -n2) || (n1 == n3 || n1 == -n3) | (n2 == n3 || n2 == -n3))
            {
                //平行 或者 重合
                return false;
            }
            else
            {
                //相交
                Vector3 start1 = line1;
                Vector3 p1 = PointToPanelTarget(start1, panel1, panel2, panel3);
                if (p1 == start1)
                {
                    start1 = line2;
                    lineVect = -lineVect;
                }
                float dis = Vector3.Distance(p1, start1);
                float ang = Vector3.Angle((p1 - start1), lineVect);
                if (ang == 180)
                {
                    //垂直
                    interPoint = p1;
                    return true;
                }
                else if (ang > 90)
                {
                    lineVect = -lineVect;
                    ang = 180 - ang;
                }
                float len = dis / UnityEngine.Mathf.Cos(ang * UnityEngine.Mathf.Deg2Rad);
                interPoint = start1 + lineVect * len;
                return true;
            }
        }

        /// <summary>
        /// 线与面是否垂直
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns></returns>
        public static bool LinePanelVertical(Vector3 line1, Vector3 line2, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            if (line1 == line2 || panel1 == panel2 || panel1 == panel3 || panel2 == panel3) return false;
            //先判断平行
            Vector3 lineVect = (line2 - line1).normalized;
            Vector3 n1 = Vector3.Cross(lineVect, (panel2 - panel1)).normalized;
            Vector3 n2 = Vector3.Cross(lineVect, (panel3 - panel1)).normalized;
            Vector3 n3 = Vector3.Cross(lineVect, (panel3 - panel2)).normalized;
            if ((n1 == n2 || n1 == -n2) || (n1 == n3 || n1 == -n3) | (n2 == n3 || n2 == -n3))
            {
                //平行 或者 重合
                return false;
            }
            else
            {
                //相交
                Vector3 start1 = line1;
                Vector3 p1 = PointToPanelTarget(start1, panel1, panel2, panel3);
                if (p1 == start1)
                {
                    start1 = line2;
                    lineVect = -lineVect;
                }
                float ang = Vector3.Angle((p1 - start1), lineVect);
                if (ang == 180)
                {
                    //垂直
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 线跟面是否平行或者重合
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        /// <returns>0:不平行也不重合 1：重合 2：平行但不重合</returns>
        public static byte LinePanelParallel(Vector3 line1, Vector3 line2, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            if (line1 == line2 || panel1 == panel2 || panel1 == panel3 || panel2 == panel3) return 0;
            //先判断平行
            Vector3 lineVect = (line2 - line1).normalized;
            Vector3 n1 = Vector3.Cross(lineVect, (panel2 - panel1)).normalized;
            Vector3 n2 = Vector3.Cross(lineVect, (panel3 - panel1)).normalized;
            Vector3 n3 = Vector3.Cross(lineVect, (panel3 - panel2)).normalized;
            if ((n1 == n2 || n1 == -n2) || (n1 == n3 || n1 == -n3) | (n2 == n3 || n2 == -n3))
            {
                //平行 或者 重合
                float dis = DisPointToPanel(line1, panel1, panel2, panel3);
                if (dis <= 0.00001f) { return 1; }
                else { return 2; }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获得线在面上的投影
        /// 返回值line1,line2
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="panel1"></param>
        /// <param name="panel2"></param>
        /// <param name="panel3"></param>
        public static void LineProjectionOnPanel(ref Vector3 line1, ref Vector3 line2, Vector3 panel1, Vector3 panel2, Vector3 panel3)
        {
            line1 = PointToPanelTarget(line1, panel1, panel2, panel3);
            line2 = PointToPanelTarget(line2, panel1, panel2, panel3);
        }

        #endregion

        #region//射线 面

        ///// <summary>
        ///// 获得射线与面的交点
        ///// </summary>
        ///// <param name="rayStart">射线起始点</param>
        ///// <param name="rayForward">射线方向向量</param>
        ///// <param name="rayDistance">射线长度</param>
        ///// <param name="ignoreBack">true:忽略背面 false:正面背面都会计算交点</param>
        ///// <param name="panel1"></param>
        ///// <param name="panel2"></param>
        ///// <param name="panel3"></param>
        ///// <param name="interPoint">交点</param>
        ///// <returns>true:有交点</returns>
        //public static bool RayCastPanel(Vector3 rayStart, Vector3 rayForward, float rayDistance, bool ignoreBack, Vector3 panel1, Vector3 panel2, Vector3 panel3, ref Vector3 interPoint)
        //{
        //    interPoint = Vector3.zero;
        //    if (rayForward == Vector3.zero)
        //    {
        //        Diagnostic.Error("Ray forward vector zero err!");
        //        return false;
        //    }
        //    if (ignoreBack)
        //    {
        //        //判断起点是否在面的正面
        //        byte res = PointPanelRelation(rayStart, panel1, panel2, panel3);
        //        if ()
        //        {

        //        }

        //    }
        //}

        #endregion

        #region//线 线

        /// <summary>
        /// 线与线之间的关系
        /// </summary>
        /// <param name="lineA1"></param>
        /// <param name="lineA2"></param>
        /// <param name="lineB1"></param>
        /// <param name="lineB2"></param>
        /// <param name="point"> 相交的交点,不相交的时候会返回Vector3.zero </param>
        /// <returns>0：参数重合错误 1:平行方向相同且共线 2：平行方向相反且共线 3:没关系的两条线 4:相交 5:垂直相交 6：平行方向相同,但不共线 7：平行方向相反,但不共线</returns>
        public static byte LineLineRelation(Vector3 lineA1, Vector3 lineA2, Vector3 lineB1, Vector3 lineB2,
            ref Vector3 point)
        {
            point = Vector3.zero;
            if (lineA1 == lineA2 || lineB1 == lineB2) return 0;
            Vector3 nA = (lineA2 - lineA1).normalized;
            Vector3 nB = (lineB2 - lineB1).normalized;
            if (nA == nB)
            {
                if (PointInLine(lineA1, lineB1, lineB2) == 5) { return 6; }
                return 1;
            }
            else if (nA == -nB)
            {
                if (PointInLine(lineA1, lineB1, lineB2) == 5) { return 7; }
                return 2;
            }
            else
            {
                Vector3 p1 = lineA1, p2 = lineA2, p3 = lineB1;
                if (p3 == p1 || p3 == p2 || PointInLine(p3, p1, p2) != 5) { p3 = lineB2; }
                Vector3 n1 = Vector3.Cross((p3 - p1), (p3 - p2)).normalized;
                p1 = lineB1;
                p2 = lineB2;
                p3 = lineA1;
                if (p3 == p1 || p3 == p2 || PointInLine(p3, p1, p2) != 5) { p3 = lineA2; }
                Vector3 n2 = Vector3.Cross((p3 - p1), (p3 - p2)).normalized;
                if (n1 == n2 || n1 == -n2)
                {
                    //共面 相交
                    point = PointToLineTarget(lineA1, lineB1, lineB2);
                    if (point != lineA1)
                    {
                        //相交
                        p1 = lineA1;
                        p2 = PointToLineTarget(p1, lineB1, lineB2);
                        if (p2 == p1)
                        {
                            p1 = lineA2;
                            nA = -nA;
                        }
                        float dis = Vector3.Distance(p2, p1);
                        float ang = Vector3.Angle((p2 - p1), nA);
                        if (ang == 180)
                        {
                            //垂直
                            point = p2;
                            return 5;
                        }
                        else if (ang > 90)
                        {
                            nA = -nA;
                            ang = 180 - ang;
                        }
                        float len = dis / UnityEngine.Mathf.Cos(ang * UnityEngine.Mathf.Deg2Rad);
                        point = p1 + nA * len;
                    }
                    return 4;
                }
                else
                {
                    return 3;
                }
            }
        }

        /// <summary>
        /// 判断两条线是否平行
        /// </summary>
        /// <param name="lineA1"></param>
        /// <param name="lineA2"></param>
        /// <param name="lineB1"></param>
        /// <param name="lineB2"></param>
        /// <returns></returns>
        public static bool LineParallel(Vector3 lineA1, Vector3 lineA2, Vector3 lineB1, Vector3 lineB2)
        {
            if (lineA1 == lineA2 || lineB1 == lineB2) return false;
            Vector3 nA = (lineA2 - lineA1).normalized;
            Vector3 nB = (lineB2 - lineB1).normalized;
            if (nA == nB || nA == -nB) return true;
            return false;
        }

        /// <summary>
        /// 判断两条线是不是相交(平行线不相交)
        /// </summary>
        /// <param name="lineA1"></param>
        /// <param name="lineA2"></param>
        /// <param name="lineB1"></param>
        /// <param name="lineB2"></param>
        /// <param name="point">交点的坐标</param>
        /// <param name="vertical">是否相互垂直</param>
        /// <returns></returns>
        public static bool LineIntersect(Vector3 lineA1, Vector3 lineA2, Vector3 lineB1, Vector3 lineB2, ref Vector3 point, ref bool vertical)
        {
            vertical = false;
            if (lineA1 == lineA2 || lineB1 == lineB2) return false;
            Vector3 nA = (lineA2 - lineA1).normalized;
            Vector3 nB = (lineB2 - lineB1).normalized;
            if (nA == nB || nA == -nB) return false;
            Vector3 p1 = lineA1, p2 = lineA2, p3 = lineB1;
            if (p3 == p1 || p3 == p2 || PointInLine(p3, p1, p2) != 5) { p3 = lineB2; }
            Vector3 n1 = Vector3.Cross((p3 - p1), (p3 - p2)).normalized;
            p1 = lineB1;
            p2 = lineB2;
            p3 = lineA1;
            if (p3 == p1 || p3 == p2 || PointInLine(p3, p1, p2) != 5) { p3 = lineA2; }
            Vector3 n2 = Vector3.Cross((p3 - p1), (p3 - p2)).normalized;
            if (n1 == n2 || n1 == -n2)
            {
                //共面 相交
                point = PointToLineTarget(lineA1, lineB1, lineB2);
                if (point != lineA1)
                {
                    //相交
                    p1 = lineA1;
                    p2 = PointToLineTarget(p1, lineB1, lineB2);
                    if (p2 == p1)
                    {
                        p1 = lineA2;
                        nA = -nA;
                    }
                    float dis = Vector3.Distance(p2, p1);
                    float ang = Vector3.Angle((p2 - p1), nA);
                    if (ang == 180)
                    {
                        //垂直
                        point = p2;
                        vertical = true;
                        return true;
                    }
                    else if (ang > 90)
                    {
                        nA = -nA;
                        ang = 180 - ang;
                    }
                    float len = dis / UnityEngine.Mathf.Cos(ang * UnityEngine.Mathf.Deg2Rad);
                    point = p1 + nA * len;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 线段与线段之间的关系
        /// </summary>
        /// <param name="lineA1"></param>
        /// <param name="lineA2"></param>
        /// <param name="lineB1"></param>
        /// <param name="lineB2"></param>
        /// <param name="point"> 相交的交点,不相交的时候会返回Vector3.zero </param>
        /// <param name="relLineA">两线相交且不平行 交点与线A的关系 0:无关 1：在线内 2：与起点重合 3：与终点重合</param>
        /// <param name="relLineB">两线相交且不平行 交点与线A的关系 0:无关 1：在线内 2：与起点重合 3：与终点重合</param>
        /// <returns>0：参数重合错误 2:没关系的两条线 3:方向相同且共线 有交集 4:平行方向相同且共线 无交集 5：平行方向相反且共线 有交集 6：方向相反且共线 无交集 7:相交 8:平行方向相同不共线 9:平行方向相反不共线</returns>
        public static byte LineSegmentRelation(Vector3 lineA1, Vector3 lineA2, Vector3 lineB1, Vector3 lineB2,
            ref Vector3 point, ref byte relLineA, ref byte relLineB)
        {
            byte res = LineLineRelation(lineA1, lineA2, lineB1, lineB2, ref point);
            relLineA = 0; relLineB = 0;
            if (res == 0) return res;
            if (res == 3) return 2;
            if (res == 1)
            {
                //方向相同且重合
                //0：在线内 1：在线内端点外 靠近端点1 2：在线内端点外 靠近端点2 3：在端点1重合 4：在端点2重合 5:不在线上
                res = PointInLine(lineA1, lineB1, lineB2);
                if (res == 0 || res == 3 || res == 4) { return 3; }
                res = PointInLine(lineA2, lineB1, lineB2);
                if (res == 0 || res == 3 || res == 4) { return 3; }
                res = PointInLine(lineB1, lineA1, lineA2);
                if (res == 0 || res == 3 || res == 4) { return 3; }
                res = PointInLine(lineB2, lineA1, lineA2);
                if (res == 0 || res == 3 || res == 4) { return 3; }
                return 4;
            }
            else if (res == 2)
            {
                //方向相反且重合
                res = PointInLine(lineA1, lineB1, lineB2);
                if (res == 0 || res == 3 || res == 4) { return 5; }
                res = PointInLine(lineA2, lineB1, lineB2);
                if (res == 0 || res == 3 || res == 4) { return 5; }
                res = PointInLine(lineB1, lineA1, lineA2);
                if (res == 0 || res == 3 || res == 4) { return 5; }
                res = PointInLine(lineB2, lineA1, lineA2);
                if (res == 0 || res == 3 || res == 4) { return 5; }
                return 6;
            }//4:相交 5：平行方向相同,但不重合 6：平行方向相反,但不重合
            else if (res == 4)
            {
                //相交
                //判断交点与线的关系
                res = PointInLine(point, lineA1, lineA2);
                if (res == 0)
                {
                    relLineA = 1;
                }
                else if (res == 3)
                {
                    relLineA = 2;
                }
                else if (res == 4)
                {
                    relLineA = 3;
                }
                res = PointInLine(point, lineB1, lineB2);
                if (res == 0)
                {
                    relLineB = 1;
                }
                else if (res == 3)
                {
                    relLineB = 2;
                }
                else if (res == 4)
                {
                    relLineB = 3;
                }
                return 7;
            }
            else if (res == 5)
            {
                //平行方向相同,但不重合
                return 8;
            }
            else
            {
                //6：平行方向相反,但不重合
                return 9;
            }
        }

        #endregion

        #region//面 面

        /// <summary>
        /// 面与面之间的关系
        /// 如果相交 返回相交的直线point1,point2
        /// </summary>
        /// <param name="panelA1"></param>
        /// <param name="panelA2"></param>
        /// <param name="panelA3"></param>
        /// <param name="panelB1"></param>
        /// <param name="panelB2"></param>
        /// <param name="panelB3"></param>
        /// <param name="point1">相交的直线上的一个点</param>
        /// <param name="point2">相交的直线上的另一个点</param>
        /// <param name="angle">夹角</param>
        /// <returns>0:参数重合错误 1:平行A面B面同方向 重合 2:平行A面B面反方向 重合 3:平行A面B面同方向 A在B的正面 4:平行A面B面同方向 A在B的反面 5:平行A面B面反方向 A在B的正面 6:平行A面B面反方向 A在B的反面  7: 垂直 8:相交 </returns>
        public static byte PanelPanelRelation(Vector3 panelA1, Vector3 panelA2, Vector3 panelA3,
            Vector3 panelB1, Vector3 panelB2, Vector3 panelB3,
            ref Vector3 point1, ref Vector3 point2, ref float angle)
        {
            if (panelA1 == panelA2 || panelA1 == panelA3 || panelA2 == panelA3
                || panelB1 == panelB2 || panelB1 == panelB3 || panelB2 == panelB3) return 0;
            Vector3 nA = Vector3.Cross((panelA2 - panelA1), (panelA3 - panelA1)).normalized;
            Vector3 nB = Vector3.Cross((panelB2 - panelB1), (panelB3 - panelB1)).normalized;
            if (nA == nB)
            {
                angle = 0;
                byte res = PointInPanel(panelA1, panelB1, panelB2, panelB3);
                if (res == 0) { return 1; }
                else if (res == 1) { return 3; }
                else { return 4; }
            }
            else if (nA == -nB)
            {
                angle = 0;
                byte res = PointInPanel(panelA1, panelB1, panelB2, panelB3);
                if (res == 0) { return 2; }
                else if (res == 1) { return 5; }
                else { return 6; }
            }
            else
            {
                float dot = Vector3.Dot(nA, nB);
                if (UnityEngine.Mathf.Abs(dot) <= 0.00001f)
                {
                    angle = 90;
                    //求交线
                    point1 = PointToPanelTarget(panelA1, panelB1, panelB2, panelB3);
                    point2 = PointToPanelTarget(panelA2, panelB1, panelB2, panelB3);
                    if (point1 == point2)
                    {
                        point2 = PointToPanelTarget(panelA3, panelB1, panelB2, panelB3);
                    }
                    return 7;
                }
                else
                {
                    angle = 180 - Vector3.Angle(nA, nB);
                    //求交线
                    point1 = PointToPanelTarget(panelA1, panelB1, panelB2, panelB3);
                    if (point1 != panelA1)
                    {
                        point1 = GetPoint(Vector3.Cross(nA, nB), panelA1, point1, angle);
                    }
                    point2 = PointToPanelTarget(panelA2, panelB1, panelB2, panelB3);
                    if (point2 != panelA2)
                    {
                        point2 = GetPoint(Vector3.Cross(nA, nB), panelA2, point2, angle);
                    }
                    if (point2 == point1)
                    {
                        point2 = PointToPanelTarget(panelA3, panelB1, panelB2, panelB3);
                        if (point2 != panelA3)
                        {
                            point2 = GetPoint(Vector3.Cross(nA, nB), panelA3, point2, angle);
                        }
                    }
                    return 8;
                }
            }
        }

        /// <summary>
        /// 求2面的一个交点
        /// </summary>
        /// <param name="n">两面的交线向量 （两面法线叉积）</param>
        /// <param name="p">A面上的一个点</param>
        /// <param name="pPanel">A面上的一个点在B面上的投影</param>
        /// <param name="angle">两面的夹角</param>
        /// <returns></returns>
        static Vector3 GetPoint(Vector3 n, Vector3 p, Vector3 pPanel, float angle)
        {
            n = Vector3.Cross(n, (pPanel - p)).normalized;
            return pPanel - n * (Vector3.Distance(p, pPanel) / UnityEngine.Mathf.Tan(angle * UnityEngine.Mathf.Deg2Rad));
        }

        #endregion

        #region// 点 多边形

        /// <summary>
        /// 点到三角形的投影是否在三角形内部
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangleP1"></param>
        /// <param name="triangleP2"></param>
        /// <param name="triangleP3"></param>
        /// <returns></returns>
        public static bool PointProInTriangle(Vector3 point, Vector3 triangleP1, Vector3 triangleP2, Vector3 triangleP3)
        {
            Vector3 p = PointToPanelTarget(point, triangleP1, triangleP2, triangleP3);
            return PointInTriangle(p, triangleP1, triangleP2, triangleP3) != 0;
        }

        /// <summary>
        /// 点到三角形的投影是否在三角形内部 效率高一些
        /// 当点在三角形边界时将会返回false
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangleP1"></param>
        /// <param name="triangleP2"></param>
        /// <param name="triangleP3"></param>
        /// <returns></returns>
        public static bool PointProInTriangleSim(Vector3 point, Vector3 triangleP1, Vector3 triangleP2, Vector3 triangleP3)
        {
            Vector3 p = PointToPanelTarget(point, triangleP1, triangleP2, triangleP3);
            return PointInTriangleSim(p, triangleP1, triangleP2, triangleP3);
        }

        /// <summary>
        /// 点是否在三角形内部 要求点需要与三角形共面
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangleP1"></param>
        /// <param name="triangleP2"></param>
        /// <param name="triangleP3"></param>
        /// <returns>0:在三角形外 1：与端点1重合 2：与端点2重合 3：与端点3重合 4：共线1,2 5：共线2,3 6:共线1,3 7:在三角形内</returns>
        public static byte PointInTriangle(Vector3 point, Vector3 triangleP1, Vector3 triangleP2, Vector3 triangleP3)
        {
            if (point == triangleP1) { return 1; }
            if (point == triangleP2) { return 2; }
            if (point == triangleP3) { return 3; }
            Vector3 v1 = (triangleP2 - triangleP1).normalized;
            Vector3 v2 = (point - triangleP1).normalized;
            if (v1 == v2)
            {
                if (Vector3.Distance(point, triangleP1) < Vector3.Distance(triangleP2, triangleP1)) { return 4; }
                else { return 0; }
            }
            if (v1 == -v2) { return 0; }
            Vector3 n1 = Vector3.Cross(v1, v2).normalized;
            v1 = (triangleP3 - triangleP2).normalized;
            v2 = (point - triangleP2).normalized;
            if (v1 == v2)
            {
                if (Vector3.Distance(point, triangleP2) < Vector3.Distance(triangleP2, triangleP3)) { return 5; }
                else { return 0; }
            }
            if (v1 == -v2) { return 0; }
            Vector3 n2 = Vector3.Cross(v1, v2).normalized;
            if (n1 != n2) { return 0; }
            v1 = (triangleP1 - triangleP3).normalized;
            v2 = (point - triangleP3).normalized;
            if (v1 == v2)
            {
                if (Vector3.Distance(point, triangleP3) < Vector3.Distance(triangleP1, triangleP3)) { return 6; }
                else { return 0; }
            }
            if (v1 == -v2) { return 0; }
            n2 = Vector3.Cross(v1, v2).normalized;
            if (n1 != n2) { return 0; }
            return 7;
        }

        /// <summary>
        /// 点是否在三角形内部 要求点需要与三角形共面  效率高一些
        /// 当点在三角形边界时将会返回false
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangleP1"></param>
        /// <param name="triangleP2"></param>
        /// <param name="triangleP3"></param>
        /// <returns></returns>
        public static bool PointInTriangleSim(Vector3 point, Vector3 triangleP1, Vector3 triangleP2, Vector3 triangleP3)
        {
            Vector3 v1 = triangleP2 - triangleP1;
            Vector3 v2 = point - triangleP1;
            Vector3 n1 = Vector3.Cross(v1, v2).normalized;
            v1 = triangleP3 - triangleP2;
            v2 = point - triangleP2;
            Vector3 n2 = Vector3.Cross(v1, v2).normalized;
            if (n1 != n2) { return false; }
            v1 = triangleP1 - triangleP3;
            v2 = point - triangleP3;
            n2 = Vector3.Cross(v1, v2).normalized;
            if (n1 != n2) { return false; }
            return true;
        }

        /// <summary>
        /// 凸多边形 判断点是否在多边形内 要求点和多边形在同一个平面 多边形点列表需要按照同一顺序传入
        /// 0:不在多边形内 1：与端点重合 2：与边线重合 3：在多边形内
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygons"></param>
        /// <returns></returns>
        public static byte PointInConvexPolygon(Vector3 point, List<Vector3> polygons)
        {
            if (polygons == null || polygons.Count <= 2) return 0;
            Vector3 p1, p2, v1, v2, n1 = Vector3.zero, n2;
            if (point == polygons[0]) return 1;
            for (int i = 1, listCount = polygons.Count; i <= listCount; ++i)
            {
                if (i == listCount)
                {
                    p1 = polygons[listCount - 1];
                    p2 = polygons[0];
                }
                else
                {
                    p1 = polygons[i - 1];
                    p2 = polygons[i];
                }
                if (point == p2) return 1;
                v1 = (p2 - p1).normalized;
                v2 = (point - p1).normalized;
                if (v1 == v2)
                {
                    if (Vector3.Distance(point, p1) < Vector3.Distance(p2, p1)) { return 2; }
                    else { return 0; }
                }
                if (v1 == -v2) { return 0; }
                if (i == 1)
                {
                    n1 = Vector3.Cross(v1, v2).normalized;
                }
                else
                {
                    n2 = Vector3.Cross(v1, v2).normalized;
                    if (n1 != n2) { return 0; }
                }
            }
            return 3;
        }

        /// <summary>
        /// 凸多边形 判断点是否在多边形内 要求点和多边形在同一个平面 多边形点列表需要按照同一顺序传入  效率高一些
        /// 如果点与端点重合或这与线重合 会返回false
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygons"></param>
        /// <returns></returns>
        public static bool PointInConvexPolygonSim(Vector3 point, List<Vector3> polygons)
        {
            if (polygons == null || polygons.Count <= 2) return false;
            Vector3 p1, p2, v1, v2, n1 = Vector3.zero, n2;
            for (int i = 1, listCount = polygons.Count; i <= listCount; ++i)
            {
                if (i == listCount)
                {
                    p1 = polygons[listCount - 1];
                    p2 = polygons[0];
                }
                else
                {
                    p1 = polygons[i - 1];
                    p2 = polygons[i];
                }
                v1 = p2 - p1;
                v2 = point - p1;
                if (i == 1)
                {
                    n1 = Vector3.Cross(v1, v2).normalized;
                }
                else
                {
                    n2 = Vector3.Cross(v1, v2).normalized;
                    if (n1 != n2) { return false; }
                }
            }
            return true;
        }

        /// <summary>
        /// 凹凸多边形都可 判断点是否在多边形内 要求点和多边形在同一个平面 多边形点列表需要按照同一顺序传入
        /// 0:不在多边形内 1：与端点重合 2：与边线重合 3：在多边形内
        /// </summary>
        /// <param name="point"></param>
        /// <param name="polygons"></param>
        /// <returns></returns>
        public static byte PointInPolygon(Vector3 point, List<Vector3> polygons)
        {
            if (polygons.Count <= 3)
            {
                return PointInConvexPolygon(point, polygons);
            }
            List<List<Vector3>> resoult = null;
            ToConvexPolygons(polygons, ref resoult);
            byte res;
            for (int i = 0, listCount = resoult.Count; i < listCount; ++i)
            {
                res = PointInConvexPolygon(point, resoult[i]);
                if (res != 0)
                {
                    return res;
                }
            }
            return 0;
        }

        #endregion

        #region//多边形

        /// <summary>
        /// 是否为凸多边形 要求多边形在同一个平面
        /// </summary>
        /// <param name="polygons"></param>
        /// <returns></returns>
        public static bool IsConvexPolygon(List<Vector3> polygons)
        {
            if (polygons == null || polygons.Count <= 2) return false;
            Vector3 p1, p2, p3;
            Vector3 v1, v2;
            Vector3 n1 = Vector3.zero;
            Vector3 n2;
            for (int i = 0, listCount = polygons.Count; i < listCount; ++i)
            {
                if (i == 0)
                {
                    p1 = polygons[listCount - 1];
                    p2 = polygons[i];
                    p3 = polygons[1];
                }
                else if (i == listCount - 1)
                {
                    p1 = polygons[i - 1];
                    p2 = polygons[i];
                    p3 = polygons[0];
                }
                else
                {
                    p1 = polygons[i - 1];
                    p2 = polygons[i];
                    p3 = polygons[i + 1];
                }
                v1 = p2 - p1;
                v2 = p3 - p2;
                if (i == 0) { n1 = Vector3.Cross(v1, v2).normalized; }
                else
                {
                    n2 = Vector3.Cross(v1, v2).normalized;
                    if (n1 != n2) { return false; }
                }
            }
            return true;
        }

        /// <summary>
        /// 将凹多边形拆分为多个三角形+一个凸多边形或三角形 三角剖分 结果中列表数量>3的是凸多边形
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="resoult"></param>
        public static void ToConvexPolygons(List<Vector3> polygons, ref List<List<Vector3>> resoult)
        {
            if (polygons == null || polygons.Count <= 2) return;
            if (polygons.Count == 3)
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(polygons);
                return;
            }
            int findIndex = IndexOfConvexPolygon(polygons);
            if (findIndex != -1)
            {
                int index0 = findIndex - 2;
                if (index0 < 0) { index0 = polygons.Count + index0; }
                int index1 = findIndex - 1;
                if (index1 < 0) { index1 = polygons.Count + index1; }
                int index2 = findIndex;
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(new List<Vector3>() { polygons[index0], polygons[index1], polygons[index2] });
                polygons.RemoveAt(index1);
                ToConvexPolygons(polygons, ref resoult);
            }
            else
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(polygons);
                return;
            }
        }

        /// <summary>
        /// 将凹多边形拆分为多个三角形
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="resoult"></param>
        /// <param name="convertType">1:嵌套式分割 2：发散式分割</param>
        public static void TrianglePolygon(List<Vector3> polygons, ref List<List<Vector3>> resoult, int convertType = 1)
        {
            if (polygons == null || polygons.Count <= 2) return;
            if (polygons.Count == 3)
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(polygons);
                return;
            }
            int findIndex = IndexOfConvexPolygon(polygons);
            if (findIndex != -1)
            {
                int index0 = findIndex - 2;
                if (index0 < 0) { index0 = polygons.Count + index0; }
                int index1 = findIndex - 1;
                if (index1 < 0) { index1 = polygons.Count + index1; }
                int index2 = findIndex;
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(new List<Vector3>() { polygons[index0], polygons[index1], polygons[index2] });
                polygons.RemoveAt(index1);
                ToConvexPolygons(polygons, ref resoult);
            }
            else
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                if (polygons.Count > 3)
                {
                    if (convertType == 1)
                    {
                        TriangleConvexPolygon(polygons, ref resoult);
                    }
                    else
                    {
                        List<Vector3> tempList = null;
                        TriangleConvexPolygon(polygons, ref tempList, ref resoult);
                    }
                }
                else
                {
                    resoult.Add(polygons);
                }
                return;
            }
        }

        /// <summary>
        /// 凸多边形拆分三角形 嵌套式分割
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="resoult"></param>
        public static void TriangleConvexPolygon(List<Vector3> polygons, ref List<Vector3> tempList, ref List<List<Vector3>> resoult)
        {
            if (polygons.Count < 3) return;
            if (polygons.Count == 3)
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(polygons);
                return;
            }
            if (resoult == null) { resoult = new List<List<Vector3>>(); }
            if (tempList == null) { tempList = new List<Vector3>(); }
            tempList.Clear();
            tempList.Add(polygons[0]);
            int index1, index2;
            for (int i = 0, listCount = polygons.Count; i < listCount;)
            {
                index1 = i + 1;
                index2 = i + 2;
                if (index2 == listCount)
                {
                    index2 = 0;
                    resoult.Add(new List<Vector3>() { polygons[i], polygons[index1], polygons[index2] });
                    break;
                }
                else if (index2 == listCount - 1)
                {
                    resoult.Add(new List<Vector3>() { polygons[i], polygons[index1], polygons[index2] });
                    tempList.Add(polygons[index2]);
                    break;
                }
                else
                {
                    resoult.Add(new List<Vector3>() { polygons[i], polygons[index1], polygons[index2] });
                    tempList.Add(polygons[index2]);
                }
                i = index2;
            }
            if (tempList.Count >= 3)
            {
                TriangleConvexPolygon(tempList, ref polygons, ref resoult);
            }
        }

        /// <summary>
        /// 凸多边形拆分三角形 发散式分割
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="resoult"></param>
        public static void TriangleConvexPolygon(List<Vector3> polygons, ref List<List<Vector3>> resoult)
        {
            if (polygons.Count < 3) return;
            if (polygons.Count == 3)
            {
                if (resoult == null) { resoult = new List<List<Vector3>>(); }
                resoult.Add(polygons);
                return;
            }
            if (resoult == null) { resoult = new List<List<Vector3>>(); }
            resoult.Add(new List<Vector3>() { polygons[0], polygons[1], polygons[2] });
            for (int i = 2, listCount = polygons.Count - 2; i <= listCount; ++i)
            {
                resoult.Add(new List<Vector3>() { polygons[0], polygons[i], polygons[i + 1] });
            }
        }

        /// <summary>
        /// 返回凹多边形第一个序号 返回的不一定是第一个凹点 返回的是第一个与之前方向不一致的点序号
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        static int IndexOfConvexPolygon(List<Vector3> points)
        {
            Vector3 p1, p2, p3, v1, v2, n1 = Vector3.zero, n2;
            for (int i = 0, listCount = points.Count; i < listCount; ++i)
            {
                if (i == 0)
                {
                    p1 = points[listCount - 1];
                    p2 = points[i];
                    p3 = points[1];
                }
                else if (i == listCount - 1)
                {
                    p1 = points[i - 1];
                    p2 = points[i];
                    p3 = points[0];
                }
                else
                {
                    p1 = points[i - 1];
                    p2 = points[i];
                    p3 = points[i + 1];
                }
                v1 = p2 - p1;
                v2 = p3 - p2;
                if (i == 0) { n1 = Vector3.Cross(v1, v2).normalized; }
                else
                {
                    n2 = Vector3.Cross(v1, v2).normalized;
                    if (n1 != n2) { return i; }
                }
            }
            return -1;
        }

        #endregion
    }
}


