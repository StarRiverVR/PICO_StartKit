using System;

namespace com.vivo.openxr
{
    public class VXRSystem
    {
        /// <summary>
        /// 获取native api 版本
        /// </summary>
        /// <returns></returns>
        public static Version GetNativeAPIVersion()
        {
            return VXRPlugin.GetNativeAPIVersion();
        }
        /// <summary>
        /// 获取sdk版本
        /// </summary>
        /// <returns></returns>
        public static Version GetSDKVersion()
        {
            Version maxPluginVersion = VXRPlugin.GetMaxPluginVersion();
            return VXRPlugin.s_sdkVersion < maxPluginVersion ? maxPluginVersion : VXRPlugin.s_sdkVersion;
        }
    } 
}