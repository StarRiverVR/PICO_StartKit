using System;
using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        /// <summary>
        /// 返回结果
        /// </summary>
        public enum Result
        {
            // Success
            Success = 0,
            EventUnavailable = 1,
            Pending = 2,
            // Failure
            Failure = -1000,
            Unsupported = -1001,
            // Error
            NotInit = -9000,
        }
        
        public class NativeBuffer : IDisposable
        {
            private bool _disposed = false;
            private int _numBytes = 0;
            private IntPtr _ptr = IntPtr.Zero;

            /// <summary>
            /// Creates a buffer of the specified size.
            /// </summary>
            public NativeBuffer(int numBytes)
            {
                Reallocate(numBytes);
            }
            ~NativeBuffer()
            {
                Dispose(false);
            }
            public void Reset(int numBytes)
            {
                Reallocate(numBytes);
            }
            public int GetCapacity()
            {
                return _numBytes;
            }
            public IntPtr GetPointer(int byteOffset = 0)
            {
                if (byteOffset < 0 || byteOffset >= _numBytes)
                    return IntPtr.Zero;
                return (byteOffset == 0) ? _ptr : new IntPtr(_ptr.ToInt64() + byteOffset);
            }
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                    return;

                Release();

                _disposed = true;
            }

            private void Reallocate(int numBytes)
            {
                Release();

                if (numBytes > 0)
                {
                    _ptr = Marshal.AllocHGlobal(numBytes);
                    _numBytes = numBytes;
                }
                else
                {
                    _ptr = IntPtr.Zero;
                    _numBytes = 0;
                }
            }
            private void Release()
            {
                if (_ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_ptr);
                    _ptr = IntPtr.Zero;
                    _numBytes = 0;
                }
            }
        }

        #region// StructLayout
        [StructLayout(LayoutKind.Sequential)]
        public struct Vector2f
        {
            public float x;
            public float y;
            public static readonly Vector2f zero = new Vector2f { x = 0.0f, y = 0.0f};
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct Vector3f
        {
            public float x;
            public float y;
            public float z;
            public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}", x, y, z);
            }

            public static implicit operator UnityEngine.Vector3(Vector3f v3f)
            {
                return new UnityEngine.Vector3() { x = v3f.x, y = v3f.y, z = v3f.z };
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Quatf
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public static readonly Quatf identity = new Quatf { x = 0.0f, y = 0.0f, z = 0.0f, w = 1.0f };
            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Posef
        {
            public Quatf Orientation;
            public Vector3f Position;

            public static readonly Posef identity = new Posef { Orientation = Quatf.identity, Position = Vector3f.zero };

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Position ({0}), Orientation({1})", Position, Orientation);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size3f
        {
            public float w;
            public float h;
            public float d;

            public static readonly Size3f zero = new Size3f { w = 0, h = 0, d = 0 };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IntPtrList
        {
            public uint Count;
            public IntPtr PtrList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Colorf
        {
            public float r;
            public float g;
            public float b;
            public float a;

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "R:{0:F3} G:{1:F3} B:{2:F3} A:{3:F3}", r, g, b, a);
            }
        }


        #endregion// StructLayout


    }
}
