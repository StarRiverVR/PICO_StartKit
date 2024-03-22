using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    [Serializable]
    public struct SerializableVector2: IDataEquata<SerializableVector2>
    {
        public float x;
        public float y;

        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(x * x + y * y );
            }
        }


        public bool Equals(SerializableVector2 other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType()!=typeof(SerializableVector2))
            {
                return false;
            }

            SerializableVector2 second = (SerializableVector2)o;
            return second==this;
        }

        int _hashCode;

        static int hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public SerializableVector2(float rX=0, float rY=0)
        {
            if (hashCode >= int.MaxValue-1)
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
        }

        public static float Dot(SerializableVector2 a, SerializableVector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static float Magnitude(SerializableVector2 a)
        {
            return Mathf.Sqrt(a.x * a.x + a.y * a.y);
        }

        public static float Distance(SerializableVector2 a, SerializableVector2 b)
        {
            float d_x = a.x - b.x;
            float d_y = a.y - b.y;
            return Mathf.Sqrt(d_x* d_x+ d_y* d_y);
        }

        // 以字符串形式返回,方便调试查看
        public override string ToString()
        {
            return String.Format("[{0}, {1}]", x, y);
        }

        // 隐式转换：将SerializableVector2 转换成 Vector2
        public static implicit operator Vector2(SerializableVector2 rValue)
        {
            return new Vector2(rValue.x, rValue.y);
        }

        // 隐式转换：将Vector2 转成 SerializableVector2
        public static implicit operator SerializableVector2(Vector2 rValue)
        {
            return new SerializableVector2(rValue.x, rValue.y);
        }

        public static bool operator ==(SerializableVector2 b, SerializableVector2 c)
        {
            return b.x== c.x && b.y == c.y;
        }

        public static bool operator !=(SerializableVector2 b, SerializableVector2 c)
        {
            return !(b == c);
        }

        public static SerializableVector2 operator +(SerializableVector2 b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x + c.x;
            res.y = b.y + c.y;
            return res;
        }

        public static SerializableVector2 operator -(SerializableVector2 b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x - c.x;
            res.y = b.y - c.y;
            return res;
        }

        public static SerializableVector2 operator *(SerializableVector2 b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x * c.x;
            res.y = b.y * c.y;
            return res;
        }

        public static SerializableVector2 operator *(float b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b * c.x;
            res.y = b * c.y;
            return res;
        }

        public static SerializableVector2 operator *(SerializableVector2 b, float c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x * c;
            res.y = b.y * c;
            return res;
        }

        public static SerializableVector2 operator /(SerializableVector2 b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x / c.x;
            res.y = b.y / c.y;
            return res;
        }

        public static SerializableVector2 operator /(float b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b / c.x;
            res.y = b / c.y;
            return res;
        }

        public static SerializableVector2 operator /(SerializableVector2 b, float c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x / c;
            res.y = b.y / c;
            return res;
        }

        public static SerializableVector2 operator %(SerializableVector2 b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x % c.x;
            res.y = b.y % c.y;
            return res;
        }

        public static SerializableVector2 operator %(float b, SerializableVector2 c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b % c.x;
            res.y = b % c.y;
            return res;
        }

        public static SerializableVector2 operator %(SerializableVector2 b, float c)
        {
            SerializableVector2 res = new SerializableVector2();
            res.x = b.x % c;
            res.y = b.y % c;
            return res;
        }
    }
}
