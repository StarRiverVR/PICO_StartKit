using UnityEngine;
using UnityEditor;
public class CurvedUI : EditorWindow
{
    [SerializeField] private float distance;

    [SerializeField] private float baseValue = 0;

    [SerializeField] private string axis = "y";

    [SerializeField] private string baseName;

    [MenuItem("Tools/CurvedUI")]
    static void CreateReLayout()
    {
        EditorWindow.GetWindow<CurvedUI>();
    }

    private bool isLoaded = false;
    private void OnGUI()
    {
        if (!isLoaded)
        {
            baseName = Selection.gameObjects[0].name;
            this.isLoaded = true;
        }


        baseName = EditorGUILayout.TextField("baseName", baseName);
        baseValue = EditorGUILayout.FloatField("baseValue", baseValue);
        axis = EditorGUILayout.TextField("axis", axis);
        distance = EditorGUILayout.FloatField("Distance", distance);

        if (GUILayout.Button("Relayout"))
        {
            var selection = Selection.gameObjects;

            foreach (var rootObj in selection)
            {
                var  y = rootObj.transform.position.y;
                rootObj.transform.LookAt(new Vector3(0,y,0), new Vector3(0,1,0));
                rootObj.transform.Rotate(0,180,0);
                

            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}