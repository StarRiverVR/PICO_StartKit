using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace com.vivo.render
{
    public class FadeInOut : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler;

        public FadeInOut()
        {
            this.renderPassEvent = RenderPassEvent.AfterRendering;
            m_ProfilingSampler = new ProfilingSampler("FadeInOut Pass");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (!UrpRenderAssetData.Data.IsOpenFadeInOut)
            {
                return;
            }
        }

        Material _fadeInOutMaterial;

        Material fadeInOutMaterial
        {
            get
            {
                if (_fadeInOutMaterial == null)
                {
                    _fadeInOutMaterial = new Material(Shader.Find("VXR/Pipeline/FadeInOut"));
                }
                return _fadeInOutMaterial;
            }
        }

        Color lastColor;

        Texture2D lastBaseMap;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!UrpRenderAssetData.Data.IsOpenFadeInOut)
            {
                return;
            }
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                if (lastColor!= UrpRenderAssetData.Data.FadeInOutColor)
                {
                    lastColor = UrpRenderAssetData.Data.FadeInOutColor;
                    fadeInOutMaterial.SetColor(UrpRenderUtil.BaseColorID, lastColor);
                }
                if (lastBaseMap!= UrpRenderAssetData.Data.FadeInOutBaseMap)
                {
                    lastBaseMap = UrpRenderAssetData.Data.FadeInOutBaseMap;
                    fadeInOutMaterial.SetTexture(UrpRenderUtil.BaseMapID, lastBaseMap);
                    if (lastBaseMap==null)
                    {
                        fadeInOutMaterial.DisableKeyword(string.Intern("_BASEMAP_ON"));
                    }
                    else
                    {
                        fadeInOutMaterial.EnableKeyword(string.Intern("_BASEMAP_ON"));
                    }
                }
                cmd.DrawMesh(UrpRenderUtil.FullScreenPlantMesh, Matrix4x4.identity, fadeInOutMaterial, 0, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {

        }

    }
}


