using System.Runtime.InteropServices;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private const string PluginEyeTracking = "VXREyeTracking";
        private sealed partial class VXRVersion_0_0_1
        {
            [DllImport(PluginEyeTracking, EntryPoint = "vxr_StartEyeTracking")]
            public static extern Result vxr_StartEyeTracking();

            [DllImport(PluginEyeTracking, EntryPoint = "vxr_GetEyeTrackingData")]
            public static extern Result vxr_GetEyeTrackingData(out EyeTrackingData data);

            [DllImport(PluginEyeTracking, EntryPoint = "vxr_StopEyeTracking")]
            public static extern Result vxr_StopEyeTracking();
        }
    }
}