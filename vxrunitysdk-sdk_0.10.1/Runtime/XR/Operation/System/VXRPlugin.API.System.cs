using com.vivo.codelibrary;
using System;
using System.Reflection;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public class UnityOpenXR
        {
            public static void SetPluginVersion()
            {
                VXRPlugin.SetPluginVersion();
            }

            public static IntPtr HookGetInstanceProcAddr(IntPtr func)
            {
#if VXR_UNSUPPORTED_PLATFORM
                return func;
#else
                return VXRVersion_0_0_2.vxr_HookGetInstanceProcAddr(func);
#endif
            }

            public static bool OnInstanceCreate(UInt64 xrInstance)
            {
#if VXR_UNSUPPORTED_PLATFORM
            return false;
#else
                Result result = VXRVersion_0_0_2.vxr_OnInstanceCreate(xrInstance);
                return result == Result.Success;
#endif
            }

            public static void OnSystemChange(ulong systemId)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSystemChange(systemId);
#endif
            }

            public static void OnSessionCreate(UInt64 xrSession)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionCreate(xrSession);
#endif
            }

            public static void OnAppSpaceChange(UInt64 xrSpace)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnAppSpaceChange(xrSpace);
#endif
            }

            public static void OnSessionStateChange(int oldState, int newState)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionStateChange(oldState, newState);
#endif
            }

            public static void OnSessionBegin(UInt64 xrSession)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionBegin(xrSession);
#endif
            }

            public static void OnSessionEnd(UInt64 xrSession)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionEnd(xrSession);
#endif
            }

            public static void OnSessionExiting(UInt64 xrSession)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionExiting(xrSession);
#endif
            }

            public static void OnSessionDestroy(UInt64 xrSession)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnSessionDestroy(xrSession);
#endif
            }

            public static void OnInstanceDestroy(UInt64 xrInstance)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnInstanceDestroy(xrInstance);
#endif
            }

            internal static void OnEnvironmentBlendModeChange(int xrEnvironmentBlendMode)
            {
#if !VXR_UNSUPPORTED_PLATFORM
                VXRVersion_0_0_2.vxr_OnEnvironmentBlendModeChange(xrEnvironmentBlendMode);
#endif
            }
        }

        #region// Version

        [AttributeUsage(AttributeTargets.Field)]
        public class PluginVersionAttribute : Attribute { }


        
        public static System.Version GetNativeAPIVersion()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Platform");
            return s_versionZero;
#else
            Result result = VXRVersion_0_6_0.vxr_GetNativeAPIVersion(out int major, out int minor, out int patch);
            if (result == Result.Success)
            {
                return new Version(major, minor, patch);
            }
            else
            {
                VLog.Error("Get Native API Version Fail");
                return s_versionZero;
            }
#endif
        }
        
        public static void SetPluginVersion()
        {

#if !VXR_UNSUPPORTED_PLATFORM
            Version version = GetMaxPluginVersion();
            if (version >= VXRVersion_0_0_0.version)
            {
                VXRVersion_0_0_0.vxr_SetPluginVersion(version.Major, version.Minor, version.Build);
                VLog.Info($" VXRPlugin->vxr_SetPluginVersion{version.ToString()}");
            }
#endif
        }
        
        public static Version GetMaxPluginVersion()
        {
            Assembly vxrPlugin = Assembly.GetAssembly(typeof(VXRPlugin));
            Type vxrPluginClass = vxrPlugin.GetType("com.vivo.openxr.VXRPlugin");
            Type[] clsassList = vxrPluginClass.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Static);
            Version latestVersion = new Version();
            foreach (var _class in clsassList)
            {
                System.Reflection.FieldInfo[] fields = _class.GetFields();
                System.Type attType = typeof(PluginVersionAttribute);
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].IsDefined(attType, false))
                    {
                        object value = fields[i].GetValue(null);
                        if (value is System.Version version)
                        {
                            if (latestVersion < version)
                            {
                                latestVersion = version;
                            }
                        }
                    }
                }
            }
            VLog.Info(" Max pluginVersion: " + latestVersion.ToString());
            return latestVersion;
        }
        #endregion
    }
}
