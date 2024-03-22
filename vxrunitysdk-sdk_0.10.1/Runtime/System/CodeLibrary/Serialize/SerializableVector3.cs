using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    [Serializable]
    public struct SerializableVector3 : IDataEquata<SerializableVector3>
    {
        public float x;
        public float y;
        public float z;

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y + z * z);
            }
        }

        public bool Equals(SerializableVector3 other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() != typeof(SerializableVector3))
            {
                return false;
            }

            SerializableVector3 second = (SerializableVector3)o;
            return second == this;
        }

        int _hashCode;

        static int hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public SerializableVector3(float rX=0, float rY=0, float rZ=0)
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
        }

        public static float Dot(SerializableVector3 a, SerializableVector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static SerializableVector3 Cross(SerializableVector3 a, SerializableVector3 b)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = a.y * b.z - a.z * b.y;
            res.y = a.z * b.x - a.x * b.z;
            res.z = a.x * b.y - a.y * b.x;
            return res;
        }

        public static float Magnitude(SerializableVector3 a)
        {
            return Mathf.Sqrt(a.x*a.x+ a.y * a.y+ a.z*a.z);
        }

        public static float Distance(SerializableVector3 a, SerializableVector3 b)
        {
            float d_x = a.x - b.x;
            float d_y = a.y - b.y;
            float d_z = a.z - b.z;
            return Mathf.Sqrt(d_x * d_x + d_y * d_y+ d_z* d_z);
        }

        // 以字符串形式返回,方便调试查看
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        // 隐式转换：将SerializableVector3 转换成 Vector3
        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        // 隐式转换：将Vector3 转成 SerializableVector3
        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }

        public static bool operator ==(SerializableVector3 b, SerializableVector3 c)
        {
            return b.x == c.x && b.y == c.y && b.z == c.z;
        }

        public static bool operator !=(SerializableVector3 b, SerializableVector3 c)
        {
            return !(b == c);
        }

        public static SerializableVector3 operator +(SerializableVector3 b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x + c.x;
            res.y = b.y + c.y;
            res.z = b.z + c.z;
            return res;
        }

        public static SerializableVector3 operator -(SerializableVector3 b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x - c.x;
            res.y = b.y - c.y;
            res.z = b.z - c.z;
            return res;
        }

        public static SerializableVector3 operator *(SerializableVector3 b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x * c.x;
            res.y = b.y * c.y;
            res.z = b.z * c.z;
            return res;
        }

        public static SerializableVector3 operator *(float b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b * c.x;
            res.y = b * c.y;
            res.z = b * c.z;
            return res;
        }

        public static SerializableVector3 operator *(SerializableVector3 b, float c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x * c;
            res.y = b.y * c;
            res.z = b.z * c;
            return res;
        }

        public static SerializableVector3 operator /(SerializableVector3 b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x / c.x;
            res.y = b.y / c.y;
            res.z = b.z / c.z;
            return res;
        }

        public static SerializableVector3 operator /(float b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b / c.x;
            res.y = b / c.y;
            res.z = b / c.z;
            return res;
        }

        public static SerializableVector3 operator /(SerializableVector3 b, float c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x / c;
            res.y = b.y / c;
            res.z = b.z / c;
            return res;
        }

        public static SerializableVector3 operator %(SerializableVector3 b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x % c.x;
            res.y = b.y % c.y;
            res.z = b.z % c.z;
            return res;
        }

        public static SerializableVector3 operator %(float b, SerializableVector3 c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b % c.x;
            res.y = b % c.y;
            res.z = b % c.z;
            return res;
        }

        public static SerializableVector3 operator %(SerializableVector3 b, float c)
        {
            SerializableVector3 res = new SerializableVector3();
            res.x = b.x % c;
            res.y = b.y % c;
            res.z = b.z % c;
            return res;
        }
    }
}

