#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || (UNITY_ANDROID && !UNITY_EDITOR))
#define VXR_UNSUPPORTED_PLATFORM
#endif

using com.vivo.codelibrary;
using System;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {

        /// <summary>
        /// SDK当前版本。
        /// 当SDK代码发生变化时增加。用于开放接口获取，与package.json中的verion版本对应。
        /// </summary>
        public static readonly Version s_sdkVersion = new Version(0, 10, 1);

        /// <summary>
        /// 初始化默认版本。
        /// </summary>
        public static readonly Version s_versionZero = new Version(0, 0, 0);
        /// <summary>
        /// 当前SDK支持的最高API版本号
        /// </summary>
        private static Version s_nativeSupportPluginVersion = new Version(0, 0, 0);
        public static Version s_supportPluginVersion
        {
            get
            {
                if (s_nativeSupportPluginVersion <= s_versionZero)
                {
                    s_nativeSupportPluginVersion = GetNativeAPIVersion();
                    VLog.Info($"Native API Version = {s_nativeSupportPluginVersion}");
                }

                return s_nativeSupportPluginVersion;
            }
        }
    }
}