using System.Runtime.InteropServices;
using com.vivo.codelibrary;
using System;

namespace com.vivo.openxr
{
    // 数据序列化/反序列化操作
    public static class VXRDeserialize
    {
        /// <summary>
        /// byte 数组转换为指定结构体。
        /// </summary>
        /// <typeparam name="T">指定结构体</typeparam>
        /// <param name="bytes">byte数组</param>
        /// <returns>转换成功的结构体对象实例</returns>
        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            T stuff;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return stuff;
        }

        /// <summary>
        /// 结构体转换为指定byte数组。
        /// </summary>
        /// <typeparam name="T">指定结构体</typeparam>
        /// <param name="t">结构体对象实例</param>
        /// <returns>byte数组</returns>
        public static byte[] StructureToByteArray<T>(T t) where T : struct
        {
            byte[] bytes = new byte[Marshal.SizeOf(t)];
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(t, handle.AddrOfPinnedObject(), false);
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                throw;
            }            
            handle.Free();
            return bytes;
        }

        /// <summary>
        /// 将ulong数组转换为IntPtr指针。
        /// 转换成功，count大于0
        /// </summary>
        /// <param name="arr">ulong数组</param>
        /// <param name="count">转换成功的数组长度</param>
        /// <returns>转换成功的IntPtr指针</returns>
        public static IntPtr UlongArrayToIntPtr(ulong[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return IntPtr.Zero;
            }

            IntPtr ptr = Marshal.AllocHGlobal(arr.Length * Marshal.SizeOf(typeof(ulong)));
            long[] tmpHandles = Array.ConvertAll(arr, x => (long)x);
            Marshal.Copy(tmpHandles, 0, ptr, arr.Length);
            return ptr;
        }

        /// <summary>
        /// 将IntPtr指针转换为ulong数组。
        /// </summary>
        /// <param name="ptr">IntPtr指针</param>
        /// <param name="count">数组长度</param>
        /// <returns>ulong数组</returns>
        public static ulong[] IntPrtToUlongArray(IntPtr ptr, uint count)
        {
            long[] array = new long[count];
            Marshal.Copy(ptr, array, 0, (int)count);
            return Array.ConvertAll(array, x => (ulong)x);
        }

        /// <summary>
        /// 将Guid数组转换为IntPtr指针。
        /// </summary>
        /// <param name="arr">Guid数组</param>
        /// <param name="count">转换成功的数组长度</param>
        /// <returns>转换成功的IntPtr指针</returns>
        public static IntPtr GuidArrayToIntPtr(Guid[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                return IntPtr.Zero;
            }
            string[] strs = Array.ConvertAll(arr, x => x.ToString());
            IntPtr[] ptrs = new IntPtr[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                ptrs[i] = Marshal.StringToHGlobalAnsi(strs[i]);
            }
            GCHandle handle = GCHandle.Alloc(ptrs, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }

        /// <summary>
        /// 将IntPtr指针转换为Guid数组。
        /// </summary>
        /// <param name="ptr">IntPtr指针</param>
        /// <param name="count">数组长度</param>
        /// <returns>Guid数组</returns>
        public static Guid[] IntPtrToGuidArray(IntPtr ptr, uint count)
        {
            Guid[] guids = new Guid[count];
            for (int i = 0; i < count; i++)
            {
                IntPtr p = Marshal.ReadIntPtr(ptr, i * IntPtr.Size);
                guids[i] = Guid.Parse(Marshal.PtrToStringAnsi(p));
            }
            return guids;
        }

        /// <summary>
        /// 将IntPtr转换为自定义结构体实例数组
        /// </summary>
        /// <typeparam name="T">自定义结构体类型</typeparam>
        /// <param name="ptr">IntPtr指针</param>
        /// <param name="count">转换成功的数组长度</param>
        /// <returns>结构体实例数组</returns>
        public static T[] IntPtrToStructureArray<T>(IntPtr ptr, uint count) where T : struct
        {
            T[] arr = new T[count];
            IntPtr current = ptr;
            for (int i = 0; i < count; i++)
            {
                arr[i] = (T)Marshal.PtrToStructure(current, typeof(T));
                current += Marshal.SizeOf<T>();
            }
            return arr;
        }
    }
}
