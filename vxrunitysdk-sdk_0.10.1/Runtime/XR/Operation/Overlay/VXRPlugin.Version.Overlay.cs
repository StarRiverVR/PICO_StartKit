using System;
using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_5_0
        {
            [DllImport(pluginName, EntryPoint = "vxr_CreateCompositionLayer")]
            public static extern Result vxr_CreateCompositionLayer(UInt32 _layerId, OverlayShape shap);

            [DllImport(pluginName, EntryPoint = "vxr_InitializeCompositionLayer")]
            public static extern Result vxr_InitializeCompositionLayer(OverlayCreateParams createParams);

            [DllImport(pluginName, EntryPoint = "vxr_GetCompositionLayerSwapchainImgCount")]
            public static extern UInt32 vxr_GetCompositionLayerSwapchainImgCount(UInt32 _layerId);

            [DllImport(pluginName, EntryPoint = "vxr_GetCompositionLayerSwapchainTexturePtr")]
            public static extern UInt32 vxr_GetCompositionLayerSwapchainTexturePtr(UInt32 _layerId, int eyeId, int imgIndex);

            [DllImport(pluginName, EntryPoint = "vxr_SubmitCompositionLayerParams")]
            public static extern Result vxr_SubmitCompositionLayerParams(UInt32 _layerId, OverlaySubmitParams submitParams);

            [DllImport(pluginName, EntryPoint = "vxr_AcquireCompositionLayerSwapchainImage")]
            public static extern Result vxr_AcquireCompositionLayerSwapchainImage(UInt32 _layerId);

            [DllImport(pluginName, EntryPoint = "vxr_GetSwapchainAcquireIndex")]
            public static extern UInt32 vxr_GetSwapchainAcquireIndex(UInt32 _layerId, int eyeId);

            [DllImport(pluginName, EntryPoint = "vxr_ResetCompositionLayer")]
            public static extern Result vxr_ResetCompositionLayer(UInt32 _layerId, OverlayShape shap);

            [DllImport(pluginName, EntryPoint = "vxr_DestroyCompositionLayer")]
            public static extern Result vxr_DestroyCompositionLayer(UInt32 _layerId);

            [DllImport(pluginName, EntryPoint = "vxr_GetAndroidSurfaceObj")]
            public static extern IntPtr vxr_GetAndroidSurfaceObj(UInt32 _layerId);

            [DllImport(pluginName, EntryPoint = "vxr_AndroidSurfaceEvent")]
            public static extern IntPtr vxr_AndroidSurfaceEvent();
        }
    }
}
