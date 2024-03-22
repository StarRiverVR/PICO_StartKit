using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static void SetFoveationRenderingLevel(FoveationLevel level)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("FFR Not Supported");
#else
            var ret = VXRVersion_0_0_2.vxr_SetFoveationRenderingLevel(level);
            if (ret != Result.Success)
            {
                VLog.Warning($"[VXRPlugin] vxr_SetFoveationRenderingLevel Fail {ret}");
            }
#endif
        }

        public static FoveationLevel GetFoveationRenderingLevel()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("FFR Not Supported");
            return FoveationLevel.None;
#else
            var ret = VXRVersion_0_0_2.vxr_GetFoveationRenderingLevel(out FoveationLevel level);
            if (ret != Result.Success)
            {
                VLog.Warning($"[VXRPlugin] vxr_GetFoveationRenderingLevel Fail {ret}");
            }

            return level;
#endif
        }

        public static void SetFoveationRenderingDynamic(bool use)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("FFR Not Supported");
#else
            var dynamic = use ? FoveationDynamic.LevelEnabled : FoveationDynamic.Disabled;
            var ret = VXRVersion_0_0_2.vxr_SetFoveationRenderingDynamic(dynamic);
            if (ret != Result.Success)
            {
                VLog.Warning($"[VXRPlugin] vxr_SetFoveationRenderingDynamic Fail {ret}");
            }

#endif
        }

        public static bool GetFoveationRenderingDynamic()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("FFR Not Supported");
            return false;
#else
            var ret = VXRVersion_0_0_2.vxr_GetFoveationRenderingDynamic(out FoveationDynamic dynamic);
            if (ret != Result.Success)
            {
                VLog.Warning($"[VXRPlugin] vxr_GetFoveationRenderingDynamic Fail {ret}");
            }

            return dynamic == FoveationDynamic.LevelEnabled;
#endif
        }
    }
}
