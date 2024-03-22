using com.vivo.openxr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.vivo.openxr
{

    public sealed partial class VXRPlugin
    {
        public static bool PollEvent(ref EventDataBuffer eventDataBuffer)
        {
    #if VXR_UNSUPPORTED_PLATFORM
                VLog.Warning("FFR Not Supported");
                eventDataBuffer = default(VXREventManager.EventDataBuffer);
                return false;
    #else
            IntPtr eventDataPtr = IntPtr.Zero;
            Result ret = VXRVersion_0_0_2.vxr_PollEvent(ref eventDataBuffer.EventType, ref eventDataPtr);

            if (ret != Result.Success || eventDataPtr == IntPtr.Zero)
            {
                return false;
            }

            if (eventDataBuffer.EventData == null)
            {
                eventDataBuffer.EventData = new byte[k_eventDataBufferSize];
            }
            Marshal.Copy(eventDataPtr, eventDataBuffer.EventData, 0, k_eventDataBufferSize);
            return true;
    #endif
        }

        public static void PollAndroidEvent(ref List<EventDataBuffer> buffList)
        {
    #if VXR_UNSUPPORTED_PLATFORM
           
    #else
            VXRVersion_0_6_0.PollAndroidEvent(ref buffList);
    #endif
        }

        public static void ReleaseAndroidVXREvent()
        {
    #if VXR_UNSUPPORTED_PLATFORM
           
    #else
            VXRVersion_0_6_0.ReleaseAndroidVXREvent();
    #endif
        }
    }

}