using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_0_2
        {

            [DllImport(pluginName, EntryPoint = "vxr_PollEvent")]
            public static extern Result vxr_PollEvent(ref VXRPlugin.EventType eventType, ref IntPtr eventData);
        }

        private sealed partial class VXRVersion_0_6_0
        {
            public static void PollAndroidEvent(ref List<EventDataBuffer> buffList)
            {
                VXREventPlatfrom.PollEvent(ref buffList);
            }

            public static void ReleaseAndroidVXREvent()
            {
                VXREventPlatfrom.ReleaseAndroidVXREvent();
            }
        }
    }

}