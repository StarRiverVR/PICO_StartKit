using System;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static Result CreateCompositionLayer(UInt32 layerId, OverlayShape shap)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return Result.Failure;
#else
            return VXRVersion_0_5_0.vxr_CreateCompositionLayer(layerId, shap);
#endif
        }

        public static Result InitializeCompositionLayer(OverlayCreateParams createParams)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return Result.Failure;
#else
            return VXRVersion_0_5_0.vxr_InitializeCompositionLayer(createParams);
#endif
        }

        public static UInt32 GetCompositionLayerSwapchainImgCount(UInt32 layerId)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_GetCompositionLayerSwapchainImgCount(layerId);
#endif
        }
        
        public static UInt32 GetCompositionLayerSwapchainTexturePtr(UInt32 layerId, int eyeId, int imgIndex)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_GetCompositionLayerSwapchainTexturePtr(layerId, eyeId, imgIndex);
#endif
        }
        
        public static Result SubmitCompositionLayerParams(UInt32 layerId, OverlaySubmitParams submitParams)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_SubmitCompositionLayerParams(layerId, submitParams);
#endif
        }

        public static Result AcquireCompositionLayerSwapchainImage(UInt32 layerId)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_AcquireCompositionLayerSwapchainImage(layerId);
#endif
        }
        
        public static UInt32 GetSwapchainAcquireIndex(UInt32 layerId, int eyeId)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_GetSwapchainAcquireIndex(layerId, eyeId);
#endif
        }

        public static Result ResetCompositionLayer(UInt32 layerId, OverlayShape shap)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_ResetCompositionLayer(layerId, shap);
#endif
        }

        public static Result DestroyCompositionLayer(UInt32 layerId)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return 0;
#else
            return VXRVersion_0_5_0.vxr_DestroyCompositionLayer(layerId);
#endif
        }

        public static IntPtr GetAndroidSurfaceObj(UInt32 layerId)
        {
#if VXR_UNSUPPORTED_PLATFORM
            return IntPtr.Zero;
#else
            return VXRVersion_0_5_0.vxr_GetAndroidSurfaceObj(layerId);
#endif
        }

        public static IntPtr AndroidSurfaceEvent()
        {
#if VXR_UNSUPPORTED_PLATFORM
            return IntPtr.Zero;
#else
            return VXRVersion_0_5_0.vxr_AndroidSurfaceEvent();
#endif
        }
    }
}
