using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, "dat")]
public class DatImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {

        var dat = new TextAsset(File.ReadAllText(ctx.assetPath));

        ctx.AddObjectToAsset("main", dat);
        ctx.SetMainObject(dat);
    }
}