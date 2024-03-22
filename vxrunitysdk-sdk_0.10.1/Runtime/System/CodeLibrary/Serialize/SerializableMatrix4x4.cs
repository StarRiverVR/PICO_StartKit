using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace com.vivo.codelibrary
{
    [Serializable]
    public class SerializableMatrix4x4 : IDataEquata<SerializableMatrix4x4>
    {

        public bool Equals(SerializableMatrix4x4 other)
        {
            return other == this;
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            if (o.GetType() != typeof(SerializableMatrix4x4))
            {
                return false;
            }

            SerializableMatrix4x4 second = (SerializableMatrix4x4)o;
            return second == this;
        }

        int _hashCode;

        static int hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public SerializableMatrix4x4(Matrix4x4 matrix4x4)
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

            //
            m00 = matrix4x4.m00;

            m10 = matrix4x4.m10;

            m20 = matrix4x4.m20;

            m30 = matrix4x4.m30;

            m01 = matrix4x4.m01;

            m11 = matrix4x4.m11;

            m21 = matrix4x4.m21;

            m31 = matrix4x4.m31;

            m02 = matrix4x4.m02;

            m12 = matrix4x4.m12;

            m22 = matrix4x4.m22;

            m32 = matrix4x4.m32;

            m03 = matrix4x4.m03;

            m13 = matrix4x4.m13;

            m23 = matrix4x4.m23;

            m33 = matrix4x4.m33;

        }

        public SerializableMatrix4x4(Vector4 column0, Vector4 column1, Vector4 column2, Vector4 column3)
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

            //

            m00 = column0.x;
            m01 = column1.x;
            m02 = column2.x;
            m03 = column3.x;
            m10 = column0.y;
            m11 = column1.y;
            m12 = column2.y;
            m13 = column3.y;
            m20 = column0.z;
            m21 = column1.z;
            m22 = column2.z;
            m23 = column3.z;
            m30 = column0.w;
            m31 = column1.w;
            m32 = column2.w;
            m33 = column3.w;
        }

        public float m00;

        public float m10;

        public float m20;

        public float m30;

        public float m01;

        public float m11;

        public float m21;

        public float m31;

        public float m02;

        public float m12;

        public float m22;

        public float m32;

        public float m03;

        public float m13;

        public float m23;

        public float m33;

        public override string ToString()
        {
            StringBuilder sb = StringBuilderPool.Instance.GetOneStringBuilder();
            sb.Append(string.Format("{0} {1} {2} {3}\n", m00, m01, m02, m03));
            sb.Append(string.Format("{0} {1} {2} {3}\n", m10, m11, m12, m13));
            sb.Append(string.Format("{0} {1} {2} {3}\n", m20, m21, m22, m23));
            sb.Append(string.Format("{0} {1} {2} {3}\n", m30, m31, m32, m33));
            string str = sb.ToString();
            StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
            return str;
        }

        public static implicit operator SerializableMatrix4x4(Matrix4x4 matrix4x4)
        {
            return new SerializableMatrix4x4(matrix4x4);
        }

        public static implicit operator Matrix4x4(SerializableMatrix4x4 matrix4x4)
        {
            Matrix4x4 newData = new Matrix4x4();
            newData.m00 = matrix4x4.m00;
            newData.m10 = matrix4x4.m10;
            newData.m20 = matrix4x4.m20;
            newData.m30 = matrix4x4.m30;
            newData.m01 = matrix4x4.m01;
            newData.m11 = matrix4x4.m11;
            newData.m21 = matrix4x4.m21;
            newData.m31 = matrix4x4.m31;
            newData.m02 = matrix4x4.m02;
            newData.m12 = matrix4x4.m12;
            newData.m22 = matrix4x4.m22;
            newData.m32 = matrix4x4.m32;
            newData.m03 = matrix4x4.m03;
            newData.m13 = matrix4x4.m13;
            newData.m23 = matrix4x4.m23;
            newData.m33 = matrix4x4.m33;
            return newData;
        }
    }
}


