using UnityEngine.XR.OpenXR.NativeTypes;

namespace com.vivo.openxr {

    public static class VXRPassthrough
    {
        /// <summary>
        /// 透视通道混合模式
        /// </summary>
        public enum PassthroughBlendMode
        {          
            /// <summary>
            /// 不透明模式
            /// </summary>
            Opaque = XrEnvironmentBlendMode.Opaque,
            /// <summary>
            /// 附加模式
            /// </summary>
            Additive = XrEnvironmentBlendMode.Additive,
            /// <summary>
            /// 透明混合模式
            /// </summary>
            AlphaBlend = XrEnvironmentBlendMode.AlphaBlend,
        }

        /// <summary>
        /// 设置透视通道开关，可用于开启或关闭VST
        /// </summary>
        /// <param name="isEnable">true：开启 false：关闭</param>
        public static void SetPassthroughEnable(bool isEnable)
        {
            if (isEnable)
            {
                VXRPlugin.SetPassthroughBlendMode((VXRPlugin.PassthroughBlendMode)PassthroughBlendMode.AlphaBlend);
            }
            else
            {
                VXRPlugin.SetPassthroughBlendMode((VXRPlugin.PassthroughBlendMode)PassthroughBlendMode.Opaque);
            }
        }

        /// <summary>
        /// 设置透视通道混合模式
        /// </summary>
        /// <param name="mode">混合模式类型，Opaque：不透明模式 Additive：附加模式 AlphaBlend：透明混合模式</param>
        public static void SetPassthroughBlendMode(PassthroughBlendMode mode)
        {
            VXRPlugin.SetPassthroughBlendMode((VXRPlugin.PassthroughBlendMode)mode);
        }

    }

}

