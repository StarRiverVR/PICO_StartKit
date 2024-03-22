using com.vivo.codelibrary;
namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static void SetPassthroughBlendMode(VXRPlugin.PassthroughBlendMode blendModel)
        {

#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported Passthrough");
#else
            VXRVersion_0_9_0.SetPassthroughBlendMode(blendModel);
#endif
        }
    }
}
