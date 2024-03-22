#if UNITY_EDITOR

#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features;
using com.vivo.codelibrary;
#if USING_XR_MANAGEMENT
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
#endif

namespace com.vivo.openxr
{
    [InitializeOnLoad]
    static class VXRAPPEditorInitializeStatic
    {
        static VXRAPPEditorInitializeStatic()
        {
#if USING_XR_SDK
            EditorApplication.update += Update;
#else
            DebugLog.Info("XR SDK Not Using !");
#endif
        }

        static bool isFirst = false;

        static void Update()
        {
            if (!isFirst)
            {
                isFirst = true;
                VXRAPPEditorInitialize.FirstInitialize();
            }
            VXRAPPEditorInitialize.UpdateInitialize();
        }

    }

    public class VXRAPPEditorInitialize
    {
        [RuntimeInitializeOnLoadMethod]
        public static void FirstInitialize()
        {
#if !USING_XR_MANAGEMENT
            DebugLog.Exception(new SystemException("XR Plugin Manager not open !"));
            return;
#endif
            List<VXRLoader> loadersList = AssetDatabase.FindAssets($"t: {nameof(VXRLoader)}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<VXRLoader>).ToList();
            if (loadersList.Count > 1)
            {
                VLog.Info($"VivoOpenXRLoader count greater than 1, count = {loadersList.Count}");
                for (int i = 0; i < loadersList.Count; ++i)
                {
                    VXRLoader loader = loadersList[i];
                    VLog.Info($"VivoOpenXRLoader index {i}:{AssetDatabase.GetAssetPath(loader)}");
                }
            }

            VXRRecommendedConfig.SetRequired();
            LoaderFresh();
        }

        public static void UpdateInitialize()
        {
            //AutoEnabledXRFeature();
        }

        static void LoaderFresh()
        {
#if !USING_XR_MANAGEMENT
            return;
#endif
            LoaderFresh(VXRCommon.CurrentSelectedVivoOpenXRFeature, VXRCommon.SelectedBuildTargetGroup);
            LoaderFresh(VXRCommon.CurrentActiveVivoOpenXRFeature, VXRCommon.ActiveBuildTargetGroup);
        }

        static void LoaderFresh(VXRFeature vivoOpenXRFeature, BuildTargetGroup targetGroup)
        {

            XRGeneralSettings settings = VXRCommon.GetXRGeneralSettingsForBuildTarget(targetGroup, true);
            if (settings == null)
            {
                VLog.Exception(new SystemException("Could not find XR Plugin Manager settings"));
                return;
            }
            //

            List<VXRLoader> loadersList = AssetDatabase.FindAssets($"t: {nameof(VXRLoader)}").Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<VXRLoader>).ToList();
            VXRLoader vivoOpenXRLoader = null;
            if (loadersList.Count > 0)
            {
                vivoOpenXRLoader = loadersList[0];
                if (loadersList.Count > 1)
                {
                    VLog.Info($"VivoOpenXRLoader count greater than 1, count = {loadersList.Count}");
                    for (int i = 0; i < loadersList.Count; ++i)
                    {
                        VXRLoader loader = loadersList[i];
                        VLog.Info($"VivoOpenXRLoader index {i}:{AssetDatabase.GetAssetPath(loader)}");
                    }
                }
            }
            else
            {
                if (!Directory.Exists(VXRPath.VivoOpenXRFileDir))
                {
                    Directory.CreateDirectory(VXRPath.VivoOpenXRFileDir);
                }
                vivoOpenXRLoader = ScriptableObject.CreateInstance<VXRLoader>();
                AssetDatabase.CreateAsset(vivoOpenXRLoader, VXRLoader.AssetPath);
                VLog.Info($"Create VivoOpenXRLoader:{VXRLoader.AssetPath}");
            }
            //
            if (vivoOpenXRFeature == null || !vivoOpenXRFeature.enabled)
            {
                settings.Manager.TryRemoveLoader(vivoOpenXRLoader);
            }
            else
            {
                settings.Manager.TryAddLoader(vivoOpenXRLoader);
            }
        }

        /// <summary>
        /// VivoOpenXRFeature Auto Enabled
        /// </summary>
        static void AutoEnabledXRFeature()
        {
            AutoEnabledXRFeature(VXRCommon.CurrentSelectedVivoOpenXRFeature);
            AutoEnabledXRFeature(VXRCommon.CurrentActiveVivoOpenXRFeature);
        }

        static void AutoEnabledXRFeature(VXRFeature vivoOpenXRFeature)
        {
            if (vivoOpenXRFeature != null && !vivoOpenXRFeature.enabled)
            {
                if (VXRCommon.UnityRunningInBatchmode)
                {
                    vivoOpenXRFeature.enabled = true;
                    VLog.Info("VivoOpenXRFeature is enabled !");
                }
                else
                {
                    bool result =
                        EditorUtility.DisplayDialog("Enable VivoOpenXRFeature",
                            "VivoOpenXRFeature must be enabled in OpenXR Feature Groups. Do you want to enable it now?",
                            "Enable", "Cancel");
                    if (result)
                    {
                        vivoOpenXRFeature.enabled = true;
                        VLog.Info("VivoOpenXRFeature is enabled !");
                    }
                }
            }
        }

    }

}

#endif


