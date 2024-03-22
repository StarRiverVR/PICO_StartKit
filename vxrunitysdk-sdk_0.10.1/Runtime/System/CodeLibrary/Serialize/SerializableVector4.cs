using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    [Serializable]
    public struct SerializableVector4 : IDataEquata<SerializableVector4>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y + z * z + w * w); ;
            }
        }

        public bool Equals(SerializableVector4 other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() != typeof(SerializableVector4))
            {
                return false;
            }

            SerializableVector4 second = (SerializableVector4)o;
            return second == this;
        }

        int _hashCode;

        static int hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public SerializableVector4(float rX=0, float rY=0, float rZ=0, float rW=0)
        {
            if (hashCode >= int.MaxValue - 1)
            {
                hashCode = 0;
            }
            else
            {
                hashCode++;
            }
            _hashCode = hashCode;

            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        public static float Dot(SerializableVector4 a, SerializableVector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static float Magnitude(SerializableVector4 a)
        {
            return Mathf.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w);
        }

        public static float Distance(SerializableVector4 a, SerializableVector4 b)
        {
            float d_x = a.x - b.x;
            float d_y = a.y - b.y;
            float d_z = a.z - b.z;
            float d_w = a.w - b.w;
            return Mathf.Sqrt(d_x * d_x + d_y * d_y + d_z * d_z+ d_w* d_w);
        }

        // 以字符串形式返回,方便调试查看
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }

        // 隐式转换：将SerializableVector4 转换成 Vector4
        public static implicit operator Vector4(SerializableVector4 rValue)
        {
            return new Vector4(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        // 隐式转换：将Vector4 转成 SerializableVector4
        public static implicit operator SerializableVector4(Vector4 rValue)
        {
            return new SerializableVector4(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        public static bool operator ==(SerializableVector4 b, SerializableVector4 c)
        {
            return b.x == c.x && b.y == c.y && b.z == c.z && b.w == c.w;
        }

        public static bool operator !=(SerializableVector4 b, SerializableVector4 c)
        {
            return !(b == c);
        }

        public static SerializableVector4 operator +(SerializableVector4 b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x + c.x;
            res.y = b.y + c.y;
            res.z = b.z + c.z;
            res.w = b.w + c.w;
            return res;
        }

        public static SerializableVector4 operator -(SerializableVector4 b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x - c.x;
            res.y = b.y - c.y;
            res.z = b.z - c.z;
            res.w = b.w - c.w;
            return res;
        }

        public static SerializableVector4 operator *(SerializableVector4 b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x * c.x;
            res.y = b.y * c.y;
            res.z = b.z * c.z;
            res.w = b.w * c.w;
            return res;
        }

        public static SerializableVector4 operator *(float b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b * c.x;
            res.y = b * c.y;
            res.z = b * c.z;
            res.w = b * c.w;
            return res;
        }

        public static SerializableVector4 operator *(SerializableVector4 b, float c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x * c;
            res.y = b.y * c;
            res.z = b.z * c;
            res.w = b.w * c;
            return res;
        }

        public static SerializableVector4 operator /(SerializableVector4 b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x / c.x;
            res.y = b.y / c.y;
            res.z = b.z / c.z;
            res.w = b.w / c.w;
            return res;
        }

        public static SerializableVector4 operator /(float b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b / c.x;
            res.y = b / c.y;
            res.z = b / c.z;
            res.w = b / c.w;
            return res;
        }

        public static SerializableVector4 operator /(SerializableVector4 b, float c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x / c;
            res.y = b.y / c;
            res.z = b.z / c;
            res.w = b.w / c;
            return res;
        }

        public static SerializableVector4 operator %(SerializableVector4 b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x % c.x;
            res.y = b.y % c.y;
            res.z = b.z % c.z;
            res.w = b.w % c.w;
            return res;
        }

        public static SerializableVector4 operator %(float b, SerializableVector4 c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b % c.x;
            res.y = b % c.y;
            res.z = b % c.z;
            res.w = b % c.w;
            return res;
        }

        public static SerializableVector4 operator %(SerializableVector4 b, float c)
        {
            SerializableVector4 res = new SerializableVector4();
            res.x = b.x % c;
            res.y = b.y % c;
            res.z = b.z % c;
            res.w = b.w % c;
            return res;
        }
    }
}

