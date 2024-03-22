#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || (UNITY_ANDROID && !UNITY_EDITOR))
#define VXR_UNSUPPORTED_PLATFORM
#endif

using AOT;
using System.Runtime.InteropServices;

namespace com.vivo.codelibrary
{
    internal class VLogPlugin
    {
        private const string pluginName = "VXRPlugin";
        public enum Result
        {
            /// Success
            Success = 0,
            Success_EventUnavailable = 1,
            Success_Pending = 2,
            /// Failure
            Failure = -1000,
            Failure_Unsupported = -1001,

            Error_NotInit = -9000,
        }


        public delegate void NativeLogCallback(string baseTag, string message, int level);
        #region// native log 接口
        [DllImport(pluginName, EntryPoint = "vxr_SetNativeLogConfig")]
        public static extern Result vxr_SetNativeLogConfig(bool debug, bool cache, string tag, int level);

        [DllImport(pluginName, EntryPoint = "vxr_SetNativeLogCallback")]
        public static extern Result vxr_SetNativeLogCallback(NativeLogCallback nativeLog);
        #endregion

        public static void SetNativeLogConfig(bool debug, bool cache, string tag, int level)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Error("[Log] Not Supported");
#else
            Result ret = vxr_SetNativeLogConfig(debug, cache, tag, level);
            if (ret != Result.Success)
            {
                VLog.Warning("[Log] SetNativeLogConfig fail");
            }

            ret = vxr_SetNativeLogCallback(GetNativeLogCallback);
            if (ret != Result.Success)
            {
                VLog.Warning("[Log] SetNativeLogCallback fail");
            }
#endif
        }
        [MonoPInvokeCallback(typeof(NativeLogCallback))]
        internal static void GetNativeLogCallback(string baseTag, string message, int level)
        {
            if (message == "")
            {
                return;
            }
            DebugLogCache.CacheLog(baseTag, message, level);
        }
    }
}