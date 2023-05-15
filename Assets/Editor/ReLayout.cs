using UnityEngine;
using UnityEditor;
public class ReLayout : EditorWindow
{
    [SerializeField] private float distance;

    [SerializeField] private float baseValue = 0;

    [SerializeField] private string axis = "z";

    [SerializeField] private string baseName;

    [MenuItem("Tools/ReLayout")]
    static void CreateReLayout()
    {
        EditorWindow.GetWindow<ReLayout>();
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
                for (var i = 0; i < rootObj.transform.childCount; i++)
                {
                    var childObj = rootObj.transform.GetChild(i);
                    var position = childObj.transform.localPosition;
                    if (axis == "x")
                    {
                        position.x = baseValue + i * distance;
                    }
                    if (axis == "y")
                    {
                        position.y = baseValue + i * distance;
                    }
                    if (axis == "z")
                    {
                        position.z = baseValue + i * distance;
                    }

                    childObj.transform.localPosition = position;

                    if(baseName != "-"){
                        childObj.name = baseName + i;
                    }

                    
                }

            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}