using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    public class ConverterHelper
    {
        /// <summary>
        /// 对象转换为存储字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToStr64(object obj)
        {
            return Convert.ToBase64String(ObjectToBytes(obj));
        }

        /// <summary>
        /// 字符串到对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object Str64ToObject(string str)
        {
            return BytesToObject(Convert.FromBase64String(str));
        }

        /// <summary>
        /// 对象转字节数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ObjectToBytes(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, obj);
            byte[] datas = stream.ToArray();
            stream.Dispose();
            return datas;
        }

        /// <summary>
        /// 字节数组转对象
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static object BytesToObject(byte[] datas)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(datas, 0, datas.Length);
            object obj = bf.Deserialize(stream);
            stream.Dispose();
            return obj;
        }

        /// <summary>
        /// 获得object字节数组
        /// </summary>
        /// <param name="structObj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// 获得object字节数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes<T>(T obj)
        {
            try
            {
                int size = Marshal.SizeOf(obj);
                byte[] bytes = new byte[size];
                IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                Marshal.StructureToPtr(obj, arrPtr, true);
                return bytes;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        /// <summary>
        /// 转换字节数组为指定类型
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="strcutType"></param>
        /// <returns></returns>
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// 转换字节数组为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T BytesToStruct<T>(byte[] bytes)
        {
            try
            {
                IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                return (T)Marshal.PtrToStructure(arrPtr, typeof(T));
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return default(T);
            }
        }
    }
}
