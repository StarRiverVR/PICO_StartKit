using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
    public class DebugLogAsset : ScriptableObject
    {
        static string path = "Debug/DebugLogAsset";

        static DebugLogAsset _data;

        public static DebugLogAsset data
        {
            get
            {
                if (_data == null)
                {
                    _data = Resources.Load<DebugLogAsset>(path);
                    if (_data == null)
                    {
#if UNITY_EDITOR
                        _data = DebugLogAsset.CreateInstance<DebugLogAsset>();
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

        public List<string> ips = new List<string>();

        public int Port = 0;
    }
}


