using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
using System.Xml;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif

namespace com.vivo.openxr
{

#if UNITY_EDITOR
    public class VXRFeatureEditorConfig
    {
        public const string OpenXrExtensionList =
            "XR_FB_display_refresh_rate " +
            "XR_FB_foveation " +
            "XR_FB_foveation_configuration " +
            "XR_FB_swapchain_update_state " +

            
            "XR_FB_foveation_vulkan " +
            "XR_KHR_vulkan_enable " +
            "XR_META_vulkan_swapchain_create_info " +

            "XR_MSFT_spatial_anchor " +
            "XR_MSFT_spatial_anchor_persistence " +

            //"XR_VARJO_foveated_rendering " +
            //"XR_FB_swapchain_update_state_vulkan " +
            "XR_FB_swapchain_update_state_opengl_es " +

            "XR_FB_composition_layer_alpha_blend " +
            //"XR_KHR_D3D11_enable " +
            //"XR_OCULUS_common_reference_spaces " +
            //"XR_EXT_performance_settings " +
            //"XR_FB_composition_layer_image_layout " +
            "XR_KHR_android_surface_swapchain " +
            "XR_FB_android_surface_swapchain_create " +
            "XR_KHR_composition_layer_color_scale_bias " +
            //"XR_FB_color_space " +
            //"XR_EXT_hand_tracking " +
            "XR_FB_swapchain_update_state " +
            "XR_FB_swapchain_update_state_opengl_es " +
            //"XR_FB_swapchain_update_state_vulkan " +
            "XR_FB_composition_layer_alpha_blend " +
            "XR_KHR_composition_layer_depth " +
            "XR_KHR_composition_layer_cylinder " +
            //"XR_KHR_composition_layer_cube " +
            //"XR_KHR_composition_layer_equirect2 " +
            //"XR_KHR_convert_timespec_time " +
            //"XR_KHR_visibility_mask " +
            //"XR_FB_render_model " +
            //"XR_FB_spatial_entity " +
            //"XR_FB_spatial_entity_query " +
            //"XR_FB_spatial_entity_storage " +
            //"XR_META_performance_metrics " +
            //"XR_FB_scene " +
            //"XR_FB_spatial_entity_container " +
            //"XR_FB_scene_capture " +
            //"XR_FB_face_tracking " +
            //"XR_FB_eye_tracking " +
            //"XR_FB_keyboard_tracking " +
            //"XR_FB_passthrough " +
            //"XR_FB_triangle_mesh " +
            //"XR_FB_passthrough_keyboard_hands " +
            //"XR_OCULUS_audio_device_guid " +
            //"XR_FB_common_events " +
            //"XR_FB_space_warp " +
            //"XR_FB_hand_tracking_capsules " +
            //"XR_FB_hand_tracking_mesh " +
            //"XR_FB_hand_tracking_aim " +
            //"XR_FB_touch_controller_pro " +
            //"XR_FB_touch_controller_proximity " +
            ""
            ;
    }
#endif
    /// <summary>
    /// Enables the vivo mobile OpenXR Loader for Android.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(
        UiName = "vivo Support",
        Desc = "Necessary to deploy a vivo compatible app.",
        Company = "vivo",
        DocumentationLink = "https://www.vivo.com",
        OpenxrExtensionStrings = VXRFeatureEditorConfig.OpenXrExtensionList,
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.Android },
        CustomRuntimeLoaderBuildTargets = new[] { BuildTarget.Android },
        FeatureId = featureId
    )]
#endif
    public class VXRFeature : OpenXRFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.vivo.openxr.feature.loader";

        private const string version = "0.2.4";

        protected override void OnSubsystemCreate()
        {
            UnityEngine.Debug.Log("Create Subsystem version:" + version);
            base.OnSubsystemCreate();
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> rules, BuildTargetGroup targetGroup)
        {
            //rules.Add(new ValidationRule(this)
            //{
            //    message = "Only the Vivo XR Test Controller Profile is supported right now.",
            //    checkPredicate = () =>
            //    {
            //        OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            //        if (null == settings)
            //            return false;

            //        bool touchFeatureEnabled = false;
            //        bool otherInteractionFeatureEnabled = false;
            //        foreach (OpenXRFeature feature in settings.GetFeatures<OpenXRInteractionFeature>())
            //        {
            //            if (feature.enabled)
            //            {
            //                if (feature is VXRControllerProfile)
            //                    touchFeatureEnabled = true;
            //                else
            //                    otherInteractionFeatureEnabled = true;
            //            }
            //        }
            //        bool enb = touchFeatureEnabled && !otherInteractionFeatureEnabled;
            //        if (!enb)
            //        {
            //            UnityEngine.Debug.LogError(" Only \"XR Plug-in Management/OpenXR/Interaction Profiles / Vivo XR Test Controller Profile\" should be selected !");
            //        }
            //        return enb;
            //    },
            //    fixIt = () =>
            //    {
            //        OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            //        if (null == settings)
            //            return;

            //        foreach (OpenXRFeature feature in settings.GetFeatures<OpenXRInteractionFeature>())
            //        {
            //            feature.enabled = (feature is VXRControllerProfile);
            //        }
            //    },
            //    error = true,
            //});
        }
#endif

        #region// OpenXR
        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            Debug.Log($"[VXRFeature] HookGetInstanceProcAddr: {func}");
            VXRPlugin.UnityOpenXR.SetPluginVersion();

            return VXRPlugin.UnityOpenXR.HookGetInstanceProcAddr(func);
        }
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            Debug.Log($"[VXRFeature] OnInstanceCreate: {xrInstance}");
            bool result = VXRPlugin.UnityOpenXR.OnInstanceCreate(xrInstance);
            if (!result)
            {
                Debug.LogError($"[VXRFeature] OnInstanceCreate returned an error[{result}]");
            }
            return result;
        }
        protected override void OnSystemChange(ulong systemId)
        {
            Debug.Log($"[VXRFeature] OnSystemChange: {systemId}");
            VXRPlugin.UnityOpenXR.OnSystemChange(systemId);

        }
        protected override void OnSessionCreate(ulong xrSession)
        {
            Debug.Log($"[VXRFeature] OnSessionCreate: {xrSession}");
            VXRPlugin.UnityOpenXR.OnSessionCreate(xrSession);
        }
        protected override void OnAppSpaceChange(ulong xrSpace)
        {
            Debug.Log($"[VXRFeature] OnAppSpaceChange: {xrSpace}");
            VXRPlugin.UnityOpenXR.OnAppSpaceChange(xrSpace);
        }
        protected override void OnSessionStateChange(int oldState, int newState)
        {
            Debug.Log($"[VXRFeature] OnSessionStateChange: {oldState} -> {newState}");
            VXRPlugin.UnityOpenXR.OnSessionStateChange(oldState, newState);
        }
        protected override void OnSessionBegin(ulong xrSession)
        {
            Debug.Log($"[VXRFeature] OnSessionBegin: {xrSession}");
            VXRPlugin.UnityOpenXR.OnSessionBegin(xrSession);
        }
        protected override void OnSessionEnd(ulong xrSession)
        {
            Debug.Log($"[VXRFeature] OnSessionEnd: {xrSession}");
            VXRPlugin.UnityOpenXR.OnSessionEnd(xrSession);
        }
        protected override void OnSessionExiting(ulong xrSession)
        {
            Debug.Log($"[VXRFeature] OnSessionExiting: {xrSession}");
            VXRPlugin.UnityOpenXR.OnSessionExiting(xrSession);
        }
        protected override void OnSessionDestroy(ulong xrSession)
        {
            Debug.Log($"[VXRFeature] OnSessionDestroy: {xrSession}");
            VXRPlugin.UnityOpenXR.OnSessionDestroy(xrSession);
        }
        protected override void OnInstanceDestroy(ulong xrInstance)
        {
            Debug.Log($"[VXRFeature] OnInstanceDestroy: {xrInstance}");
            VXRPlugin.UnityOpenXR.OnInstanceDestroy(xrInstance);
        }
        protected override void OnEnvironmentBlendModeChange(XrEnvironmentBlendMode xrEnvironmentBlendMode)
        {
            Debug.Log($"[VXRFeature] OnEnvironmentBlendModeChange: {xrEnvironmentBlendMode}");
            VXRPlugin.UnityOpenXR.OnEnvironmentBlendModeChange((int)xrEnvironmentBlendMode);
        }
        #endregion

        public static void ChangeEnvironmentBlendMode(XrEnvironmentBlendMode model)
        {
            Debug.LogWarning("SnapdragonOpenXRLoader feature CheckEnvironmentBlendMode  " + model);
            SetEnvironmentBlendMode(model);            
        }

    }
}
