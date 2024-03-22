using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// d3d规则矩阵
    /// </summary>
    public struct D3DMatrix
    {
        /// <summary>
        /// 0矩阵 4x4
        /// </summary>
        public static D3DMatrix4x4 zero4x4
        {
            get
            {
                return new D3DMatrix4x4();
            }
        }

        /// <summary>
        /// 0矩阵 3x3
        /// </summary>
        public static D3DMatrix3x3 zero3x3
        {
            get
            {
                return new D3DMatrix3x3();
            }
        }

        /// <summary>
        /// 单位矩阵 4x4
        /// </summary>
        public static D3DMatrix4x4 identity4x4
        {
            get
            {
                D3DMatrix4x4 newMatrix = new D3DMatrix4x4();
                D3DMatrix4x4.Identity(ref newMatrix);
                return newMatrix;
            }
        }

        /// <summary>
        /// 单位矩阵 3x3
        /// </summary>
        public static D3DMatrix3x3 identity3x3
        {
            get
            {
                D3DMatrix3x3 newMatrix = new D3DMatrix3x3();
                D3DMatrix3x3.Identity(ref newMatrix);
                return newMatrix;
            }
        }

    }

    /// <summary>
    /// d3d规则矩阵 3x3
    /// </summary>
    public struct D3DMatrix3x3
    {
        //
        public float m00;

        public float m01;

        public float m02;
        //
        public float m10;

        public float m11;

        public float m12;
        //
        public float m20;

        public float m21;

        public float m22;
        //

        /// <summary>
        /// 所引器
        /// </summary>
        /// <param name="r">0-2</param>
        /// <param name="c">0-2</param>
        /// <returns></returns>
        public float this[int r, int c]
        {
            get
            {
                if (r < 0 || r > 2 || c < 0 || c > 2)
                {
                    throw new System.IndexOutOfRangeException();
                }
                if (r == 0)
                {
                    if (c == 0)
                    {
                        return m00;
                    }
                    if (c == 1)
                    {
                        return m01;
                    }
                    if (c == 2)
                    {
                        return m02;
                    }
                }
                if (r == 1)
                {
                    if (c == 0)
                    {
                        return m10;
                    }
                    if (c == 1)
                    {
                        return m11;
                    }
                    if (c == 2)
                    {
                        return m12;
                    }
                }
                if (r == 2)
                {
                    if (c == 0)
                    {
                        return m20;
                    }
                    if (c == 1)
                    {
                        return m21;
                    }
                    if (c == 2)
                    {
                        return m22;
                    }
                }
                return 0;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
#if UNITY_5
            sb.AppendLine(string.Format("{0},{1},{2}", this.m00, this.m01, this.m02));
            sb.AppendLine(string.Format("{0},{1},{2}", this.m10, this.m11, this.m12));
            sb.AppendLine(string.Format("{0},{1},{2}", this.m20, this.m21, this.m22));
#else
            sb.AppendLine($"{this.m00,5:F3},{this.m01,5:F3},{this.m02,5:F3}");
            sb.AppendLine($"{this.m10,5:F3},{this.m11,5:F3},{this.m12,5:F3}");
            sb.AppendLine($"{this.m20,5:F3},{this.m21,5:F3},{this.m22,5:F3}");
#endif
            return sb.ToString();
        }

        /// <summary>
        /// 转置矩阵
        /// </summary>
        public D3DMatrix3x3 transpose
        {
            get
            {
                D3DMatrix3x3 res = this;
                Transpose(ref res);
                return res;
            }
        }

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static D3DMatrix3x3 identity
        {
            get
            {
                D3DMatrix3x3 newMatrix = new D3DMatrix3x3();
                D3DMatrix3x3.Identity(ref newMatrix);
                return newMatrix;
            }
        }

        /// <summary>
        /// 零值化
        /// </summary>
        public static void Zero(ref D3DMatrix3x3 target)
        {
            target.m00 = 0;
            target.m01 = 0;
            target.m02 = 0;
            //
            target.m10 = 0;
            target.m11 = 0;
            target.m12 = 0;
            //
            target.m20 = 0;
            target.m21 = 0;
            target.m22 = 0;
        }

        /// <summary>
        /// 单位化
        /// </summary>
        public static void Identity(ref D3DMatrix3x3 target)
        {
            target.m00 = 1;
            target.m01 = 0;
            target.m02 = 0;
            //
            target.m10 = 0;
            target.m11 = 1;
            target.m12 = 0;
            //
            target.m20 = 0;
            target.m21 = 0;
            target.m22 = 1;
        }

        /// <summary>
        /// 转置矩阵
        /// </summary>
        /// <param name="target"></param>
        public static void Transpose(ref D3DMatrix3x3 target)
        {
            D3DMatrix3x3 old = target;
            target.m00 = old.m00;
            target.m01 = old.m10;
            target.m02 = old.m20;
            //
            target.m10 = old.m01;
            target.m11 = old.m11;
            target.m12 = old.m21;
            //
            target.m20 = old.m02;
            target.m21 = old.m12;
            target.m22 = old.m22;
        }

        #region//旋转

        /// <summary>
        /// 获得旋转矩阵 X轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix3x3 GetRotateX(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix3x3 res = new D3DMatrix3x3();
            res.m00 = 1;
            res.m11 = cos;
            res.m12 = sin;
            res.m21 = -sin;
            res.m22 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  X轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateX(Vector3 pos, float angle)
        {
            D3DMatrix3x3 matr = GetRotateX(angle);
            return pos * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 Y轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix3x3 GetRotateY(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix3x3 res = new D3DMatrix3x3();
            res.m11 = 1;
            res.m00 = cos;
            res.m02 = -sin;
            res.m20 = sin;
            res.m22 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  Y轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateY(Vector3 pos, float angle)
        {
            D3DMatrix3x3 matr = GetRotateY(angle);
            return pos * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 Z轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix3x3 GetRotateZ(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix3x3 res = new D3DMatrix3x3();
            res.m22 = 1;
            res.m00 = cos;
            res.m01 = sin;
            res.m10 = -sin;
            res.m11 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  Z轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateZ(Vector3 pos, float angle)
        {
            D3DMatrix3x3 matr = GetRotateZ(angle);
            return pos * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 XYZ轴
        /// cosY*cosZ+sinX*sinY*sinZ; sinX*cosZ; -sinY*cosZ+sinX*cosY*sinZ
        /// -cosY*sinZ+sinX*sinY*cosZ; cosX*cosZ; sinY*sinZ+sinX*cosY*cosZ
        /// cosX*sinY; -sinX; cosX*cosY
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix3x3 GetRotateXYZ(Vector3 angle)
        {
            //
            float x = angle.x * UnityEngine.Mathf.Deg2Rad;
            float cosX = UnityEngine.Mathf.Cos(x);
            float sinX = UnityEngine.Mathf.Sin(x);
            //
            float y = angle.y * UnityEngine.Mathf.Deg2Rad;
            float cosY = UnityEngine.Mathf.Cos(y);
            float sinY = UnityEngine.Mathf.Sin(y);
            //
            float z = angle.z * UnityEngine.Mathf.Deg2Rad;
            float cosZ = UnityEngine.Mathf.Cos(z);
            float sinZ = UnityEngine.Mathf.Sin(z);
            //
            float cosY_cosZ = cosY * cosZ;
            float cosY_sinZ = cosY * sinZ;
            float sinX_sinY = sinX * sinY;

            D3DMatrix3x3 resX = new D3DMatrix3x3();
            resX.m00 = cosY_cosZ + sinX_sinY * sinZ;
            resX.m01 = sinX * cosZ;
            resX.m02 = -sinY * cosZ + sinX * cosY_sinZ;
            resX.m10 = -cosY_sinZ + sinX_sinY * cosZ;
            resX.m11 = cosX * cosZ;
            resX.m12 = sinY * sinZ + sinX * cosY_cosZ;
            resX.m20 = cosX * sinY;
            resX.m21 = -sinX;
            resX.m22 = cosX * cosY;

            return resX;
        }

        /// <summary>
        /// 获得旋转矩阵 XYZ轴
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector3 GetRotateXYZ(Vector3 pos, Vector3 angle)
        {
            D3DMatrix3x3 matr = GetRotateXYZ(angle);
            return pos * matr;
        }

        #endregion

        #region//运算符

        public static D3DMatrix3x3 operator *(D3DMatrix3x3 a, D3DMatrix3x3 b)
        {
            D3DMatrix3x3 resoult = new D3DMatrix3x3();
            //
            resoult.m00 = a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20;
            resoult.m01 = a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21;
            resoult.m02 = a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22;
            //
            resoult.m10 = a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20;
            resoult.m11 = a.m10 * b.m01 + a.m11 * b.m11 + a.m12 * b.m21;
            resoult.m12 = a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22;
            //
            resoult.m20 = a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20;
            resoult.m21 = a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21;
            resoult.m22 = a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22;
            //
            return resoult;
        }

        public static Vector3 operator *(Vector3 pos, D3DMatrix3x3 b)
        {
            float x = pos.x * b.m00 + pos.y * b.m10 + pos.z * b.m20;
            float y = pos.x * b.m01 + pos.y * b.m11 + pos.z * b.m21;
            float z = pos.x * b.m02 + pos.y * b.m12 + pos.z * b.m22;
            return new Vector3(x, y, z);
        }

        public static D3DMatrix3x3 operator *(float f, D3DMatrix3x3 b)
        {
            D3DMatrix3x3 res = b;
            res.m00 *= f;
            res.m01 *= f;
            res.m02 *= f;
            //
            res.m10 *= f;
            res.m11 *= f;
            res.m12 *= f;
            //
            res.m20 *= f;
            res.m21 *= f;
            res.m22 *= f;
            return res;
        }

        public static D3DMatrix3x3 operator *(D3DMatrix3x3 a, float f)
        {
            return f * a;
        }

        public static bool operator ==(D3DMatrix3x3 a, D3DMatrix3x3 b)
        {
            int count = 3;
            for (int i = 0; i < count; ++i)
            {
                for (int j = 0; j < count; ++j)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator !=(D3DMatrix3x3 a, D3DMatrix3x3 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (D3DMatrix3x3)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// d3d规则矩阵 4x4
    /// </summary>
    public struct D3DMatrix4x4
    {
        //
        public float m00;

        public float m01;

        public float m02;

        public float m03;
        //
        public float m10;

        public float m11;

        public float m12;

        public float m13;
        //
        public float m20;

        public float m21;

        public float m22;

        public float m23;
        //
        public float m30;

        public float m31;

        public float m32;

        public float m33;

        /// <summary>
        /// 所引器
        /// </summary>
        /// <param name="r">0-3</param>
        /// <param name="c">0-3</param>
        /// <returns></returns>
        public float this[int r, int c]
        {
            get
            {
                if (r < 0 || r > 3 || c < 0 || c > 3)
                {
                    throw new System.IndexOutOfRangeException();
                }
                if (r == 0)
                {
                    if (c == 0)
                    {
                        return m00;
                    }
                    if (c == 1)
                    {
                        return m01;
                    }
                    if (c == 2)
                    {
                        return m02;
                    }
                    if (c == 3)
                    {
                        return m03;
                    }
                }
                if (r == 1)
                {
                    if (c == 0)
                    {
                        return m10;
                    }
                    if (c == 1)
                    {
                        return m11;
                    }
                    if (c == 2)
                    {
                        return m12;
                    }
                    if (c == 3)
                    {
                        return m13;
                    }
                }
                if (r == 2)
                {
                    if (c == 0)
                    {
                        return m20;
                    }
                    if (c == 1)
                    {
                        return m21;
                    }
                    if (c == 2)
                    {
                        return m22;
                    }
                    if (c == 3)
                    {
                        return m23;
                    }
                }
                if (r == 3)
                {
                    if (c == 0)
                    {
                        return m30;
                    }
                    if (c == 1)
                    {
                        return m31;
                    }
                    if (c == 2)
                    {
                        return m32;
                    }
                    if (c == 3)
                    {
                        return m33;
                    }
                }
                return 0;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

#if UNITY_5
            sb.AppendLine(string.Format("{0},{1},{2},{3}", this.m00, this.m01, this.m02, this.m03));
            sb.AppendLine(string.Format("{0},{1},{2},{3}", this.m10, this.m11, this.m12, this.m13));
            sb.AppendLine(string.Format("{0},{1},{2},{3}", this.m20, this.m21, this.m22, this.m23));
            sb.AppendLine(string.Format("{0},{1},{2},{3}", this.m30, this.m31, this.m32, this.m33));
#else
            sb.AppendLine($"{this.m00,5:F3},{this.m01,5:F3},{this.m02,5:F3},{this.m03,5:F3}");
            sb.AppendLine($"{this.m10,5:F3},{this.m11,5:F3},{this.m12,5:F3},{this.m13,5:F3}");
            sb.AppendLine($"{this.m20,5:F3},{this.m21,5:F3},{this.m22,5:F3},{this.m23,5:F3}");
            sb.AppendLine($"{this.m30,5:F3},{this.m31,5:F3},{this.m32,5:F3},{this.m33,5:F3}");
#endif

            return sb.ToString();
        }

        /// <summary>
        /// 转置矩阵
        /// </summary>
        public D3DMatrix4x4 transpose
        {
            get
            {
                D3DMatrix4x4 res = this;
                Transpose(ref res);
                return res;
            }
        }

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static D3DMatrix4x4 identity
        {
            get
            {
                D3DMatrix4x4 newMatrix = new D3DMatrix4x4();
                D3DMatrix4x4.Identity(ref newMatrix);
                return newMatrix;
            }
        }

        /// <summary>
        /// 零值化
        /// </summary>
        public static void Zero(ref D3DMatrix4x4 target)
        {
            target.m00 = 0;
            target.m01 = 0;
            target.m02 = 0;
            target.m03 = 0;
            //
            target.m10 = 0;
            target.m11 = 0;
            target.m12 = 0;
            target.m13 = 0;
            //
            target.m20 = 0;
            target.m21 = 0;
            target.m22 = 0;
            target.m23 = 0;
            //
            target.m30 = 0;
            target.m31 = 0;
            target.m32 = 0;
            target.m33 = 0;
        }

        /// <summary>
        /// 单位化
        /// </summary>
        public static void Identity(ref D3DMatrix4x4 target)
        {
            target.m00 = 1;
            target.m01 = 0;
            target.m02 = 0;
            target.m03 = 0;
            //
            target.m10 = 0;
            target.m11 = 1;
            target.m12 = 0;
            target.m13 = 0;
            //
            target.m20 = 0;
            target.m21 = 0;
            target.m22 = 1;
            target.m23 = 0;
            //
            target.m30 = 0;
            target.m31 = 0;
            target.m32 = 0;
            target.m33 = 1;
        }

        /// <summary>
        /// 转置矩阵
        /// </summary>
        /// <param name="target"></param>
        public static void Transpose(ref D3DMatrix4x4 target)
        {
            D3DMatrix4x4 old = target;
            target.m00 = old.m00;
            target.m01 = old.m10;
            target.m02 = old.m20;
            target.m03 = old.m30;
            //
            target.m10 = old.m01;
            target.m11 = old.m11;
            target.m12 = old.m21;
            target.m13 = old.m31;
            //
            target.m20 = old.m02;
            target.m21 = old.m12;
            target.m22 = old.m22;
            target.m23 = old.m32;
            //
            target.m30 = old.m03;
            target.m31 = old.m13;
            target.m32 = old.m23;
            target.m33 = old.m33;
        }

        #region//旋转

        /// <summary>
        /// 获得旋转矩阵 X轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetRotateX(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m00 = 1;
            res.m11 = cos;
            res.m12 = sin;
            res.m21 = -sin;
            res.m22 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  X轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateX(Vector3 pos, float angle)
        {
            D3DMatrix4x4 matr = GetRotateX(angle);
            return new Vector4(pos.x, pos.y, pos.z, 0) * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 Y轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetRotateY(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m11 = 1;
            res.m00 = cos;
            res.m02 = -sin;
            res.m20 = sin;
            res.m22 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  Y轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateY(Vector3 pos, float angle)
        {
            D3DMatrix4x4 matr = GetRotateY(angle);
            return pos * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 Z轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetRotateZ(float angle)
        {
            float f = angle * UnityEngine.Mathf.Deg2Rad;
            float cos = UnityEngine.Mathf.Cos(f);
            float sin = UnityEngine.Mathf.Sin(f);
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m22 = 1;
            res.m00 = cos;
            res.m01 = sin;
            res.m10 = -sin;
            res.m11 = cos;
            return res;
        }

        /// <summary>
        /// 向量旋转  Z轴
        /// </summary>
        /// <param name="pos">向量</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Vector3 GetRotateZ(Vector3 pos, float angle)
        {
            D3DMatrix4x4 matr = GetRotateZ(angle);
            return pos * matr;
        }

        /// <summary>
        /// 获得旋转矩阵 XYZ轴
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetRotateXYZ(Vector3 angle)
        {
            //
            float x = angle.x * UnityEngine.Mathf.Deg2Rad;
            float cosX = UnityEngine.Mathf.Cos(x);
            float sinX = UnityEngine.Mathf.Sin(x);
            //
            float y = angle.y * UnityEngine.Mathf.Deg2Rad;
            float cosY = UnityEngine.Mathf.Cos(y);
            float sinY = UnityEngine.Mathf.Sin(y);
            //
            float z = angle.z * UnityEngine.Mathf.Deg2Rad;
            float cosZ = UnityEngine.Mathf.Cos(z);
            float sinZ = UnityEngine.Mathf.Sin(z);
            //
            float cosY_cosZ = cosY * cosZ;
            float cosY_sinZ = cosY * sinZ;
            float sinX_sinY = sinX * sinY;

            D3DMatrix4x4 resX = new D3DMatrix4x4();
            resX.m00 = cosY_cosZ + sinX_sinY * sinZ;
            resX.m01 = sinX * cosZ;
            resX.m02 = -sinY * cosZ + sinX * cosY_sinZ;
            resX.m10 = -cosY_sinZ + sinX_sinY * cosZ;
            resX.m11 = cosX * cosZ;
            resX.m12 = sinY * sinZ + sinX * cosY_cosZ;
            resX.m20 = cosX * sinY;
            resX.m21 = -sinX;
            resX.m22 = cosX * cosY;

            return resX;
        }

        /// <summary>
        /// 获得旋转矩阵 XYZ轴
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="angle"></param> 
        /// <returns></returns>
        public static Vector3 GetRotateXYZ(Vector3 pos, Vector3 angle)
        {
            D3DMatrix4x4 matr = GetRotateXYZ(angle);
            return pos * matr;
        }

        #endregion

        #region// 世界 -》 屏幕

        /// <summary>
        /// 获得一个指定位置指定方向的观察视图矩阵
        /// </summary>
        /// <param name="eye">观察位置</param>
        /// <param name="lookAt">观察前方向</param>
        /// <param name="up">观察上方向</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetViewMatrix(Vector3 eye, Vector3 lookAt, Vector3 up)
        {
            Vector3 zaxis = (lookAt - eye).normalized;
            Vector3 xaxis = Vector3.Cross(up, zaxis).normalized;
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);
            float dotX = Vector3.Dot(eye, xaxis);
            float dotY = Vector3.Dot(eye, yaxis);
            float dotZ = Vector3.Dot(eye, zaxis);
            return GetViewMatrixAxis(xaxis, yaxis, zaxis, dotX, dotY, dotZ);
        }

        /// <summary>
        /// 获得一个指定位置指定方向的观察视图矩阵
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetViewMatrix(Transform cam)
        {
            Vector3 eye = cam.position;
            Vector3 zaxis = cam.forward;
            Vector3 xaxis = cam.right;
            Vector3 yaxis = cam.up;
            float dotX = Vector3.Dot(eye, xaxis);
            float dotY = Vector3.Dot(eye, yaxis);
            float dotZ = Vector3.Dot(eye, zaxis);
            return GetViewMatrixAxis(cam.right, cam.up, cam.forward, dotX, dotY, dotZ);
        }

        static D3DMatrix4x4 GetViewMatrixAxis(Vector3 xaxis, Vector3 yaxis, Vector3 zaxis,
            float dotX, float dotY, float dotZ)
        {
            //转置矩阵 用于将世界坐标变换成局部坐标
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m33 = 1;
            res.m00 = xaxis.x; res.m01 = yaxis.x; res.m02 = zaxis.x;
            res.m10 = xaxis.y; res.m11 = yaxis.y; res.m12 = zaxis.y;
            res.m20 = xaxis.z; res.m21 = yaxis.z; res.m22 = zaxis.z;
            res.m30 = -dotX; res.m31 = -dotY; res.m32 = -dotZ;
            return res;
        }

        /// <summary>
        /// 获得一个投影矩阵 用于对观察视图矩阵的坐标进行投影转换
        /// </summary>
        /// <param name="aovY">纵向夹角</param>
        /// <param name="aspect">横宽比例</param>
        /// <param name="nearPlant">近剪裁面</param>
        /// <param name="farPlant">远剪裁面</param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetProjectionMatrix(float aovY, float aspect, float nearPlant, float farPlant)
        {
            float angle = aovY * 0.5f * UnityEngine.Mathf.Deg2Rad;
            float dis = farPlant - nearPlant;
            float yScale = UnityEngine.Mathf.Cos(angle) / UnityEngine.Mathf.Sin(angle);
            float xScale = yScale / aspect;
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m00 = xScale;
            res.m11 = yScale;
            res.m22 = farPlant / dis; res.m23 = 1;
            res.m32 = (-farPlant * nearPlant) / dis;
            return res;
        }

        /// <summary>
        /// 获得一个投影矩阵 用于对观察视图矩阵的坐标进行投影转换
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetProjectionMatrix(Camera cam)
        {
            return GetProjectionMatrix(cam.fieldOfView, cam.aspect, cam.nearClipPlane, cam.farClipPlane);
        }

        /// <summary>
        /// 获得一个屏幕矩阵 用于将投影矩阵左边转换为屏幕坐标
        /// </summary>
        /// <param name="w">屏幕宽 像素</param>
        /// <param name="h">屏幕高度 像素</param>
        /// <param name="offsetW"></param>
        /// <param name="offsetH"></param>
        /// <returns></returns>
        public static D3DMatrix4x4 GetScreenMatrix(int w, int h)
        {
            float halfW = w * 0.5f;
            float halfH = h * 0.5f;
            D3DMatrix4x4 res = new D3DMatrix4x4();
            res.m00 = halfW;
            //D3D Y轴朝下
            //res.m11 = -halfH;
            //unity OpenGL Y轴朝上
            res.m11 = halfH;
            res.m30 = halfW; res.m31 = halfH; res.m33 = 1;
            return res;
        }

        /// <summary>
        /// 获得一个世界点在指定的视图空间所在的屏幕坐标位置
        /// </summary>
        /// <param name="worldPoint">世界点坐标</param>
        /// <param name="eye">相机位置</param>
        /// <param name="lookAt">相机前方向</param>
        /// <param name="up">相机上</param>
        /// <param name="aovY">相机视野</param>
        /// <param name="nearPlant">相机近剪裁面</param>
        /// <param name="farPlant">相机远剪裁面</param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        public static bool WorldToScreenPoint(Vector3 worldPoint, Vector3 eye, Vector3 lookAt, Vector3 up,
            float aovY, float nearPlant, float farPlant, ref Vector3 screenPoint)
        {
            int w = Screen.width;
            int h = Screen.height;
            D3DMatrix4x4 viewMatr = GetViewMatrix(eye, lookAt, up);
            D3DMatrix4x4 pMatr = GetProjectionMatrix(aovY, (float)w / h, nearPlant, farPlant);
            D3DMatrix4x4 scMatr = GetScreenMatrix(w, h);
            Vector4 res = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1) * viewMatr * pMatr;
            float z = res.z;
            res *= scMatr;
            screenPoint.x = res.x / res.w;
            screenPoint.y = res.y / res.w;
            screenPoint.z = z;
            if (screenPoint.x < 0 || screenPoint.x > w || screenPoint.y < 0 || screenPoint.y > h || screenPoint.z < 0 || screenPoint.z > farPlant)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获得一个世界点在指定的视图空间所在的屏幕坐标位置
        /// </summary>
        /// <param name="worldPoint">世界点坐标</param>
        /// <param name="cam">相机</param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        public static bool WorldToScreenPoint(Vector3 worldPoint, Camera cam, ref Vector3 screenPoint)
        {
            int w = Screen.width;
            int h = Screen.height;
            D3DMatrix4x4 viewMatr = GetViewMatrix(cam.transform);
            D3DMatrix4x4 pMatr = GetProjectionMatrix(cam.fieldOfView, (float)w / h, cam.nearClipPlane, cam.farClipPlane);
            D3DMatrix4x4 scMatr = GetScreenMatrix(w, h);
            Vector4 res = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1) * viewMatr * pMatr;
            float z = res.z;
            res *= scMatr;
            screenPoint.x = res.x / res.w;
            screenPoint.y = res.y / res.w;
            screenPoint.z = z;
            if (screenPoint.x < 0 || screenPoint.x > w || screenPoint.y < 0 || screenPoint.y > h || screenPoint.z < 0 || screenPoint.z > cam.farClipPlane)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获得一个世界点在指定的视图空间所在的屏幕坐标位置
        /// </summary>
        /// <param name="worldPoint">世界点坐标</param>
        /// <param name="viewMatr">视图矩阵</param>
        /// <param name="pMatr">投影矩阵</param>
        /// <param name="scMatr">屏幕矩阵</param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        public static bool WorldToScreenPoint(Vector3 worldPoint, ref D3DMatrix4x4 viewMatr, ref D3DMatrix4x4 pMatr, ref D3DMatrix4x4 scMatr, ref Vector3 screenPoint)
        {
            int w = Screen.width;
            int h = Screen.height;
            Vector4 res = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1) * viewMatr * pMatr;
            float z = res.z;
            res *= scMatr;
            screenPoint.x = res.x / res.w;
            screenPoint.y = res.y / res.w;
            screenPoint.z = z;
            float farPlant = pMatr.m32 * (1f - (pMatr.m22 / (pMatr.m22 - 1)));
            //float nearPlant = farPlant * (((pMatr.m22 - 1)/ pMatr.m22));
            if (screenPoint.x < 0 || screenPoint.x > w || screenPoint.y < 0 || screenPoint.y > h || screenPoint.z < 0 || screenPoint.z > farPlant)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region//运算符

        public static D3DMatrix4x4 operator *(D3DMatrix4x4 a, D3DMatrix4x4 b)
        {
            D3DMatrix4x4 resoult = new D3DMatrix4x4();
            //
            resoult.m00 = a.m00 * b.m00 + a.m01 * b.m10 + a.m02 * b.m20 + a.m03 * b.m30;
            resoult.m01 = a.m00 * b.m01 + a.m01 * b.m11 + a.m02 * b.m21 + a.m03 * b.m31;
            resoult.m02 = a.m00 * b.m02 + a.m01 * b.m12 + a.m02 * b.m22 + a.m03 * b.m32;
            resoult.m03 = a.m00 * b.m03 + a.m01 * b.m13 + a.m02 * b.m23 + a.m03 * b.m33;
            //
            resoult.m10 = a.m10 * b.m00 + a.m11 * b.m10 + a.m12 * b.m20 + a.m13 * b.m30;
            resoult.m11 = a.m10 * b.m11 + a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
            resoult.m12 = a.m10 * b.m02 + a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
            resoult.m13 = a.m10 * b.m03 + a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;
            //
            resoult.m20 = a.m20 * b.m00 + a.m21 * b.m10 + a.m22 * b.m20 + a.m23 * b.m30;
            resoult.m21 = a.m20 * b.m01 + a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
            resoult.m22 = a.m20 * b.m02 + a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
            resoult.m23 = a.m20 * b.m03 + a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;
            //
            resoult.m00 = a.m30 * b.m00 + a.m31 * b.m10 + a.m32 * b.m20 + a.m33 * b.m30;
            resoult.m01 = a.m30 * b.m01 + a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
            resoult.m02 = a.m30 * b.m02 + a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
            resoult.m03 = a.m30 * b.m03 + a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;
            //
            return resoult;
        }

        public static Vector4 operator *(Vector4 pos, D3DMatrix4x4 b)
        {
            float x = pos.x * b.m00 + pos.y * b.m10 + pos.z * b.m20 + pos.w * b.m30;
            float y = pos.x * b.m01 + pos.y * b.m11 + pos.z * b.m21 + pos.w * b.m31;
            float z = pos.x * b.m02 + pos.y * b.m12 + pos.z * b.m22 + pos.w * b.m32;
            float w = pos.x * b.m03 + pos.y * b.m13 + pos.z * b.m23 + pos.w * b.m33;
            return new Vector4(x, y, z, w);
        }

        public static Vector4 operator *(D3DMatrix4x4 a, Vector4 vect)
        {
            float x = a.m00 * vect.x + a.m01 * vect.y + a.m02 * vect.z + a.m03 * vect.w;
            float y = a.m10 * vect.x + a.m11 * vect.y + a.m12 * vect.z + a.m13 * vect.w;
            float z = a.m20 * vect.x + a.m21 * vect.y + a.m22 * vect.z + a.m23 * vect.w;
            float w = a.m30 * vect.x + a.m31 * vect.y + a.m32 * vect.z + a.m33 * vect.w;
            return new Vector4(x, y, z, w);
        }

        public static D3DMatrix4x4 operator *(float f, D3DMatrix4x4 b)
        {
            D3DMatrix4x4 res = b;
            res.m00 *= f;
            res.m01 *= f;
            res.m02 *= f;
            res.m03 *= f;
            //
            res.m10 *= f;
            res.m11 *= f;
            res.m12 *= f;
            res.m13 *= f;
            //
            res.m20 *= f;
            res.m21 *= f;
            res.m22 *= f;
            res.m23 *= f;
            //
            res.m30 *= f;
            res.m31 *= f;
            res.m32 *= f;
            res.m33 *= f;
            return res;
        }

        public static D3DMatrix4x4 operator *(D3DMatrix4x4 a, float f)
        {
            return f * a;
        }

        public static bool operator ==(D3DMatrix4x4 a, D3DMatrix4x4 b)
        {
            int count = 4;
            for (int i = 0; i < count; ++i)
            {
                for (int j = 0; j < count; ++j)
                {
                    if (a[i, j] != b[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool operator !=(D3DMatrix4x4 a, D3DMatrix4x4 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this == (D3DMatrix4x4)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// 获得世界坐标在指定的视图空间中所对应的屏幕坐标
    /// </summary>
    public class World2ScreenPoint
    {
        Camera cam;

        Transform camTransform;

        D3DMatrix4x4 viewMatr;

        D3DMatrix4x4 pMatr;

        D3DMatrix4x4 scMatr;

        Vector3 lastZaxis = Vector3.zero;

        Vector3 lastEye;

        float lastAovY;

        float lastNearPlant;

        float lastFarPlant;

        int lastW;

        int lastH;

        /// <summary>
        /// 获得一个世界点在指定的视图空间所在的屏幕坐标位置
        /// </summary>
        /// <param name="worldPoint">世界点坐标</param>
        /// <param name="eye">相机位置</param>
        /// <param name="lookAt">相机前方向</param>
        /// <param name="up">相机上</param>
        /// <param name="aovY">相机视野</param>
        /// <param name="nearPlant">相机近剪裁面</param>
        /// <param name="farPlant">相机远剪裁面</param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        public bool WorldToScreenPoint(Vector3 worldPoint, Vector3 eye, Vector3 lookAt, Vector3 up,
            float aovY, float nearPlant, float farPlant, ref Vector3 screenPoint)
        {
            Vector3 zaxis = (lookAt - eye).normalized;
            if (zaxis != lastZaxis || lastEye != eye)
            {
                lastZaxis = zaxis;
                lastEye = eye;
                viewMatr = D3DMatrix4x4.GetViewMatrix(eye, lookAt, up);
            }
            if (lastAovY != aovY || lastNearPlant != nearPlant || lastFarPlant != farPlant)
            {
                if (lastW != Screen.width || lastH != Screen.width)
                {
                    lastW = Screen.width;
                    lastH = Screen.height;
                    scMatr = D3DMatrix4x4.GetScreenMatrix(lastW, lastH);
                }
                lastAovY = aovY;
                lastNearPlant = nearPlant;
                lastFarPlant = farPlant;
                pMatr = D3DMatrix4x4.GetProjectionMatrix(aovY, (float)lastW / lastH, nearPlant, farPlant);
            }
            else
            {
                if (lastW != Screen.width || lastH != Screen.width)
                {
                    lastW = Screen.width;
                    lastH = Screen.height;
                    scMatr = D3DMatrix4x4.GetScreenMatrix(lastW, lastH);
                    pMatr = D3DMatrix4x4.GetProjectionMatrix(aovY, (float)lastW / lastH, nearPlant, farPlant);
                }
            }
            Vector4 res = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1) * viewMatr * pMatr;
            float z = res.z;
            res *= scMatr;
            screenPoint.x = res.x / res.w;
            screenPoint.y = res.y / res.w;
            screenPoint.z = z;
            if (screenPoint.x < 0 || screenPoint.x > lastW || screenPoint.y < 0 || screenPoint.y > lastH || screenPoint.z < 0 || screenPoint.z > farPlant)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获得一个世界点在指定的视图空间所在的屏幕坐标位置
        /// </summary>
        /// <param name="worldPoint">世界点坐标</param>
        /// <param name="viewMatr">视图矩阵</param>
        /// <param name="pMatr">投影矩阵</param>
        /// <param name="scMatr">屏幕矩阵</param>
        /// <param name="screenPoint">屏幕坐标</param>
        /// <returns>返回是否在视野内</returns>
        public bool WorldToScreenPoint(Vector3 worldPoint, Camera cam, ref Vector3 screenPoint)
        {
            if (cam == null) return false;
            if (this.cam != cam)
            {
                this.cam = cam;
                this.camTransform = cam.transform;
            }
            if (this.camTransform.forward != lastZaxis || lastEye != this.camTransform.position)
            {
                lastZaxis = this.camTransform.forward;
                lastEye = this.camTransform.position;
                viewMatr = D3DMatrix4x4.GetViewMatrix(this.camTransform);
            }
            if (lastAovY != this.cam.fieldOfView || lastNearPlant != this.cam.nearClipPlane || lastFarPlant != this.cam.farClipPlane)
            {
                if (lastW != Screen.width || lastH != Screen.width)
                {
                    lastW = Screen.width;
                    lastH = Screen.height;
                    scMatr = D3DMatrix4x4.GetScreenMatrix(lastW, lastH);
                }
                lastAovY = this.cam.fieldOfView;
                lastNearPlant = this.cam.nearClipPlane;
                lastFarPlant = this.cam.farClipPlane;
                pMatr = D3DMatrix4x4.GetProjectionMatrix(cam.fieldOfView, (float)lastW / lastH, cam.nearClipPlane, cam.farClipPlane);
            }
            else
            {
                if (lastW != Screen.width || lastH != Screen.width)
                {
                    lastW = Screen.width;
                    lastH = Screen.height;
                    scMatr = D3DMatrix4x4.GetScreenMatrix(lastW, lastH);
                    pMatr = D3DMatrix4x4.GetProjectionMatrix(cam.fieldOfView, (float)lastW / lastH, cam.nearClipPlane, cam.farClipPlane);
                }
            }
            Vector4 res = new Vector4(worldPoint.x, worldPoint.y, worldPoint.z, 1) * viewMatr * pMatr;
            float z = res.z;
            res *= scMatr;
            screenPoint.x = res.x / res.w;
            screenPoint.y = res.y / res.w;
            screenPoint.z = z;
            float farPlant = pMatr.m32 * (1f - (pMatr.m22 / (pMatr.m22 - 1)));
            Debug.Log("farPlant:" + farPlant);
            float nearPlant = farPlant * (((pMatr.m22 - 1) / pMatr.m22));
            Debug.Log("nearPlant:" + farPlant);
            if (screenPoint.x < 0 || screenPoint.x > lastW || screenPoint.y < 0 || screenPoint.y > lastH || screenPoint.z < 0 || screenPoint.z > farPlant)
            {
                return false;
            }
            return true;
        }

    }
}


