#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using com.vivo.codelibrary;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.render
{
    [ExcludeFromPreset]
    [Tooltip("VXRRenderFeature")]
    public class UrpRenderFeature : ScriptableRendererFeature
    {

        FadeInOut fadeInOut;

        public override void Create()
        {
            fadeInOut = new FadeInOut();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(fadeInOut);
        }

    }

#if UNITY_EDITOR

    [UnityEditor.InitializeOnLoad]
    public class UrpRenderFeatureEditor
    {
        static UrpRenderFeatureEditor()
        {
            UnityEditor.EditorApplication.delayCall -= DoSomethingPrepare;
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;

            UnityEditor.EditorApplication.update -= Update;
            UnityEditor.EditorApplication.update += Update;
        }

        static int UpdateCount = 0;

        static void Update()
        {
            UpdateCount++;
            if (UpdateCount>=300)
            {
                UpdateCount = 0;
                DoSomethingPrepare();
            }
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
               InitUniversalRenderPipelineAsset(GetCurPipelineAsset(GetBuildTargetGroupName(BuildTarget.StandaloneWindows64)));
               InitUniversalRenderPipelineAsset(GetCurPipelineAsset(GetBuildTargetGroupName(BuildTarget.Android)));
            }
        }

        static void InitUniversalRenderPipelineAsset(List<UniversalRenderPipelineAsset> res)
        {
            List<UniversalRenderPipelineAsset> tempList = new List<UniversalRenderPipelineAsset>();
            for (int i=0;i< res.Count;++i)
            {
                UniversalRenderPipelineAsset data = res[i];
                if (data!=null && !tempList.Contains(data))
                {
                    tempList.Add(data);
                    FieldInfo fieldInfo = data.GetType().GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance);
                    ScriptableRendererData[] rendererDataList = fieldInfo.GetValue(data) as ScriptableRendererData[];

                    UniversalRendererData universalRendererData = null;
                    for (int j=0;j< rendererDataList.Length;++j)
                    {
                        ScriptableRendererData renderData = rendererDataList[j];
                        if (renderData is UniversalRendererData)
                        {
                            universalRendererData = renderData as UniversalRendererData;
                            break;
                        }
                    }

                    if (universalRendererData!=null)
                    {
                        Type universalRendererDataType = universalRendererData.GetType();
                        FieldInfo rendererFeaturesFieldInfo = universalRendererDataType.GetField("m_RendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
                        List<ScriptableRendererFeature> rendererFeatures = (List<ScriptableRendererFeature>)rendererFeaturesFieldInfo.GetValue(universalRendererData);

                        bool find = false;
                        for (int z=0;z< rendererFeatures.Count;++z)
                        {
                            if (rendererFeatures[z] is UrpRenderFeature)
                            {
                                find = true;
                            }
                        }

                        if (!find)
                        {
                            Type featureType = typeof(UrpRenderFeature);
                            ScriptableObject component = CreateInstance(featureType.Name);
                            component.name = $"{featureType.Name}";
                            Undo.RegisterCreatedObjectUndo(component, "Add Renderer Feature");

                            if (EditorUtility.IsPersistent(universalRendererData))
                            {
                                AssetDatabase.AddObjectToAsset(component, universalRendererData);
                            }

                            MethodInfo methodInfo = universalRendererDataType.GetMethod("ValidateRendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
                            ScriptableRendererFeature addData = component as ScriptableRendererFeature;
                            rendererFeatures.Add(addData);
                            methodInfo.Invoke(universalRendererData,null);

                            EditorUtility.SetDirty(universalRendererData);
                            AssetDatabase.Refresh();

                        }

                    }
                }
            }
        }

        static ScriptableObject CreateInstance(string className)
        {
            return ScriptableObject.CreateInstance(className);
        }

        static string GetBuildTargetGroupName(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneLinux64:
                    {
                        return "Windows";
                    }
                case BuildTarget.Android:
                    {
                        return "Android";
                    }
                case BuildTarget.iOS:
                    {
                        return "iOS";
                    }
            }
            return null;
        }

        static List<UniversalRenderPipelineAsset> GetCurPipelineAsset(string buildTargetGroupName)
        {
            List<RenderPipelineAsset> renderPipelineAssets = new List<RenderPipelineAsset>();
            QualitySettings.GetAllRenderPipelineAssetsForPlatform(buildTargetGroupName, ref renderPipelineAssets);
            if (renderPipelineAssets==null || renderPipelineAssets.Count==0)
            {
                return null;
            }
            List<UniversalRenderPipelineAsset> res = new List<UniversalRenderPipelineAsset>();
            for (int i=0;i< renderPipelineAssets.Count;++i)
            {
                if (renderPipelineAssets[i] is UniversalRenderPipelineAsset)
                {
                    UniversalRenderPipelineAsset resData = renderPipelineAssets[i] as UniversalRenderPipelineAsset;
                    res.Add(resData);
                }
                else
                {
                    res.Add(null);
                }
            }
            return res;
        }

    }

#endif

}


