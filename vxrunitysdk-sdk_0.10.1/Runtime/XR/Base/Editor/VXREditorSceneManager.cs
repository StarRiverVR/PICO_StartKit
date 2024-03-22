using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.vivo.openxr;
using com.vivo.codelibrary;
#if USING_RENDER_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.Rendering;

namespace com.vivo.openxr
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class VXREditorSceneManager
    {
#if UNITY_EDITOR

        public static bool ScenesDataChanged = false;

        static VXREditorSceneManager()
        {
            UnityEditor.EditorApplication.hierarchyChanged -= HierarchyChanged;
            UnityEditor.EditorApplication.hierarchyChanged += HierarchyChanged;

            UnityEditor.EditorApplication.update -= Update;
            UnityEditor.EditorApplication.update += Update;
        }

        static void Update()
        {
            if (ScenesDataChanged)
            {
                ScenesDataChanged = false;
                if (!Application.isPlaying)
                {
                    SaveScene();
                }
            }
        }

        static long saveMS;

        static void SaveScene()
        {
            saveMS = TimeHelper.GetTimeStamp_MS();
            SaveThread();
        }

        static bool isSaveThread = false;

        static void SaveThread()
        {
            if (isSaveThread)
            {
                return;
            }
            isSaveThread = true;
            DelayFunHelper.DelayRun(SaveThreadDelay, null, null, 1.5f);
        }

        static void SaveThreadDelay()
        {
            long curSaveMS = TimeHelper.GetTimeStamp_MS();
            if ((curSaveMS - saveMS) > 1500)
            {
                UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
                string assetPath = scene.path;
                if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets/"))
                {
                    UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                }
                isSaveThread = false;
            }
            else
            {
                DelayFunHelper.DelayRun(SaveThreadDelay, null, null, 1.5f);
            }
        }

        static void HierarchyChanged()
        {
            if (!Application.isPlaying)
            {
                //VXRUIItemFresh = true;
                //VXRUIPanel[] vxrUIPanels = VXRUIPanel.FindObjectsOfType<VXRUIPanel>();
                //Debug.Log(vxrUIPanels.Length);
            }
        }

#endif
    }
}


