#if UNITY_EDITOR && USING_RENDER_URP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Rendering.Universal.ShaderUtils;
using RenderQueue = UnityEngine.Rendering.RenderQueue;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;

public class VivoLit
{

    static class Property
    {
        public static readonly string SpecularWorkflowMode = "_WorkflowMode";
        public static readonly string SurfaceType = "_Surface";
        public static readonly string BlendMode = "_Blend";
        public static readonly string AlphaClip = "_AlphaClip";
        public static readonly string SrcBlend = "_SrcBlend";
        public static readonly string DstBlend = "_DstBlend";
        public static readonly string ZWrite = "_ZWrite";
        public static readonly string CullMode = "_Cull";
        public static readonly string CastShadows = "_CastShadows";
        public static readonly string ReceiveShadows = "_ReceiveShadows";
        public static readonly string QueueOffset = "_QueueOffset";

        // for ShaderGraph shaders only
        public static readonly string ZTest = "_ZTest";
        public static readonly string ZWriteControl = "_ZWriteControl";
        public static readonly string QueueControl = "_QueueControl";

        // Global Illumination requires some properties to be named specifically:
        public static readonly string EmissionMap = "_EmissionMap";
        public static readonly string EmissionColor = "_EmissionColor";
    }

    public static MaterialProperty FindProperty(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory)
    {
        for (int index = 0; index < properties.Length; ++index)
        {
            if (properties[index] != null && properties[index].name == propertyName)
                return properties[index];
        }
        if (propertyIsMandatory)
            throw new ArgumentException("Could not find MaterialProperty: '" + propertyName + "', Num properties: " + (object)properties.Length);
        return null;
    }

    protected MaterialEditor materialEditor { get; set; }
    protected MaterialProperty surfaceTypeProp { get; set; }
    protected MaterialProperty blendModeProp { get; set; }
    protected MaterialProperty cullingProp { get; set; }
    protected MaterialProperty ztestProp { get; set; }
    protected MaterialProperty zwriteProp { get; set; }
    protected MaterialProperty alphaClipProp { get; set; }
    protected MaterialProperty alphaCutoffProp { get; set; }
    protected MaterialProperty castShadowsProp { get; set; }
    protected MaterialProperty receiveShadowsProp { get; set; }
    protected MaterialProperty baseMapProp { get; set; }
    protected MaterialProperty baseColorProp { get; set; }
    protected MaterialProperty emissionMapProp { get; set; }
    protected MaterialProperty emissionColorProp { get; set; }
    protected MaterialProperty queueOffsetProp { get; set; }
    protected MaterialProperty queueControlProp { get; set; }

    public bool m_FirstTimeApply = true;

    public struct LitProperties
    {
        // Surface Option Props
        public MaterialProperty workflowMode;

        // Surface Input Props
        public MaterialProperty metallic;
        public MaterialProperty specColor;
        public MaterialProperty metallicGlossMap;
        public MaterialProperty specGlossMap;
        public MaterialProperty smoothness;
        public MaterialProperty smoothnessMapChannel;
        public MaterialProperty bumpMapProp;
        public MaterialProperty bumpScaleProp;
        public MaterialProperty parallaxMapProp;
        public MaterialProperty parallaxScaleProp;
        public MaterialProperty occlusionStrength;
        public MaterialProperty occlusionMap;

        // Advanced Props
        public MaterialProperty highlights;
        public MaterialProperty reflections;

        public MaterialProperty clearCoat;  // Enable/Disable dummy property
        public MaterialProperty clearCoatMap;
        public MaterialProperty clearCoatMask;
        public MaterialProperty clearCoatSmoothness;

        public LitProperties(MaterialProperty[] properties)
        {
            // Surface Option Props
            workflowMode = BaseShaderGUI.FindProperty("_WorkflowMode", properties, false);
            // Surface Input Props
            metallic = BaseShaderGUI.FindProperty("_Metallic", properties);
            specColor = BaseShaderGUI.FindProperty("_SpecColor", properties, false);
            metallicGlossMap = BaseShaderGUI.FindProperty("_MetallicGlossMap", properties);
            specGlossMap = BaseShaderGUI.FindProperty("_SpecGlossMap", properties, false);
            smoothness = BaseShaderGUI.FindProperty("_Smoothness", properties, false);
            smoothnessMapChannel = BaseShaderGUI.FindProperty("_SmoothnessTextureChannel", properties, false);
            bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
            bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);
            parallaxMapProp = BaseShaderGUI.FindProperty("_ParallaxMap", properties, false);
            parallaxScaleProp = BaseShaderGUI.FindProperty("_Parallax", properties, false);
            occlusionStrength = BaseShaderGUI.FindProperty("_OcclusionStrength", properties, false);
            occlusionMap = BaseShaderGUI.FindProperty("_OcclusionMap", properties, false);
            // Advanced Props
            highlights = BaseShaderGUI.FindProperty("_SpecularHighlights", properties, false);
            reflections = BaseShaderGUI.FindProperty("_EnvironmentReflections", properties, false);

            clearCoat = BaseShaderGUI.FindProperty("_ClearCoat", properties, false);
            clearCoatMap = BaseShaderGUI.FindProperty("_ClearCoatMap", properties, false);
            clearCoatMask = BaseShaderGUI.FindProperty("_ClearCoatMask", properties, false);
            clearCoatSmoothness = BaseShaderGUI.FindProperty("_ClearCoatSmoothness", properties, false);
        }
    }
    private LitProperties litProperties;

    public struct LitDetailProperties
    {
        public MaterialProperty detailMask;
        public MaterialProperty detailAlbedoMapScale;
        public MaterialProperty detailAlbedoMap;
        public MaterialProperty detailNormalMapScale;
        public MaterialProperty detailNormalMap;

        public LitDetailProperties(MaterialProperty[] properties)
        {
            detailMask = BaseShaderGUI.FindProperty("_DetailMask", properties, false);
            detailAlbedoMapScale = BaseShaderGUI.FindProperty("_DetailAlbedoMapScale", properties, false);
            detailAlbedoMap = BaseShaderGUI.FindProperty("_DetailAlbedoMap", properties, false);
            detailNormalMapScale = BaseShaderGUI.FindProperty("_DetailNormalMapScale", properties, false);
            detailNormalMap = BaseShaderGUI.FindProperty("_DetailNormalMap", properties, false);
        }
    }
    private LitDetailProperties litDetailProperties;

    public virtual void FindProperties(MaterialProperty[] properties)
    {
        var material = materialEditor?.target as Material;
        if (material == null)
            return;

        surfaceTypeProp = FindProperty(Property.SurfaceType, properties, false);
        blendModeProp = FindProperty(Property.BlendMode, properties, false);
        cullingProp = FindProperty(Property.CullMode, properties, false);
        zwriteProp = FindProperty(Property.ZWriteControl, properties, false);
        ztestProp = FindProperty(Property.ZTest, properties, false);
        alphaClipProp = FindProperty(Property.AlphaClip, properties, false);

        // ShaderGraph Lit and Unlit Subtargets only
        castShadowsProp = FindProperty(Property.CastShadows, properties, false);
        queueControlProp = FindProperty(Property.QueueControl, properties, false);

        // ShaderGraph Lit, and Lit.shader
        receiveShadowsProp = FindProperty(Property.ReceiveShadows, properties, false);

        // The following are not mandatory for shadergraphs (it's up to the user to add them to their graph)
        alphaCutoffProp = FindProperty("_Cutoff", properties, false);
        baseMapProp = FindProperty("_BaseMap", properties, false);
        baseColorProp = FindProperty("_BaseColor", properties, false);
        emissionMapProp = FindProperty(Property.EmissionMap, properties, false);
        emissionColorProp = FindProperty(Property.EmissionColor, properties, false);
        queueOffsetProp = FindProperty(Property.QueueOffset, properties, false);
        litProperties = new LitProperties(properties);
        litDetailProperties = new LitDetailProperties(properties);
    }

    public void SetOnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        materialEditor = materialEditorIn;
        Material material = materialEditor.target as Material;
        FindProperties(properties);

        // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching some existing
        // material to a universal shader.
        if (m_FirstTimeApply)
        {
            OnOpenGUI(material, materialEditorIn);
            m_FirstTimeApply = false;
        }

        ShaderPropertiesGUI(material);
    }

    public void ShaderPropertiesGUI(Material material)
    {
        m_MaterialScopeList.DrawHeaders(materialEditor, material);
    }

#region//Enum

    [Flags]
    public enum Expandable
    {
        SurfaceOptions = 1 << 0,
        SurfaceInputs = 1 << 1,
        Advanced = 1 << 2,
        Details = 1 << 3,
    }
    public enum SurfaceType
    {
        Opaque,
        Transparent
    }
    public enum RenderFace
    {
        Front = 2,
        Back = 1,
        Both = 0
    }
    public enum QueueControl
    {
        Auto = 0,
        UserOverride = 1
    }
    public enum ZWriteControl
    {
        Auto = 0,
        ForceEnabled = 1,
        ForceDisabled = 2
    }
    public enum ZTestMode  // the values here match UnityEngine.Rendering.CompareFunction
    {
        Disabled = 0,
        Never = 1,
        Less = 2,
        Equal = 3,
        LEqual = 4,     // default for most rendering
        Greater = 5,
        NotEqual = 6,
        GEqual = 7,
        Always = 8,
    }
    public enum WorkflowMode
    {
        Specular = 0,
        Metallic
    }
    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }
    public enum BlendMode
    {
        Alpha,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Premultiply, // Physically plausible transparency mode, implemented as alpha pre-multiply
        Additive,
        Multiply
    }
    public enum MaterialUpdateType
    {
        CreatedNewMaterial,
        ChangedAssignedShader,
        ModifiedShader,
        ModifiedMaterial
    }
    public enum ShaderID
    {
        Unknown = -1,

        Lit = ShaderPathID.Lit,
        SimpleLit = ShaderPathID.SimpleLit,
        Unlit = ShaderPathID.Unlit,
        TerrainLit = ShaderPathID.TerrainLit,
        ParticlesLit = ShaderPathID.ParticlesLit,
        ParticlesSimpleLit = ShaderPathID.ParticlesSimpleLit,
        ParticlesUnlit = ShaderPathID.ParticlesUnlit,
        BakedLit = ShaderPathID.BakedLit,
        SpeedTree7 = ShaderPathID.SpeedTree7,
        SpeedTree7Billboard = ShaderPathID.SpeedTree7Billboard,
        SpeedTree8 = ShaderPathID.SpeedTree8,

        // ShaderGraph IDs start at 1000, correspond to subtargets
        SG_Unlit = 1000,        // UniversalUnlitSubTarget
        SG_Lit,                 // UniversalLitSubTarget
    }
#endregion

#region//GUIContent

    public static readonly string[] surfaceTypeNames = Enum.GetNames(typeof(SurfaceType));
    public static readonly string[] blendModeNames = Enum.GetNames(typeof(BlendMode));
    public static readonly string[] renderFaceNames = Enum.GetNames(typeof(RenderFace));
    public static readonly string[] zwriteNames = Enum.GetNames(typeof(ZWriteControl));
    public static readonly string[] queueControlNames = Enum.GetNames(typeof(QueueControl));

    // need to skip the first entry for ztest (ZTestMode.Disabled is not a valid value)
    public static readonly int[] ztestValues = ((int[])Enum.GetValues(typeof(ZTestMode))).Skip(1).ToArray();
    public static readonly string[] ztestNames = Enum.GetNames(typeof(ZTestMode)).Skip(1).ToArray();

    // Categories
    public static readonly GUIContent SurfaceOptions =
        EditorGUIUtility.TrTextContent("Surface Options", "Controls how URP Renders the material on screen.");

    public static readonly GUIContent SurfaceInputs = EditorGUIUtility.TrTextContent("Surface Inputs",
        "These settings describe the look and feel of the surface itself.");

    public static readonly GUIContent AdvancedLabel = EditorGUIUtility.TrTextContent("Advanced Options",
        "These settings affect behind-the-scenes rendering and underlying calculations.");

    public static readonly GUIContent surfaceType = EditorGUIUtility.TrTextContent("Surface Type",
        "Select a surface type for your texture. Choose between Opaque or Transparent.");

    public static readonly GUIContent blendingMode = EditorGUIUtility.TrTextContent("Blending Mode",
        "Controls how the color of the Transparent surface blends with the Material color in the background.");

    public static readonly GUIContent cullingText = EditorGUIUtility.TrTextContent("Render Face",
        "Specifies which faces to cull from your geometry. Front culls front faces. Back culls backfaces. None means that both sides are rendered.");

    public static readonly GUIContent zwriteText = EditorGUIUtility.TrTextContent("Depth Write",
        "Controls whether the shader writes depth.  Auto will write only when the shader is opaque.");

    public static readonly GUIContent ztestText = EditorGUIUtility.TrTextContent("Depth Test",
        "Specifies the depth test mode.  The default is LEqual.");

    public static readonly GUIContent alphaClipText = EditorGUIUtility.TrTextContent("Alpha Clipping",
        "Makes your Material act like a Cutout shader. Use this to create a transparent effect with hard edges between opaque and transparent areas.");

    public static readonly GUIContent alphaClipThresholdText = EditorGUIUtility.TrTextContent("Threshold",
        "Sets where the Alpha Clipping starts. The higher the value is, the brighter the  effect is when clipping starts.");

    public static readonly GUIContent castShadowText = EditorGUIUtility.TrTextContent("Cast Shadows",
        "When enabled, this GameObject will cast shadows onto any geometry that can receive them.");

    public static readonly GUIContent receiveShadowText = EditorGUIUtility.TrTextContent("Receive Shadows",
        "When enabled, other GameObjects can cast shadows onto this GameObject.");

    public static readonly GUIContent baseMap = EditorGUIUtility.TrTextContent("Base Map",
        "Specifies the base Material and/or Color of the surface. If you’ve selected Transparent or Alpha Clipping under Surface Options, your Material uses the Texture’s alpha channel or color.");

    public static readonly GUIContent emissionMap = EditorGUIUtility.TrTextContent("Emission Map",
        "Determines the color and intensity of light that the surface of the material emits.");

    public static readonly GUIContent normalMapText =
        EditorGUIUtility.TrTextContent("Normal Map", "Designates a Normal Map to create the illusion of bumps and dents on this Material's surface.");

    public static readonly GUIContent bumpScaleNotSupported =
        EditorGUIUtility.TrTextContent("Bump scale is not supported on mobile platforms");

    public static readonly GUIContent fixNormalNow = EditorGUIUtility.TrTextContent("Fix now",
        "Converts the assigned texture to be a normal map format.");

    public static readonly GUIContent queueSlider = EditorGUIUtility.TrTextContent("Sorting Priority",
        "Determines the chronological rendering order for a Material. Materials with lower value are rendered first.");

    public static readonly GUIContent queueControl = EditorGUIUtility.TrTextContent("Queue Control",
        "Controls whether render queue is automatically set based on material surface type, or explicitly set by the user.");

    public static readonly GUIContent documentationIcon = EditorGUIUtility.TrIconContent("_Help", $"Open Reference for URP Shaders.");

    public static readonly GUIContent detailInputs = EditorGUIUtility.TrTextContent("Detail Inputs",
        "These settings define the surface details by tiling and overlaying additional maps on the surface.");
    public static readonly GUIContent detailMaskText = EditorGUIUtility.TrTextContent("Mask",
        "Select a mask for the Detail map. The mask uses the alpha channel of the selected texture. The Tiling and Offset settings have no effect on the mask.");
    public static readonly GUIContent detailAlbedoMapText = EditorGUIUtility.TrTextContent("Base Map",
        "Select the surface detail texture.The alpha of your texture determines surface hue and intensity.");
    public static readonly GUIContent detailAlbedoMapScaleInfo = EditorGUIUtility.TrTextContent("Setting the scaling factor to a value other than 1 results in a less performant shader variant.");
    public static readonly GUIContent detailNormalMapText = EditorGUIUtility.TrTextContent("Normal Map",
                "Designates a Normal Map to create the illusion of bumps and dents in the details of this Material's surface.");
    public static GUIContent occlusionText = EditorGUIUtility.TrTextContent("Occlusion Map",
                "Sets an occlusion map to simulate shadowing from ambient lighting.");
    public static GUIContent clearCoatText = EditorGUIUtility.TrTextContent("Clear Coat",
                "A multi-layer material feature which simulates a thin layer of coating on top of the surface material." +
                "\nPerformance cost is considerable as the specular component is evaluated twice, once per layer.");
    public static GUIContent clearCoatMaskText = EditorGUIUtility.TrTextContent("Mask",
                "Specifies the amount of the coat blending." +
                "\nActs as a multiplier of the clear coat map mask value or as a direct mask value if no map is specified." +
                "\nThe map specifies clear coat mask in the red channel and clear coat smoothness in the green channel.");
    public static GUIContent clearCoatSmoothnessText = EditorGUIUtility.TrTextContent("Smoothness",
                "Specifies the smoothness of the coating." +
                "\nActs as a multiplier of the clear coat map smoothness value or as a direct smoothness value if no map is specified.");
    public static GUIContent heightMapText = EditorGUIUtility.TrTextContent("Height Map",
                "Defines a Height Map that will drive a parallax effect in the shader making the surface seem displaced.");
    public static readonly string[] metallicSmoothnessChannelNames = { "Metallic Alpha", "Albedo Alpha" };
    public static readonly string[] specularSmoothnessChannelNames = { "Specular Alpha", "Albedo Alpha" };
    public static GUIContent metallicMapText =
                EditorGUIUtility.TrTextContent("Metallic Map", "Sets and configures the map for the Metallic workflow.");
    public static GUIContent specularMapText =
                EditorGUIUtility.TrTextContent("Specular Map", "Designates a Specular Map and specular color determining the apperance of reflections on this Material's surface.");
    public static GUIContent smoothnessText = EditorGUIUtility.TrTextContent("Smoothness",
                "Controls the spread of highlights and reflections on the surface.");
    public static GUIContent smoothnessMapChannelText =
                EditorGUIUtility.TrTextContent("Source",
                    "Specifies where to sample a smoothness map from. By default, uses the alpha channel for your map.");
    public static GUIContent highlightsText = EditorGUIUtility.TrTextContent("Specular Highlights",
                "When enabled, the Material reflects the shine from direct lighting.");
    public static GUIContent reflectionsText =
                EditorGUIUtility.TrTextContent("Environment Reflections",
                    "When enabled, the Material samples reflections from the nearest Reflection Probes or Lighting Probe.");
    public static GUIContent workflowModeText = EditorGUIUtility.TrTextContent("Workflow Mode",
            "Select a workflow that fits your textures. Choose between Metallic or Specular.");

#endregion

    static readonly string[] workflowModeNames = Enum.GetNames(typeof(WorkflowMode));
    protected virtual uint materialFilter => uint.MaxValue;
    readonly MaterialHeaderScopeList m_MaterialScopeList = new MaterialHeaderScopeList(uint.MaxValue & ~(uint)Expandable.Advanced);

    public void OnOpenGUI(Material material, MaterialEditor materialEditor)
    {
        var filter = (Expandable)materialFilter;

        // Generate the foldouts
        if (filter.HasFlag(Expandable.SurfaceOptions))
            m_MaterialScopeList.RegisterHeaderScope(SurfaceOptions, (uint)Expandable.SurfaceOptions, DrawSurfaceOptions);

        if (filter.HasFlag(Expandable.SurfaceInputs))
            m_MaterialScopeList.RegisterHeaderScope(SurfaceInputs, (uint)Expandable.SurfaceInputs, DrawSurfaceInputs);

        if (filter.HasFlag(Expandable.Details))
            FillAdditionalFoldouts(m_MaterialScopeList);

        if (filter.HasFlag(Expandable.Advanced))
            m_MaterialScopeList.RegisterHeaderScope(AdvancedLabel, (uint)Expandable.Advanced, DrawAdvancedOptions);
    }
    public void ValidateMaterial(Material material)
    {
        SetMaterialKeywords(material,SetMaterialKeywords, SetDetailMaterialKeywords);
    }
    public static void SetDetailMaterialKeywords(Material material)
    {
        if (material.HasProperty("_DetailAlbedoMap") && material.HasProperty("_DetailNormalMap") && material.HasProperty("_DetailAlbedoMapScale"))
        {
            bool isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
            bool hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
            CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
            CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
        }
    }
    static void SetupSpecularWorkflowKeyword(Material material, out bool isSpecularWorkflow)
    {
        isSpecularWorkflow = false;     // default is metallic workflow
        if (material.HasProperty(Property.SpecularWorkflowMode))
            isSpecularWorkflow = ((WorkflowMode)material.GetFloat(Property.SpecularWorkflowMode)) == WorkflowMode.Specular;
        CoreUtils.SetKeyword(material, "_SPECULAR_SETUP", isSpecularWorkflow);
    }
    public static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
    {
        int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
        if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
            return SmoothnessMapChannel.AlbedoAlpha;

        return SmoothnessMapChannel.SpecularMetallicAlpha;
    }
    public static void SetMaterialKeywords(Material material)
    {
        SetupSpecularWorkflowKeyword(material, out bool isSpecularWorkFlow);

        // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
        // (MaterialProperty value might come from renderer material property block)
        var specularGlossMap = isSpecularWorkFlow ? "_SpecGlossMap" : "_MetallicGlossMap";
        var hasGlossMap = material.GetTexture(specularGlossMap) != null;

        CoreUtils.SetKeyword(material, "_METALLICSPECGLOSSMAP", hasGlossMap);

        if (material.HasProperty("_SpecularHighlights"))
            CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                material.GetFloat("_SpecularHighlights") == 0.0f);
        if (material.HasProperty("_EnvironmentReflections"))
            CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                material.GetFloat("_EnvironmentReflections") == 0.0f);
        if (material.HasProperty("_OcclusionMap"))
            CoreUtils.SetKeyword(material, "_OCCLUSIONMAP", material.GetTexture("_OcclusionMap"));

        if (material.HasProperty("_ParallaxMap"))
            CoreUtils.SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));

        if (material.HasProperty("_SmoothnessTextureChannel"))
        {
            var opaque = IsOpaque(material);
            CoreUtils.SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A",
                GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha && opaque);
        }

        // Clear coat keywords are independent to remove possiblity of invalid combinations.
        if (ClearCoatEnabled(material))
        {
            var hasMap = material.HasProperty("_ClearCoatMap") && material.GetTexture("_ClearCoatMap") != null;
            if (hasMap)
            {
                CoreUtils.SetKeyword(material, "_CLEARCOAT", false);
                CoreUtils.SetKeyword(material, "_CLEARCOATMAP", true);
            }
            else
            {
                CoreUtils.SetKeyword(material, "_CLEARCOAT", true);
                CoreUtils.SetKeyword(material, "_CLEARCOATMAP", false);
            }
        }
        else
        {
            CoreUtils.SetKeyword(material, "_CLEARCOAT", false);
            CoreUtils.SetKeyword(material, "_CLEARCOATMAP", false);
        }
    }
    private static bool ClearCoatEnabled(Material material)
    {
        return material.HasProperty("_ClearCoat") && material.GetFloat("_ClearCoat") > 0.0;
    }
    static void SetMaterialSrcDstBlendProperties(Material material, UnityEngine.Rendering.BlendMode srcBlend, UnityEngine.Rendering.BlendMode dstBlend)
    {
        if (material.HasProperty(Property.SrcBlend))
            material.SetFloat(Property.SrcBlend, (float)srcBlend);

        if (material.HasProperty(Property.DstBlend))
            material.SetFloat(Property.DstBlend, (float)dstBlend);
    }
    static void SetupMaterialBlendModeInternal(Material material, out int automaticRenderQueue)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        bool alphaClip = false;
        if (material.HasProperty(Property.AlphaClip))
            alphaClip = material.GetFloat(Property.AlphaClip) >= 0.5;
        CoreUtils.SetKeyword(material, ShaderKeywordStrings._ALPHATEST_ON, alphaClip);

        // default is to use the shader render queue
        int renderQueue = material.shader.renderQueue;
        material.SetOverrideTag("RenderType", "");      // clear override tag
        if (material.HasProperty(Property.SurfaceType))
        {
            SurfaceType surfaceType = (SurfaceType)material.GetFloat(Property.SurfaceType);
            bool zwrite = false;
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT, surfaceType == SurfaceType.Transparent);
            if (surfaceType == SurfaceType.Opaque)
            {
                if (alphaClip)
                {
                    renderQueue = (int)RenderQueue.AlphaTest;
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                }
                else
                {
                    renderQueue = (int)RenderQueue.Geometry;
                    material.SetOverrideTag("RenderType", "Opaque");
                }

                SetMaterialSrcDstBlendProperties(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
                zwrite = true;
                material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                material.DisableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
            }
            else // SurfaceType Transparent
            {
                BlendMode blendMode = (BlendMode)material.GetFloat(Property.BlendMode);

                material.DisableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                material.DisableKeyword(ShaderKeywordStrings._ALPHAMODULATE_ON);

                // Specific Transparent Mode Settings
                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        SetMaterialSrcDstBlendProperties(material,
                            UnityEngine.Rendering.BlendMode.SrcAlpha,
                            UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        break;
                    case BlendMode.Premultiply:
                        SetMaterialSrcDstBlendProperties(material,
                            UnityEngine.Rendering.BlendMode.One,
                            UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.EnableKeyword(ShaderKeywordStrings._ALPHAPREMULTIPLY_ON);
                        break;
                    case BlendMode.Additive:
                        SetMaterialSrcDstBlendProperties(material,
                            UnityEngine.Rendering.BlendMode.SrcAlpha,
                            UnityEngine.Rendering.BlendMode.One);
                        break;
                    case BlendMode.Multiply:
                        SetMaterialSrcDstBlendProperties(material,
                            UnityEngine.Rendering.BlendMode.DstColor,
                            UnityEngine.Rendering.BlendMode.Zero);
                        material.EnableKeyword(ShaderKeywordStrings._ALPHAMODULATE_ON);
                        break;
                }

                // General Transparent Material Settings
                material.SetOverrideTag("RenderType", "Transparent");
                zwrite = false;
                material.EnableKeyword(ShaderKeywordStrings._SURFACE_TYPE_TRANSPARENT);
                renderQueue = (int)RenderQueue.Transparent;
            }

            // check for override enum
            if (material.HasProperty(Property.ZWriteControl))
            {
                var zwriteControl = (ZWriteControl)material.GetFloat(Property.ZWriteControl);
                if (zwriteControl == ZWriteControl.ForceEnabled)
                    zwrite = true;
                else if (zwriteControl == ZWriteControl.ForceDisabled)
                    zwrite = false;
            }
            SetMaterialZWriteProperty(material, zwrite);
            material.SetShaderPassEnabled("DepthOnly", zwrite);
        }
        else
        {
            // no surface type property -- must be hard-coded by the shadergraph,
            // so ensure the pass is enabled at the material level
            material.SetShaderPassEnabled("DepthOnly", true);
        }

        // must always apply queue offset, even if not set to material control
        if (material.HasProperty(Property.QueueOffset))
            renderQueue += (int)material.GetFloat(Property.QueueOffset);

        automaticRenderQueue = renderQueue;
    }
    static void SetMaterialZWriteProperty(Material material, bool zwriteEnabled)
    {
        if (material.HasProperty(Property.ZWrite))
            material.SetFloat(Property.ZWrite, zwriteEnabled ? 1.0f : 0.0f);
    }
    static void UpdateMaterialSurfaceOptions(Material material, bool automaticRenderQueue)
    {
        // Setup blending - consistent across all Universal RP shaders
        SetupMaterialBlendModeInternal(material, out int renderQueue);

        // apply automatic render queue
        if (automaticRenderQueue && (renderQueue != material.renderQueue))
            material.renderQueue = renderQueue;

        bool isShaderGraph = IsShaderGraph(material);

        // Cast Shadows
        bool castShadows = true;
        if (material.HasProperty(Property.CastShadows))
        {
            castShadows = (material.GetFloat(Property.CastShadows) != 0.0f);
        }
        else
        {
            if (isShaderGraph)
            {
                // Lit.shadergraph or Unlit.shadergraph, but no material control defined
                // enable the pass in the material, so shader can decide...
                castShadows = true;
            }
            else
            {
                // Lit.shader or Unlit.shader -- set based on transparency
                castShadows = IsOpaque(material);
            }
        }
        material.SetShaderPassEnabled("ShadowCaster", castShadows);

        // Receive Shadows
        if (material.HasProperty(Property.ReceiveShadows))
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._RECEIVE_SHADOWS_OFF, material.GetFloat(Property.ReceiveShadows) == 0.0f);
    }
    public static void SetMaterialKeywords(Material material, Action<Material> shadingModelFunc = null, Action<Material> shaderFunc = null)
    {
        UpdateMaterialSurfaceOptions(material, automaticRenderQueue: true);

        // Setup double sided GI based on Cull state
        if (material.HasProperty(Property.CullMode))
            material.doubleSidedGI = (RenderFace)material.GetFloat(Property.CullMode) != RenderFace.Front;

        // Temporary fix for lightmapping. TODO: to be replaced with attribute tag.
        if (material.HasProperty("_MainTex"))
        {
            material.SetTexture("_MainTex", material.GetTexture("_BaseMap"));
            material.SetTextureScale("_MainTex", material.GetTextureScale("_BaseMap"));
            material.SetTextureOffset("_MainTex", material.GetTextureOffset("_BaseMap"));
        }
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", material.GetColor("_BaseColor"));

        // Emission
        if (material.HasProperty(Property.EmissionColor))
            MaterialEditor.FixupEmissiveFlag(material);

        bool shouldEmissionBeEnabled =
            (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;

        // Not sure what this is used for, I don't see this property declared by any Unity shader in our repo...
        // I'm guessing it is some kind of legacy material upgrade support thing?  Or maybe just dead code now...
        if (material.HasProperty("_EmissionEnabled") && !shouldEmissionBeEnabled)
            shouldEmissionBeEnabled = material.GetFloat("_EmissionEnabled") >= 0.5f;

        CoreUtils.SetKeyword(material, ShaderKeywordStrings._EMISSION, shouldEmissionBeEnabled);

        // Normal Map
        if (material.HasProperty("_BumpMap"))
            CoreUtils.SetKeyword(material, ShaderKeywordStrings._NORMALMAP, material.GetTexture("_BumpMap"));

        // Shader specific keyword functions
        shadingModelFunc?.Invoke(material);
        shaderFunc?.Invoke(material);
    }

    public static void DoDetailArea(LitDetailProperties properties, MaterialEditor materialEditor)
    {
        materialEditor.TexturePropertySingleLine(detailMaskText, properties.detailMask);
        materialEditor.TexturePropertySingleLine(detailAlbedoMapText, properties.detailAlbedoMap,
            properties.detailAlbedoMap.textureValue != null ? properties.detailAlbedoMapScale : null);
        if (properties.detailAlbedoMapScale.floatValue != 1.0f)
        {
            EditorGUILayout.HelpBox(detailAlbedoMapScaleInfo.text, MessageType.Info, true);
        }
        materialEditor.TexturePropertySingleLine(detailNormalMapText, properties.detailNormalMap,
            properties.detailNormalMap.textureValue != null ? properties.detailNormalMapScale : null);
        materialEditor.TextureScaleOffsetProperty(properties.detailAlbedoMap);
    }

    public void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
    {
        materialScopesList.RegisterHeaderScope(detailInputs, Expandable.Details, _ => DoDetailArea(litDetailProperties, materialEditor));
    }

    public void DrawBaseProperties(Material material)
    {
        if (baseMapProp != null && baseColorProp != null) // Draw the baseMap, most shader will have at least a baseMap
        {
            materialEditor.TexturePropertySingleLine(baseMap, baseMapProp, baseColorProp);
        }
    }

    public static void Inputs(LitProperties properties, MaterialEditor materialEditor, Material material)
    {
        DoMetallicSpecularArea(properties, materialEditor, material);
        BaseShaderGUI.DrawNormalArea(materialEditor, properties.bumpMapProp, properties.bumpScaleProp);

        if (HeightmapAvailable(material))
            DoHeightmapArea(properties, materialEditor);

        if (properties.occlusionMap != null)
        {
            materialEditor.TexturePropertySingleLine(occlusionText, properties.occlusionMap,
                properties.occlusionMap.textureValue != null ? properties.occlusionStrength : null);
        }

        // Check that we have all the required properties for clear coat,
        // otherwise we will get null ref exception from MaterialEditor GUI helpers.
        if (ClearCoatAvailable(material))
            DoClearCoat(properties, materialEditor, material);
    }
    private static bool ClearCoatAvailable(Material material)
    {
        return material.HasProperty("_ClearCoat")
            && material.HasProperty("_ClearCoatMap")
            && material.HasProperty("_ClearCoatMask")
            && material.HasProperty("_ClearCoatSmoothness");
    }
    public static void DoClearCoat(LitProperties properties, MaterialEditor materialEditor, Material material)
    {
        materialEditor.ShaderProperty(properties.clearCoat, clearCoatText);
        var coatEnabled = material.GetFloat("_ClearCoat") > 0.0;

        EditorGUI.BeginDisabledGroup(!coatEnabled);
        {
            materialEditor.TexturePropertySingleLine(clearCoatMaskText, properties.clearCoatMap, properties.clearCoatMask);

            EditorGUI.indentLevel += 2;

            // Texture and HDR color controls
            materialEditor.ShaderProperty(properties.clearCoatSmoothness, clearCoatSmoothnessText);

            EditorGUI.indentLevel -= 2;
        }
        EditorGUI.EndDisabledGroup();
    }
    private static bool HeightmapAvailable(Material material)
    {
        return material.HasProperty("_Parallax")
            && material.HasProperty("_ParallaxMap");
    }
    private static void DoHeightmapArea(LitProperties properties, MaterialEditor materialEditor)
    {
        materialEditor.TexturePropertySingleLine(heightMapText, properties.parallaxMapProp,
            properties.parallaxMapProp.textureValue != null ? properties.parallaxScaleProp : null);
    }
    public static void DoMetallicSpecularArea(LitProperties properties, MaterialEditor materialEditor, Material material)
    {
        string[] smoothnessChannelNames;
        bool hasGlossMap = false;
        if (properties.workflowMode == null ||
            (WorkflowMode)properties.workflowMode.floatValue == WorkflowMode.Metallic)
        {
            hasGlossMap = properties.metallicGlossMap.textureValue != null;
            smoothnessChannelNames = metallicSmoothnessChannelNames;
            materialEditor.TexturePropertySingleLine(metallicMapText, properties.metallicGlossMap,
                hasGlossMap ? null : properties.metallic);
        }
        else
        {
            hasGlossMap = properties.specGlossMap.textureValue != null;
            smoothnessChannelNames = specularSmoothnessChannelNames;
            BaseShaderGUI.TextureColorProps(materialEditor, specularMapText, properties.specGlossMap,
                hasGlossMap ? null : properties.specColor);
        }
        DoSmoothness(materialEditor, material, properties.smoothness, properties.smoothnessMapChannel, smoothnessChannelNames);
    }
    public static bool IsOpaque(Material material)
    {
        bool opaque = true;
        if (material.HasProperty(Property.SurfaceType))
            opaque = ((BaseShaderGUI.SurfaceType)material.GetFloat(Property.SurfaceType) == BaseShaderGUI.SurfaceType.Opaque);
        return opaque;
    }
    public static void DoSmoothness(MaterialEditor materialEditor, Material material, MaterialProperty smoothness, MaterialProperty smoothnessMapChannel, string[] smoothnessChannelNames)
    {
        EditorGUI.indentLevel += 2;

        materialEditor.ShaderProperty(smoothness, smoothnessText);

        if (smoothnessMapChannel != null) // smoothness channel
        {
            var opaque = IsOpaque(material);
            EditorGUI.indentLevel++;
            EditorGUI.showMixedValue = smoothnessMapChannel.hasMixedValue;
            if (opaque)
            {
                EditorGUI.BeginChangeCheck();
                var smoothnessSource = (int)smoothnessMapChannel.floatValue;
                smoothnessSource = EditorGUILayout.Popup(smoothnessMapChannelText, smoothnessSource, smoothnessChannelNames);
                if (EditorGUI.EndChangeCheck())
                    smoothnessMapChannel.floatValue = smoothnessSource;
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup(smoothnessMapChannelText, 0, smoothnessChannelNames);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUI.showMixedValue = false;
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel -= 2;
    }

    public void DrawSurfaceInputs(Material material)
    {
        DrawBaseProperties(material);
        Inputs(litProperties, materialEditor, material);
        DrawEmissionProperties(material, true);
        DrawTileOffset(materialEditor, baseMapProp);
    }
    protected static void DrawTileOffset(MaterialEditor materialEditor, MaterialProperty textureProp)
    {
        if (textureProp != null)
            materialEditor.TextureScaleOffsetProperty(textureProp);
    }
    protected void DrawEmissionProperties(Material material, bool keyword)
    {
        var emissive = true;

        if (!keyword)
        {
            DrawEmissionTextureProperty();
        }
        else
        {
            emissive = materialEditor.EmissionEnabledProperty();
            using (new EditorGUI.DisabledScope(!emissive))
            {
                DrawEmissionTextureProperty();
            }
        }

        // If texture was assigned and color was black set color to white
        if ((emissionMapProp != null) && (emissionColorProp != null))
        {
            var hadEmissionTexture = emissionMapProp?.textureValue != null;
            var brightness = emissionColorProp.colorValue.maxColorComponent;
            if (emissionMapProp.textureValue != null && !hadEmissionTexture && brightness <= 0f)
                emissionColorProp.colorValue = Color.white;
        }

        if (emissive)
        {
            // Change the GI emission flag and fix it up with emissive as black if necessary.
            materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true);
        }
    }
    private void DrawEmissionTextureProperty()
    {
        if ((emissionMapProp == null) || (emissionColorProp == null))
            return;

        using (new EditorGUI.IndentLevelScope(2))
        {
            materialEditor.TexturePropertyWithHDRColor(emissionMap, emissionMapProp, emissionColorProp, false);
        }
    }
    public static bool IsShaderGraph(Material material)
    {
        var shaderGraphTag = material.GetTag("ShaderGraphShader", false, null);
        return !string.IsNullOrEmpty(shaderGraphTag);
    }

    //[MenuItem("Tools/dfsdfsdf")]
    //static void KKK()
    //{

    //   // Assembly assemblys =  System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.ShaderGraph.ShaderGraphImporter));
    //   // UnityEngine.Debug.Log(assemblys);


    //    Assembly assembly = Assembly.Load("Unity.ShaderGraph.Editor");
    //    Type type = assembly.GetType("UnityEditor.ShaderGraph.ShaderGraphImporter");
    //    UnityEngine.Debug.Log(type);

    //    //MethodInfo methodInfo = type.GetMethod("ReadMaterialRawRenderQueue", BindingFlags.NonPublic| BindingFlags.Static);
    //    //UnityEngine.Debug.Log(methodInfo);


    //}

    static MethodInfo GetMethodInfo(string assemblyName,string typeName,string methName, BindingFlags bindingFlags)
    {
        Type type = GetType(assemblyName, typeName);
        MethodInfo methodInfo = type.GetMethod(methName, bindingFlags);
        return methodInfo;
    }

    static Type GetType(string assemblyName, string typeName)
    {
        Assembly assembly = Assembly.Load(assemblyName);
        Type type = assembly.GetType(typeName);
        return type;
    }

    public static int ReadMaterialRawRenderQueue(Material mat)
    {
        MethodInfo methodInfo = GetMethodInfo("UnityEngine.UI", "UnityEditor.Rendering.Universal.MaterialAccess", "ReadMaterialRawRenderQueue", BindingFlags.NonPublic | BindingFlags.Static);
        object[] objs = new object[] { mat };
        object ob = methodInfo.Invoke(null, objs);
        return (int)ob;
    }
    public static void UpdateMaterialRenderQueueControl(Material material)
    {
        //
        // Render Queue Control handling
        //
        // Check for a raw render queue (the actual serialized setting - material.renderQueue has already been converted)
        // setting of -1, indicating that the material property should be inherited from the shader.
        // If we find this, add a new property "render queue control" set to 0 so we will
        // always know to follow the surface type of the material (this matches the hand-written behavior)
        // If we find another value, add the the property set to 1 so we will know that the
        // user has explicitly selected a render queue and we should not override it.
        //
        bool isShaderGraph = IsShaderGraph(material); // Non-shadergraph materials use automatic behavior
        int rawRenderQueue = ReadMaterialRawRenderQueue(material);
        if (!isShaderGraph || rawRenderQueue == -1)
        {
            material.SetFloat(Property.QueueControl, (float)QueueControl.Auto); // Automatic behavior - surface type override
        }
        else
        {
            material.SetFloat(Property.QueueControl, (float)QueueControl.UserOverride); // User has selected explicit render queue
        }
    }
    public static bool GetAutomaticQueueControlSetting(Material material)
    {
        // If a Shader Graph material doesn't yet have the queue control property,
        // we should not engage automatic behavior until the shader gets reimported.
        bool automaticQueueControl = !IsShaderGraph(material);
        if (material.HasProperty(Property.QueueControl))
        {
            var queueControl = material.GetFloat(Property.QueueControl);
            if (queueControl < 0.0f)
            {
                // The property was added with a negative value, indicating it needs to be validated for this material
                UpdateMaterialRenderQueueControl(material);
            }
            automaticQueueControl = (material.GetFloat(Property.QueueControl) == (float)QueueControl.Auto);
        }
        return automaticQueueControl;
    }
    public void DrawAdvancedOptions(Material material)
    {
        if (litProperties.reflections != null && litProperties.highlights != null)
        {
            materialEditor.ShaderProperty(litProperties.highlights, highlightsText);
            materialEditor.ShaderProperty(litProperties.reflections, reflectionsText);
        }
        // Only draw the sorting priority field if queue control is set to "auto"
        bool autoQueueControl = GetAutomaticQueueControlSetting(material);
        if (autoQueueControl)
            DrawQueueOffsetField();
        materialEditor.EnableInstancingField();
    }
    private const int queueOffsetRange = 50;
    protected void DrawQueueOffsetField()
    {
        if (queueOffsetProp != null)
            materialEditor.IntSliderShaderProperty(queueOffsetProp, -queueOffsetRange, queueOffsetRange, queueSlider);
    }
    public void DoPopup(GUIContent label, MaterialProperty property, string[] options)
    {
        if (property != null)
            materialEditor.PopupShaderProperty(property, label, options);
    }
    public void DrawSurfaceOptions(Material material)
    {
        // Use default labelWidth
        EditorGUIUtility.labelWidth = 0f;

        if (litProperties.workflowMode != null)
            DoPopup(workflowModeText, litProperties.workflowMode, workflowModeNames);

        DoPopup(surfaceType, surfaceTypeProp, surfaceTypeNames);
        if ((surfaceTypeProp != null) && ((SurfaceType)surfaceTypeProp.floatValue == SurfaceType.Transparent))
            DoPopup(blendingMode, blendModeProp, blendModeNames);

        DoPopup(cullingText, cullingProp, renderFaceNames);
        DoPopup(zwriteText, zwriteProp, zwriteNames);

        if (ztestProp != null)
            materialEditor.IntPopupShaderProperty(ztestProp, ztestText.text, ztestNames, ztestValues);

        DrawFloatToggleProperty(alphaClipText, alphaClipProp);

        if ((alphaClipProp != null) && (alphaCutoffProp != null) && (alphaClipProp.floatValue == 1))
            materialEditor.ShaderProperty(alphaCutoffProp, alphaClipThresholdText, 1);

        DrawFloatToggleProperty(castShadowText, castShadowsProp);
        DrawFloatToggleProperty(receiveShadowText, receiveShadowsProp);
    }
    public static void DrawFloatToggleProperty(GUIContent styles, MaterialProperty prop)
    {
        if (prop == null)
            return;

        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = prop.hasMixedValue;
        bool newValue = EditorGUILayout.Toggle(styles, prop.floatValue == 1);
        if (EditorGUI.EndChangeCheck())
            prop.floatValue = newValue ? 1.0f : 0.0f;
        EditorGUI.showMixedValue = false;
    }
    public static void SetupMaterialBlendMode(Material material)
    {
        SetupMaterialBlendModeInternal(material, out int renderQueue);

        // apply automatic render queue
        if (renderQueue != material.renderQueue)
            material.renderQueue = renderQueue;
    }

    public static bool IsShaderGraphAsset(Shader shader)
    {
        var path = AssetDatabase.GetAssetPath(shader);
        var importer = AssetImporter.GetAtPath(path);
        Type type = GetType("Unity.ShaderGraph.Editor", "UnityEditor.ShaderGraph.ShaderGraphImporter");
        if (importer.GetType()== type)
        {
            return true;
        }
        return false;
        //return importer is UnityEditor.ShaderGraph.ShaderGraphImporter;
    }

    static ShaderID GetShaderID(Shader shader)
    {
        if (IsShaderGraphAsset(shader))
        {
            //UniversalMetadata meta;
            //if (!shader.TryGetMetadataOfType<UniversalMetadata>(out meta))
            //    return ShaderID.Unknown;
            //return meta.shaderID;
            return ShaderID.Lit;
        }
        else
        {
            ShaderPathID pathID = UnityEngine.Rendering.Universal.ShaderUtils.GetEnumFromPath(shader.name);
            return (ShaderID)pathID;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="material"></param>
    /// <param name="updateType"></param>
    /// <param name="shaderID"></param>
    static void UpdateMaterial(Material material, MaterialUpdateType updateType,ShaderID shaderID = ShaderID.Unknown)
    {
        // if unknown, look it up from the material's shader
        // NOTE: this will only work for asset-based shaders..
        if (shaderID == ShaderID.Unknown)
            shaderID = GetShaderID(material.shader);

        switch (shaderID)
        {
            case ShaderID.Lit:
                SetMaterialKeywords(material, SetMaterialKeywords);
                break;
            case ShaderID.SimpleLit:
                //SimpleLitShader.SetMaterialKeywords(material, SimpleLitGUI.SetMaterialKeywords);
                break;
            case ShaderID.Unlit:
                //UnlitShader.SetMaterialKeywords(material);
                break;
            case ShaderID.ParticlesLit:
                //ParticlesLitShader.SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, ParticleGUI.SetMaterialKeywords);
                break;
            case ShaderID.ParticlesSimpleLit:
                //ParticlesSimpleLitShader.SetMaterialKeywords(material, SimpleLitGUI.SetMaterialKeywords, ParticleGUI.SetMaterialKeywords);
                break;
            case ShaderID.ParticlesUnlit:
                //ParticlesUnlitShader.SetMaterialKeywords(material, null, ParticleGUI.SetMaterialKeywords);
                break;
            case ShaderID.SG_Lit:
                //ShaderGraphLitGUI.UpdateMaterial(material, updateType);
                break;
            case ShaderID.SG_Unlit:
                //ShaderGraphUnlitGUI.UpdateMaterial(material, updateType);
                break;
            default:
                break;
        }
    }
    public void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader, System.Action baseAC)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        // _Emission property is lost after assigning Standard shader to the material
        // thus transfer it before assigning the new shader
        if (material.HasProperty("_Emission"))
        {
            material.SetColor("_EmissionColor", material.GetColor("_Emission"));
        }

        // Clear all keywords for fresh start
        // Note: this will nuke user-selected custom keywords when they change shaders
        material.shaderKeywords = null;

        baseAC();
        

        // Setup keywords based on the new shader
        UpdateMaterial(material, MaterialUpdateType.ChangedAssignedShader);

        if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
        {
            SetupMaterialBlendMode(material);
            return;
        }

        SurfaceType surfaceType = SurfaceType.Opaque;
        BlendMode blendMode = BlendMode.Alpha;
        if (oldShader.name.Contains("/Transparent/Cutout/"))
        {
            surfaceType = SurfaceType.Opaque;
            material.SetFloat("_AlphaClip", 1);
        }
        else if (oldShader.name.Contains("/Transparent/"))
        {
            // NOTE: legacy shaders did not provide physically based transparency
            // therefore Fade mode
            surfaceType = SurfaceType.Transparent;
            blendMode = BlendMode.Alpha;
        }
        material.SetFloat("_Blend", (float)blendMode);

        material.SetFloat("_Surface", (float)surfaceType);
        if (surfaceType == SurfaceType.Opaque)
        {
            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else
        {
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }

        if (oldShader.name.Equals("Standard (Specular setup)"))
        {
            material.SetFloat("_WorkflowMode", (float)WorkflowMode.Specular);
            Texture texture = material.GetTexture("_SpecGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }
        else
        {
            material.SetFloat("_WorkflowMode", (float)WorkflowMode.Metallic);
            Texture texture = material.GetTexture("_MetallicGlossMap");
            if (texture != null)
                material.SetTexture("_MetallicSpecGlossMap", texture);
        }
    }



}



#endif
