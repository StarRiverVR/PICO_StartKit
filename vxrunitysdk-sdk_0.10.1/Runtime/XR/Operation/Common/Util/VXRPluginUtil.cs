using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.vivo.openxr
{
    /// <summary>
    /// VXRPlugin.API/Data/Version相关扩展与工具方法。
    /// </summary>
    public static class VXRPluginUtil
    {
        public static Vector3 ToVector3(this VXRPlugin.Vector3f v3f)
        {
            return new Vector3() { x = v3f.x, y = v3f.y, z = v3f.z };
        }
        
        public static VXRPlugin.Vector3f ToVector3f(this Vector3 v3)
        {
            return new VXRPlugin.Vector3f() { x = v3.x, y = v3.y, z = v3.z };
        }

        public static VXRPlugin.Quatf ToQuatf(this Quaternion q)
        {
            return new VXRPlugin.Quatf() { x = q.x, y = q.y, z = q.z, w = q.w };
        }

        /// <summary>
        /// 转换位置信息。
        /// </summary>
        /// <param name="position">坐标</param>
        /// <param name="rotation">方向</param>
        /// <returns></returns>
        public static VXRPlugin.Posef ToPose(Vector3 position, Quaternion rotation)
        {
            VXRPlugin.Posef pose;
            pose.Position.x = position.x;
            pose.Position.y = position.y;
            pose.Position.z = position.z;
            pose.Orientation.x = rotation.x;
            pose.Orientation.y = rotation.y;
            pose.Orientation.z = rotation.z;
            pose.Orientation.w = rotation.w;

            return pose;
        }
        
        public static Vector3 GetVector3(this VXRPlugin.Posef pose)
        {
            return new Vector3()
            {
                x = pose.Position.x,
                y = pose.Position.y,
                z = pose.Position.y,
            };
        }
        
        public static Quaternion GetQuaternion(this VXRPlugin.Posef pose)
        {
            return new Quaternion()
            {
                x = pose.Orientation.x,
                y = pose.Orientation.y,
                z = pose.Orientation.z,
                w = pose.Orientation.w
            };
        }

        /// <summary>
        /// 将ulong数组转换为Native结构体，传递给C++
        /// 使用完毕后需要手动释放 Marshal.FreeHGlobal(ptr);
        /// </summary>
        /// <param name="arr">目标数组</param>
        /// <returns>Native结构体</returns>
        public static VXRPlugin.IntPtrList ToIntPtrList(ulong[] arr)
        {
            int count = arr == null ? 0 : arr.Length;
            VXRPlugin.IntPtrList stru = new VXRPlugin.IntPtrList();
            stru.PtrList = VXRDeserialize.UlongArrayToIntPtr(arr);
            stru.Count = (uint)count;
            return stru;

        }

        /// <summary>
        /// Native 结构体转换为ulong数组
        /// </summary>
        /// <param name="stru">Native结构体</param>
        /// <param name="arr">目标类型数组</param>
        /// <returns>是否转换成功</returns>
        public static bool ToArray(this VXRPlugin.IntPtrList stru, out ulong[] arr)
        {
            arr = VXRDeserialize.IntPrtToUlongArray(stru.PtrList, stru.Count);
            return true;
        }

        /// <summary>
        /// Guid数组转换为Native结构体。
        /// </summary>
        /// <param name="array">Guid数组</param>
        /// <returns>Native 结构体</returns>
        public static VXRPlugin.IntPtrList ToIntPtrList(Guid[] arr)
        {
            int count = arr == null?0:arr.Length ;
            VXRPlugin.IntPtrList stru = new VXRPlugin.IntPtrList();
            stru.PtrList = VXRDeserialize.GuidArrayToIntPtr(arr);
            stru.Count = (uint)count;
            return stru;
        }

        /// <summary>
        /// Native 结构转换为自定义结构体数组。
        /// </summary>
        /// <typeparam name="T">自定义结构体类型</typeparam>
        /// <param name="stru">Native结构体</param>
        /// <param name="arr">目标数组</param>
        /// <returns>是否转换成功</returns>
        public static bool ToStructureArray<T>(this VXRPlugin.IntPtrList stru, out T[] arr) where T : struct
        {
            arr = VXRDeserialize.IntPtrToStructureArray<T>(stru.PtrList, stru.Count);
            return true;
        }

        /// <summary>
        /// 删除Native结构体。 
        /// </summary>
        /// <param name="stru">Native结构体</param>
        public static void Free(this VXRPlugin.IntPtrList stru)
        {
            Marshal.FreeHGlobal(stru.PtrList);
        }
    }
}