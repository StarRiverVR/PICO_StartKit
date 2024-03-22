using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.vivo.openxr;
using UnityEngine.SceneManagement;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class SceneSelectPanel : MonoBehaviour
{
    public List<SceneData> SceneList = new List<SceneData>();

    public Button QuitAppButton;

    public Text LogText;

    private void Start()
    {
        for (int i = 0; i < SceneList.Count; ++i)
        {
            SceneData sceneData = SceneList[i];
            sceneData.LoadButton.onClick.AddListener(()=> {
                SceneDataButtonClick(sceneData);
            });
        }

        QuitAppButton.onClick.AddListener(QuitAppButtonClick);
    }

    private void Update()
    {
#if UNITY_EDITOR
        EditorFresh();
#endif
    }

    void SceneDataButtonClick(SceneData sceneData)
    {
        SceneManager.LoadScene(sceneData.SceneName);
    }

    void QuitAppButtonClick()
    {
        Application.Quit();
    }

#if UNITY_EDITOR

    void EditorFresh()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < SceneList.Count; ++i)
            {
                SceneData sceneData = SceneList[i];
                if (sceneData != null && sceneData.SceneAsset != null)
                {
                    sceneData.AssetPath = AssetDatabase.GetAssetPath(SceneList[i].SceneAsset.GetInstanceID());
                    sceneData.SceneName = SceneList[i].SceneAsset.name;
                }
            }

            int addCount = 0;
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < SceneList.Count; ++i)
            {
                SceneData sceneData = SceneList[i];
                if (sceneData != null && sceneData.SceneAsset != null)
                {
                    sceneData.IsFind = false;
                    for (int j = 0; j < scenes.Length; ++j)
                    {
                        EditorBuildSettingsScene editorBuildSettingsScene = scenes[j];
                        if (editorBuildSettingsScene.path.CompareTo(sceneData.AssetPath) == 0)
                        {
                            sceneData.IsFind = true;
                            break;
                        }
                    }
                    if (!sceneData.IsFind)
                    {
                        addCount++;
                    }
                }
            }

            if (addCount > 0)
            {
                int index = scenes.Length;
                System.Array.Resize<EditorBuildSettingsScene>(ref scenes, scenes.Length + addCount);
                for (int i = 0; i < SceneList.Count; ++i)
                {
                    SceneData sceneData = SceneList[i];
                    if (!sceneData.IsFind)
                    {
                        EditorBuildSettingsScene newSetData = new EditorBuildSettingsScene(sceneData.AssetPath, true);
                        scenes[index] = newSetData;
                        index++;
                    }
                }
                EditorBuildSettings.scenes = scenes;
                AssetDatabase.Refresh();
            }
        }
    }

#endif

    [Serializable]
    public class SceneData
    {
#if UNITY_EDITOR

        public SceneAsset SceneAsset;

#endif

        public Button LoadButton;

        [HideInInspector]
        public string AssetPath;
        
        public string SceneName;

        public bool IsFind;
    }
}


