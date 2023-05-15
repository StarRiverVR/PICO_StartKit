// PlayerPrefs.DeleteAll()

using UnityEngine;
using UnityEditor;
public class ClearUserData : EditorWindow
{
    [MenuItem("Tools/ClearUserData")]
    static void CreateReLayout()
    {
        EditorWindow.GetWindow<ClearUserData>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("ClearUserData"))
        {
           PlayerPrefs.DeleteAll();
        }

        GUI.enabled = false;
    }
}