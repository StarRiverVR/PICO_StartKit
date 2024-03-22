using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    [Serializable]
    public struct SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SerializableColor(float rX, float rY, float rZ,float rA)
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
        public static implicit operator Color(SerializableColor rValue)
        {
            return new Color(rValue.r, rValue.g, rValue.b, rValue.a);
        }

        // 隐式转换：将Color 转成 SerializableColor
        public static implicit operator SerializableColor(Color rValue)
        {
            return new SerializableColor(rValue.r, rValue.g, rValue.b, rValue.a);
        }

        public static bool operator ==(SerializableColor b, SerializableColor c)
        {
            return b.r == c.r && b.g == c.g && b.b == c.b && b.a == c.a;
        }

        public static bool operator !=(SerializableColor b, SerializableColor c)
        {
            return !(b == c);
        }

        public static bool operator ==(SerializableColor b, Color c)
        {
            return b.r == c.r && b.g == c.g && b.b == c.b && b.a == c.a;
        }

        public static bool operator !=(SerializableColor b, Color c)
        {
            return !(b == c);
        }

        public static SerializableColor operator +(SerializableColor b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r + c.r;
            res.g = b.g + c.g;
            res.b = b.b + c.b;
            res.a = b.a + c.a;
            return res;
        }

        public static SerializableColor operator -(SerializableColor b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r - c.r;
            res.g = b.g - c.g;
            res.b = b.b - c.b;
            res.a = b.a - c.a;
            return res;
        }

        public static SerializableColor operator *(SerializableColor b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r * c.r;
            res.g = b.g * c.g;
            res.b = b.b * c.b;
            res.a = b.a * c.a;
            return res;
        }

        public static SerializableColor operator *(float b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b * c.r;
            res.g = b * c.g;
            res.b = b * c.b;
            res.a = b * c.a;
            return res;
        }

        public static SerializableColor operator *(SerializableColor b, float c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r * c;
            res.g = b.g * c;
            res.b = b.b * c;
            res.a = b.a * c;
            return res;
        }

        public static SerializableColor operator /(SerializableColor b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r / c.r;
            res.g = b.g / c.g;
            res.b = b.b / c.b;
            res.a = b.a / c.a;
            return res;
        }

        public static SerializableColor operator /(float b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b / c.r;
            res.g = b / c.g;
            res.b = b / c.b;
            res.a = b / c.a;
            return res;
        }


        public static SerializableColor operator /(SerializableColor b, float c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r / c;
            res.g = b.g / c;
            res.b = b.b / c;
            res.a = b.a / c;
            return res;
        }

        public static SerializableColor operator %(SerializableColor b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r % c.r;
            res.g = b.g % c.g;
            res.b = b.b % c.b;
            res.a = b.a % c.a;
            return res;
        }

        public static SerializableColor operator %(float b, SerializableColor c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b % c.r;
            res.g = b % c.g;
            res.b = b % c.b;
            res.a = b % c.a;
            return res;
        }

        public static SerializableColor operator %(SerializableColor b, float c)
        {
            SerializableColor res = new SerializableColor();
            res.r = b.r % c;
            res.g = b.g % c;
            res.b = b.b % c;
            res.a = b.a % c;
            return res;
        }
    }
}


