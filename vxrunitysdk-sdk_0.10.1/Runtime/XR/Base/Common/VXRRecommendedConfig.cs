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
#endif
#if USING_XR_MANAGEMENT && UNITY_EDITOR
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
#endif
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using com.vivo.codelibrary;
using UnityEngine.Rendering;

#if UNITY_EDITOR

namespace com.vivo.openxr
{
    /// <summary>
    /// 推荐配置
    /// </summary>
    public class VXRRecommendedConfig
    {

        #region//必选配置

        /// <summary>
        /// 必选配置
        /// </summary>
        public static void SetRequired()
        {
            SetColorSpace(ColorSpace.Linear);
            BuildSetXREyeTrackedFoveatedRendering();
            CloseGraphicsJobs();
        }

        /// <summary>
        /// ColorSpace.Linear
        /// </summary>
        /// <param name="colorSpace"></param>
        public static void SetColorSpace(ColorSpace colorSpace)
        {
            if (PlayerSettings.colorSpace!= colorSpace)
            {
                PlayerSettings.colorSpace = colorSpace;
            }
        }

        /// <summary>
        ///  USING_XR_EYE_TRACKED_FOVEATED_RENDERING [vivo.openxrsdk.runtime.asmdef]
        /// Need to use Vulkan for Graphics APIs, IL2CPP for scripting backend, and ARM64 for target architectures when using eye-tracked foveated rendering
        /// Set target architectures to ARM64, scripting backend to IL2CPP, and Graphics APIs to Vulkan for this build.
        /// </summary>
        public static void BuildSetXREyeTrackedFoveatedRendering()
        {
#if USING_XR_SDK && UNITY_2021_3_OR_NEWER && USING_XR_EYE_TRACKED_FOVEATED_RENDERING
            for (int i=0;i< VXRCommon.TargetGroups.Length;++i)
            {
                if (VXRCommon.TargetGroups[i]== BuildTargetGroup.Android)
                {
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
                }
                PlayerSettings.SetScriptingBackend(VXRCommon.TargetGroups[i], ScriptingImplementation.IL2CPP);
                if (VXRCommon.TargetGroups[i] == BuildTargetGroup.Android)
                {
                    PlayerSettings.SetGraphicsAPIs(VXRCommon.BuildTargets[i], new[] { GraphicsDeviceType.Vulkan });
                }else if (VXRCommon.TargetGroups[i] == BuildTargetGroup.Standalone)
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(VXRCommon.BuildTargets[i], false);
                    PlayerSettings.SetGraphicsAPIs(VXRCommon.BuildTargets[i], new[] { GraphicsDeviceType.Direct3D11 });
                }
            }
#endif
        }

        /// <summary>
        /// Disable Graphics Jobs  //是否产生负优化 待验证
        /// </summary>
        public static void CloseGraphicsJobs()
        {
#if USING_XR_SDK
            PlayerSettings.graphicsJobs = false;
#endif
        }

        #endregion

        #region//推荐配置

        /// <summary>
        /// 推荐配置
        /// </summary>
        public static void SetRecommended()
        {
            SetGraphicsAPIs();
        }

        public static void SetGraphicsAPIs()
        {
            for (int i = 0; i < VXRCommon.TargetGroups.Length; ++i)
            {
                if (VXRCommon.TargetGroups[i] == BuildTargetGroup.Android)
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(VXRCommon.BuildTargets[i], false);
                    PlayerSettings.SetGraphicsAPIs(VXRCommon.BuildTargets[i], new[] { GraphicsDeviceType.Vulkan });
                }
                else if (VXRCommon.TargetGroups[i] == BuildTargetGroup.Standalone)
                {
                    PlayerSettings.SetUseDefaultGraphicsAPIs(VXRCommon.BuildTargets[i], false);
                    PlayerSettings.SetGraphicsAPIs(VXRCommon.BuildTargets[i], new[] { GraphicsDeviceType.Direct3D11 });
                }
            }
        }

        #endregion

        #region//可选配置

        /// <summary>
        /// 可选配置
        /// </summary>
        public static void SetOptional()
        {
            //VXRCommon.ReleaseMode
            //   EditorUserBuildSettings.exportAsGoogleAndroidProject = previousExportAsGoogleAndroidProject;
            //    EditorUserBuildSettings.development
        }

        #endregion

    }
}

#endif

