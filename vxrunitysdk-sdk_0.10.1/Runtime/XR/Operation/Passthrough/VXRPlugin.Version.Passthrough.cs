using UnityEngine.XR.OpenXR.NativeTypes;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_9_0
        {
            public static void SetPassthroughBlendMode(VXRPlugin.PassthroughBlendMode blendMode)
            {
                VXRFeature.ChangeEnvironmentBlendMode((XrEnvironmentBlendMode)blendMode);
            }

        }
    }
    

}
