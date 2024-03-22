#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace com.vivo.codelibrary
{
    public class LayerHelper
    {

        /// <summary>
        /// 获得包含指定层的 CullingMask
        /// </summary>
        /// <returns></returns>
        public static int GetCullingMask(string[] layers)
        {
            return LayerMask.GetMask(layers);
        }

        /// <summary>
        /// 判断指定层是否在集合中
        /// </summary>
        /// <returns></returns>
        public static bool IsInCullingMask(string layerName, int cullingMask)
        {
            int layerNum = LayerMask.NameToLayer(layerName);
            int res = cullingMask & (1 << layerNum);
            if (res == 0)
            {
                //不包含
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 取消或者开启两个碰撞建的碰撞反应
        /// </summary>
        /// <param name="collider1"></param>
        /// <param name="collider2"></param>
        /// <param name="ignore"></param>
        public static void IgnoreCollision(Collider collider1, Collider collider2, bool ignore = true)
        {
            Physics.IgnoreCollision(collider1, collider2, ignore);
        }

        /// <summary>
        /// 勾选层级targeLayer
        /// </summary>
        /// <param name="cullingMask"></param>
        /// <param name="targeLayer"></param>
        /// <returns></returns>
        public static int AddLayer(int cullingMask, int targeLayer)
        {
            int res = (cullingMask | 1 << targeLayer);
            return res;
        }

        /// <summary>
        /// 取消勾选层级targeLayer
        /// </summary>
        /// <param name="cullingMask"></param>
        /// <param name="targeLayer"></param>
        /// <returns></returns>
        public static int RemoveLayer(int cullingMask, int targeLayer)
        {
            int res = (cullingMask & ~(1 << targeLayer));
            return res;
        }

        /// <summary>
        /// 获得层
        /// </summary>
        /// <param name="OpenLayer">开启的层</param>
        /// <param name="CloseLayer">关闭的层</param>
        /// <returns></returns>
        static LayerMask CutLayerCullingMask(List<string> OpenLayer, List<string> CloseLayer)
        {
            LayerMask mask = new LayerMask();
            for (int i = 0; i < OpenLayer.Count; ++i)
            {
                int layerNum = LayerMask.NameToLayer(OpenLayer[i]);
                mask = mask | (1 << layerNum);
            }
            for (int i = 0; i < CloseLayer.Count; ++i)
            {
                int layerNum = LayerMask.NameToLayer(CloseLayer[i]);
                mask = mask | (0 << layerNum);
            }
            return mask;
        }

        static SimpleListPool<List<string>, string> stringlistPool = new SimpleListPool<List<string>, string>();

        static LayerMask CutLayerMask(List<string> maskLayers)
        {
            List<string> OpenLayer = stringlistPool.Spawn();//开启的层
            OpenLayer.Add("Default");
            OpenLayer.Add("Terrain");
            List<string> CloseLayer = stringlistPool.Spawn();//关闭的层
            for (int i = 0; i < maskLayers.Count; i++)
            {
                CloseLayer.Add(maskLayers[i]);
            }
            LayerMask mask = new LayerMask();
            for (int i = 0; i < OpenLayer.Count; ++i)
            {
                int layerNum = LayerMask.NameToLayer(OpenLayer[i]);
                mask = mask | (1 << layerNum);
            }
            for (int i = 0; i < CloseLayer.Count; ++i)
            {
                int layerNum = LayerMask.NameToLayer(CloseLayer[i]);
                mask = mask | (0 << layerNum);
            }
            stringlistPool.Recycle(OpenLayer);
            stringlistPool.Recycle(CloseLayer);
            return mask;
        }


#if UNITY_EDITOR
        

        static void ReadTag()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "tags")
                {
                    var count = it.arraySize;

                    for (int i = 0; i < count; ++i)
                    {
                        var dataPoint = it.GetArrayElementAtIndex(i);
                        //Debug.Log(dataPoint.stringValue);
                    }
                }
            }
        }

        static void ReadSortingLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    var count = it.arraySize;
                    for (int i = 0; i < count; ++i)
                    {
                        var dataPoint = it.GetArrayElementAtIndex(i);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                //Debug.Log(dataPoint.stringValue);
                            }
                        }
                    }
                }
            }
        }

        static void ReadLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "layers")
                {
                    for (int i = 0; i < it.arraySize; ++i)
                    {
                        if (i == 3 || i == 6 || i == 7) continue;
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        //Debug.Log(dataPoint.stringValue);
                    }
                }
            }
        }

        public static void AddTag(string tag)
        {
            if (!isHasTag(tag))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "tags")
                    {
                        it.InsertArrayElementAtIndex(it.arraySize);
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                        dataPoint.stringValue = tag;
                        tagManager.ApplyModifiedProperties();
                        return;
                    }
                }
            }
        }

        static void AddSortingLayer(string sortingLayer)
        {
            if (!isHasSortingLayer(sortingLayer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "m_SortingLayers")
                    {
                        Debug.Log("SortingLayers" + it.arraySize);
                        it.InsertArrayElementAtIndex(it.arraySize);
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(it.arraySize - 1);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                dataPoint.stringValue = sortingLayer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        public static void AddLayer(string layer)
        {
            if (!isHasLayer(layer))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty it = tagManager.GetIterator();
                while (it.NextVisible(true))
                {
                    if (it.name == "layers")
                    {
                        for (int i = 0; i < it.arraySize; ++i)
                        {
                            if (i == 3 || i == 6 || i == 7) continue;
                            SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(dataPoint.stringValue))
                            {
                                dataPoint.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }

        static bool isHasTag(string tag)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; ++i)
            {
                if (UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
                    return true;
            }
            return false;
        }

        static bool isHasSortingLayer(string sortingLayer)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    for (int i = 0; i < it.arraySize; ++i)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                if (dataPoint.stringValue == sortingLayer) return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        static bool isHasLayer(string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; ++i)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                    return true;
            }
            return false;
        }
#endif
    }
}


#endif


