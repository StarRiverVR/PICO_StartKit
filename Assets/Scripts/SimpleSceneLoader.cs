using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleSceneLoader : MonoBehaviour
{
    // 在 Unity Editor 中，你可以将你想要加载的场景拖到这个数组中
    public string[] scenesToLoad;

    void Start()
    {
        foreach (string scene in scenesToLoad)
        {
            StartCoroutine(LoadSceneAndDisableCamera(scene));
        }
    }

    IEnumerator LoadSceneAndDisableCamera(string sceneName)
    {
        // 异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 找到新场景中的所有相机并禁用它们
        foreach (Camera camera in FindObjectsOfType<Camera>())
        {
            Debug.Log("camera scnenName: " + camera.gameObject.scene.name + " camera: " + camera.gameObject.name);
            if (camera.gameObject.scene.name == sceneName)
            {
                camera.gameObject.SetActive(false);
            }
        }
    }
}
