#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
#endif
#if USING_XR_MANAGEMENT && UNITY_EDITOR
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
#endif
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using com.vivo.codelibrary;

#if USING_RENDER_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.Rendering;

namespace com.vivo.openxr
{
    public class VXRCommon
    {
        /// <summary>
        /// Build Mode
        /// </summary>
        public static VXRAsset.BuildReleaseMode ReleaseMode
        {
            get
            {
                return VXRAsset.data == null ? VXRAsset.BuildReleaseMode.Release : VXRAsset.data.ReleaseMode;
            }
        }

        public static bool isUnityXROn
        {
            get
            {
#if USING_XR_UNITYXR
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isUnityURPOn
        {
            get
            {
#if USING_RENDER_URP
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isUnityXRManagementOn
        {
            get
            {
#if USING_XR_MANAGEMENT
                return true;
#else
                return false;
#endif
            }
        }

        public static bool isDeviceSupported
        {
            get
            {
#if !USING_XR_UNITYXR || !USING_XR_MANAGEMENT
                return false;
#endif

#if UNITY_ANDROID

            #if UNITY_EDITOR
                return true;
            #else
                return VXRNativeMethods.GetIsSupportedDevice();
            #endif

#endif
            return false;
            }
        }

#if UNITY_EDITOR

        static BuildTargetGroup[] targetGroups;

        public static BuildTargetGroup[] TargetGroups
        {
            get
            {
                if (targetGroups==null)
                {
                    targetGroups = new BuildTargetGroup[] { BuildTargetGroup.Android };
                }
                return targetGroups;
            }
        }

        static BuildTarget[] buildTargets;

        public static BuildTarget[] BuildTargets
        {
            get
            {
                if (buildTargets == null)
                {

                    buildTargets = new BuildTarget[TargetGroups.Length];
                    for (int i=0;i< buildTargets.Length;++i)
                    {
                        buildTargets[i] = TargetGroups[i].ToBuildTarget();
                    }
                }
                return buildTargets;
            }
        }

        /// <summary>
        /// PlayerSet , Switch Platform
        /// </summary>
        public static BuildTarget ActiveBuildTarget
        {
            get
            {
                return EditorUserBuildSettings.activeBuildTarget;
            }
        }

        /// <summary>
        /// PlayerSet ,Build Settings/Switch Platform
        /// </summary>
        public static BuildTargetGroup ActiveBuildTargetGroup
        {
            get
            {
                return BuildPipeline.GetBuildTargetGroup(ActiveBuildTarget);
            }
        }

        /// <summary>
        /// UI Select ,Project Settings/ XR Plug-in Management
        /// </summary>
        public static BuildTargetGroup SelectedBuildTargetGroup
        {
            get
            {
                return EditorUserBuildSettings.selectedBuildTargetGroup;
            }
        }

#endif

#if USING_XR_MANAGEMENT && UNITY_EDITOR
        public static XRGeneralSettings GetXRGeneralSettingsForBuildTarget(BuildTargetGroup buildTargetGroup, bool create)
        {
            XRGeneralSettings settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            if (!create || settings != null)
            {
                return settings;
            }

            // Method A : Reflection
            // Hardcoded name of the method we're trying to call
            // May require maintenance here to align with newer versions of the UnityEngine.XR.Management plugin
            const string getOrCreateMethodName = "GetOrCreate";
            MethodInfo getOrCreateMethod = typeof(XRGeneralSettingsPerBuildTarget).GetMethod(getOrCreateMethodName,
                BindingFlags.Static | BindingFlags.NonPublic);
            getOrCreateMethod?.Invoke(null, null);

            EditorBuildSettings.TryGetConfigObject<XRGeneralSettingsPerBuildTarget>(XRGeneralSettings.k_SettingsKey, out XRGeneralSettingsPerBuildTarget buildTargetSettings);
            if (buildTargetSettings != null && !buildTargetSettings.HasManagerSettingsForBuildTarget(buildTargetGroup))
            {
                buildTargetSettings.CreateDefaultManagerSettingsForBuildTarget(buildTargetGroup);
            }

            settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTargetGroup);
            return settings;
        }

        public static VXRFeature GetVivoOpenXRFeature(BuildTargetGroup targetGroup)
        {
            bool findBuildTargetGroup = false;
            BuildTargetGroup[] targetGroups = VXRCommon.TargetGroups;
            for (int i = 0; i < targetGroups.Length; ++i)
            {
                BuildTargetGroup buildTargetGroup = targetGroups[i];
                if (targetGroup == buildTargetGroup)
                {
                    findBuildTargetGroup = true;
                    break;
                }
            }

            if (findBuildTargetGroup)
            {
                OpenXRFeature openXRFeature = FeatureHelpers.GetFeatureWithIdForBuildTarget(targetGroup, VXRFeature.featureId);
                if (openXRFeature != null)
                {
                    return openXRFeature as VXRFeature;
                }
            }

            return null;
        }

        public static VXRFeature CurrentSelectedVivoOpenXRFeature
        {
            get
            {
                BuildTargetGroup activeBuildTargetGroup = VXRCommon.SelectedBuildTargetGroup;
                return GetVivoOpenXRFeature(activeBuildTargetGroup);
            }
        }

        public static VXRFeature CurrentActiveVivoOpenXRFeature
        {
            get
            {
                BuildTargetGroup activeBuildTargetGroup = VXRCommon.ActiveBuildTargetGroup;
                return GetVivoOpenXRFeature(activeBuildTargetGroup);
            }
        }

        public static VXRControllerProfile GetVivoOpenXRControllerProfile(BuildTargetGroup targetGroup)
        {
            OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (null == settings)
                return null;
            foreach (OpenXRFeature feature in settings.GetFeatures<OpenXRInteractionFeature>())
            {
                if (feature is VXRControllerProfile)
                {
                    return feature as VXRControllerProfile;
                }
            }
            return null;
        }

        public static VXRControllerProfile CurrentSelectedVivoOpenXRControllerProfile
        {
            get
            {
                return GetVivoOpenXRControllerProfile(VXRCommon.SelectedBuildTargetGroup);
            }
        }

        public static VXRControllerProfile CurrentActiveVivoOpenXRControllerProfile
        {
            get
            {
                return GetVivoOpenXRControllerProfile(VXRCommon.ActiveBuildTargetGroup);
            }
        }

#endif

        public static bool UnityRunningInBatchmode
        {
            get
            {
                if (System.Environment.CommandLine.Contains("-batchmode"))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// ColorSpace.Linear
        /// </summary>
        /// <param name="colorSpace"></param>
        public static void SetColorSpace(ColorSpace colorSpace)
        {
#if UNITY_EDITOR
            PlayerSettings.colorSpace = colorSpace;
#endif
        }

#region//URP

        public static bool IsURP
        {
            get
            {
                bool isURP = false;
#if USING_RENDER_URP
                if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline is UniversalRenderPipelineAsset)
                {
                    isURP = true;
                }
                if (!isURP && GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
                {
                    isURP = true;
                }
#endif

                return isURP;
            }
        }

#if USING_RENDER_URP
        public static UniversalRenderPipelineAsset UrpAsset
        {
            get
            {
                if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline is UniversalRenderPipelineAsset)
                {
                    return QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
                }
                if (GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
                {
                    return GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                }
                return null;
            }
        }
#endif

#endregion

#region//MSAA


        public static UnityEngine.Rendering.MSAASamples MsaaSample
        {
            get
            {
                return (UnityEngine.Rendering.MSAASamples)MsaaSampleCount;
            }
            set
            {
                MsaaSampleCount = (int)value;
            }
        }

        public static int MsaaSampleCount
        {
            get
            {
#if USING_RENDER_URP
                UniversalRenderPipelineAsset urpAssetData = VXRCommon.UrpAsset;
                if (urpAssetData != null)
                {
                    return urpAssetData.msaaSampleCount;
                }
#endif
                return QualitySettings.antiAliasing;
            }
            set
            {
                bool change = false;
                UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = null;
#if !UNITY_EDITOR
                if (Application.isPlaying)
                {
                    xrDisplaySubsystem = VXRCommon.GetDisplaySubsystem() as UnityEngine.XR.XRDisplaySubsystem;
                }
#endif
#if USING_RENDER_URP
                UniversalRenderPipelineAsset urpAssetData = VXRCommon.UrpAsset;
                if (urpAssetData != null && urpAssetData.msaaSampleCount != value)
                {
                    urpAssetData.msaaSampleCount = value;
                    if (urpAssetData.msaaSampleCount == (int)UnityEngine.Rendering.MSAASamples.None)
                    {
                        Camera.main.allowMSAA = false;
                    }
                    else
                    {
                        Camera.main.allowMSAA = true;
                    }
                    if (xrDisplaySubsystem!=null)
                    {
                        xrDisplaySubsystem.SetMSAALevel(urpAssetData.msaaSampleCount);
                    }
                    change = true;
                }
                else if (QualitySettings.antiAliasing != value)
                {
                    QualitySettings.antiAliasing = value;
                    if (QualitySettings.antiAliasing == (int)UnityEngine.Rendering.MSAASamples.None)
                    {
                        Camera.main.allowMSAA = false;
                    }
                    else
                    {
                        Camera.main.allowMSAA = true;
                    }
                    if (xrDisplaySubsystem!=null)
                    {
                        xrDisplaySubsystem.SetMSAALevel(QualitySettings.antiAliasing);
                    }
                    change = true;
                }
#else
                if (QualitySettings.antiAliasing != value)
                {
                    QualitySettings.antiAliasing = value;
                    if (QualitySettings.antiAliasing == (int)UnityEngine.Rendering.MSAASamples.None)
                    {
                        Camera.main.allowMSAA = true;
                    }
                    else
                    {
                        Camera.main.allowMSAA = false;
                    }
                    if (xrDisplaySubsystem!=null)
                    {
                        xrDisplaySubsystem.SetMSAALevel(QualitySettings.antiAliasing);
                    }
                    change = true;
                }
#endif

#if UNITY_EDITOR
                if (change && !Application.isPlaying)
                {
                    VXREditorSceneManager.ScenesDataChanged = true;
                }
#endif
            }
        }

        #endregion


#region//render sacle

        public static float EyeTextureResolutionScale
        {
            get
            {
#if USING_RENDER_URP
                if (UrpAsset!=null)
                {
                    return UrpAsset.renderScale;
                }
#endif
                return UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
            }
            set
            {
                float res = value;
                res = Mathf.Clamp(res,0.5f,2f);
                if (UnityEngine.XR.XRSettings.eyeTextureResolutionScale!= res)
                {
                    UnityEngine.XR.XRSettings.eyeTextureResolutionScale = res;
                }
#if USING_RENDER_URP
                if (UrpAsset != null)
                {
                    if (UrpAsset.renderScale!= res)
                    {
                        UrpAsset.renderScale = res;
                    }
                }
#endif
            }
        }

        public static float RenderViewportScale
        {
            get
            {
                return UnityEngine.XR.XRSettings.renderViewportScale;
            }
            set
            {
                if (UnityEngine.XR.XRSettings.renderViewportScale!=value)
                {
                    UnityEngine.XR.XRSettings.renderViewportScale = value;
                }
            }
        }

        #endregion

#region //Fade In Out

        public static void StartFadeInOutAnim(float fadeInOutStartDelay, float fadeInOutAnimLenght,
            Color fadeInOutStartColor, Color fadeInOutEndColor, Texture2D fadeInOutTexture)
        {
            com.vivo.render.BuiltInPostRender builtInFadeInOut = Camera.main.gameObject.GetComponent<com.vivo.render.BuiltInPostRender>();
            if (builtInFadeInOut == null)
            {
                builtInFadeInOut = Camera.main.gameObject.AddComponent<com.vivo.render.BuiltInPostRender>();
            }
            com.vivo.render.FadeInOutAnim.Instance.StartFadeInOutAnim(fadeInOutStartDelay, fadeInOutAnimLenght,
             fadeInOutStartColor, fadeInOutEndColor, fadeInOutTexture);
        }

#endregion

#region//权限

        /// <summary>
        /// Android Returns whether the <see cref="permission"/> has been granted.
        /// 眼动权限: com.oculus.permission.EYE_TRACKING 
        /// </summary>
        /// <param name="permissionStr"><see cref="Permission"/> to be checked.</param>
        public static bool IsPermissionGranted(string permissionStr)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(permissionStr);
#else
            return true;
#endif
        }

        /// <summary>
        /// Android 权限申请
        /// </summary>
        /// <param name="permissionStrs"></param>
        /// <param name="callBack"></param>
        public static void PermissionRequest(string[] permissionStrs, System.Action<string> callBack)
        {
            UnityEngine.Android.Permission.RequestUserPermissions(permissionStrs, BuildPermissionCallbacks(callBack));
        }

        private static UnityEngine.Android.PermissionCallbacks BuildPermissionCallbacks(System.Action<string> callBack)
        {
            var permissionCallbacks = new UnityEngine.Android.PermissionCallbacks();
            permissionCallbacks.PermissionDenied += permissionId =>
            {
                VLog.Info($"Permission {permissionId} was denied.");
            };
            permissionCallbacks.PermissionDeniedAndDontAskAgain += permissionId =>
            {
                VLog.Info(
                    $"Permission {permissionId} was denied and blocked from being requested again.");
            };
            permissionCallbacks.PermissionGranted += permissionId =>
            {
                VLog.Info($"Permission {permissionId} was granted.");
                callBack?.Invoke(permissionId);
            };
            return permissionCallbacks;
        }

#endregion

#region//pakage

#if UNITY_EDITOR

        /// <summary>
        ///  "com.unity.xr.openxr"
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static bool IsPackageInstalled(string packageName)
        {
            ListRequest _packageManagerListRequest = Client.List(offlineMode: false, includeIndirectDependencies: true);
            bool res = (_packageManagerListRequest.Status == StatusCode.Success) && (_packageManagerListRequest.Result?.Any(package => package.name == packageName) ?? false);
            return res;
        }

#endif

#endregion

#region//子系统

        static List<UnityEngine.XR.XRDisplaySubsystem> s_displaySubsystems;

        public static IntegratedSubsystem GetDisplaySubsystem()
        {
#if USING_XR_SDK
            if (s_displaySubsystems==null)
            {
                s_displaySubsystems = new List<UnityEngine.XR.XRDisplaySubsystem>();
            }
            SubsystemManager.GetInstances(s_displaySubsystems);
            if (s_displaySubsystems.Count>0)
            {
                return s_displaySubsystems[0];
            }
            //foreach (UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem in displays)
            //{
            //    if (xrDisplaySubsystem.SubsystemDescriptor.id.CompareTo(VXRNameConfig.DisplaySubsystemDescriptorsId) == 0 && xrDisplaySubsystem.running)
            //    {
            //        m_Display = xrDisplaySubsystem;
            //        return m_Display;
            //    }
            //}
#endif
            VLog.Error("No active Vivo display subsystem was found.");
            return null;
        }

        static List<UnityEngine.XR.XRDisplaySubsystemDescriptor> s_displaySubsystemDescriptors;

        public static UnityEngine.XR.XRDisplaySubsystemDescriptor GetCurrentDisplaySubsystemDescriptor()
        {
            if (s_displaySubsystemDescriptors == null)
                s_displaySubsystemDescriptors = new List<UnityEngine.XR.XRDisplaySubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(s_displaySubsystemDescriptors);
            if (s_displaySubsystemDescriptors.Count > 0)
                return s_displaySubsystemDescriptors[0];
            return null;
        }

        static List<UnityEngine.XR.XRInputSubsystem> s_inputSubsystems;

        public static IntegratedSubsystem GetInputSubsystem()
        {
#if USING_XR_SDK
            if (s_inputSubsystems==null)
            {
                s_inputSubsystems = new List<UnityEngine.XR.XRInputSubsystem>();
            }
            SubsystemManager.GetInstances(s_inputSubsystems);
            if (s_inputSubsystems.Count>0)
            {
                return s_inputSubsystems[0];
            }
            //foreach (UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem in displays)
            //{
            //    if (xrDisplaySubsystem.SubsystemDescriptor.id.CompareTo(VXRNameConfig.InputSubsystemDescriptorsId) == 0 && xrDisplaySubsystem.running)
            //    {
            //        m_Input = xrDisplaySubsystem;
            //        return m_Input;
            //    }
            //}
#endif
            VLog.Error("No active Vivo input subsystem was found.");
            return null;
        }

#endregion

#region//帧耗时  GPU  CPU

        /// <summary>
        /// Reports the time the application spent on the GPU last frame in seconds.
        /// </summary>
        public static float LastFramGPUTimeFromDisplaySubsystem
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;
                    float val;
                    xrDisplaySubsystem.TryGetAppGPUTimeLastFrame(out val);
                    return val;
                }
#endif
                return default;
            }
        }

        /// <summary>
        /// Reports the time the application spen on the GPU last frame in seconds.
        /// </summary>
        public static float LastFramGPUTimeFromXRStats
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.appgputime", out val);
                    return val;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Reports the time the application spent on the CPU last frame in seconds.
        /// </summary>
        public static float LastFramCPUTimeFromXRStats
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.appcputime", out val);
                    return val;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Reports the time the compositor spent on the GPU last frame in seconds.
        /// </summary>
        public static float LastFramCompositorGPUTimeFromDisSubsystem
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    xrDisplaySubsystem.TryGetCompositorGPUTimeLastFrame(out val);
                    return val;
                }
#endif

                return default;

            }
        }

        /// <summary>
        /// Reports the time the compositor spent on the GPU last frame in seconds.
        /// </summary>
        public static float LastFramCompositorGPUTimeFromXRStats
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.compositorgputime", out val);
                    return val;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Reports the time the compositor spent on the CPU last frame in seconds.
        /// </summary>
        public static float LastFramCompositorCPUTimeFromXRStats
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.compositorcputime", out val);
                    return val;
                }
#endif
                return default;
            }
        }

#endregion

#region//利用率 GPU cpu

        /// <summary>
        /// Reports the GPU utilization as a value from 0.0 - 1.0.
        /// </summary>
        public static float GPUUtilization
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.gpuutil", out val);
                    return val;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Reports the average CPU utilization as a value from 0.0 - 1.0.
        /// </summary>
        public static float CPUUtilizationAverage
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.cpuutilavg", out val);
                    return val;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Reports the worst CPU utilization as a value from 0.0 - 1.0.
        /// </summary>
        public static float CPUUtilizationWorst
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.cpuutilworst", out val);
                    return val;
                }
#endif
                return default;

            }
        }

#endregion

#region//时钟频率 GPU CPU

        /// <summary>
        /// Reports the CPU clock frequency
        /// </summary>
        public static float CPUClockFrequency
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;
                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.cpuclockfreq", out val);
                    return val;
                }
#endif
                return default;
            }
        }

        /// <summary>
        /// Reports the GPU clock frequency
        /// </summary>
        public static float GPUClockFrequency
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;
                    float val;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "perfmetrics.gpuclockfreq", out val);
                    return val;
                }
#endif
                return default;

            }
        }

#endregion

#region//丢弃帧数

        /// <summary>
        /// 获取 XR 插件报告的丢弃的帧数。
        /// </summary>
        public static int DroppedFrameCount
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    int val;
                    xrDisplaySubsystem.TryGetDroppedFrameCount(out val);
                    return val;
                }
#endif
                return default;

            }
        }

#endregion

#region//绘制次数

        /// <summary>
        /// 获取 XR 插件报告的当前帧绘制到设备中的次数
        /// </summary>
        public static int FramePresentCount
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    int val;
                    xrDisplaySubsystem.TryGetFramePresentCount(out val);
                    return val;
                }
#endif
                return default;

            }
        }

#endregion

#region//绘制延迟

        /// <summary>
        /// Reports the latency from when the last predicted tracking information was queried by the application to the moment the middle scaline of the target frame is illuminated on the display. //帧延迟
        /// </summary>
        public static float MotionToPhoton
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;
                    float val;
                    bool bl = xrDisplaySubsystem.TryGetMotionToPhoton(out val);
                    return val;
                }
#endif
                return -1f;
            }
        }

        public static bool MotionToPhotonBool
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;
                    float val;
                    bool bl = xrDisplaySubsystem.TryGetMotionToPhoton(out val);
                    return bl;
                }
#endif
                return false;
            }
        }

#endregion

#region//电池

        /// <summary>
        /// Gets the current battery temperature in degrees Celsius. //电池温度 摄氏度
        /// </summary>
        public static float BatteryTemp
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float batteryTemp;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "batteryTemperature", out batteryTemp);
                    return batteryTemp;
                }
#endif
                return default;
            }
        }


        /// <summary>
        /// Gets the current available battery charge, ranging from 0.0 (empty) to 1.0 (full). //电池电量
        /// </summary>
        public static float BatteryLevel
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float batteryLevel;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "batteryLevel", out batteryLevel);
                    return batteryLevel;
                }
#endif
                return default;
            }
        }

#endregion

#region//性能选项

        /// <summary>
        /// If true, the system is running in a reduced performance mode to save power. //true:系统节能降低性能
        /// </summary>
        public static bool PowerSavingMode
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float powerSavingMode;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "powerSavingMode", out powerSavingMode);
                    return !(powerSavingMode == 0.0f);
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Returns the recommended amount to scale GPU work in order to maintain framerate. //返回推荐量缩放量 viewportScale and textureScale
        /// Can be used to adjust viewportScale and textureScale
        /// </summary>
        public static float AdaptivePerformanceScale
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float performanceScale;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "adaptivePerformanceScale", out performanceScale);
                    return performanceScale;
                }
#endif
                return default;

            }
        }

        /// <summary>
        /// Gets the current CPU performance level, integer in the range 0 - 3. //性能等级
        /// </summary>
        public static int CPULevel
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float cpuLevel;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "cpuLevel", out cpuLevel);
                    return (int)cpuLevel;
                }
#endif
                return default;
            }
        }

        /// <summary>
        /// Gets the current GPU performance level, integer in the range 0 - 3. //性能等级
        /// </summary>
        public static int GPULevel
        {
            get
            {
#if USING_XR_SDK
                IntegratedSubsystem integratedSubsystem = GetDisplaySubsystem();
                if (integratedSubsystem != null)
                {
                    UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem = (UnityEngine.XR.XRDisplaySubsystem)integratedSubsystem;

                    float gpuLevel;
                    UnityEngine.XR.Provider.XRStats.TryGetStat(xrDisplaySubsystem, "cpuLevel", out gpuLevel);
                    return (int)gpuLevel;
                }
#endif
                return default;
            }
        }

        #endregion
    }
}


