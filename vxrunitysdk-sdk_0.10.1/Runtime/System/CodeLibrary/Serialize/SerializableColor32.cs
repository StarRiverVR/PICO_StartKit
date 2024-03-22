using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    [Serializable]
    public struct SerializableColor32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public SerializableColor32(byte rX, byte rY, byte rZ, byte rA)
        {
            r = rX;
            g = rY;
            b = rZ;
            a = rA;
        }

        // 以字符串形式返回,方便调试查看
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", r, g, b, a);
        }

        // 隐式转换：将SerializableColor 转换成 Color
        public static implicit operator Color32(SerializableColor32 rValue)
        {
            return new Color32(rValue.r, rValue.g, rValue.b, rValue.a);
        }

        public static bool operator ==(SerializableColor32 b, SerializableColor32 c)
        {
            return b.r == c.r && b.g == c.g && b.b == c.b && b.a == c.a;
        }

        public static bool operator !=(SerializableColor32 b, SerializableColor32 c)
        {
            return !(b == c);
        }

        public static bool operator ==(SerializableColor32 b, Color32 c)
        {
            return b.r == c.r && b.g == c.g && b.b == c.b && b.a == c.a;
        }

        public static bool operator !=(SerializableColor32 b, Color32 c)
        {
            return !(b == c);
        }

        // 隐式转换：将Color 转成 SerializableColor32
        public static implicit operator SerializableColor32(Color32 rValue)
        {
            return new SerializableColor32(rValue.r, rValue.g, rValue.b, rValue.a);
        }

        public static SerializableColor32 operator +(SerializableColor32 b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r + c.r);
            res.g = (byte)(b.g + c.g);
            res.b = (byte)(b.b + c.b);
            res.a = (byte)(b.a + c.a);
            return res;
        }

        public static SerializableColor32 operator -(SerializableColor32 b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r - c.r);
            res.g = (byte)(b.g - c.g);
            res.b = (byte)(b.b - c.b);
            res.a = (byte)(b.a - c.a);
            return res;
        }

        public static SerializableColor32 operator *(SerializableColor32 b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r * c.r);
            res.g = (byte)(b.g * c.g);
            res.b = (byte)(b.b * c.b);
            res.a = (byte)(b.a * c.a);
            return res;
        }

        public static SerializableColor32 operator *(float b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b * c.r);
            res.g = (byte)(b * c.g);
            res.b = (byte)(b * c.b);
            res.a = (byte)(b * c.a);
            return res;
        }

        public static SerializableColor32 operator *(SerializableColor32 b, float c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r * c);
            res.g = (byte)(b.g * c);
            res.b = (byte)(b.b * c);
            res.a = (byte)(b.a * c);
            return res;
        }

        public static SerializableColor32 operator /(SerializableColor32 b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r / c.r);
            res.g = (byte)(b.g / c.g);
            res.b = (byte)(b.b / c.b);
            res.a = (byte)(b.a / c.a);
            return res;
        }

        public static SerializableColor32 operator /(float b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b / c.r);
            res.g = (byte)(b / c.g);
            res.b = (byte)(b / c.b);
            res.a = (byte)(b / c.a);
            return res;
        }


        public static SerializableColor32 operator /(SerializableColor32 b, float c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r / c);
            res.g = (byte)(b.g / c);
            res.b = (byte)(b.b / c);
            res.a = (byte)(b.a / c);
            return res;
        }

        public static SerializableColor32 operator %(SerializableColor32 b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r % c.r);
            res.g = (byte)(b.g % c.g);
            res.b = (byte)(b.b % c.b);
            res.a = (byte)(b.a % c.a);
            return res;
        }

        public static SerializableColor32 operator %(float b, SerializableColor32 c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b % c.r);
            res.g = (byte)(b % c.g);
            res.b = (byte)(b % c.b);
            res.a = (byte)(b % c.a);
            return res;
        }

        public static SerializableColor32 operator %(SerializableColor32 b, float c)
        {
            SerializableColor32 res = new SerializableColor32();
            res.r = (byte)(b.r % c);
            res.g = (byte)(b.g % c);
            res.b = (byte)(b.b % c);
            res.a = (byte)(b.a % c);
            return res;
        }
    }
}


