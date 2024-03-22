using UnityEngine.XR.OpenXR.NativeTypes;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public enum PassthroughBlendMode
        {
            Opaque = XrEnvironmentBlendMode.Opaque,
            Additive = XrEnvironmentBlendMode.Additive,
            AlphaBlend = XrEnvironmentBlendMode.AlphaBlend,
        }
    }
}
