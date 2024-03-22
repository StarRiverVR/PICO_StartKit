
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.OpenXR.Features;

namespace com.vivo.editor
{
    /// <summary>
    /// 未选择 vivo feature 时 不导入vivo库
    /// </summary>
    public class VXRGradleGeneration : IPostGenerateGradleAndroidProject, IPreprocessBuildWithReport
    {
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            string vxrRootPath = PluginPathHelper.GetUtilitiesRootPath();

            var vxrFeature = FeatureHelpers.GetFeatureWithIdForBuildTarget(report.summary.platformGroup, com.vivo.openxr.VXRFeature.featureId);

            var importers = PluginImporter.GetAllImporters();
            foreach (var importer in importers)
            {
                if (!importer.GetCompatibleWithPlatform(report.summary.platform))
                    continue;
                string fullAssetPath = Path.Combine(Directory.GetCurrentDirectory(), importer.assetPath);
#if UNITY_EDITOR_WIN
                fullAssetPath = fullAssetPath.Replace("/", "\\");
#endif
                UnityEngine.Debug.Log(fullAssetPath);
                if (fullAssetPath.StartsWith(vxrRootPath) && fullAssetPath.EndsWith(".aar"))
                {
                    importer.SetIncludeInBuildDelegate(path => vxrFeature.enabled);
                }
            }
        }
    }
}
