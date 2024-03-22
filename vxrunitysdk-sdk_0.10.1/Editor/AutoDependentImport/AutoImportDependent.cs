using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.vivo.editor
{
    /// <summary>
    /// 自动导入依赖Samples与依赖Package
    /// </summary>
    public class AutoImportDependent : AssetPostprocessor
    {
        static void AutoImport()
        {
            DependentConfig[] dConfigs = LoadScriptableObject<DependentConfig, ScriptableObject>();
            if (dConfigs.Length>0)
            {
                for (int i = 0; i < dConfigs.Length; i++)
                {
                    if (dConfigs[i].dependentPackages.Count>0)
                    {
                        foreach (var config in dConfigs[i].dependentPackages)
                        {
                            ImportPackageHelp.ImportPackage(config.PackageName,config.PackageVersion);
                        }
                    }

                    if (dConfigs[i].dependentSamples.Count>0)
                    {
                        foreach (var config in dConfigs[i].dependentSamples)
                        {
                            ImportPackageHelp.ImportPackageSample(config.PackageName,config.SampleName);
                        }
                    }
                }
            }
    
        }
        
        public static T[] LoadScriptableObject<T,I>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name).Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToList().FindAll(t => t is I).ToArray();
        }

        public static void OnPostprocessAllAssets(string[]importedAsset,string[] deletedAssets,string[] movedAssets,string[]movedFromAssetPaths)
        {
            AutoImport();
        }

    }
}