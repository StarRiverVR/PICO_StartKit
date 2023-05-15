using UnityEngine;
using UnityEditor;

public class ChangeMaterial : EditorWindow
{
    [SerializeField] private Material pre_material;
    [SerializeField] private Material material;

    [MenuItem("Tools/ChangeMaterial")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ChangeMaterial>();
    }
    public void OnGUI()
    {
        this.pre_material = (Material)EditorGUILayout.ObjectField("PreMaterial", this.pre_material, typeof(Material), false);
        this.material = (Material)EditorGUILayout.ObjectField("Material", this.material, typeof(Material), false);
        // (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (GUILayout.Button("Relayout"))
        {
            foreach (var gameObj in Selection.gameObjects)
            {

                foreach (var renderer in gameObj.GetComponentsInChildren<MeshRenderer>())
                {
                    Debug.Log(renderer.gameObject.name);
                    Debug.Log(renderer.sharedMaterial?.name);

                    if(this.pre_material == null && renderer.sharedMaterial == null ){
                        renderer.sharedMaterial = this.material;
                    }
                    else if(this.pre_material == null) continue;
                    else if (renderer.sharedMaterial.name == this.pre_material.name)
                        renderer.sharedMaterial = this.material;
                }
            }
        }
        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }

}