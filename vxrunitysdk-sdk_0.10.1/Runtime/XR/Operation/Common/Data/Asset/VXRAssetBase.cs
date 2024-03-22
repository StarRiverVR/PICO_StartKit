using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class VXRAssetBase<T> : ScriptableObject where T : VXRAssetBase<T>
    {
        internal static string GetAssetPath(string dir)
        {
            return VXRPath.VivoOpenXRUnityDir + dir + typeof(T).Name + ".asset";
        }

        internal static string GetResourcesPath(string dir)
        {
            return dir + typeof(T).Name;
        }

        internal static string GetResourcesAssetPath(string dir)
        {
            return VXRPath.VivoOpenXRUnityDir + "Resources/" + dir + typeof(T).Name + ".asset";
        }

        static T _data;

        public static T GetData(ref string dir,bool isResources=false)
        {
            if(_data != null)
            {


                return _data;
            }
            if (isResources)
            {
                _data = Resources.Load<T>(GetResourcesPath(dir));
            }
            else
            {

#if UNITY_EDITOR
                _data = AssetDatabase.LoadAssetAtPath<T>(GetAssetPath(dir));
#endif
            }

            if (_data ==null)
            {
                _data = CreateInstance<T>();
                _data.OnCreateInstance();
#if UNITY_EDITOR
                string filePath = null;
                string saveDir = null;
                string savePath = null;
                if (isResources)
                {
                    savePath = GetResourcesAssetPath(dir);
                }
                else
                {
                    savePath = GetAssetPath(dir);
                }
                filePath = savePath.AssetPathToFilePath();
                saveDir = System.IO.Path.GetDirectoryName(filePath);
                if (!System.IO.Directory.Exists(saveDir))
                {
                    System.IO.Directory.CreateDirectory(saveDir);
                }
                AssetDatabase.CreateAsset(_data, savePath);
                AssetDatabase.Refresh();
                _data = AssetDatabase.LoadAssetAtPath<T>(savePath);
                Debug.Log("ScriptableObject Path:" + savePath);
#endif
            }
            return _data;
        }

        protected virtual void OnCreateInstance()
        {

        }

        public void SaveOnEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}


