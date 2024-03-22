using System;
using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_0_0
        {
            #region// Version
            [DllImport(pluginName, EntryPoint = "vxr_SetPluginVersion")]
            public static extern Result vxr_SetPluginVersion(int major, int minor, int patch);
            #endregion
        }
        private sealed partial class VXRVersion_0_0_2
        {
            #region // OpenXR
            [DllImport(pluginName, EntryPoint = "vxr_HookGetInstanceProcAddr")]
            internal static extern IntPtr vxr_HookGetInstanceProcAddr(IntPtr xrGetInstanceProcAddr);
            [DllImport(pluginName, EntryPoint = "vxr_OnInstanceCreate")]
            public static extern Result vxr_OnInstanceCreate(UInt64 xrInstance);
            [DllImport(pluginName, EntryPoint = "vxr_OnSystemChange")]
            internal static extern Result vxr_OnSystemChange(UInt64 systemId);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionCreate")]
            internal static extern Result vxr_OnSessionCreate(UInt64 xrSession);
            [DllImport(pluginName, EntryPoint = "vxr_OnAppSpaceChange")]
            internal static extern Result vxr_OnAppSpaceChange(UInt64 xrSpace);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionStateChange")]
            internal static extern Result vxr_OnSessionStateChange(int oldState, int newState);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionBegin")]
            internal static extern Result vxr_OnSessionBegin(UInt64 xrSession);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionEnd")]
            internal static extern Result vxr_OnSessionEnd(UInt64 xrSession);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionExiting")]
            internal static extern Result vxr_OnSessionExiting(UInt64 xrSession);
            [DllImport(pluginName, EntryPoint = "vxr_OnSessionDestroy")]
            internal static extern Result vxr_OnSessionDestroy(UInt64 xrSession);
            [DllImport(pluginName, EntryPoint = "vxr_OnInstanceDestroy")]
            internal static extern Result vxr_OnInstanceDestroy(UInt64 xrInstance);
            [DllImport(pluginName, EntryPoint = "vxr_OnEnvironmentBlendModeChange")]
            internal static extern Result vxr_OnEnvironmentBlendModeChange(int xrEnvironmentBlendMode);
            #endregion
        }

        private sealed partial class VXRVersion_0_6_0
        {
            #region// Version
            [DllImport(pluginName, EntryPoint = "vxr_GetNativeAPIVersion")]
            public static extern Result vxr_GetNativeAPIVersion(out int major, out int minor, out int patch);
            #endregion
        }
    }
}
