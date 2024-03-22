
#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
#if USING_RENDER_URP
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.Rendering;

internal class VivoStandardShaderGUI : ShaderGUI
{

    #region//URP

    public static bool IsURP
    {
        get
        {
            bool isURP = false;
#if USING_RENDER_URP
            if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline is UniversalRenderPipelineAsset)
            {
                isURP = true;
            }
            if (!isURP && GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                isURP = true;
            }
#endif

            return isURP;
        }
    }

#if USING_RENDER_URP
    public static UniversalRenderPipelineAsset UrpAsset
    {
        get
        {
            if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline is UniversalRenderPipelineAsset)
            {
                return QualitySettings.renderPipeline as UniversalRenderPipelineAsset;
            }
            if (GraphicsSettings.currentRenderPipeline != null && GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset)
            {
                return GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            }
            return null;
        }
    }
#endif

    #endregion


    VivoBuiltIn vivoBuiltIn;
#if USING_RENDER_URP
    VivoLit vivoLit;
#endif

    public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        if (IsURP)
        {
#if USING_RENDER_URP
            if (vivoLit==null)
            {
                vivoLit = new VivoLit();
            }
            if (materialEditorIn == null)
                throw new ArgumentNullException("materialEditorIn");
            vivoLit.SetOnGUI(materialEditorIn, properties);
#endif
        }
        else
        {
            if (vivoBuiltIn == null)
            {
                vivoBuiltIn = new VivoBuiltIn();
            }
            vivoBuiltIn.SetOnGUI(materialEditorIn, properties, FindProperty, FindProperty);
        }

    }

    public override void ValidateMaterial(Material material)
    {
        if (IsURP)
        {
#if USING_RENDER_URP
            if (vivoLit == null)
            {
                vivoLit = new VivoLit();
            }
            vivoLit.ValidateMaterial(material);
#endif
        }
        else
        {
            if (vivoBuiltIn == null)
            {
                vivoBuiltIn = new VivoBuiltIn();
            }
            vivoBuiltIn.ValidateMaterial(material);
        }
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        if (IsURP)
        {
#if USING_RENDER_URP
            if (vivoLit == null)
            {
                vivoLit = new VivoLit();
            }
            vivoLit.AssignNewShaderToMaterial(material, oldShader, newShader, ()=> {
                base.AssignNewShaderToMaterial(material, oldShader, newShader);
            });
#endif
        }
        else
        {
            if (vivoBuiltIn == null)
            {
                vivoBuiltIn = new VivoBuiltIn();
            }
            vivoBuiltIn.AssignNewShaderToMaterial(material, oldShader, newShader, ()=> {
                base.AssignNewShaderToMaterial(material, oldShader, newShader);
            });
        }
    }

}

#endif
