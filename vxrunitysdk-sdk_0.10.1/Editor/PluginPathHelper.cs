using System.IO;
using UnityEditor;
using UnityEngine;


namespace com.vivo.editor
{
    // VXR SDK路径
    // 本文件路径必须在[SDK根目录/Editor/]目录下
    public class PluginPathHelper : ScriptableObject
    {
        // 获取SDK根目录绝对路径
        public static string GetUtilitiesRootPath()
        {
            var so = ScriptableObject.CreateInstance(typeof(PluginPathHelper));
            var script = MonoScript.FromScriptableObject(so);
            string assetPath = AssetDatabase.GetAssetPath(script);
            Debug.Log(assetPath);
            var editorDir = Directory.GetParent(assetPath);
            if (editorDir == null)
            {
                throw new DirectoryNotFoundException($"Unable to find parent directory of {assetPath}");
            }

            string editorPath = editorDir.FullName;

            var ovrDir = Directory.GetParent(editorPath);
            if (ovrDir == null)
            {
                throw new DirectoryNotFoundException($"Unable to find parent directory of {editorPath}");
            }
            return ovrDir.FullName;
        }
    }
}