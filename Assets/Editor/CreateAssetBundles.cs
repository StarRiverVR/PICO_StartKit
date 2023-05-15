using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        // string assetBundleDirectory = "Assets/StreamingAssets";
        // if(!Directory.Exists(assetBundleDirectory))
        // {
        //     Directory.CreateDirectory(assetBundleDirectory);
        // }
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, 
                                        BuildAssetBundleOptions.None, 
                                        BuildTarget.Android);
    }
}