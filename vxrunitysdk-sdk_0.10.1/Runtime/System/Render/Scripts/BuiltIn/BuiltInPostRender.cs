using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.vivo.render
{

    public class BuiltInPostRender : MonoBehaviour
    {

        Material _builtInFadeInOutMat;

        Material builtInFadeInOutMat
        {
            get
            {
                if (_builtInFadeInOutMat == null)
                {
                    _builtInFadeInOutMat = new Material(Shader.Find("VXR/Pipeline/BuiltInFadeInOut"));
                }
                return _builtInFadeInOutMat;
            }
        }

        Mesh _fullScreenPlantMesh;

        Mesh fullScreenPlantMesh
        {
            get
            {
                if (_fullScreenPlantMesh == null)
                {
                    _fullScreenPlantMesh = Resources.Load<Mesh>("FBX/FullScreenPlantMesh");
                }
                return _fullScreenPlantMesh;
            }
        }

        CommandBuffer _drawBuffer;

        CommandBuffer drawBuffer
        {
            get
            {
                if (_drawBuffer == null)
                {
                    _drawBuffer = new CommandBuffer() { name = "BuiltInFadeInOutBuffer" };
                }
                return _drawBuffer;
            }
        }

        private void OnPostRender()
        {
            if (!UrpRenderAssetData.Data.IsOpenFadeInOut)
            {
                return;
            }
            builtInFadeInOutMat.SetColor(string.Intern("_BaseColor"), UrpRenderAssetData.Data.FadeInOutColor);
            builtInFadeInOutMat.SetTexture(string.Intern("_BaseMap"), UrpRenderAssetData.Data.FadeInOutBaseMap);
            drawBuffer.Clear();
            drawBuffer.DrawMesh(fullScreenPlantMesh, Matrix4x4.identity, builtInFadeInOutMat);
            Graphics.ExecuteCommandBuffer(drawBuffer);
        }

    }

}
