using System;
using com.vivo.codelibrary;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace com.vivo.editor
{
    public class ImportPackageHelp
    {
        public static void ImportPackage(string packageName, string packageVersion)
        {
            UnityEditor.PackageManager.PackageInfo packageInfo =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{packageName}");
            if (packageInfo == null)
            {
                UnityEditor.PackageManager.Client.Add($"{packageName}@{packageVersion}");
            }
            else
            {
                VLog.Info(
                    $"存在Package path={packageInfo.assetPath},name={packageInfo.name},version={packageInfo.version}");
            }
        }

        
        public static void ImportPackageSample(string packageName, string sampleName)
        {
            UnityEditor.PackageManager.PackageInfo packageInfo =
                UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{packageName}");
            if (packageInfo!=null)
            {
                var packageSamples = Sample.FindByPackage(packageName, String.Empty);
                if (packageSamples == null)
                {
                    VLog.Error($"Couldn't find sample of the {packageName} for import the{sampleName};aborting.");
                    return;
                }

                foreach (var packageSample in packageSamples)
                {
                    if (packageSample.displayName != sampleName)
                    {
                        continue;
                    }

                    if (!packageSample.isImported)
                    {
                        packageSample.Import(Sample.ImportOptions.OverridePreviousImports);
                    }

                    break;
                }
            }
          
        }
    }
}
