#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_XR_SDK
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.Management;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif

using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class VXRAsset : ScriptableObject
    {
        static string path = "VXR/VXRAsset";

        static VXRAsset _data;

        public static VXRAsset data
        {
            get
            {
                if (_data == null)
                {
                    _data = Resources.Load<VXRAsset>(path);
                    if (_data == null)
                    {
#if UNITY_EDITOR
                        _data = VXRAsset.CreateInstance<VXRAsset>();
                        string assetPath = "Assets/Resources/" + path + ".asset";
                        string filePath = assetPath.AssetPathToFilePath();
                        string dir = System.IO.Path.GetDirectoryName(filePath);
                        if (!System.IO.Directory.Exists(dir))
                        {
                            System.IO.Directory.CreateDirectory(dir);
                        }
                        AssetDatabase.CreateAsset(_data, assetPath);
#endif
                    }
                }
                return _data;
            }
        }

        public BuildReleaseMode ReleaseMode = VXRAsset.BuildReleaseMode.Release;

        public enum BuildReleaseMode
        {
            Debug,
            Release,
        }
    }
}


