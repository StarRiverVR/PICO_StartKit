using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.codelibrary;

namespace com.vivo.render
{
    public class UrpRenderAssetData : ScriptableObject
    {
        static UrpRenderAssetData data;

        public static UrpRenderAssetData Data
        {
            get
            {
                if (data==null)
                {
                    data = Resources.Load<UrpRenderAssetData>("AssetData/UrpRenderAssetData");
                }
                if (data==null)
                {
                    data = CreateInstance<UrpRenderAssetData>();
                    DataInit(data);
#if UNITY_EDITOR
                    string assetPath = "Assets/Resources/AssetData/UrpRenderAssetData.asset";
                    string filePath = assetPath.AssetPathToFilePath();
                    string dir = System.IO.Path.GetDirectoryName(filePath);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    UnityEditor.AssetDatabase.CreateAsset(data, assetPath);
                    UnityEditor.AssetDatabase.Refresh();
                    data = Resources.Load<UrpRenderAssetData>("AssetData/UrpRenderAssetData");
                    //


#endif
                }

                return data;
            }
        }

        static void DataInit(UrpRenderAssetData data)
        {
            data.FadeInOutColor = new Color(0,0,0,1);
        }

        public bool IsOpenFadeInOut = false;

        public Color FadeInOutColor ;

        public Texture2D FadeInOutBaseMap;

    }
}



