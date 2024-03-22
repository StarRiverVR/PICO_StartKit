using System;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static bool GetDisplayRefreshRate(out float rate)
        {
#if VXR_UNSUPPORTED_PLATFORM
            rate = 90f;
            return true;
#else
            return VXRVersion_0_0_2.vxr_GetDisplayRefreshRate(out rate) == Result.Success;
#endif
        }

        public static bool SetDisplayRefreshRate(float rate)
        {
#if VXR_UNSUPPORTED_PLATFORM
            rate = 90f;
            return true;
#else
            return VXRVersion_0_0_2.vxr_SetDisplayRefreshRate(rate) == Result.Success;
#endif
        }

        public static float[] GetRefreshRatesAvailable()
        {
#if VXR_UNSUPPORTED_PLATFORM
            return null;
#else
            Result ret = VXRVersion_0_0_2.vxr_EnumerateDisplayRefreshRates(IntPtr.Zero, out int count);
            if (ret == Result.Success)
            {
                if (count > 0)
                {
                    var buff = new NativeBuffer(sizeof(float) * count);
                    ret = VXRVersion_0_0_2.vxr_EnumerateDisplayRefreshRates(buff.GetPointer(), out count);
                    if (ret == Result.Success)
                    {
                        if (count > 0)
                        {
                            var array = new float[count];
                            System.Runtime.InteropServices.Marshal.Copy(buff.GetPointer(), array, 0, count);
                            return array;
                        }
                    }
                }
            }

            return null;
#endif
        }
    }
}
