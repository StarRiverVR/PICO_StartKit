using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ChangeFont : EditorWindow
{

    [SerializeField] private Font font;

    [MenuItem("Tools/ChangeFont")]
    static void CreateReLayout()
    {
        EditorWindow.GetWindow<ChangeFont>();
    }

    private void OnGUI()
    {
        font = (Font)EditorGUILayout.ObjectField(font, typeof(Font), true, GUILayout.MinWidth(100f));
        if (GUILayout.Button("ChangeFont"))
        {
            var tArray = Resources.FindObjectsOfTypeAll(typeof(Text));
            for (int i = 0; i < tArray.Length; i++)
            {
                Text t = tArray[i] as Text;
                // Commit changes , Without this code ,unity You won't notice any changes in the editor , At the same time, the changes will not be saved 
                Undo.RecordObject(t, t.gameObject.name);
                if (font)
                    t.font = font;
            }

        }

        GUI.enabled = false;
    }
}